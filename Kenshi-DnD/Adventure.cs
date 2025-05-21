using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    [Serializable]
    public class Adventure
    {
        public Dictionary<string, Hero[]> savedSquads;
        string id;
        string factionName;
        const int MINIMUM_NUMBER_LENGTH = 9;
        int cats;
        int color;
        DateTime startDate;
        DateTime hoursPlayed;

        Dice myDice;

        Hero[] heroes;
        Hero[] currentSquad;
        


        PlayerInventory playerInventory;
        Item[] alreadyObtainedItems;

        Faction[] allFactions;
        Region[] allRegions;
        Monster[] allMonsters;
        string[] titles;
        string[] backgrounds;
        string[] names;
        Item[] allItems;
        Race[] allRaces;
        Limb[] allLimbs;
        const int MAX_HEROES = 12;
        const int MAX_SQUADS = 10;
        const int MAX_SQUAD_LENGTH = 4;
        public Adventure(string name, Hero hero, Random rnd, Dice myDice, int startingCats,string factionName,int factionColor, Faction[] allFactions, Region[] allRegions, Monster[] allMonsters,
            string[] titles, string[] backgrounds, string[] names, Item[] allItems, Race[] allRaces, Limb[] allLimbs)
        {
            this.id = GenerateId(name, rnd);
            this.cats = startingCats;
            this.myDice = myDice;
            startDate = DateTime.Now;
            hoursPlayed = DateTime.Now;
            this.factionName = factionName;
            this.color = factionColor;
            heroes = new Hero[MAX_HEROES];
            heroes[0] = hero;
            savedSquads = new Dictionary<string, Hero[]> { };
            //Adds the first squad, which is the one that is created when the game starts
            savedSquads.Add("Mi primera squad", new Hero[MAX_SQUAD_LENGTH]);
            savedSquads["Mi primera squad"][0] = hero;
            currentSquad = savedSquads["Mi primera squad"];

            playerInventory = new PlayerInventory();
            alreadyObtainedItems = new Item[allItems.Length * 2];
            Debug.WriteLine(id);
            this.allFactions = allFactions;
            this.allRegions = allRegions;
            this.allMonsters = allMonsters;
            this.titles = titles;
            this.backgrounds = backgrounds;
            this.names = names;
            this.allItems = allItems;
            this.allRaces = allRaces;
            this.allLimbs = allLimbs;
        }
        private string GenerateId(string name, Random rnd)
        {
            //Generates a random number
            int randomNum = rnd.Next(0, 1000000000);
            //Temporal variable used to count the number's length
            int temp = randomNum;
            //Number's lenth, cannot be zero or less
            int numberCount = 1;
            //The zeroes we add
            string zeroes = "";
            Debug.WriteLine(randomNum);

            //Formats the name, replacing spaces with underscores
            string formattedName = "";
            for (int i = 0; i < name.Length; i += 1)
            {
                if (name[i] == ' ')
                {
                    formattedName += "_";
                }
                else
                {
                    formattedName += name[i];
                }
            }

            //Counts how many numbers there are
            while (temp >= 10)
            {
                numberCount += 1;
                temp /= 10;
            };
            // Calculates the zeroes remaining
            numberCount = MINIMUM_NUMBER_LENGTH - numberCount;

            //Adds the remaining zeroes for aesthetic purpose
            for (int i = 0; i < numberCount; i+=1)
            {
                zeroes += "0";
            }

            //Gets the country's region
            string country = RegionInfo.CurrentRegion.TwoLetterISORegionName;
            //Will serve as default parameter, in case the country isn't included, or it somehow fails at getting the region correctly
            string continent = "n";
            //If the country is in dictionary, will set continent to the continent's number
            if (RegionContinentDictionary.zonesToContinents.ContainsKey(country))
            {
                continent = RegionContinentDictionary.zonesToContinents[country];
            }
            // name: My First Adventure
            // country: Ethiopia
            // number generated: 123456
            // 
            // output: My_First_Adventure2000123456
            return name + continent + zeroes + randomNum;
        }
        public Item[] GetAlreadyObtainedItems()
        {
            return alreadyObtainedItems;
        }
        public string GetId()
        {
            return id;
        }
        public PlayerInventory GetInventory()
        {
            return playerInventory;
        }
        public int GetCats()
        {
            return cats;
        }
        public Dice GetDice()
        {
            return myDice;
        }
        public bool SpendIfHasEnough(int cost)
        {
            if(cost <= cats && cost >= 0)
            {
                this.cats -= cost;
                return true;
            }
            Debug.WriteLine("Not enough money");
            return false;
        }
        public void BuyItem(Item item)
        {
            int cost = item.GetValue();
            cats -= cost;
            bool consumible = false;
            if (item is MeleeItem)
            {
                {
                    MeleeItem meleeItem = (MeleeItem)item;
                    if (meleeItem.BreaksOnUse())
                    {
                        consumible = true;
                    }
                }

                if (item.GetRarity() == Rarity.Rarities.Meitou || item.GetRarity() == Rarity.Rarities.Edgewalker && !consumible)
                {
                    for (int i = 0; i < alreadyObtainedItems.Length; i += 1)
                    {
                        if (alreadyObtainedItems[i] == null)
                        {
                            alreadyObtainedItems[i] = item;
                            break;
                        }
                    }
                }
                playerInventory.AddItem(item.GetCopy());

            }
            else
            {
                if (item.GetRarity() == Rarity.Rarities.Meitou || item.GetRarity() == Rarity.Rarities.Edgewalker)
                {
                    for (int i = 0; i < alreadyObtainedItems.Length; i += 1)
                    {
                        if (alreadyObtainedItems[i] == null)
                        {
                            alreadyObtainedItems[i] = item;
                            break;
                        }
                    }
                }
                playerInventory.AddItem(item.GetCopy());
            }
        } 
        public void HireHero(Hero hero)
        {
            int cost = hero.GetCompetencyCost();

            int countOfHeros = 0;

            for(int i = 0; i < heroes.Length; i += 1)
            {
                if (heroes[i] != null) { countOfHeros += 1; }
            }

            if(cats < cost || countOfHeros >= MAX_HEROES || hero.IsHired())
            {
                Debug.WriteLine("Cost too high o too much heroes or is already hired");
                return;
            }
            
            cats -= cost;
            for(int i = 0; i < heroes.Length; i++)
            {
                if(heroes[i] == null)
                {
                    Debug.WriteLine("Hired in adventure");
                    hero.Hire();
                    heroes[i] = hero;
                    break;
                }
            }
        }
        public Hero[] GetAmputees()
        {

            int count = 0;
            for(int i = 0; i < heroes.Length; i += 1)
            {
                if (heroes[i] != null)
                {
                    if(heroes[i].IsAmputee())
                    {
                        count += 1;
                    }
                }
            }
            Hero[] amputees = new Hero[count];
            count = 0;
            for (int i = 0; i < heroes.Length; i += 1)
            {
                if (heroes[i] != null)
                {
                    if (heroes[i].IsAmputee())
                    {
                        amputees[count] = heroes[i];
                        count += 1;
                    }
                }
            }
            return amputees;
        }
        public Dictionary<string,Hero[]> GetSavedSquads()
        {
            return savedSquads;
        }
        public string GetCurrentSquadName()
        {
            for(int i = 0; i < savedSquads.Count; i += 1)
            {
                if (savedSquads.ElementAt(i).Value == currentSquad)
                {
                    return savedSquads.ElementAt(i).Key;
                }
            }
            Debug.WriteLine("GetCurrentSquadName failed, returning first key");
            return savedSquads.First().Key;
        }
        public Hero[] GetCurrentSquad()
        {
            int count = 0;
            for(int i = 0; i < currentSquad.Length; i += 1)
            {
                if (currentSquad[i] != null)
                {
                    count += 1;
                }
            }
            Hero[] heroesToReturn = new Hero[count];
            count = 0;
            int iterator = 0;
            do
            {
                if (currentSquad[iterator] != null)
                {
                    heroesToReturn[count] = currentSquad[iterator];
                    count += 1;
                }
                iterator += 1;

            }while (count != heroesToReturn.Length);

            return heroesToReturn;
        }
        public bool IsInCurrentSquad(Hero hero)
        {
            bool found = false;
            for(int i = 0; i < currentSquad.Length; i+=1)
            {
                if (currentSquad[i] == hero)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }
        public void AddHeroInSquad(Hero hero)
        {
            
            for (int i = 0; i < currentSquad.Length; i += 1)
            {
                if (currentSquad[i] == null)
                {
                    currentSquad[i] = hero;
                    Debug.WriteLine("Added");
                    break;
                }
            }
        }
        public void RemoveHeroFromSquad(Hero hero)
        {
            for (int i = 0; i < currentSquad.Length; i += 1)
            {
                if (currentSquad[i] == hero)
                {
                    currentSquad[i] = null;
                    Debug.WriteLine("Removed");
                    break;
                }
            }
            //This was made by autocomplete. However it is just perfect,
            //it just moves the rest of the heroes to the left when it encounters a null value
            for (int i = 0; i < currentSquad.Length; i += 1)
            {
                if (currentSquad[i] == null)
                {
                    for (int j = i; j < currentSquad.Length - 1; j += 1)
                    {
                        currentSquad[j] = currentSquad[j + 1];
                    }
                    break;
                }
            }
        }
        public void CreateSquad(string squadName)
        {
            Hero[] newSquad = new Hero[MAX_SQUAD_LENGTH];
            for (int i = 0; i < MAX_SQUAD_LENGTH; i += 1)
            {
                newSquad[i] = currentSquad[i];
            }
            while (savedSquads.ContainsKey(squadName))
                {
                squadName += " copia";
                }
           
            savedSquads.Add(squadName, newSquad);
            SetCurrentSquad(squadName);
            Debug.WriteLine("Squad created " + squadName);
        }
        public void DeleteSquad(string squadName)
        {
            if (savedSquads.ContainsKey(squadName))
            {
                Debug.WriteLine("Squad successfully deleted : " + squadName);
                savedSquads.Remove(squadName);

            }
            else
            {
                Debug.WriteLine("Squad not found to delete?");
            }
        }
        public void SetCurrentSquad(string squadName)
        {
            for (int i = 0; i < savedSquads.Count; i += 1)
            {
                if (savedSquads.ElementAt(i).Key == squadName)
                {
                    currentSquad = savedSquads.ElementAt(i).Value;
                    break;
                }
            }
        }
        public int GetSquadCount()
        {
            return savedSquads.Count;
        }

        public string GetFactionName()
        {
            return "@" + color + "@" + factionName +"@";
        }
        public int GetColor()
        {
            return color;
        }
        public int GetHeroesCount()
        {
            int count = 0;

            for(int i = 0; i < heroes.Length; i += 1)
            {
                if (heroes[i] != null)
                {
                    count += 1;
                }
            }
            Debug.WriteLine("Count of heroes: " + count);
            return count;
        }
        public Hero[] GetHeroes()
        {
            return heroes;
        }
        public DateTime GetStartDate()
        {
            return startDate;
        }
        public DateTime GetTimePlayed()
        {
            return hoursPlayed;
        }

        public Item[] GetAllItems()
        {
            return allItems;
        }
        public Race[] GetAllRaces()
        {
            return allRaces;
        }
        public Limb[] GetAllLimbs()
        {
            return allLimbs;
        }
        public Faction[] GetAllFactions()
        {
            return allFactions;
        }
        public Region[] GetAllRegions()
        {
            return allRegions;
        }
        public Monster[] GetAllMonsters()
        {
            return allMonsters;
        }
        public string[] GetTitles()
        {
            return titles;
        }
        public string[] GetBackgrounds()
        {
            return backgrounds;
        }
        public string[] GetNames()
        {
            return names;
        }
        
    }
}
