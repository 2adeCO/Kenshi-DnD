using System.Diagnostics;

namespace Kenshi_DnD
{
    // Class that represents an enemy, with their faction, immunities, stats...
    [Serializable]
    public class Monster : ITurnable
    {
        // Name
        string name;
        // Faction
        Faction faction;
        // Stats
        int resistance;
        int strength;
        int hp;
        int agility;
        // Max cats that they can drop
        int cats;
        // Ammount of XP they give
        int xpDrop;
        // Possibility to drop an item
        bool canDropItem;
        // Immunity towards player's damage
        Immunities.Immunity immunity;

        // Constructor without immunity parsing
        public Monster(string name, int hp, Faction faction, int strength, int resistance, int agility, Immunities.Immunity immunity, int cats, int xpDrop, bool canDropItem)
        {
            this.name = name;
            this.hp = hp;
            this.faction = faction;
            this.strength = strength;
            this.resistance = resistance;
            this.agility = agility;
            this.immunity = immunity;
            this.cats = cats;
            this.xpDrop = xpDrop;
            this.canDropItem = canDropItem;
        }
        // Constructor with immunity parsing
        public Monster(string name, int hp, Faction faction, int strength, int resistance, int agility, string immunity, int cats, int xpDrop, bool canDropItem)
        {
            this.name = name;
            this.hp = hp;
            this.faction = faction;
            this.strength = strength;
            this.resistance = resistance;
            this.agility = agility;
            this.immunity = ImmunityParser(immunity);
            this.cats = cats;
            this.xpDrop = xpDrop;
            this.canDropItem = canDropItem;
        }
        // Parsing of immunity
        private Immunities.Immunity ImmunityParser(string immunity)
        {
            switch (immunity)
            {
                case "ResistantToMelee":
                    {
                        return Immunities.Immunity.ResistantToMelee;
                    }
                case "ImmuneToMelee":
                    {
                        return Immunities.Immunity.ImmuneToMelee;
                    }
                case "ImmuneToMeleeAndResistantToRanged":
                    {
                        return Immunities.Immunity.ImmuneToMeleeAndResistantToRanged;
                    }
                case "ResistantToRanged":
                    {
                        return Immunities.Immunity.ResistantToRanged;
                    }
                case "ImmuneToRanged":
                    {
                        return Immunities.Immunity.ImmuneToRanged;
                    }

                case "ImmuneToRangedAndResistantToMelee":
                    {
                        return Immunities.Immunity.ImmuneToRangedAndResistantToMelee;
                    }
                case "ResistantToBoth":
                    {
                        return Immunities.Immunity.ResistantToBoth;
                    }
                default:
                    { 
                    return Immunities.Immunity.None;
                    } 
            }
        }
        // Returns a copy of the monster
        public Monster GetCopy()
        {
            return new Monster(name, hp, faction, strength, resistance, agility, immunity, cats, xpDrop, canDropItem);
        }
        // Getters and setters
        public string GetName()
        {
            return "@"+ faction.GetFactionColor() + "@" + name +"@";
        }
        
        public Faction GetFaction()
        {
            return faction;
        }
        public bool CanDropItem()
        {
            return canDropItem;
        }
        public bool IsAlive()
        {
            return (this.hp > 0);
        }
        public int GetResistance()
        {
            return resistance;
        }
        public int GetStrength()
        {
            return strength;
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
        public int GetXpDrop()
        {
            return xpDrop;
        }
        public Immunities.Immunity GetImmunity()
        {
            return immunity;
        }
        // Returns an ammount of all possible cats
        public int GetPossibleCats(Random rnd)
        {
            float catsMultiplier = 1.0f;
            int possibility = rnd.Next(0, 20);
            switch (possibility)
            {
                case < 12:
                    {
                        catsMultiplier = 0.25f;
                        break;
                    }
                case < 16:
                    {
                        catsMultiplier = 0.5f;
                        break;
                    }
                case < 19:
                    {
                        catsMultiplier = 0.75f;
                        break;
                    }
                case < 20:
                    {
                        catsMultiplier = 1.0f;
                        break;
                    }

            }
            return (int)(cats * catsMultiplier);
        }
        // Info of the monster
        public override string ToString()
        {
            return

                (hp != 0 ? "Puntos de vida: " + hp + "\n" : "Cadáver\n") +
                "Fuerza bruta: " + strength + "\n" +
                "Resistencia: " + resistance + "\n" +
                "Agilidad: " + agility + "\n" +
                (cats != 0 ? "Cats: " + cats + "\n" : "Muerto de hambre \n") +
                "Experiencia: " + xpDrop + "\n" +
                "Inmunidad: " + GetImmunityDescription() + "\n";
        }
        // Returns a string representing the immunity
        public string GetImmunityDescription()
        {
            switch (immunity)
            {
                case Immunities.Immunity.ResistantToMelee: return "Resistencia a Fuerza Bruta";
                case Immunities.Immunity.ImmuneToMelee: return "Inmunidad a Fuerza Bruta";
                case Immunities.Immunity.ImmuneToMeleeAndResistantToRanged: return "Inmunidad a Fuerza Bruta y Resistencia a Destreza";
                case Immunities.Immunity.ResistantToRanged: return "Resistencia a Destreza";
                case Immunities.Immunity.ImmuneToRanged: return "Inmunidad a Destreza";
                case Immunities.Immunity.ImmuneToRangedAndResistantToMelee: return "Inmunidad a Destreza y Resistencia a Fuerza Bruta";
                case Immunities.Immunity.ResistantToBoth: return "Resistencia a Destreza y Fuerza Bruta";
                case Immunities.Immunity.None: return "Sin inmunidad";
                default: return "Error en inmunidad";
            }
        }

    }
}
