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
        public RangedItem(int buff, StatModifier statToModify, int difficulty, string name, int value, int resellValue, int limbsNeeded, bool isRare)
            : base(name, value, resellValue, limbsNeeded, statToModify, isRare)
        {
            this.difficulty = difficulty;
        }
        public override StatModifier UseItem(Hero hero)
        {
            
            if (CanUseItem(hero))
            {
                alreadyUsed = true;
                return statToModify;
            }
            return null;
        }
        
        public override string ToString()
        {
            return base.ToString() + "\n" +
                   "Dificultad: " + difficulty + "\n";
        }
    }
}
