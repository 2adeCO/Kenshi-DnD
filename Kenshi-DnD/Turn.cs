namespace Kenshi_DnD
{
    // Manages combat turns: Each Iturnable has a progress and with each advance turn it will increase by their agility
    // Once the progress has surpassed the attack goal, it will return a true indicating it can attack on this turn.
    // If surpassed, the attack goal will be deducted from the attack progress. - Santiago Cabrero
    class Turn
    {
        // Fighter
        ITurnable fighter;
        // Progress to an attack
        int attackProgress;
        // Attack threshold 
        const int ATTACK_GOAL = 10;
        // Constructor
        public Turn(ITurnable fighter)
        {
            this.fighter = fighter;
            attackProgress = 0;
        }
        // Advances the progress if the fighter is alive
        public void AdvanceTurn()
        {
            if (fighter.IsAlive())
            {
                // If the fighter's agility is below 1, treat it as if the fighter had an agility of 1
                attackProgress += fighter.GetAgility() < 1 ? 1 : fighter.GetAgility();
            }
        }
        // Returns if the goal has been surpassed and deducts points if so
        public bool IsTurnComplete()
        {
            if (attackProgress >= ATTACK_GOAL)
            {
                attackProgress -= ATTACK_GOAL;
                return true;
            }
            return false;
        }
        // Returns fighter
        public ITurnable GetFighter()
        {
            return fighter;
        }
    }
}
