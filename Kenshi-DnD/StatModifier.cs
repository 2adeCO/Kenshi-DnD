using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    public class StatModifier
    {
        //All of these are buffs and debuffs to the ints of the hero
        //Offensive ints
        //Brute Force is associated with Sheks, skeletons and Hive Soldiers
        //Dexterity is associated with Humans(All types), Hive Princes and Hive Workers 
        int bruteForce;
        int dexterity;

        //Defensive ints
        //Health points
        //Toughness is max health points
        int hp;

        //Resistance is defense against Brute Force
        int resistance;

        //Agility determines how fast the hero interacts with the enemies
        int agility;


        public StatModifier(int bruteForce, int dexterity, int hp, int resistance, int agility)
        {
            this.bruteForce = bruteForce;
            this.dexterity = dexterity;

            this.hp = hp;
            this.resistance = resistance;
            this.agility = agility;
        }
        public int GetBruteForce()
        {
            return bruteForce;
        }
        public int GetDexterity()
        {
            return dexterity;
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
        public void SetDexterity(int dexterity)
        {
            this.dexterity = dexterity;
        }

        public void SetHp(int hp)
        {
            this.hp = hp;
        }
        public void SetResistance(int resistance)
        {
            this.resistance = resistance;
        }
        public override string ToString()
        {
            return 
                (hp != 0 ? "Puntos de vida: " + hp + "\n" : "") +
                (bruteForce != 0 ? "Fuerza bruta: " + bruteForce + "\n" : "") +
                (dexterity != 0 ? "Destreza: " + dexterity + "\n" : "") +
                (resistance != 0 ? "Resistencia: " + resistance + "\n" : "") +
                (agility != 0 ? "Agilidad: " + agility : "");
        }
    }
}
