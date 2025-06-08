namespace Kenshi_DnD
{
    // Class that represents a limb of a hero, which modifies their stats and is used for holding items. - Santiago Cabrero
    [Serializable]
    public class Limb : StatModifier
    {
        // Name of limb
        string name;
        // Value of limb when bought
        int value;
        // Rarity of limb, used to upgrade base stats
        Rarity.Rarities rarity;
        // Tells if the item is being used
        bool beingUsed;
        // Constructor
        public Limb(string name, int value, int bruteForce, int dexterity, int hp, int resistance, int agility)
            : base(bruteForce, dexterity, hp, resistance, agility)
        {
            this.name = name;
            this.value = value;
            this.beingUsed = false;
        }
        // Getters and setters
        public string GetName()
        {
            return name;
        }
        public bool GetBeingUsed()
        {
            return beingUsed;
        }
        public void SetBeingUsed(bool isUsed)
        {
            beingUsed = isUsed;
        }
        public int GetValue()
        {
            return value;
        }
        public void SetRarity(Rarity.Rarities rarity)
        {
            this.rarity = rarity;
            double costMultiplier = 1.0;
            int buff = 0;

            switch (rarity)
            {
                case Rarity.Rarities.Junk:
                    {
                        costMultiplier = 1.0;
                        buff = 1;
                        break;
                    }
                case Rarity.Rarities.RustCovered:
                    {
                        costMultiplier = 1.10;
                        buff = 2;
                        break;
                    }
                case Rarity.Rarities.Catun:
                    {
                        costMultiplier = 1.25;
                        buff = 3;
                        break;
                    }
                case Rarity.Rarities.Mk:
                    {
                        costMultiplier = 1.50;
                        buff = 4;
                        break;
                    }
                case Rarity.Rarities.Edgewalker:
                    {
                        costMultiplier = 1.75;
                        buff = 5;
                        break;
                    }
                case Rarity.Rarities.Meitou:
                    {
                        costMultiplier = 2.0;
                        buff = 6;
                        break;
                    }
            }
            UpgradeStat(Stats.Stat.HP, buff);

            value = (int)(value * costMultiplier);
        }

        public override Limb GetCopy()
        {
            int bruteForce = GetBruteForce();
            int dexterity = GetDexterity();
            int hp = GetHp();
            int resistance = GetResistance();
            int agility = GetAgility();
            return new Limb(name, value, bruteForce, dexterity, hp, resistance, agility);

        }
        // Returns a string that represents the rarity
        public string RarityToString()
        {
            //Kenshi's actual weapon tiers go like this:
            //Rusted Junk, Rusting Blade, Mid-Grade Salvage, Old Re-Fitted Blade, Re-fitted Blade,
            //Catun No. 1, Catun No. 2, Catun No. 3,
            //Mk I, Mk II, Mk III,
            //Edge Type 1, Edge Type 2, Edge Type 3, Meitou
            //For obvious reasons I kept it simpler.
            //Also, in Kenshi anything below Edge is garbage, I won't make it so harsh in this game haha
            switch (rarity)
            {
                case Rarity.Rarities.Junk:
                    {
                        return "@8@Chatarra@";
                    }
                case Rarity.Rarities.RustCovered:
                    {
                        return "@7@Llena de óxido@";
                    }
                case Rarity.Rarities.Catun:
                    {
                        return "@6@Catun No. 1@";
                    }
                case Rarity.Rarities.Mk:
                    {
                        return "@2@Mk I@";
                    }
                case Rarity.Rarities.Edgewalker:
                    {
                        return "@3@Edgewalker I@";
                    }
                case Rarity.Rarities.Meitou:
                    {
                        return "@6@Meitu@";
                    }
                default:
                    {
                        return "Rareza no puesta";
                    }

            }
        }
        // Returns an int that represents a color in MainWindow.cs GetBrushByNum
        public int GetRarityColor()
        {
            switch (rarity)
            {
                case Rarity.Rarities.Junk:
                    {
                        return 8;
                    }
                case Rarity.Rarities.RustCovered:
                    {
                        return 7;
                    }
                case Rarity.Rarities.Catun:
                    {
                        return 6;
                    }
                case Rarity.Rarities.Mk:
                    {
                        return 2;
                    }
                case Rarity.Rarities.Edgewalker:
                    {
                        return 3;
                    }
                case Rarity.Rarities.Meitou:
                    {
                        return 6;
                    }
                default:
                    {
                        return 9;
                    }
            }
        }
    }
}