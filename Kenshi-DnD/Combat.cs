using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Kenshi_DnD
{
    class Combat
    {
        Hero[] heroes;
        Monster[] enemies;
        Dice myDice;
        List<Turn> everyTurn;
        List<ITurnable> turnOrder;
        Random rnd;
        int turnIndex;
        public Combat(Hero[] newHeroes, Monster[] newEnemies, Dice myDice, Inventory myInventory)
        {
            rnd = new Random();
            this.heroes = newHeroes;
            this.enemies = newEnemies;
            this.myDice = myDice;
            everyTurn = new List<Turn>();
            for (int i = 0; i < heroes.Length; i++)
            {
                everyTurn.Add(new Turn(heroes[i]));
            }
            for (int i = 0; i < enemies.Length; i++)
            {
                everyTurn.Add(new Turn(enemies[i]));
            }
            
            for(int i = 0; i < everyTurn.Count; i++)
            {
                for (int j = 0; j < everyTurn.Count - 1; j++)
                {
                    if (everyTurn[j].GetFighter().GetAgility() < everyTurn[j + 1].GetFighter().GetAgility())
                    {
                        ITurnable temp = everyTurn[j].GetFighter();
                        everyTurn[j] = everyTurn[j + 1];
                        everyTurn[j + 1].SetFighter(temp);
                    }
                }
            }
            DecideNextNTurns(24,true);
        }
       
        public void DecideNextNTurns(int numOfTurns, bool isNewList)
        {
            if (isNewList)
            {
                turnOrder = new List<ITurnable>();
            }

            int currentSize = turnOrder.Count;
            do
            {
                for (int i = 0; i < everyTurn.Count; i++)
                {
                    everyTurn[i].AdvanceTurn();
                }
                for (int i = 0; i < everyTurn.Count; i++)
                {
                    if (everyTurn[i].IsTurnComplete())
                    {
                        turnOrder.Add(everyTurn[i].GetFighter());
                    }
                }
                //If numOfTurns is 3, and in the same AdvanceTurn, more than 3 complete the attack
                //Every one will pass
            } while (turnOrder.Count >= currentSize + numOfTurns);

        }
        
        public void NextTurn()
        {
            ITurnable attacker = turnOrder[turnIndex];
            if (attacker is Hero)
            {
                
                HeroAttacks((Hero)attacker, enemies[rnd.Next(0,enemies.Length)]);
            }
            else
            {
                MonsterAttacks((Monster)attacker, heroes[rnd.Next(0,heroes.Length)]);
            }
            turnIndex += 1;
            if(turnOrder.Count - turnIndex < 6)
            {
                DecideNextNTurns(12,false);
            }
        }
        public void MonsterAttacks(Monster attacker, Hero defender) 
        {
            Debug.WriteLine("Monster attacks");
            int attackerStat;
            int defenderStat;

            attackerStat = attacker.GetStrength();
            defenderStat = defender.GetResistance();

            int hits = myDice.PlayDice(attackerStat - defenderStat);
            Debug.WriteLine("Hits: " + hits);
            defender.SetHp(defender.GetHp() - hits);
            Debug.WriteLine("Defender health: " + defender.GetHp());
            if (defender.GetHp() <= 0)
            {
                Debug.WriteLine("Killed!");
            }
            else
            {
                Debug.WriteLine("Not killed");
            }

        }
        public void HeroAttacks(Hero attacker, Monster defender)
        {
            Debug.WriteLine("Hero attacks");
            int attackerStat;
            int defenderStat;
            int defenderHealth;

            attackerStat= attacker.GetBruteForce();
            defenderStat = defender.GetResistance();
            defenderHealth = defender.GetHp();
            Debug.WriteLine("Dices to attack: " + (attackerStat - defenderStat));
            Debug.WriteLine("Defender health: " + defenderHealth);
            if (myDice.PlayDice(attackerStat - defenderStat) >= defenderHealth)
            {
                Debug.WriteLine("Killed!");
                defender.SetHp(0);
            }
            else
            {
                Debug.WriteLine("Not killed");
            }

        }

    }
}
