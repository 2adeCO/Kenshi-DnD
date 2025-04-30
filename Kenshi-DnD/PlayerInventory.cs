namespace Kenshi_DnD
{
    class PlayerInventory : Inventory
    {
        public PlayerInventory()
        {
            base.items = new List<Item>();
        }
    }
}
