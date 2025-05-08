namespace Kenshi_DnD
{
    public class Inventory
    {
        protected List<Item> items;
        public Inventory()
        {
            items = new List<Item>();
        }
        public void AddItem(Item item)
        {
            items.Add(item);
        }
        public int SellItem(Item item)
        {
            for (int i = 0; i < items.Count; i+=1)
            {
                if (items[i].Equals(item))
                {
                    items.RemoveAt(i);
                    return item.GetResellValue();
                }
            }
            return 0;
        }
        public bool ContainsItem(Item item)
        {
            for (int i = 0; i < items.Count; i+=1)
            {
                if (items[i].Equals(item))
                {
                    return true;
                }
            }
            return false;
        }
        public void RemoveItem(Item item)
        {
            for (int i = 0; i < items.Count; i+=1)
            {
                if (items[i].Equals(item))
                {
                    items.RemoveAt(i);
                    return;
                }
            }
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

        //0 is all, 1 is unused, 2 is used
        public Item[] GetConsumables(int returnSelect)
        {
            Item[] itemsToReturn;
            int count = 0;
            switch (returnSelect)
            {
                case 0:
                    {
                        for (int i = 0; i < items.Count; i+=1)
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
                        for (int i = 0; i < items.Count; i+=1)
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
                        for (int i = 0; i < items.Count; i+=1)
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
                        for (int i = 0; i < items.Count; i+=1)
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
                        for (int i = 0; i < items.Count; i+=1)
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
                        for (int i = 0; i < items.Count; i+=1)
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
                        return null;
                    }
            }
            return itemsToReturn;
        }
        public Item[] GetRanged(int returnSelect)
        {
            Item[] itemsToReturn;
            int count = 0;
            switch (returnSelect)
            {
                case 0:
                    {
                        for (int i = 0; i < items.Count; i+=1)
                        {
                            if (items[i] is RangedItem)
                            {
                                count += 1;
                            }
                        }

                        itemsToReturn = new Item[count];
                        count = 0;
                        for (int i = 0; i < items.Count; i+=1)
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
                        for (int i = 0; i < items.Count; i+=1)
                        {
                            if (!items[i].IsUsed() && items[i] is RangedItem)
                            {
                                count += 1;

                            }
                        }

                        itemsToReturn = new Item[count];
                        count = 0;
                        for (int i = 0; i < items.Count; i+=1)
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
                        for (int i = 0; i < items.Count; i+=1)
                        {
                            if (items[i].IsUsed() && items[i] is RangedItem)
                            {
                                count += 1;

                            }
                        }

                        itemsToReturn = new Item[count];
                        count = 0;
                        for (int i = 0; i < items.Count; i+=1)
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
                        return null;
                    }
            }
            return itemsToReturn;
        }
        public Item[] GetMelee(int returnSelect)
        {
            Item[] itemsToReturn;
            int count = 0;
            switch (returnSelect)
            {
                case 0:
                    {
                        for (int i = 0; i < items.Count; i+=1)
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
                        for (int i = 0; i < items.Count; i+=1)
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
                        for (int i = 0; i < items.Count; i+=1)
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
                        for (int i = 0; i < items.Count; i+=1)
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
                        for (int i = 0; i < items.Count; i+=1)
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
                        for (int i = 0; i < items.Count; i+=1)
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
                        return null;
                    }
            }
            return itemsToReturn;
        }
        private bool IsItemConsumible(Item item)
        {
            if (item is MeleeItem)
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
