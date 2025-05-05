using System.Diagnostics;

namespace Kenshi_DnD
{
    class Dice
    {
        int sides;
        int minWin;

        string lastPlay;
        int winHistory;
        Random random;
        public Dice(int sides, int minWin,Random random)
        {
            this.sides = sides;
            this.minWin = minWin;
            this.lastPlay = "";
            this.winHistory = 0;
            this.random = random;
        }
        private int Roll()
        {
            int roll = random.Next(1, sides + 1);
            AddRollToHistory(roll);
            if (IsSuccess(roll))
            {
                winHistory += 1;
            }
            return roll;
        }
        private bool IsSuccess(int roll)
        {
            if (roll >= minWin)
            {
                return true;
            }
            return false;
        }
        public int PlayDice(int numOfRolls)
        {
            winHistory = 0;
            lastPlay = "";
            int totalWins = 0;
            if (numOfRolls <= 0)
            {
                Debug.WriteLine("<0 number of rolls");
                return 0;
            }
            for (int i = 0; i < numOfRolls; i++)
            {
                if (IsSuccess(Roll()))
                {
                    totalWins += 1;
                }
            }
            Debug.WriteLine("Dices wins: " + totalWins);
            return totalWins;
        }
        private void AddRollToHistory(int roll)
        {
            if (lastPlay == "")
            {
                lastPlay = roll + "";
            }
            else
            {
                lastPlay += ", " + roll;
            }
        }
        public string GetRollHistory()
        {
            return "@917@Dados:@ " + ((lastPlay == "") ? ("No se consiguieron dados"): (lastPlay +"\n\n"+
                "@917@Veces ganadas:@ " + winHistory));
        }
    }
}
