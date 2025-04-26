using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class RangedItem : Item
    {
        protected int difficulty;
        public RangedItem(string name, int difficulty, int value, int resellValue, int limbsNeeded, StatModifier statsToModify, bool isRare)
            : base(name, value, resellValue, limbsNeeded, statsToModify, isRare)
        {
            this.difficulty = difficulty;
        }
        public RangedItem(string name,StatModifier statToModify, int difficulty, int value, int resellValue, int limbsNeeded, bool isRare)
            : base(name, value, resellValue, limbsNeeded, statToModify, isRare)
        {
            this.difficulty = difficulty;
        }
        public override string AnnounceUse()
        {
            return "¡El héroe coge distancia... y usa " + base.name + "!";
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                   "Dificultad: " + difficulty + "\n";
        }
    }
}
