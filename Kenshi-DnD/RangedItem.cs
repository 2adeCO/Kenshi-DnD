namespace Kenshi_DnD
{
    class RangedItem : Item
    {
        int difficulty;
        int ammo;
        public RangedItem(string name, int difficulty, int ammo, int value, int resellValue, int limbsNeeded, StatModifier statsToModify, Rarity.Rarities rarity)
            : base(name, value, resellValue, limbsNeeded, statsToModify, rarity)
        {
            this.ammo = ammo;
            this.difficulty = difficulty;
        }
        public override string AnnounceUse()
        {
            return "¡El héroe coge distancia... y usa " + "@" + GetRarityColor() + "@" + name + "@" + "!";
        }
        public int GetDifficulty()
        {
            return difficulty;
        }
        public void AddAmmo(int ammo)
        {
            this.ammo += ammo;
        }
        public int GetAmmo()
        {
            return ammo;
        }
        public void ShootAmmo()
        {
            this.ammo -= 1;
        }
        public override string ToString()
        {
            return base.ToString() + "\n" +
                   "Dificultad: " + difficulty + "\n";
        }
    }
}
