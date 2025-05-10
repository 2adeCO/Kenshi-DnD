using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        string id;
        const int MINIMUM_NUMBER_LENGTH = 9;
        int cats;
        DateTime startDate;
        DateTime hoursPlayed;

        Dice myDice;

        Hero[] heroes;
        Hero[] currentSquad;
        Hero[][] savedSquads;

        PlayerInventory playerInventory;

        List<Item> alreadyObtainedItems;
        
        public Adventure(string name, Hero hero, Random rnd, Dice myDice)
        {
            this.id = GenerateId(name, rnd);
            this.cats = 200;
            this.myDice = myDice;
            startDate = DateTime.Now;
            hoursPlayed = DateTime.Now;

            heroes = new Hero[12];
            heroes[0] = hero;

            savedSquads = new Hero[10][];

            savedSquads[0] = new Hero[4];
            savedSquads[0][0] = hero;
            currentSquad = savedSquads[0];

            playerInventory = new PlayerInventory();
            alreadyObtainedItems = new List<Item>();
            Debug.WriteLine(id);
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
        public DateTime GetStartDate()
        {
            return startDate;
        }
        public DateTime GetTimePlayed()
        {
            return hoursPlayed;
        }
    }
}
