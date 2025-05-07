namespace Kenshi_DnD
{
    internal class HeroInventory : Inventory
    {
        public HeroInventory() : base()
        {
        }

        public string MakeAllItemsDisponible()
        {
            string itemNames = "";
            for (int i = 0; i < items.Count; i+=1)
            {
                itemNames += items[i].GetName() + (i == items.Count - 1 ? "" : ", ");
                items[i].UnUse();

            }

            return itemNames;
        }
        public StatModifier GetAllStats()
        {
            Item[] meleeItems = base.GetMelee(2);
            Item[] rangedItems = base.GetRanged(2);

            Item[] allNonConsumableItems = new Item[meleeItems.Length + rangedItems.Length];

            StatModifier allStats;
            int bruteForce = 0;
            int dexterity = 0;
            int hp = 0;
            int resistance = 0;
            int agility = 0;

            for (int i = 0; i < allNonConsumableItems.Length; i+=1)
            {
                bruteForce += allNonConsumableItems[i].GetStatToModify().GetBruteForce();
                dexterity += allNonConsumableItems[i].GetStatToModify().GetDexterity();
                hp += allNonConsumableItems[i].GetStatToModify().GetHp();
                resistance += allNonConsumableItems[i].GetStatToModify().GetResistance();
                agility += allNonConsumableItems[i].GetStatToModify().GetAgility();
            }
            allStats = new StatModifier(bruteForce, dexterity, hp, resistance, agility);
            return allStats;
        }
        // 1 = Brute Force, 2 = Dexterity, 3 = HP, 4 = Resistance, 5 = Agility
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
