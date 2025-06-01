using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class DBNotFoundException : Exception
    {
        public DBNotFoundException() : base("Base de datos no encontrada, usando XML en su lugar...")
        {
        }
    }
}

