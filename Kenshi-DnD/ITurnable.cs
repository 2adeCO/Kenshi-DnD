namespace Kenshi_DnD
{
    // Interface that requires combat fighters to have these methods for turn management
    interface ITurnable
    {
        // Getters
        public int GetAgility();
        public bool IsAlive();
        public string GetName();
    }
}
