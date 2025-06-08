namespace Kenshi_DnD
{
    // Class that represents different types of people in kenshi, such as humans, shek, skeletons or hives.
    // The class of a hero applies buff and/or debuffs to the stats of the hero. - Santiago Cabrero
    [Serializable]
    public class Race : StatModifier
    {
        // Name of the race
        string name;
        // Constructor
        public Race(string name, int bruteForce, int dexterity, int toughness, int resistance, int agility)
            : base(bruteForce, dexterity, toughness, resistance, agility)
        {
            this.name = name;
        }
        // Getter
        public string GetName()
        {
            return name;
        }
        
    }
}
