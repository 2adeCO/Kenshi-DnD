using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    public class Immunities
    {
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
