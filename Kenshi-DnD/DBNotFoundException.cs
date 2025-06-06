using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    // Custom exception thrown when the database is not found, it's meant to be a silent message as the program switches to XML data if DB is not found
    class DBNotFoundException : Exception
    {
        public DBNotFoundException() : base("Base de datos no encontrada, usando XML en su lugar...")
        {
        }
    }
}

