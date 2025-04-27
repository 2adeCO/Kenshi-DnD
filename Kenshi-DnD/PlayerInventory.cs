using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class PlayerInventory : Inventory
    {
        public PlayerInventory()
        {
            base.items = new List<Item>();
        }
    }
}
