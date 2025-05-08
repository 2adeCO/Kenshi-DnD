namespace Kenshi_DnD
{
    public class PlayerInventory : Inventory
    {
        public PlayerInventory()
        {
            base.items = new List<Item>();
        }
    }
}
