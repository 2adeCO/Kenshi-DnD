namespace Kenshi_DnD
{
    // These are used by enemies to cut in half or block hero's damage. - Santiago Cabrero
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
