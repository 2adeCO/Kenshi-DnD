using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    internal class HeroInventory : Inventory
    {
        public HeroInventory()
        {
            base.items = new List<Item>();
        }

        public void AllItemsDisponible()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].IsUsed())
                {
                    items[i].UnUse();
                }
            }
            Debug.WriteLine("All items are available");
        }
        public StatModifier GetAllStats()
        {
            StatModifier allStats;
            int bruteForce = 0;
            int dexterity = 0;
            int hp = 0;
            int resistance = 0;
            int agility = 0;

            for (int i = 0; i < items.Count; i++)
            {
                bruteForce += items[i].GetStatToModify().GetBruteForce();
                dexterity += items[i].GetStatToModify().GetDexterity();
                hp += items[i].GetStatToModify().GetHp();
                resistance += items[i].GetStatToModify().GetResistance();
                agility += items[i].GetStatToModify().GetAgility();
            }
            allStats = new StatModifier(bruteForce, dexterity, hp, resistance, agility);
            return allStats;
        }
        // 1 = Brute Force, 2 = Dexterity, 3 = HP, 4 = Resistance, 5 = Agility
        public int GetStat(int option)
        {
            int stat = 0;
            switch (option)
            {
                case 1:
                    for (int i = 0; i < items.Count; i++)
                    {
                        stat += items[i].GetStatToModify().GetBruteForce();
                    }
                    break;
                case 2:
                    for (int i = 0; i < items.Count; i++)
                    {
                        stat += items[i].GetStatToModify().GetDexterity();
                    }
                    break;
                case 3:
                    for (int i = 0; i < items.Count; i++)
                    {
                        stat += items[i].GetStatToModify().GetHp();
                    }
                    break;
                case 4:
                    for (int i = 0; i < items.Count; i++)
                    {
                        stat += items[i].GetStatToModify().GetResistance();
                    }
                    break;
                case 5:
                    for (int i = 0; i < items.Count; i++)
                    {
                        stat += items[i].GetStatToModify().GetAgility();
                    }
                    break;
            }
            return stat;
        }
        public bool AreRangedItems()
        {
            for(int i=0; i < items.Count; i++)
            {
                if (items[i] is RangedItem )
                {
                    return true;
                }
            }
            return false;
        }
        public bool AreMeleeItems()
        {
            for (int i = 0; i < items.Count; i++)
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
            for (int i = 0; i < items.Count; i++)
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
