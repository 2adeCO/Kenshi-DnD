using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class Stat
    {
        int id;
        int value;
        public Stat(int id, int value)
        {
            this.id = id;
            this.value = value;
        }
        public int GetId() { return id; }
        public int GetValue() { return value; }
    }
}
