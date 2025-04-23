using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class ConsumableItem : Item
    {
        protected bool breaksOnUse;
        protected bool canRevive;
        public ConsumableItem(string name, int value, int resellValue, int limbsNeeded, bool breaksOnUse, bool canRevive) 
            : base(name,value,resellValue,limbsNeeded)
        {
            this.breaksOnUse = breaksOnUse;
            this.canRevive = canRevive;
        }
    }
}
