using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class Monster
    {
        string name;
        int resistance;
        int strength;
        int hp;
        int agility;
        int cats;
        int xpDrop;
        //Value of 0 is no immunity
        //1 is resistance to brute force, 2 is immunity to brute force, 3 is immunity to brute force and resistance to skill
        //Value of -1 is resistance to skill, -2 is immunity to skill, -3 is immunity to skill and resistance to brute force
        int immunity;
        public Monster(string name, int resistance, int strength, int hp, int agility, int cats, int xpDrop, int immunity)
        {
            this.name = name;
            this.resistance = resistance;
            this.strength = strength;
            this.hp = hp;
            this.agility = agility;
            this.cats = cats;
            this.xpDrop = xpDrop;
            this.immunity = immunity;
        }
        public string GetName()
        {
            return name;
        }
        public void SetName(string name)
        {
            this.name = name;
        }
        public int GetResistance()
        {
            return resistance;
        }
        public void SetResistance(int resistance)
        {
            this.resistance = resistance;
        }
        public int GetStrength()
        {
            return strength;
        }
        public void SetStrength(int strength)
        {
            this.strength = strength;
        }
        public int GetHp()
        {
            return hp;
        }
        public void SetHp(int hp)
        {
            this.hp = hp;
        }
        public int GetAgility()
        {
            return agility;
        }
        public void SetAgility(int agility)
        {
            this.agility = agility;
        }
        public int GetCats()
        {
            return cats;
        }
        public void SetCats(int cats)
        {
            this.cats = cats;
        }
        public int GetXpDrop()
        {
            return xpDrop;
        }
        public void SetXpDrop(int xpDrop)
        {
            this.xpDrop = xpDrop;
        }
        public int GetImmunity()
        {
            return immunity;
        }
        public void SetImmunity(int immunity)
        {
            this.immunity = immunity;
        }
    }
}
