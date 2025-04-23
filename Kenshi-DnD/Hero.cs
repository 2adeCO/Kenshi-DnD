using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class Hero
    {
        //Name and title, example: "Carlos, the database scholar"
        string name;
        string title;

        //Offensive stats
        //Brute Force is associated with Sheks, skeletons and Hive Soldiers
        //Skill is associated with Humans(All types), Hive Princes and Hive Workers 
        Stat bruteForce;
        Stat skill;

        //Defensive stats
        //Health points
        //Toughness is max health points
        int toughness;
        int hp;
        //Resistance is defense against Brute Force
        //Perception is defense against Skill
        Stat resistance;
        //Agility determines how fast the hero interacts with the enemies
        Stat agility;
        //Race is a key component in the world building of this game. The races are: Human, Shek, Skeleton and Hive
        Race race;
        //Subrace specifies the exact type of race that the character is. The subraces are:
        //Human: Greenlander, scorchlander
        //Hive: Hive Prince, Hive Worker, Hive Soldier
        //The other races don't have subrace
        Subrace subrace;
        //Inventory

        //Background story is a semi randomly asigned tale to give the hero a bit of depth. Depending on their circunstances, these will vary.
        //For example: If a Shek is a rebel, their background could tell how they rejected Esata the Stone Golem(Shek Kingdom's Queen) for her untraditionalist ways.
        //If a human is a merchant noble, their background could tell how they inherited their father's slave farm.
        string backgroundStory;

        //Recruitment date is the date when the hero was recruited.
        DateTime recruitmentDate;
        //Level determines roughly how strong the hero is. However, this is not a direct correlation, as the hero's stats are built upon leveling up.
        int level;
        //Experience is the amount of experience the hero has. It determines how much the hero has leveled up.
        int experience;
        //Limbs can be lost and bought in the kenshi universe.
        Limb[] limbs;
        public Hero(string name, string title, Stat bruteForce, Stat skill, int toughness, int hp, 
            Stat resistance, Stat agility, string backgroundStory, int level, int experience, Race race, Subrace subrace, Limb[] limbs)
        {
            this.name = name;
            this.title = title;
            this.bruteForce = bruteForce;
            this.skill = skill;
            this.toughness = toughness;
            this.hp = hp;
            this.resistance = resistance;
            this.agility = agility;
            this.backgroundStory = backgroundStory;
            this.recruitmentDate = DateTime.Now;
            this.level = level;
            this.experience = experience;
            this.race = race;
            this.subrace = subrace;
            this.limbs = limbs;
        }
        public Hero(string name, string title, Stat bruteForce, Stat skill, int toughness,
            Stat resistance, Stat agility,Race race, Subrace subrace, Limb[] limbs)
        {
            this.name = name;
            this.title = title;
            this.bruteForce = bruteForce;
            this.skill = skill;
            this.toughness = toughness;
            this.hp = toughness;
            this.resistance = resistance;
            this.agility = agility;
            this.level = 1;
            this.backgroundStory = "";
            this.recruitmentDate = DateTime.Now;
            this.race = race;
            this.subrace = subrace;
            this.limbs = limbs;
        }
        
        public string GetName()
        {
            return name;
        }
        public string GetTitle()
        {
            return title;
        }
        public Stat GetBruteForce()
        {
            return bruteForce;
        }
        public Stat GetSkill()
        {
            return skill;
        }
        public int GetToughness()
        {
            return toughness;
        }
        public int GetHp()
        {
            return hp;
        }
        public Stat GetResistance()
        {
            return resistance;
        }
        public Stat GetAgility()
        {
            return agility;
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
        public void SetBruteForce(Stat bruteForce)
        {
            this.bruteForce = bruteForce;
        }
        public void SetSkill(Stat skill)
        {
            this.skill = skill;
        }
        public void SetToughness(int toughness)
        {
            this.toughness = toughness;
        }
        public void SetHp(int hp)
        {
            this.hp = hp;
        }
        public void SetResistance(Stat resistance)
        {
            this.resistance = resistance;
        }
        public void SetAgility(Stat agility)
        {
            this.agility = agility;
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
            return $"{name}, {title} - " +
                $"{bruteForce.ToString()}, {skill.ToString}, {toughness.ToString()}, HP: {hp}," +
                $" {resistance.ToString()}, {agility.ToString()}, Nivel: {level}";
        }
    }
}
