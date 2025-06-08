namespace Kenshi_DnD
{
    // Class that saves all the player's items. - Santiago Cabrero
    [Serializable]
    public class PlayerInventory : Inventory
    {
        // Constructor
        public PlayerInventory()
        {
            base.items = new List<Item>();
        }
        // Fills the ammo of all the ranged items
        public void FillAmmo()
        {
            for (int i = 0; i< items.Count; i++)
            {
                if (items[i] is RangedItem)
                {
                    // If the item is a ranged item, fill its ammo
                    ((RangedItem)items[i]).FillAmmo();
                }
            }
        }
    }
}
