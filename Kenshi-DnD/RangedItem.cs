using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class RangedItem : Item
    {
        int difficulty;
        int ammo;
        public RangedItem(string name, int difficulty, int ammo, int value, int resellValue, int limbsNeeded, StatModifier statsToModify, bool isRare)
            : base(name, value, resellValue, limbsNeeded, statsToModify, isRare)
        {
            this.ammo = ammo;
            this.difficulty = difficulty;
        }
        public override string AnnounceUse()
        {
            return "¡El héroe coge distancia... y usa " + base.name + "!";
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
