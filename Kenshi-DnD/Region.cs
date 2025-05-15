using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    [Serializable]
    public class Region
    {
        string name;
        string description;
        bool hasBar;
        bool hasShop;
        bool hasLimbHospital;
        bool hasContrabandMarket;
        bool hasRangedShop;
        List<Faction> factions;
        const int DEFAULT_SHOP_SIZE = 3;

        public Region(string name, string description, bool hasBar, bool hasShop, bool hasLimbHospital, bool hasContrabandMarket, bool hasRangedShop)
        {
            this.name = name;
            this.description = description;
            factions = new List<Faction>();
            this.hasBar = hasBar;
            this.hasShop = hasShop;
            this.hasLimbHospital = hasLimbHospital;
            this.hasContrabandMarket = hasContrabandMarket;
            this.hasRangedShop = hasRangedShop;
        }
        public void AddFaction(Faction faction)
        {
            factions.Add(faction);
        }
        public string GetName()
        {
            return name;
        }
        public string GetDescription()
        {
            return description;
        }
        public List<Faction> GetFactions()
        {
            return factions;
        }
        public bool HasBar() { return hasBar; }
        public bool HasShop() { return hasShop; }
        public bool HasLimbHospital() { return hasLimbHospital; }
        public bool HasContrabandMarket() { return hasContrabandMarket; }
        public bool HasRangedShop() { return hasRangedShop; }

        public Hero[] GoToBar(Adventure myAdventure, Random rnd)
        {
            //This makes 20 the max number of possible heroes, as max relations is 100
            int numberOfHeroes = rnd.Next(0, (GetRelations() / 5) + 1);

            if (numberOfHeroes > 0)
            {
                Hero[] heroesInBar = new Hero[numberOfHeroes];
                
                for (int i = 0; i < numberOfHeroes; i++)
                {
                    int rarity = rnd.Next(0, 10);
                    switch (rarity)
                    {
                        case < 5:
                            {
                                heroesInBar[i] = GenerateRandomHero(myAdventure, rnd, Competency.StartCompetency.Apprentice);
                                break;
                            }
                        case < 8:
                            {
                                heroesInBar[i] = GenerateRandomHero(myAdventure, rnd, Competency.StartCompetency.Intermediate);
                                break;
                            }
                        case <= 9:
                            {
                                heroesInBar[i] = GenerateRandomHero(myAdventure, rnd, Competency.StartCompetency.Master);
                                break;
                            }
                    }
                    Debug.WriteLine(heroesInBar[i].GetNameAndTitle());

                }
                return heroesInBar;
            }
            else
            {
                return null;
            }

        }
        public Item[] GoToShop(Adventure myAdventure, Random rnd)
        {
            Item[] itemsInShop = new Item[DEFAULT_SHOP_SIZE];
            Item[] allItems = myAdventure.GetAllItems();
            for (int i = 0; i < DEFAULT_SHOP_SIZE; i++)
            {
                Item itemInShop = null;
                int rarityDecider = rnd.Next(0,4);
                Rarity.Rarities rarity;
                switch (rarityDecider)
                {
                    case 0:
                        {
                            rarity= Rarity.Rarities.Junk;
                            break;
                        }
                    case 1:
                        {
                            rarity = Rarity.Rarities.RustCovered;
                            break;
                        }

                    case 2:
                        {
                            rarity = Rarity.Rarities.Catun;
                            break;
                        }
                    case 3:
                        {
                            rarity = Rarity.Rarities.Mk;
                            break;
                        }
                    default:
                        {
                            //Shouldn't ever trigger this case, however VS makes me put it. I'll put a Debug.WriteLine just in case
                            Debug.WriteLine("Region didn't know what rarity to put");
                            rarity = Rarity.Rarities.Junk;
                            break;
                        }
                }
                do
                {
                    int itemIndex = rnd.Next(0, allItems.Length);
                    if (allItems[itemIndex] is not RangedItem)
                    {
                        itemInShop = allItems[itemIndex];
                    }

                } while (itemInShop == null);

                itemsInShop[i] = itemInShop.GetCopy();
                itemsInShop[i].SetRarity(rarity);
            }
            return itemsInShop;
        }
        //public Item[] GoToContrabandMarket(Adventure myAdventure, Random rnd)
        //{

            //}
            //public Item[] GoToRangedShop(Adventure myAdventure, Random rnd)
            //{

            //}
            //public Limb[] GoToLimbHospital(Adventure myAdventure, Random rnd)
            //{

            //}
        public int GetRelations()
        {
            int relations = 0;
            bool hasAnimals = false;
            for(int i = 0; i< factions.Count; i += 1)
            {
                if (factions[i].GetFactionName() != "@2@Reino Animal@")
                {
                    Debug.WriteLine(factions[i].GetFactionName() + " " + factions[i].GetRelation());
                    relations += factions[i].GetRelation();
                }
                else
                {
                    hasAnimals = true;
                }
            }
            Debug.WriteLine(relations / (factions.Count -
                (hasAnimals ? 1 : 0)));
            return relations / (factions.Count - 
                (hasAnimals ? 1 : 0));
        }
        private Hero GenerateRandomHero(Adventure myAdventure,Random rnd,Competency.StartCompetency competency)
        {
            int points = 5;
            int level = 0;
            switch (competency)
            {
                case Competency.StartCompetency.Apprentice:
                    {
                        points += 1;
                        level += 1;
                        break;
                    }
                case Competency.StartCompetency.Intermediate:
                    {
                        points += 3;
                        level += 3;
                        break;
                    }
                case Competency.StartCompetency.Master:
                    {
                        points += 5;
                        level += 5;
                        break;
                    }
            }
            int bruteForce = 0;
            int dexterity = 0;
            int resistance = 0;
            int agility = 0;
            for(int i = 0; i < points; i += 1)
            {
                int option = rnd.Next(0, 4);

                switch (option)
                {
                    case 0:
                        {
                            bruteForce += 1;
                            break;
                        }
                    case 1:
                        {
                            dexterity += 1;
                            break;
                        }
                    case 2:
                        {
                            resistance += 1;
                            break;
                        }
                    case 3:
                        {
                            agility += 1;
                            break;
                        }
                }
            }

            string name = myAdventure.GetNames()[rnd.Next(0, myAdventure.GetNames().Length)];
            string title = myAdventure.GetTitles()[rnd.Next(0, myAdventure.GetTitles().Length)];
            string background = myAdventure.GetBackgrounds()[rnd.Next(0, myAdventure.GetBackgrounds().Length)];
            Race race = null;
            Race subrace = null;
            do
            {
                Race tempRace = myAdventure.GetAllRaces()[rnd.Next(0, myAdventure.GetAllRaces().Length)];
                if (RacesDictionary.RacesAndSubraces.ContainsKey(tempRace.GetName()))
                {
                    race = tempRace;
                }

            } while (race == null);
            do
            {
                Race tempSubrace = myAdventure.GetAllRaces()[rnd.Next(0, myAdventure.GetAllRaces().Length)];

                if (RacesDictionary.RacesAndSubraces[race.GetName()].Contains(tempSubrace.GetName()))
                {
                    subrace = tempSubrace;
                }
            } while (subrace == null);


            Hero hero = new Hero(name, title, background, bruteForce, dexterity, resistance, agility, level, race, subrace, GenerateLimbs(), competency);
            return hero;
        }
        private Limb[] GenerateLimbs()
        {
            Limb[] limbs = new Limb[4];

            for (int i = 0; i < limbs.Length; i += 1)
            {
                limbs[i] = new Limb("Extremidad normal", 0, 0, 0, 0, 0);
            }
            return limbs;
        }

    }
        
}
