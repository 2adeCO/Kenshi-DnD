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
            for (int i = 0; i < items.Count; i++)
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
            for (int i = 0; i < items.Count; i++)
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
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].IsUsed())
                {
                    items[i].UnUse();
                }
            }
            Debug.WriteLine("All items are available");
        }
        //0 is all, 1 is unused, 2 is used
        public Item[] GetConsumables(int returnSelect)
        {
            Item[] itemsToReturn;
            int count = 0;
            Debug.Write("Consumibles - ");
            switch (returnSelect)
            {
                case 0:
                    {
                        Debug.WriteLine("All");
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i] is MeleeItem)
                            {
                                if (IsItemConsumible(items[i]))
                                {
                                    count += 1;
                                }
                            }
                        }

                        itemsToReturn = new Item[count];
                        count = 0;
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i] is MeleeItem)
                            {
                                if (IsItemConsumible(items[i]))
                                {
                                    itemsToReturn[count] = items[i];
                                    count += 1;
                                }
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        Debug.WriteLine("Unused");
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (!items[i].IsUsed() && items[i] is MeleeItem)
                            {
                                if (IsItemConsumible(items[i]))
                                {
                                    count += 1;
                                }
                            }
                        }

                        itemsToReturn = new Item[count];
                        count = 0;
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (!items[i].IsUsed() && items[i] is MeleeItem)
                            {
                                if (IsItemConsumible(items[i]))
                                {
                                    itemsToReturn[count] = items[i];
                                    count += 1;
                                }
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        Debug.WriteLine("Used");
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i].IsUsed() && items[i] is MeleeItem)
                            {
                                MeleeItem meleeItem = (MeleeItem)items[i];
                                if (IsItemConsumible(items[i]))
                                {
                                    count += 1;
                                }
                            }
                        }

                        itemsToReturn = new Item[count];
                        count = 0;
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i].IsUsed() && items[i] is MeleeItem)
                            {
                                if (IsItemConsumible(items[i]))
                                {
                                    itemsToReturn[count] = items[i];
                                    count += 1;
                                }
                            }
                        }
                        break;
                    }
                default:
                    {
                        Debug.WriteLine("Which items to return not specified");
                        return null;
                    }
            }
            return itemsToReturn;
        }
        public Item[] GetRanged(int returnSelect)
        {
            Item[] itemsToReturn;
            int count = 0;
            Debug.Write("Ranged - ");
            switch (returnSelect)
            {
                case 0:
                    {
                        Debug.WriteLine("All");
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i] is RangedItem)
                            {
                                count += 1;
                            }
                        }

                        itemsToReturn = new Item[count];
                        count = 0;
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i] is RangedItem)
                            {
                                itemsToReturn[count] = items[i];
                                count += 1;
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        Debug.WriteLine("Unused");
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (!items[i].IsUsed() && items[i] is RangedItem)
                            {
                                count += 1;

                            }
                        }

                        itemsToReturn = new Item[count];
                        count = 0;
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (!items[i].IsUsed() && items[i] is RangedItem)
                            {
                                itemsToReturn[count] = items[i];
                                count += 1;
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        Debug.WriteLine("Used");
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i].IsUsed() && items[i] is RangedItem)
                            {
                                count += 1;

                            }
                        }

                        itemsToReturn = new Item[count];
                        count = 0;
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i].IsUsed() && items[i] is RangedItem)
                            {
                                itemsToReturn[count] = items[i];
                                count += 1;
                            }
                        }
                        break;
                    }
                default:
                    {
                        Debug.WriteLine("Which items to return not specified");
                        return null;
                    }
                }
            return itemsToReturn;
        }

        
        public Item[] GetMelee(int returnSelect)
        {
            Item[] itemsToReturn;
            int count = 0;
            Debug.Write("Melee - ");
            switch (returnSelect)
            {
                case 0:
                    {
                        Debug.WriteLine("All");
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i] is MeleeItem)
                            {
                                if (!IsItemConsumible(items[i]))
                                {
                                    count += 1;
                                }
                            }
                        }

                        itemsToReturn = new Item[count];
                        count = 0;
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i] is MeleeItem)
                            {
                                if (!IsItemConsumible(items[i]))
                                {
                                    itemsToReturn[count] = items[i];
                                    count += 1;
                                }
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        Debug.WriteLine("Unused");
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (!items[i].IsUsed() && items[i] is MeleeItem)
                            {
                                if (!IsItemConsumible(items[i]))
                                {
                                    count += 1;
                                }
                            }
                        }

                        itemsToReturn = new Item[count];
                        count = 0;
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (!items[i].IsUsed() && items[i] is MeleeItem)
                            {
                                if (!IsItemConsumible(items[i]))
                                {
                                    itemsToReturn[count] = items[i];
                                    count += 1;
                                }
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        Debug.WriteLine("Used");
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i].IsUsed() && items[i] is MeleeItem)
                            {
                                if (!IsItemConsumible(items[i]))
                                {
                                    count += 1;
                                }
                            }
                        }

                        itemsToReturn = new Item[count];
                        count = 0;
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i].IsUsed() && items[i] is MeleeItem)
                            {
                                if (!IsItemConsumible(items[i]))
                                {
                                    itemsToReturn[count] = items[i];
                                    count += 1;
                                }
                            }
                        }
                        break;
                    }
                default:
                    {
                        Debug.WriteLine("Which items to return not specified");
                        return null;
                    }
            }
            return itemsToReturn;
        }
        private bool IsItemConsumible(Item item)
        {
            if(item is MeleeItem)
            {
                MeleeItem meleeItem = (MeleeItem)item;
                if (meleeItem.BreaksOnUse())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
 