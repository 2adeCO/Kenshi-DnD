using System.Diagnostics;

namespace Kenshi_DnD
{
    // Class that has info about regions, what they offer, and what they have. - Santiago Cabrero
    [Serializable]
    public class Region
    {
        // Name
        string name;
        // Description
        string description;
        // Which commerces they have
        bool hasBar;
        bool hasShop;
        bool hasLimbHospital;
        bool hasContrabandMarket;
        bool hasRangedShop;
        // Tells if the player has access to the ranged shop or if can buy items
        bool hasAccessToRangedShop;
        bool canBuyRangedItems;
        // If true, updates the shops when the zone is entered
        bool updateToken;

        // Contents of the commerces
        Hero[] heroesInBar;
        Item[] shop;
        Limb[] limbHospital;
        Item[] contrabandMarket;
        Item[] rangedShop;
        List<Faction> factions;
        // Default const value
        const int DEFAULT_LIMB_HOSPITAL_SIZE = 2;
        const int DEFAULT_SHOP_SIZE = 3;
        // Constructor
        public Region(string name, string description, bool hasBar, bool hasShop, bool hasLimbHospital, bool hasContrabandMarket, bool hasRangedShop)
        {
            this.name = name;
            this.description = description;
            factions = new List<Faction>();
            updateToken = true;
            this.hasBar = hasBar;
            this.hasShop = hasShop;
            this.hasLimbHospital = hasLimbHospital;
            this.hasContrabandMarket = hasContrabandMarket;
            this.hasRangedShop = hasRangedShop;
            hasAccessToRangedShop = false;
        }
        // Affects all the relations of all factions in the zone
        public void AffectsRelations(Adventure myAdventure, Random rnd)
        {
            for (int i = 0; i < factions.Count; i += 1)
            {
                factions[i].AffectRelations(myAdventure, rnd);
            }
        }
        // Improves or worsens relations with the factions in the region
        public void ImproveRelations(int amount, Zone myZone)
        {
            for (int i = 0; i < factions.Count; i += 1)
            {
                myZone.UpdateLog("¡La relación con " + factions[i].GetFactionName() + " mejoró " + amount + "!");
                factions[i].AddOrSubtractRelation(amount);
            }
        }
        // Getters and setters
        public bool CanBuyRangedItems()
        {
            return canBuyRangedItems;
        }
        public void SetCanBuyRangedItems(bool canBuyRangedItems)
        {
            this.canBuyRangedItems = canBuyRangedItems;
        }
        public bool HasAccessToRangedShop()
        {
            return hasAccessToRangedShop;
        }
        public void SetAccessToRangedShop(bool hasAccessToRangedShop)
        {
            this.hasAccessToRangedShop = hasAccessToRangedShop;
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
        // Adds a faction
        public void AddFaction(Faction faction)
        {
            factions.Add(faction);
        }
        // Sets the token to false if is true and returns true, else return false
        public bool ConsumeToken()
        {
            if (updateToken)
            {
                updateToken = false;
                return true;
            }
            return false;
        }
        // Sets the token to true if is null
        public void GainToken()
        {
            if (!updateToken)
            {
                updateToken = true;
            }
        }
        // Updates the bar contents
        public void GoToBar(Adventure myAdventure, Random rnd)
        {
            //This makes 20 the max number of possible heroes, as max relations is 100
            int numberOfHeroes = rnd.Next(0, (GetRelations() / 5) + 1);

            if (numberOfHeroes > 0)
            {
                Hero[] heroesInBar = new Hero[numberOfHeroes];

                for (int i = 0; i < numberOfHeroes; i++)
                {
                    int experience = rnd.Next(0, 10);
                    // Generates a hero with a variable ammount of points
                    switch (experience)
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
                }
                this.heroesInBar = heroesInBar;
            }
            else
            {
                this.heroesInBar = null;
            }

        }
        // Updates the hospital contents
        public void GoToHospital(Adventure myAdventure, Random rnd)
        {
            limbHospital = new Limb[DEFAULT_LIMB_HOSPITAL_SIZE];
            Limb[] allLimbs = myAdventure.GetAllLimbs();
            for (int i = 0; i < DEFAULT_LIMB_HOSPITAL_SIZE; i++)
            {
                int rarityDecider = rnd.Next(0, 20);
                Rarity.Rarities rarity;
                //Chances:
                //Junk: 5 in 20
                //RustCovered: 5 in 20
                //Catun: 4 in 20
                //Mk: 3 in 20
                //Edgewalker: 2 in 20
                //Meitou: 1 in 20
                switch (rarityDecider)
                {
                    case < 5:
                        {
                            rarity = Rarity.Rarities.Junk;
                            break;
                        }
                    case < 10:
                        {
                            rarity = Rarity.Rarities.RustCovered;
                            break;
                        }
                    case < 14:
                        {
                            rarity = Rarity.Rarities.Catun;
                            break;
                        }
                    case < 17:
                        {
                            rarity = Rarity.Rarities.Mk;
                            break;
                        }
                    case < 19:
                        {
                            rarity = Rarity.Rarities.Edgewalker;
                            break;
                        }
                    case < 20:
                        {
                            rarity = Rarity.Rarities.Meitou;
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
                Limb limb = allLimbs[rnd.Next(0, allLimbs.Length)].GetCopy();
                limb.SetRarity(rarity);
                limbHospital[i] = limb;
            }
        }
        public Limb[] GetLimbHospital()
        {
            return limbHospital;
        }
        // Calculates the cost of sleeping in bar, 10 cats per hurt hero, if no one is hurt, then 10 cats
        public int GetSleepCost(Adventure myAdventure)
        {
            Hero[] heroes = myAdventure.GetHeroes();
            int cost = 0;
            for (int i = 0; i < heroes.Length; i++)
            {
                if (heroes[i] != null)
                {
                    if (heroes[i].GetHp() != heroes[i].GetToughness())
                    {
                        cost += 10;
                    }
                }
            }
            if (cost == 0)
            {
                cost = 10;
            }

            return cost;
        }
        // Each hero recovers 10 hp
        public void SleepInBar(Adventure myAdventure, int cost, Zone myZone)
        {
            if (!myAdventure.SpendIfHasEnough(cost))
            {
                return;
            }
            myZone.UpdateLog("Tus héroes durmieron la noche plácidamente");
            Hero[] heroes = myAdventure.GetHeroes();
            for (int i = 0; i < heroes.Length; i++)
            {
                if (heroes[i] != null)
                {
                    if (heroes[i].GetHp() != heroes[i].GetToughness())
                    {
                        heroes[i].Heal(10);
                        heroes[i].GainXp(1);
                    }
                }
            }

        }
        public Hero[] GetHeroesInBar()
        {
            return heroesInBar;
        }
        // Updates the items in the shop
        public void GoToShop(Adventure myAdventure, Random rnd)
        {
            Item[] itemsInShop = new Item[DEFAULT_SHOP_SIZE];
            Item[] allItems = myAdventure.GetAllItems();
            for (int i = 0; i < DEFAULT_SHOP_SIZE; i++)
            {
                Item itemInShop = null;
                int rarityDecider = rnd.Next(0, 4);
                Rarity.Rarities rarity;
                switch (rarityDecider)
                {
                    case 0:
                        {
                            rarity = Rarity.Rarities.Junk;
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
                itemsInShop[i].UpgradeRarity(rarity);
            }
            this.shop = itemsInShop;
        }
        public Item[] GetShop()
        {
            return shop;
        }
        // Updates the items in the contraband shop
        public void GoToContrabandMarket(Adventure myAdventure, Random rnd)
        {
            Item[] itemsInContraband = new Item[DEFAULT_SHOP_SIZE];
            Item[] allItems = myAdventure.GetAllItems();
            for (int i = 0; i < DEFAULT_SHOP_SIZE; i++)
            {
                Item itemInContraband = null;
                int rarityDecider = rnd.Next(0, 4);
                Rarity.Rarities rarity;
                switch (rarityDecider)
                {

                    case < 3:
                        {
                            rarity = Rarity.Rarities.Edgewalker;
                            break;
                        }
                    case 3:
                        {
                            rarity = Rarity.Rarities.Meitou;
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
                    Item item = allItems[itemIndex].GetCopy();
                    item.SetRarity(rarity);
                    if (!HasAlreadyBeenObtained(myAdventure, item))
                    {
                        itemInContraband = allItems[itemIndex];
                    }

                } while (itemInContraband == null);

                itemsInContraband[i] = itemInContraband.GetCopy();
                itemsInContraband[i].UpgradeRarity(rarity);
            }
            this.contrabandMarket = itemsInContraband;
        }
        // Updates items in ranged shop
        public void GoToRangedShop(Adventure myAdventure, Random rnd)
        {
            Item[] itemsInShop = new Item[DEFAULT_SHOP_SIZE];
            Item[] allItems = myAdventure.GetAllItems();
            for (int i = 0; i < DEFAULT_SHOP_SIZE; i++)
            {
                Item itemInShop = null;
                int rarityDecider = rnd.Next(0, 4);
                Rarity.Rarities rarity;
                switch (rarityDecider)
                {
                    case 0:
                        {
                            rarity = Rarity.Rarities.Junk;
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
                    if (allItems[itemIndex] is not MeleeItem)
                    {
                        itemInShop = allItems[itemIndex];
                    }

                } while (itemInShop == null);

                itemsInShop[i] = itemInShop.GetCopy();
                itemsInShop[i].UpgradeRarity(rarity);
            }
            this.rangedShop = itemsInShop;
        }
        public Item[] GetRangedShop()
        {
            return rangedShop;
        }

        public Item[] GetContrabandMarket()
        {
            return contrabandMarket;
        }
        // Tells if an item has already been obtained
        private bool HasAlreadyBeenObtained(Adventure myAdventure, Item item)
        {
            Item[] obtainedItems = myAdventure.GetAlreadyObtainedItems();
            for (int i = 0; i < obtainedItems.Length; i++)
            {
                if (obtainedItems[i] != null)
                {
                    if (obtainedItems[i].GetName() == item.GetName())
                    {
                        if (obtainedItems[i].RarityToString() == item.RarityToString())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        // Gets the average relations minus animals
        public int GetRelations()
        {
            int relations = 0;
            bool hasAnimals = false;
            for (int i = 0; i < factions.Count; i += 1)
            {
                if (factions[i].GetFactionName() != "@2@Reino Animal@")
                {
                    relations += factions[i].GetRelation();
                }
                else
                {
                    hasAnimals = true;
                }
            }
            return relations / (factions.Count -
                (hasAnimals ? 1 : 0));
        }
        // Generates a random hero with random stats, race, subrace, name, title, background....
        private Hero GenerateRandomHero(Adventure myAdventure, Random rnd, Competency.StartCompetency competency)
        {
            int points = 3;
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
            for (int i = 0; i < points; i += 1)
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
        // Generates default limbs
        private Limb[] GenerateLimbs()
        {
            Limb[] limbs = new Limb[4];

            for (int i = 0; i < limbs.Length; i += 1)
            {
                limbs[i] = new Limb("Extremidad normal", 0, 0, 0, 0, 0, 0);
            }
            return limbs;
        }
        // Override of the method to string, returns the commerces of the region, all relations and hostilities of the factions
        public override string ToString()
        {
            string factionInfo = "";
            for (int i = 0; i < factions.Count; i += 1)
            {
                factionInfo += "Relación con " + factions[i].GetFactionName() + ": " + factions[i].GetRelation() + "\n"
                    + factions[i].GetFactionDescription() + "\n" +
                    factions[i].HostilityToString() + "\n";
            }


            return name + "\n" +
                description + "\n" +
                   ((hasBar) ? ("Bar\n") : ("")) +
                   ((hasShop) ? ("Tienda general\n") : ("")) +
                   ((hasLimbHospital) ? ("Mecánico de extremidades\n") : ("")) +
                   ((hasRangedShop) ? ("Club de tiro privado\n") : ("")) +
                   ((hasContrabandMarket) ? ("Mercado de contrabando\n") : ("")) +
                   factionInfo;

        }
    }

}
