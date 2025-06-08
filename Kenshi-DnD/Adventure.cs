using System.Globalization;

namespace Kenshi_DnD
{
    // Class that saves the player's progress. - Santiago Cabrero
    [Serializable]
    public class Adventure
    {
        // This Dictionary saves the squads and their names
        public Dictionary<string, Hero[]> savedSquads;
        // The adventure's id is a string that is generated when the adventure is created
        string id;
        // The faction name is the player's faction name
        string factionName;
        // Makes the ID put zeroes at the beginning, so it has a fixed length
        const int MINIMUM_NUMBER_LENGTH = 9;
        // The player's money, in cats(they are not actual cats, they are named after Second Empire's exiled ruler Cat-Lon)
        int cats;
        // Is used by the UI to display some things a particular color, see the MainWindow.xaml.cs method GetBrushByNum() for exact color codes
        int color;
        // The date when the adventure started
        DateTime startDate;
        // The time played
        TimeSpan hoursPlayed;
        // The custom dice used in the adventure, used to play combat
        Dice myDice;

        // All of the heroes gotten by the player in the adventure
        Hero[] heroes;
        // The current squad is the one that fights when the player is attacked or attacks
        Hero[] currentSquad;
        // The name of the current squad, tried to use savedSquads.Keys, but at the end, it was easier to just use a string to avoid complications
        string currentSquadName;
        // The current region the player is in, used to put the player in the correct zone when they exit combat
        Region currentRegion;
        // The player's inventory, will hold all of the items the player has obtained
        PlayerInventory playerInventory;
        // Keeps track of the rare items the player has already obtained, so they don't appear again in Contraband shop
        Item[] alreadyObtainedItems;

        // All of the factions, regions, titles, backgrounds... used in the game, so they can be accessed from the adventure 
        Faction[] allFactions;
        Region[] allRegions;
        string[] titles;
        string[] backgrounds;
        string[] names;
        Item[] allItems;
        Race[] allRaces;
        Limb[] allLimbs;

        // The maximum number of heroes, squads and squad length
        const int MAX_HEROES = 12;
        const int MAX_SQUADS = 10;
        const int MAX_SQUAD_LENGTH = 4;
        // The only constructor, it initializes the adventure with the given parameters
        public Adventure(string name, Hero hero, Random rnd, Dice myDice, int startingCats, string factionName, int factionColor, Faction[] allFactions, Region[] allRegions,
            string[] titles, string[] backgrounds, string[] names, Item[] allItems, Race[] allRaces, Limb[] allLimbs)
        {
            this.id = GenerateId(name, rnd);
            this.cats = startingCats;
            this.myDice = myDice;
            startDate = DateTime.Now;
            hoursPlayed = new TimeSpan(0, 0, 0);
            this.factionName = factionName;
            this.color = factionColor;
            heroes = new Hero[MAX_HEROES];
            heroes[0] = hero;
            savedSquads = new Dictionary<string, Hero[]> { };
            //Adds the first squad, which is the one that is created when the game starts
            savedSquads.Add("Mi primera squad", new Hero[MAX_SQUAD_LENGTH]);
            savedSquads["Mi primera squad"][0] = hero;
            currentSquad = savedSquads["Mi primera squad"];
            currentSquadName = "Mi primera squad";
            playerInventory = new PlayerInventory();
            alreadyObtainedItems = new Item[allItems.Length * 2];
            this.allFactions = allFactions;
            this.allRegions = allRegions;
            this.titles = titles;
            this.backgrounds = backgrounds;
            this.names = names;
            this.allItems = allItems;
            this.allRaces = allRaces;
            this.allLimbs = allLimbs;
        }
        // Generates a unique ID for the adventure based on the name, a random number, the region and some formatting
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

            //Formats the name, replacing spaces with underscores
            string formattedName = "";
            for (int i = 0; i < name.Length; i += 1)
            {
                if (char.IsWhiteSpace(name[i]))
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
            }
            ;
            // Calculates the zeroes remaining
            numberCount = MINIMUM_NUMBER_LENGTH - numberCount;

            //Adds the remaining zeroes for aesthetic purpose
            for (int i = 0; i < numberCount; i += 1)
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
            return formattedName + continent + zeroes + randomNum;
        }
        // Getters and setters for the adventure's properties
        public Region GetCurrentRegion()
        {
            return currentRegion;
        }
        public void SetCurrentRegion(Region region)
        {
            currentRegion = region;
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
        // Used for the player's buying needs, if they have enough money, it will spend the money and return true, otherwise it will return false
        public bool SpendIfHasEnough(int cost)
        {
            if (cost <= cats && cost >= 0)
            {
                this.cats -= cost;
                return true;
            }
            return false;
        }
        // Removes an item from the player's inventory and adds its resell value to the player's money
        public void SellItem(Item item)
        {
            int cost = item.GetResellValue();
            cats += cost;

            playerInventory.RemoveItem(item);
        }
        // Buys an item, if the player has enough money, it will add the item to the player's inventory and add it to the alreadyObtainedItems array if it is a rare item
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
                playerInventory.AddItem(item);

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
                playerInventory.AddItem(item);
            }
        }
        // Hires a hero if the limits allow the player to do so
        public void HireHero(Hero hero)
        {

            if ( GetHeroesCount() >= MAX_HEROES || hero.IsHired())
            {
                return;
            }

            for (int i = 0; i < heroes.Length; i++)
            {
                if (heroes[i] == null)
                {
                    hero.Hire();
                    heroes[i] = hero;
                    break;
                }
            }
        }
        // Unhiring a hero will remove it from the heroes array and from all squads, also giving the player its competency cost back in cats
        // I am aware that I probably should have used a List<Hero> instead of an array, but I didn't contemplate it at the start of the project
        // It just works fine for this project
        public void UnhireHero(Hero hero)
        {
            for (int i = 0; i < heroes.Length; i += 1)
            {
                if (heroes[i] != null)
                {
                    if (heroes[i] == hero)
                    {
                        heroes[i] = null;
                        break;
                    }
                }
            }
            // Compacts the heroes array
            int insertIndex = 0;
            Hero[] compactedHeroes = new Hero[heroes.Length];
            for (int i = 0; i < heroes.Length; i++)
            {
                if (heroes[i] != null)
                {
                    compactedHeroes[insertIndex] = heroes[i];
                    insertIndex++;
                }
            }
            heroes = compactedHeroes;
            List<string> keys = savedSquads.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {

                Hero[] squad = savedSquads[keys[i]];
                bool removed = false;
                // Removes the hero from all squads
                for (int j = 0; j < squad.Length; j++)
                {
                    if (squad[j] != null)
                    {
                        if (squad[j] == hero)
                        {
                            squad[j] = null;
                            removed = true;
                        }
                    }
                }
                // If removed, compacts the array
                if (removed)
                {
                    bool empty = true;
                    insertIndex = 0;
                    Hero[] compactedSquad = new Hero[squad.Length];
                    for (int j = 0; j < squad.Length; j += 1)
                    {
                        if (squad[j] != null)
                        {
                            compactedSquad[insertIndex] = squad[j];
                            insertIndex++;
                            empty = false;
                        }
                    }
                    if (empty)
                    {
                        if (savedSquads[keys[i]] == currentSquad)
                        {
                            if (i == 0) 
                            {
                                // If the squad that is empty is the current, and the first squad, we set the current squad to the next one
                                SetCurrentSquad(keys[i + 1]);
                            }
                            else
                            {
                                // If the squad that is empty is the current, but not the first one, we set the current squad to the first one
                                SetCurrentSquad(keys[0]);
                            }
                        }
                        
                        DeleteSquad(keys[i]);
                    }
                    else
                    {
                        savedSquads[keys[i]] = compactedSquad;
                    }
                }
                cats += hero.GetCompetencyCost();

            }
        }
        // Returns an array of heroes that are amputees, used to show the player which heroes need a limb replacement
        public Hero[] GetAmputees()
        {

            int count = 0;
            for (int i = 0; i < heroes.Length; i += 1)
            {
                if (heroes[i] != null)
                {
                    if (heroes[i].IsAmputee())
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
        // Gains a certain amount of cats, used when the player wins a fight or sells an item
        public void GainCats(int amount)
        {
            if (amount < 0)
            {
                return;
            }
            cats += amount;
        }
        // Returns the saved squads dictionary, for UI purposes
        public Dictionary<string, Hero[]> GetSavedSquads()
        {
            return savedSquads;
        }
        // Returns the current squad name, for UI purposes
        public string GetCurrentSquadName()
        {
            return currentSquadName;
        }
        // Returns the current squad without null values, which is the one that is fighting when the player is attacked or attacks
        public Hero[] GetCurrentSquad()
        {
            int count = 0;
            for (int i = 0; i < currentSquad.Length; i += 1)
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

            } while (count != heroesToReturn.Length);

            return heroesToReturn;
        }
        // Checks if a hero is in the current squad, used to avoid adding the same hero multiple times and UI purposes
        public bool IsInCurrentSquad(Hero hero)
        {
            bool found = false;
            for (int i = 0; i < currentSquad.Length; i += 1)
            {
                if (currentSquad[i] == hero)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }
        // Adds a hero to the current squad, if it is not already in the squad
        public void AddHeroInSquad(Hero hero)
        {
            if (IsInCurrentSquad(hero))
            {
                return;
            }
            for (int i = 0; i < currentSquad.Length; i += 1)
            {
                if (currentSquad[i] == null)
                {
                    currentSquad[i] = hero;
                    break;
                }
            }
        }
        // Removes a hero from the current squad, if it is in the squad, and compacts the squad array
        public void RemoveHeroFromSquad(Hero hero)
        {
            if (!IsInCurrentSquad(hero))
            {
                return;
            }
            for (int i = 0; i < currentSquad.Length; i += 1)
            {
                if (currentSquad[i] == hero)
                {
                    currentSquad[i] = null;
                    break;
                }
            }
            //This was made by autocomplete. However it is just perfect,
            //it just moves the rest of the heroes to the left when it encounters a null value
            int insertIndex = 0;
            Hero[] compactedSquad = new Hero[currentSquad.Length];
            for (int i = 0; i < currentSquad.Length; i++)
            {
                if (currentSquad[i] != null)
                {
                    compactedSquad[insertIndex] = currentSquad[i];
                    insertIndex++;
                }
            }
            string currentName = GetCurrentSquadName();
            currentSquad = compactedSquad;
            savedSquads[currentName] = currentSquad;

        }
        // Checks if the player can hire more heroes, used to avoid hiring more than the maximum allowed
        public bool CanHire()
        {
            return GetHeroesCount() < MAX_HEROES;
        }
        // Creates a new squad copy with the current squad's heroes, and adds it to the saved squads dictionary, also sets the current squad to the new squad
        public void CreateSquad(string squadName)
        {
            if(savedSquads.Count >= MAX_SQUADS)
            {
                return;
            }
            // Copies the current squad heroes to a new array
            Hero[] newSquad = new Hero[MAX_SQUAD_LENGTH];
            for (int i = 0; i < MAX_SQUAD_LENGTH; i += 1)
            {
                newSquad[i] = currentSquad[i];
            }
            // Adds a " copia" to the squad name if it already exists, to avoid overwriting existing squads
            while (savedSquads.ContainsKey(squadName))
            {
                squadName += " copia";
            }

            savedSquads.Add(squadName, newSquad);
            SetCurrentSquad(squadName);
        }
        // Deletes a squad from the saved squads dictionary, if it exists, and sets the current squad to the first squad if the deleted squad was the current one
        public void DeleteSquad(string squadName)
        {
            if (savedSquads.ContainsKey(squadName))
            {
                savedSquads.Remove(squadName);
            }
        }
        // Sets the current squad to the squad with the given name, if it exists, and updates the current squad name
        public void SetCurrentSquad(string squadName)
        {
            currentSquad = savedSquads[squadName];
            currentSquadName = squadName;
        }
        // Returns the number of squads saved in the adventure, used to avoid creating more squads than the maximum allowed
        public int GetSquadCount()
        {
            return savedSquads.Count;
        }
        // Returns the faction name with the color code, used to display the faction name in the UI
        public string GetFactionName()
        {
            return "@" + color + "@" + factionName + "@";
        }
        // Returns the faction color, used for UI purposes
        public int GetColor()
        {
            return color;
        }
        // Gets the number of heroes in the adventure
        public int GetHeroesCount()
        {
            int count = 0;

            for (int i = 0; i < heroes.Length; i += 1)
            {
                if (heroes[i] != null)
                {
                    count += 1;
                }
            }
            return count;
        }
        // Returns the heroes array, used to display the heroes in the UI
        public Hero[] GetHeroes()
        {
            return heroes;
        }
        // Returns some info about the adventure, used for UI purposes
        public DateTime GetStartDate()
        {
            return startDate;
        }
        public TimeSpan GetTimePlayed()
        {
            return hoursPlayed;
        }
        // Adds a second for every tick of the DispatcherTimer started in MainWindow.xaml.cs
        public void AddSecondToAdventure()
        {
            hoursPlayed += new TimeSpan(0, 0, 1);
        }
        // Returns data of the game
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
        //Gives a token to all regions, used when the player has a fight, regions update their items and heroes
        public void GainToken()
        {
            for(int i = 0; i < allRegions.Length; i += 1)
            {                
               allRegions[i].GainToken();
            }
        }
        

    }
}
