namespace Kenshi_DnD
{
    [Serializable]
    public class Race : StatModifier
    {
        string name;
        public Race(string name, int bruteForce, int dexterity, int toughness, int resistance, int agility)
            : base(bruteForce, dexterity, toughness, resistance, agility)
        {
            this.name = name;
        }
        public string GetName()
        {
            return name;
        }
        public void SetName(string name)
        {
            this.name = name;
        }
        
        
    }
}
