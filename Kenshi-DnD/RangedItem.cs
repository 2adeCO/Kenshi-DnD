using System.Xml.Linq;

namespace Kenshi_DnD
{
    [Serializable]
    class RangedItem : Item
    {
        int difficulty;
        int ammo;
        public RangedItem(string name, string description, int value, int resellValue, int limbsNeeded, int difficulty, int ammo, StatModifier statToModify)
            : base(name, description, value, resellValue, limbsNeeded, statToModify)
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
