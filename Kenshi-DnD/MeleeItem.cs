namespace Kenshi_DnD
{
    // Item which is melee, which can be consumable or not. - Santiago Cabrero
    [Serializable]
    class MeleeItem : Item
    {
        // If item can revive another player
        protected bool canRevive;
        // If item is consumible
        protected bool breaksOnUse;
        // Constructor
        public MeleeItem(string name, string description, int value, int resellValue, int limbsNeeded, bool canRevive, StatModifier statToModify, bool breaksOnUse)
            : base(name, description, value, resellValue, limbsNeeded, statToModify)
        {
            this.canRevive = canRevive;
            this.breaksOnUse = breaksOnUse;
        }
        // Returns a string that announces use
        public override string AnnounceUse()
        {
            return "¡El héroe usa " + "@" + GetRarityColor() + "@" + name + "@!";
        }
        // Melee item rarity upgrade affects on bruteforce if not consumible, if consumible affects hp and resistance
        public override void UpgradeRarity(Rarity.Rarities rarity)
        {
            SetRarity(rarity);
            double costMultiplier = 1.0;
            int buff = 0;

            switch (rarity)
            {
                case Rarity.Rarities.Junk:
                    {
                        costMultiplier = 1.0;
                        buff = 0;
                        break;
                    }
                case Rarity.Rarities.RustCovered:
                    {
                        costMultiplier = 1.10;
                        buff = 1;
                        break;
                    }
                case Rarity.Rarities.Catun:
                    {
                        costMultiplier = 1.25;
                        buff = 2;
                        break;
                    }
                case Rarity.Rarities.Mk:
                    {
                        costMultiplier = 1.50;
                        buff = 3;
                        break;
                    }
                case Rarity.Rarities.Edgewalker:
                    {
                        costMultiplier = 1.75;
                        buff = 4;
                        break;
                    }
                case Rarity.Rarities.Meitou:
                    {
                        costMultiplier = 2.0;
                        buff = 5;
                        break;
                    }
            }
            if (breaksOnUse)
            {
                statToModify.UpgradeStat(Stats.Stat.HP, buff);
                statToModify.UpgradeStat(Stats.Stat.Resistance, buff / 2);
            }
            else
            {
                statToModify.UpgradeStat(Stats.Stat.BruteForce, buff);
            }

            value = (int)(value * costMultiplier);
            resellValue = (int)(value * 0.5);
        }
        // Returns a copy of the item
        public override Item GetCopy()
        {
            StatModifier statCopy = GetStatToModify().GetCopy();

            Item itemCopy = new MeleeItem(name, description, value, resellValue, limbsNeeded, canRevive, statCopy, breaksOnUse);
            itemCopy.SetRarity(rarity);

            return itemCopy;
        }
        // Getters
        public bool BreaksOnUse()
        {
            return breaksOnUse;
        }
        public bool CanRevive()
        {
            return canRevive;
        }
        // Virtual override method that returns info about the item
        public override string ItemInfo()
        {
            return base.ItemInfo() + "\n" +
                   (canRevive ? "Puede revivir en uso\n" : "") +
                   "De un solo uso: " + (breaksOnUse ? "Sí" : "No") + "\n";
        }
    }
}
