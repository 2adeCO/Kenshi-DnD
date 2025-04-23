using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    internal class MeleeItem : Item
    {
        
        protected bool breaksOnUse;
        
        public MeleeItem(int buff, int statToModify, bool breaksOnUse, string name, int value, int resellValue, int limbsNeeded)
            : base(name, value, resellValue, limbsNeeded)
        {
            this.buff = buff;
            this.statToModify = statToModify;
            this.breaksOnUse = breaksOnUse;
        }
        public override void UseItem(Hero hero)
        {
            // Implement the logic for using the melee item
        }
        public override bool CanUseItem(Hero hero)
        {
            // Implement the logic to check if the hero can use the melee item
            return base.CanUseItem(hero);
        }
    }
    
}
