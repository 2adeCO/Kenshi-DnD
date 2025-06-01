using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class XMLNotFoundException : Exception
    {
        public XMLNotFoundException() : base("XML no encontrado, faltan datos para jugar al juego.")
        {
        }
    }
}
