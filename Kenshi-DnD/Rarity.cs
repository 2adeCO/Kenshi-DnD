using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    //Kenshi's actual weapon tiers go like this:
    //Rusted Junk, Rusting Blade, Mid-Grade Salvage, Old Re-Fitted Blade, Re-fitted Blade,
    //Catun No. 1, Catun No. 2, Catun No. 3,
    //Mk I, Mk II, Mk III,
    //Edge Type 1, Edge Type 2, Edge Type 3, Meitou
    //For obvious reasons I kept it simpler.
    //Also, in Kenshi anything below Edge is garbage, I won't make it so harsh in this game haha
    public class Rarity
    {
        public enum Rarities
        {
            Junk,
            RustCovered,
            Catun,
            Mk,
            Edgewalker,
            Meitou

        }

    }
}
