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
        //Value of 0 is no immunity
        //1 is resistance to brute force, 2 is immunity to brute force, 3 is immunity to brute force and resistance to dexterity
        //Value of -1 is resistance to dexterity, -2 is immunity to dexterity, -3 is immunity to dexterity and resistance to brute force
        int immunity;
        public Monster(string name, int hp, Faction faction, int strength, int resistance, int agility, int immunity, int cats, int xpDrop)
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
        }
        public string GetName()
        {
            return "@"+ faction.GetFactionColor() + name +"@";
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
        public int GetImmunity()
        {
            return immunity;
        }
        public void SetImmunity(int immunity)
        {
            this.immunity = immunity;
        }
        public override string ToString()
        {
            return
                (name != "" ? "Nombre: " + GetName() + "\n" : "") +
                (hp != 0 ? "Puntos de vida: " + hp + "\n" : "Cadáver\n") +
                "Fuerza bruta: " + strength + "\n" +
                "Resistencia: " + resistance + "\n" +
                "Agilidad: " + agility + "\n" +
                (cats != 0 ? "Cats: " + cats + "\n" : "Muerto de hambre \n") +
                "Experiencia: " + xpDrop + "\n" +
                "Inmunidad: " + GetImmunityDescription() + "\n";
        }

        private string GetImmunityDescription()
        {
            switch (immunity)
            {
                case 1: return "Resistencia a Fuerza Bruta";
                case 2: return "Inmunidad a Fuerza Bruta";
                case 3: return "Inmunidad a Fuerza Bruta y Resistencia a Destreza";
                case -1: return "Resistencia a Destreza";
                case -2: return "Inmunidad a Destreza";
                case -3: return "Inmunidad a Destreza y Resistencia a Fuerza Bruta";
                default: return "Sin inmunidad";
            }
        }

    }
}
