namespace Kenshi_DnD
{
    [Serializable]
    public class Limb : StatModifier
    {
        string name;
        bool beingUsed;
        public Limb(string name, int bruteForce, int dexterity, int toughness, int hp, int resistance, int agility)
            : base(bruteForce, dexterity, hp, resistance, agility)
        {
            this.name = name;
            this.beingUsed = false;
        }
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
    }
}