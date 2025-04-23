using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class Inventory
    {
        List<Item> items;
        public Inventory()
        {
            Debug.WriteLine("Inventory created");
            items = new List<Item>();
        }
        public void AddItem(Item item)
        {
            items.Add(item);
            Debug.WriteLine("Item added");
        }
        public int SellItem(Item item)
        {
            for(int i = 0; i < items.Count; i++)
            {
                if (items[i].Equals(item))
                {
                    items.RemoveAt(i);
                    Debug.WriteLine("Item sold for: " + item.GetResellValue());
                    return item.GetResellValue();
                }
            }
            return 0;
        }
        public StatModifier UseItem(Item item, Hero hero)
        {
            for(int i = 0; i < items.Count; i++)
            {
                if (items[i].Equals(item))
                {
                    if (item.CanUseItem(hero))
                    {
                        
                        StatModifier stat = item.UseItem(hero);
                        if (item is MeleeItem)
                        {
                            Debug.WriteLine("Item is a melee item");
                            if (((MeleeItem)item).BreaksOnUse())
                            {
                                Debug.WriteLine("Item breaks on use");
                                items.RemoveAt(i);
                            }
                        }
                        else
                        {
                            Debug.WriteLine("Item is ranged item");
                        }

                            return stat;
                    }
                }
            }
            Debug.WriteLine("Can't find object or is being used");
            return null;
        }
        public string ShowInfo(Item item)
        {
            if (items.Contains(item))
            {
                return item.ToString();
            }
            else
            {
                return "Item not found in inventory.";
            }
        }
        public void AllItemsDisponible()
        {
            for(int i = 0; i < items.Count; i++)
            {
                if (items[i].GetAlreadyUsed())
                {
                    items[i].UnUse();
                }
            }
            Debug.WriteLine("All items are available");
        }
    }
}
