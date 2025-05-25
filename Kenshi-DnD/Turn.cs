namespace Kenshi_DnD
{
    class Turn
    {
        ITurnable fighter;
        int attackProgress;
        const int ATTACK_GOAL = 10;

        public Turn(ITurnable fighter)
        {
            this.fighter = fighter;
            attackProgress = 0;
        }
        public void SetFighter(ITurnable fighter)
        {
            this.fighter = fighter;
        }
        public void AdvanceTurn()
        {
            if (fighter.IsAlive()) 
            {
                attackProgress += fighter.GetAgility() < 1 ? 1 : fighter.GetAgility();
            }
        }
        public bool IsTurnComplete()
        {
            if (attackProgress >= ATTACK_GOAL)
            {
                attackProgress -= ATTACK_GOAL;
                return true;
            }
            return false;
        }
        public ITurnable GetFighter()
        {
            return fighter;
        }
    }
}
