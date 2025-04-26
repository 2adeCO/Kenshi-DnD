using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class MeleeItem : Item
    {
        protected bool canRevive;
        protected bool breaksOnUse;
        public MeleeItem(string name, int value, int resellValue, int limbsNeeded, bool canRevive, StatModifier statToModify, bool breaksOnUse, bool isRare)
            : base(name, value, resellValue, limbsNeeded, statToModify, isRare)
        {
            this.canRevive = canRevive;
            this.breaksOnUse = breaksOnUse;
        }
       public override string AnnounceUse()
        {
            return "¡El héroe usa " + base.name + "!";
        }
        public bool BreaksOnUse()
        {
            return breaksOnUse;
        }
        public override string ToString()
        {
            return base.ToString() + "\n" +
                   (canRevive ? "Puede revivir en uso\n" : "") +
                   "Duradera: " + (breaksOnUse ? "Sí" : "No") + "\n";
        }
    }
}
