using System.Diagnostics;

namespace Kenshi_DnD
{
    [Serializable]
    public class Dice
    {
        int sides;
        int minWin;

        string lastPlay;
        int winHistory;

        public Dice(int sides, int minWin)
        {
            this.sides = sides;
            this.minWin = minWin;
            this.lastPlay = "";
            this.winHistory = 0;
        }
        private int Roll(Random random)
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
        public int PlayDice(int numOfRolls,Random random)
        {
            winHistory = 0;
            lastPlay = "";
            int totalWins = 0;
            if (numOfRolls <= 0)
            {
                Debug.WriteLine("<0 number of rolls");
                return 0;
            }
            for (int i = 0; i < numOfRolls; i+=1)
            {
                if (IsSuccess(Roll(random)))
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
        public override string ToString()
        {
            return "Caras: " + sides + ", Valor mínimo para ganar: " + minWin;
        }
    }
}
