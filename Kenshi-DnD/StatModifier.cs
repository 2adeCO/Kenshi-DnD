using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    abstract class StatModifier
    {
        //All of these are buffs and debuffs to the stats of the hero
        //Offensive stats
        //Brute Force is associated with Sheks, skeletons and Hive Soldiers
        //Skill is associated with Humans(All types), Hive Princes and Hive Workers 
        int bruteForce;
        int skill;

        //Defensive stats
        //Health points
        //Toughness is max health points
        int toughness;
        int hp;

        //Resistance is defense against Brute Force
        //Perception is defense against Skill
        int resistance;

        //Agility determines how fast the hero interacts with the enemies
        int agility;

        public int GetBruteForce()
        {
            return bruteForce;
        }
        public int GetSkill()
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
        public int GetResistance()
        {
            return resistance;
        }

        public int GetAgility()
        {
            return agility;
        }
        public void SetBruteForce(int bruteForce)
        {
            this.bruteForce = bruteForce;
        }
        public void SetSkill(int skill)
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
        public void SetResistance(int resistance)
        {
            this.resistance = resistance;
        }
        public void SetAgility(int agility)
        {
            this.agility = agility;
        }
        public StatModifier(int bruteForce, int skill, int toughness, int hp, int resistance, int agility)
        {
            this.bruteForce = bruteForce;
            this.skill = skill;
            this.toughness = toughness;
            this.hp = hp;
            this.resistance = resistance;
            this.agility = agility;
        }
    }
}
