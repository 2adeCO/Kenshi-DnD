namespace Kenshi_DnD
{
    // Inventory of a hero, used for combat
    [Serializable]
    public class HeroInventory : Inventory
    {
        // Constructor
        public HeroInventory() : base()
        {
        }
        // Removes all items from hero's inventory
        public string MakeAllItemsDisponible(Hero hero)
        {
            string itemNames = "";
            for (int i = 0; i < items.Count; i+=1)
            {
                itemNames += items[i].GetName() + (i == items.Count - 1 ? "" : ", ");
                items[i].SetAlreadyUsed(false);
            }
            items.RemoveRange(0,items.Count);

            return itemNames;
        }
        // Gets the stats of the items in the inventory
        public int GetStat(Stats.Stat opt)
        {
            int stat = 0;
            switch (opt)
            {
                case Stats.Stat.BruteForce:
                    for (int i = 0; i < items.Count; i+=1)
                    {
                        stat += items[i].GetStatToModify().GetBruteForce();
                    }
                    break;
                case Stats.Stat.Dexterity:
                    for (int i = 0; i < items.Count; i+=1)
                    {
                        stat += items[i].GetStatToModify().GetDexterity();
                    }
                    break;
                case Stats.Stat.HP:
                    for (int i = 0; i < items.Count; i+=1)
                    {
                        stat += items[i].GetStatToModify().GetHp();
                    }
                    break;
                case Stats.Stat.Resistance:
                    for (int i = 0; i < items.Count; i+=1)
                    {
                        stat += items[i].GetStatToModify().GetResistance();
                    }
                    break;
                case Stats.Stat.Agility:
                    for (int i = 0; i < items.Count; i+=1)
                    {
                        stat += items[i].GetStatToModify().GetAgility();
                    }
                    break;
            }
            return stat;
        }
        // Returns if said items are in the inventory
        public bool AreRangedItems()
        {
            for (int i = 0; i < items.Count; i+=1)
            {
                if (items[i] is RangedItem)
                {
                    return true;
                }
            }
            return false;
        }
        public bool AreMeleeItems()
        {
            for (int i = 0; i < items.Count; i+=1)
            {
                if (items[i] is MeleeItem)
                {
                    MeleeItem item = (MeleeItem)items[i];
                    if (!item.BreaksOnUse())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool AreConsumableItems()
        {
            for (int i = 0; i < items.Count; i+=1)
            {
                if (items[i] is MeleeItem)
                {
                    MeleeItem item = (MeleeItem)items[i];
                    if (item.BreaksOnUse())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
