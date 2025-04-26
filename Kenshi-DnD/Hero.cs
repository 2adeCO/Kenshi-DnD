using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class Hero : ITurnable
    {
        //Name and title, example: "Carlos, the database scholar"
        string name;
        string title;

        //Offensive ints
        //Brute Force is associated with Sheks, skeletons and Hive Soldiers
        //Dexterity is associated with Humans(All types), Hive Princes and Hive Workers 
        Inventory personalInventory;
        StatModifier heroStats;
        //Defensive ints
        //Health points
        //Toughness is max health points
        int toughness;
        //Resistance is defense against Brute Force
        //Perception is defense against Dexterity
        //Agility determines how fast the hero interacts with the enemies
        //Race is a key component in the world building of this game. The races are: Human, Shek, Skeleton and Hive
        Race race;
        //Race specifies the exact type of race that the character is. The subraces are:
        //Human: Greenlander, scorchlander
        //Hive: Hive Prince, Hive Worker, Hive Soldier
        //The other races don't have subrace
        Race subrace;
        //Inventory

        //Background story is a semi randomly asigned tale to give the hero a bit of depth. Depending on their circunstances, these will vary.
        //For example: If a Shek is a rebel, their background could tell how they rejected Esata the Stone Golem(Shek Kingdom's Queen) for her untraditionalist ways.
        //If a human is a merchant noble, their background could tell how they inherited their father's slave farm.
        string backgroundStory;

        //Recruitment date is the date when the hero was recruited.
        DateTime recruitmentDate;
        //Level determines roughly how strong the hero is. However, this is not a direct correlation, as the hero's ints are built upon leveling up.
        int level;
        //Experience is the amount of experience the hero has. It determines how much the hero has leveled up.
        int experience;
        //Limbs can be lost and bought in the kenshi universe.
        Limb[] limbs;
        public Hero(string name, string title, int bruteForce, int dexterity, int toughness, int hp, 
            int resistance, int agility, string backgroundStory, int level, int experience, Race race, Race subrace, Limb[] limbs)
        {
            heroStats = new StatModifier(bruteForce, dexterity, resistance, hp, agility);
            this.name = name;
            this.title = title;
            this.toughness = toughness;
            this.backgroundStory = backgroundStory;
            this.recruitmentDate = DateTime.Now;
            this.level = level;
            this.experience = experience;
            this.race = race;
            this.subrace = subrace;
            this.limbs = limbs;
            personalInventory = new Inventory();
        }
        public Hero(string name, string title, int bruteForce, int dexterity, int toughness,
            int resistance, int agility,Race race, Race subrace, Limb[] limbs)
        {
            this.name = name;
            this.title = title;
            heroStats = new StatModifier(bruteForce, dexterity, resistance, toughness, agility);
            this.toughness = toughness;

            this.level = 1;
            this.backgroundStory = "";
            this.recruitmentDate = DateTime.Now;
            this.race = race;
            this.subrace = subrace;
            this.limbs = limbs;
            personalInventory = new Inventory();
        }
        
        public string GetName()
        {
            return name;
        }
        public string GetTitle()
        {
            return title;
        }
        public int GetBruteForce()
        {
            return heroStats.GetBruteForce();
        }
        public int GetDexterity()
        {
            return heroStats.GetDexterity();
        }
        public int GetToughness()
        {
            return toughness;
        }
        public int GetHp()
        {
            return heroStats.GetHp();
        }
        public bool IsAlive()
        {
            return (this.heroStats.GetHp() > 0);
        }
        public int GetResistance()
        {
            return heroStats.GetResistance();
        }
        public int GetAgility()
        {
            return heroStats.GetAgility();
        }
        public string GetBackgroundStory()
        {
            return backgroundStory;
        }
        public DateTime GetRecruitmentDate()
        {
            return recruitmentDate;
        }
        public int GetLevel()
        {
            return level;
        }
        public int GetExperience()
        {
            return experience;
        }
        public void SetName(string name)
        {
            this.name = name;
        }
        public void SetTitle(string title)
        {
            this.title = title;
        }
        public void SetToughness(int toughness)
        {
            this.toughness = toughness;
        }
        public void SetHp(int hp)
        {
            this.heroStats.SetHp(hp);
        }
        public void SetBackgroundStory(string backgroundStory)
        {
            this.backgroundStory = backgroundStory;
        }
        public void SetRecruitmentDate(DateTime recruitmentDate)
        {
            this.recruitmentDate = recruitmentDate;
        }
        public void SetLevel(int level)
        {
            this.level = level;
        }
        public void SetExperience(int experience)
        {
            this.experience = experience;
        }
        public Limb[] GetLimbs()
        {
            return limbs;
        }
        public Inventory GetInventory()
        {
            return personalInventory;
        }
        public void AddItemToInventory(Item item)
        {
            if (item.CanUseItem(this))
            {
                personalInventory.AddItem(item);
            }
        }
        public bool IsInInventory(Item item)
        {
            return personalInventory.ContainsItem(item);
        }
        public void RemoveItemFromInventory(Item item)
        {
            personalInventory.RemoveItem(item);
        }
        public void PutLimb(Limb newLimb)
        {
            for(int i = 0; i < limbs.Length; i++)
            {
                if (limbs[i] == null)
                {
                    limbs[i] = newLimb;
                    break;
                }
            }
        }

        public override string ToString()
        {
            return $"{name}, {title} - ";
        }
    }
}
