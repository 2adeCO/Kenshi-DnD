using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    public class Immunities
    {
        // These are used by enemies to cut in half or block hero's damage
        public enum Immunity
        {
            None,
            ResistantToRanged,
            ImmuneToRanged,
            ImmuneToRangedAndResistantToMelee,
            ResistantToMelee,
            ImmuneToMelee,
            ImmuneToMeleeAndResistantToRanged,
            ResistantToBoth,
        }
    }
}
