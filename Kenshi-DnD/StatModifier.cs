namespace Kenshi_DnD
{
    // Class that modifies the stats of the hero, be it their own stats, their items, race... - Santiago Cabrero
    [Serializable]
    public class StatModifier
    {
        //Offensive ints
        //Brute Force is skill used when using a melee item
        //Dexterity is skill used when using a ranged item
        int bruteForce;
        int dexterity;

        //Defensive ints
        //Health points
        int hp;
        // Defense against attacks
        int resistance;

        //Agility determines how fast the hero interacts with the enemies
        int agility;

        // Constructor
        public StatModifier(int bruteForce, int dexterity, int hp, int resistance, int agility)
        {
            this.bruteForce = bruteForce;
            this.dexterity = dexterity;
            this.hp = hp;
            this.resistance = resistance;
            this.agility = agility;
        }
        // Getters 
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
        // Upgrades a stat
        public void UpgradeStat(Stats.Stat stat, int upgrade)
        {
            switch (stat)
            {
                case Stats.Stat.BruteForce:
                    {
                        bruteForce += upgrade;
                        break;
                    }
                case Stats.Stat.Dexterity:
                    {
                        dexterity += upgrade;
                        break;
                    }
                case Stats.Stat.HP:
                    {
                        hp += upgrade;
                        break;
                    }
                case Stats.Stat.Resistance:
                    {
                        resistance += upgrade;
                        break;
                    }
                case Stats.Stat.Agility:
                    {
                        agility += upgrade;
                        break;
                    }
            }
        }
        // Returns a copy of the Stats
        public virtual StatModifier GetCopy()
        {
            return new StatModifier(bruteForce, dexterity, hp, resistance, agility);
        }
        // Overrides ToString to show the stats and their values
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
