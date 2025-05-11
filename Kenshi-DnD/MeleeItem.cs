namespace Kenshi_DnD
{
    [Serializable]
    class MeleeItem : Item
    {
        protected bool canRevive;
        protected bool breaksOnUse;
        public MeleeItem(string name, int value, int resellValue, int limbsNeeded, bool canRevive, StatModifier statToModify, bool breaksOnUse, Rarity.Rarities rarity)
            : base(name, value, resellValue, limbsNeeded, statToModify, rarity)
        {
            this.canRevive = canRevive;
            this.breaksOnUse = breaksOnUse;
        }
        public override string AnnounceUse()
        {
            return "¡El héroe usa " + "@"+ GetRarityColor() +"@"+ name + "@!";
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
