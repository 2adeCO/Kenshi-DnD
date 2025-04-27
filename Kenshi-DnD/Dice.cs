using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class Dice
    {
        int sides;
        int minWin;
        Random random;
        public Dice(int sides, int minWin)
        {
            this.sides = sides;
            this.minWin = minWin;
            random = new Random();
        }
        public int Roll()
        {
            return random.Next(1, sides + 1);
        }
        public bool IsSuccess(int roll)
        {
            if (roll >= minWin)
            {
                return true;
            }
            return false;
        }
        public int PlayDice(int numOfRolls)
        {
            int totalWins = 0;
            if(numOfRolls <= 0)
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
    }
}
