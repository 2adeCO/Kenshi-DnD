namespace Kenshi_DnD
{
    [Serializable]
    class MeleeItem : Item
    {
        protected bool canRevive;
        protected bool breaksOnUse;
        public MeleeItem(string name,string description, int value, int resellValue, int limbsNeeded, bool canRevive, StatModifier statToModify, bool breaksOnUse)
            : base(name,description, value, resellValue, limbsNeeded, statToModify)
        {
            this.canRevive = canRevive;
            this.breaksOnUse = breaksOnUse;
        }
        public override string AnnounceUse()
        {
            return "¡El héroe usa " + "@"+ GetRarityColor() +"@"+ name + "@!";
        }
        public override void SetRarity(Rarity.Rarities rarity)
        {
            this.rarity = rarity;
            int costIncrease = 0;
            int buff = 0;

            switch (rarity)
            {
                case Rarity.Rarities.Junk:
                    {
                        buff = 1;
                        costIncrease = 0;
                        break;
                    }
                case Rarity.Rarities.RustCovered:
                    {
                        costIncrease = 100;
                        buff = 2; break;
                    }
                case Rarity.Rarities.Catun:
                    {
                        costIncrease = 500;
                        buff = 3; break;
                    }
                case Rarity.Rarities.Mk:
                    {
                        costIncrease = 750;
                        buff = 4; break;
                    }
                case Rarity.Rarities.Edgewalker:
                    {
                        costIncrease = 1500;
                        buff = 5; break;
                    }
                case Rarity.Rarities.Meitou:
                    {
                        costIncrease = 2500;
                        buff = 6; break;
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
            value = value + costIncrease;
            resellValue = resellValue + costIncrease / 2;
        }
        public override Item GetCopy()
        {
            StatModifier statCopy = GetStatToModify().GetCopy();

            Item itemCopy = new MeleeItem(name,description,value,resellValue,limbsNeeded,canRevive,statCopy,breaksOnUse);

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
