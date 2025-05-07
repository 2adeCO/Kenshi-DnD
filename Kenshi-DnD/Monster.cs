namespace Kenshi_DnD
{
    class Monster : ITurnable
    {
        string name;
        Faction faction;
        int resistance;
        int strength;
        int hp;
        int agility;
        int cats;
        int xpDrop;
        Item drop;
        Immunities.Immunity immunity; 
        public Monster(string name, int hp, Faction faction, int strength, int resistance, int agility, Immunities.Immunity immunity, int cats, int xpDrop, Item drop)
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
            this.drop = drop;
        }
        public string GetName()
        {
            return "@"+ faction.GetFactionColor() + "@" + name +"@";
        }
        
        public void SetName(string name)
        {
            this.name = name;
        }
        public Faction GetFaction()
        {
            return faction;
        }
        public void SetFaction(Faction faction)
        {
            this.faction = faction;
        }
        public bool IsAlive()
        {
            return (this.hp > 0);
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
        public Immunities.Immunity GetImmunity()
        {
            return immunity;
        }
        public Item GetItemDrop()
        {
            return drop;
        }
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

        public string GetImmunityDescription()
        {
            switch (immunity)
            {
                case Immunities.Immunity.ResistantToMelee: return "Resistencia a Fuerza Bruta";
                case Immunities.Immunity.ImmuneToMelee: return "Inmunidad a Fuerza Bruta";
                case Immunities.Immunity.ImmuneToMeleeAndResistantToRanged: return "Inmunidad a Fuerza Bruta y Resistencia a Destreza";
                case Immunities.Immunity.ResistantToRanged: return "Resistencia a Destreza";
                case Immunities.Immunity.ImmuneToRanged: return "Inmunidad a Destreza";
                case Immunities.Immunity.ImmunteToRangedAndResistantToMelee: return "Inmunidad a Destreza y Resistencia a Fuerza Bruta";
                case Immunities.Immunity.ResistantToBoth: return "Resistencia a Destreza y Fuerza Bruta";
                case Immunities.Immunity.None: return "Sin inmunidad";
                default: return "Error en inmunidad";
            }
        }

    }
}
