namespace Kenshi_DnD
{
    // Class that has the functionality of a custom dice and can be thrown to see what the results are, plus giving the info of the last roll. - Santiago Cabrero
    [Serializable]
    public class Dice
    {
        // Number of sides 
        int sides;
        // Minimum number that counts as win
        int minWin;

        // String of the last play
        string lastPlay;
        // Number of successful roll in a play
        int winHistory;

        // Constructor
        public Dice(int sides, int minWin)
        {
            this.sides = sides;
            this.minWin = minWin;
            this.lastPlay = "";
            this.winHistory = 0;
        }
        // A single roll, returns a number in the dice
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
        // Returns a bool depending on the roll's number being lower/higher or equal than minWin
        private bool IsSuccess(int roll)
        {
            if (roll >= minWin)
            {
                return true;
            }
            return false;
        }
        // A play of the dice, will return how many rolls were successful
        public int PlayDice(int numOfRolls,Random random)
        {
            // Resets the string values
            winHistory = 0;
            lastPlay = "";
            // Variable of current wins
            int totalWins = 0;
            // Returns if no win can occur
            if (numOfRolls <= 0)
            {
                return 0;
            }
            // Counts all the success of the rolls
            for (int i = 0; i < numOfRolls; i+=1)
            {
                if (IsSuccess(Roll(random)))
                {
                    totalWins += 1;
                }
            }
            // Returns the wins
            return totalWins;
        }
        // Adds a piece of string that represents a roll, to lastplay
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
        // Returns a string of the play
        public string GetRollHistory()
        {
            return "@917@Dados:@ " + ((lastPlay == "") ? ("No se consiguieron dados"): (lastPlay +"\n\n"+
                "@917@Veces ganadas:@ " + winHistory));
        }
        // Returns a string that represents the dice
        public override string ToString()
        {
            return "Caras: " + sides + ", Valor mínimo para ganar: " + minWin;
        }
    }
}
