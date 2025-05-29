namespace Kenshi_DnD
{
    [Serializable]
    public class PlayerInventory : Inventory
    {
        public PlayerInventory()
        {
            base.items = new List<Item>();
        }
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
