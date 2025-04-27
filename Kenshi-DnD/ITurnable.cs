using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    interface ITurnable
    {
        public int GetAgility();
        public bool IsAlive();
        public string GetName();
    }
}
