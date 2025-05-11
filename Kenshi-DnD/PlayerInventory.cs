namespace Kenshi_DnD
{
    [Serializable]
    public class PlayerInventory : Inventory
    {
        public PlayerInventory()
        {
            base.items = new List<Item>();
        }
    }
}
