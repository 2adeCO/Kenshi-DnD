namespace Kenshi_DnD
{
    [Serializable]
    class MeleeItem : Item
    {
        protected bool canRevive;
        protected bool breaksOnUse;
        public MeleeItem(string name, string description, int value, int resellValue, int limbsNeeded, bool canRevive, StatModifier statToModify, bool breaksOnUse)
            : base(name, description, value, resellValue, limbsNeeded, statToModify)
        {
            this.canRevive = canRevive;
            this.breaksOnUse = breaksOnUse;
        }
        public override string AnnounceUse()
        {
            return "¡El héroe usa " + "@" + GetRarityColor() + "@" + name + "@!";
        }
        public override void SetRarity(Rarity.Rarities rarity)
        {
            this.rarity = rarity;
            double costMultiplier = 1.0;
            int buff = 0;

            switch (rarity)
            {
                case Rarity.Rarities.Junk:
                    { 
                        costMultiplier = 1.0;  
                        buff = 1;
                        break;
                    }
                case Rarity.Rarities.RustCovered:
                    {
                        costMultiplier = 1.10; 
                        buff = 2;
                        break;
                    }
                case Rarity.Rarities.Catun:
                    {
                        costMultiplier = 1.25; 
                        buff = 3;
                        break;
                    }
                case Rarity.Rarities.Mk:
                    {
                        costMultiplier = 1.50; 
                        buff = 4;
                        break;
                    }
                case Rarity.Rarities.Edgewalker:
                    {
                        costMultiplier = 1.75; 
                        buff = 5;
                        break;
                    }
                case Rarity.Rarities.Meitou:
                    {
                        costMultiplier = 2.0;   
                        buff = 6;
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
        public override Item GetCopy()
        {
            StatModifier statCopy = GetStatToModify().GetCopy();

            Item itemCopy = new MeleeItem(name, description, value, resellValue, limbsNeeded, canRevive, statCopy, breaksOnUse);
            itemCopy.SetRarity(rarity);

            return itemCopy;
        }
        public bool BreaksOnUse()
        {
            return breaksOnUse;
        }
        public bool CanRevive()
        {
            return canRevive;
        }
        public override string ToString()
        {
            return base.ToString() + "\n" +
                   (canRevive ? "Puede revivir en uso\n" : "") +
                   "De un solo uso: " + (breaksOnUse ? "Sí" : "No") + "\n";
        }
    }
}
