using System.Diagnostics;

namespace Kenshi_DnD
{
    class Combat
    {
        CombatWindow window;
        Hero[] heroes;
        Monster[] enemies;
        Dice myDice;
        //Keeps track of every turn progress
        List<Turn> everyTurn;
        //Generates a list of fighters in the order they must attack
        List<ITurnable> turnOrder;
        Random rnd;
        int combatState;
        int turnIndex;
        public Combat(Hero[] newHeroes, Monster[] newEnemies, Dice myDice, Inventory myInventory, CombatWindow window)
        {
            this.window = window;
            combatState = 0;
            rnd = new Random();
            turnIndex = 0;
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

            DecideNextNTurns(24, true);
            this.window = window;
        }
        public ITurnable GetCurrentAttacker()
        {
            return turnOrder[turnIndex];
        }

        public void DecideNextNTurns(int numOfTurns, bool isNewList)
        {
            if (isNewList)
            {
                turnOrder = new List<ITurnable>();
            }

            int currentSize = turnOrder.Count;
            int numOfAddedTurns = 0;
            int debugCounter = 0;
            List<ITurnable> tempList;
            do
            {
                debugCounter += 1;
                tempList = new List<ITurnable>();
                for (int i = 0; i < everyTurn.Count; i++)
                {
                    if (everyTurn[i].GetFighter().IsAlive())
                    {
                        everyTurn[i].AdvanceTurn();
                    }
                }
                for (int i = 0; i < everyTurn.Count; i++)
                {
                    while (everyTurn[i].IsTurnComplete())
                    {
                        tempList.Add(everyTurn[i].GetFighter());
                        numOfAddedTurns += 1;
                    }
                }
                tempList = SortByAgility(tempList);
                for (int i = 0; i < tempList.Count; i++)
                {
                    Debug.WriteLine("Added to turnorder: " + tempList[i].GetName() + debugCounter);
                    turnOrder.Add(tempList[i]);
                }
                //If numOfTurns is 3, and in the same AdvanceTurn, more than 3 complete the attack
                //Every one will pass
            } while (numOfAddedTurns <= numOfTurns);
            Debug.WriteLine("Turn order decided for " + numOfAddedTurns + " turns");
        }

        public void NextTurn(Monster monsterTarget)
        {
            ITurnable attacker = turnOrder[turnIndex];
            if (attacker is Hero)
            {
                HeroAttacks((Hero)attacker, monsterTarget);
            }
            else
            {
                MonsterAttacks((Monster)attacker, DecideVictim());
            }
            UpdateGameState();
            if (GetGameState() != 0)
            {
                return;
            }
            turnIndex += 1;
            AdvanceTurnIfItsDead();
            if (turnOrder.Count - turnIndex < 6)
            {
                DecideNextNTurns(12, false);
            }
        }
        public void AdvanceIndex()
        {
            turnIndex += 1;
            if (turnOrder.Count - turnIndex < 6)
            {
                DecideNextNTurns(12, false);
            }
        }
        public void MonsterAttacks(Monster attacker, Hero defender)
        {

            Debug.WriteLine(attacker.GetName() + " attacks " + defender.GetName());
            int attackerStat;
            int defenderStat;

            attackerStat = attacker.GetStrength();
            defenderStat = defender.GetStat(4);


            int hits = myDice.PlayDice(attackerStat - defenderStat);
            Debug.WriteLine("Hits: " + hits);
            defender.Hurt(hits);

            Debug.WriteLine("Defender health: " + defender.GetHp());
            if (defender.GetHp() <= 0)
            {
                Debug.WriteLine("Killed!");
            }
            else
            {
                Debug.WriteLine("Not killed");
            }
            DamageLog(attacker.GetName() + " ataca a " + defender.GetName() + " y le hace " + hits + " puntos de daño");

        }
        public void HeroAttacks(Hero attacker, Monster defender)
        {
            if (attacker.AreConsumableItems())
            {

            }
            if (attacker.AreRangedItems())
            {
                DamageLog(attacker.GetName() + " coge distancia... y apunta a " + defender.GetName());
                RangeAttack(attacker, defender);
            }
            else
            {
                if (attacker.AreMeleeItems())
                {
                    DamageLog(attacker.GetName() + " arremete contra " + defender.GetName());
                    MeleeAttack(attacker, defender);
                }
                //Add martial arts
            }


        }
        private void ConsumableAction(Hero user)
        {

            Item[] uncastedConsumableItems = user.GetInventory().GetConsumables(2);

            MeleeItem[] consumableItems = new MeleeItem[uncastedConsumableItems.Length];

            int hpBoost = 0;

            for (int i = 0; i < consumableItems.Length; i++)
            {
                if (consumableItems[i] != null)
                {
                    hpBoost += consumableItems[i].GetStatToModify().GetHp();
                }
            }
            user.Heal(hpBoost);

        }
        private void MeleeAttack(Hero attacker, Monster defender)
        {
            Debug.WriteLine(attacker.GetName() + " attacks" + defender.GetName());
            int attackerStat;
            int defenderStat;
            int defenderHealth;

            attackerStat = attacker.GetStat(1);
            defenderStat = defender.GetResistance();
            defenderHealth = defender.GetHp();
            Debug.WriteLine("Dices to attack: " + (attackerStat - defenderStat));
            Debug.WriteLine("Defender health: " + defenderHealth);
            if (defender.GetImmunity() == 2)
            {
                Debug.WriteLine("Defender is immune to melee attacks. You wasted ammo");
                return;
            }
            else
            {
                if (defender.GetImmunity() == -3 || defender.GetImmunity() == 1)
                {
                    Debug.WriteLine("Defender is resistant to melee attacks");
                    attackerStat /= 2;
                }
            }
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
        private void RangeAttack(Hero attacker, Monster defender)
        {
            Debug.WriteLine(attacker.GetName() + " attacks " + defender.GetName());
            int attackerStat;
            int defenderStat;
            int defenderHealth;
            attackerStat = attacker.GetStat(2);

            Item[] uncastedRangedItems = attacker.GetInventory().GetRanged(2);

            RangedItem[] rangedItems = new RangedItem[uncastedRangedItems.Length];

            for (int i = 0; i < uncastedRangedItems.Length; i++)
            {
                rangedItems[i] = (RangedItem)uncastedRangedItems[i];
            }

            int misses = 0;
            int damage = 0;
            int emptyAmmoWeapons = 0;
            int permittedMisses = myDice.PlayDice(attackerStat / 3);
            Debug.WriteLine("Number of ranged weapons " + rangedItems.Length);
            Debug.WriteLine("Permitted misses: " + permittedMisses);
            do
            {
                emptyAmmoWeapons = 0;
                for (int i = 0; i < rangedItems.Length; i++)
                {
                    if (rangedItems[i].GetAmmo() <= 0)
                    {
                        emptyAmmoWeapons += 1;
                        Debug.WriteLine(rangedItems[i].GetName() + " is out of ammo");

                    }
                    else
                    {
                        int diceNum = myDice.PlayDice(attackerStat - (rangedItems[i].GetDifficulty() + (defender.GetAgility() / 2)));
                        if (diceNum >= rangedItems[i].GetDifficulty())
                        {
                            Debug.WriteLine("Hit!");
                            damage += rangedItems[i].GetStatToModify().GetBruteForce();
                            rangedItems[i].ShootAmmo();
                        }
                        else
                        {
                            Debug.WriteLine("Missed!");
                            misses += 1;
                            rangedItems[i].ShootAmmo();
                        }
                    }

                }
            } while (misses <= permittedMisses && emptyAmmoWeapons != rangedItems.Length);


            if (defender.GetImmunity() == -2)
            {
                Debug.WriteLine("Defender is immune to ranged attacks. You wasted ammo");
                return;
            }
            else
            {
                if (defender.GetImmunity() == -1 || defender.GetImmunity() == 3)
                {
                    Debug.WriteLine("Defender is resistant to ranged attacks");
                    damage /= 2;
                }
            }

            defenderStat = defender.GetResistance();

            defenderHealth = defender.GetHp();


            if (myDice.PlayDice(damage - defenderStat) >= defenderHealth)
            {
                Debug.WriteLine("Killed!");
                defender.SetHp(0);
            }
            else
            {
                Debug.WriteLine("Not killed");
            }
        }
        private void DamageLog(string message)
        {
            window.UpdateLogUI(message);
        }
        private void AdvanceTurnIfItsDead()
        {
            if (!turnOrder[turnIndex].IsAlive())
            {
                Debug.WriteLine("Someone is not alive");
                do
                {
                    Debug.WriteLine("Checking next..");
                    AdvanceIndex();
                } while (!turnOrder[turnIndex].IsAlive());
            }
        }
        private Hero DecideVictim()
        {
            int count = 0;
            for (int i = 0; i < heroes.Length; i++)
            {
                if (heroes[i].IsAlive())
                {
                    Debug.WriteLine("Victim counted " + i);
                    count++;
                }
            }
            Hero[] heroesToAttack = new Hero[count];
            count = 0;
            for (int i = 0; i < heroes.Length; i++)
            {
                if (heroes[i].IsAlive())
                {
                    Debug.WriteLine("People alive:" + i);
                    heroesToAttack[count] = heroes[i];
                    count += 1;
                }
            }

            return heroesToAttack[rnd.Next(0, count)];
        }
        public int GetGameState()
        {
            Debug.WriteLine("Combat state is " + combatState);
            return combatState;
        }
        private void UpdateGameState()
        {
            bool lost = true;
            bool won = true;

            for (int i = 0; i < heroes.Length; i++)
            {
                if (heroes[i].IsAlive())
                {
                    lost = false;
                }
            }
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].IsAlive())
                {
                    won = false;
                }
            }

            if (!lost && !won)
            {
                combatState = 0;
            }
            else
            {
                if (won)
                {
                    combatState = 1;
                }
                else
                {
                    combatState = -1;
                }
            }

        }
        private List<ITurnable> SortByAgility(List<ITurnable> list)
        {
            bool errorFound = true;
            do
            {
                errorFound = false;
                for (int i = 0; i < list.Count - 1; i++)
                {
                    if (list[i].GetAgility() < list[i + 1].GetAgility())
                    {
                        ITurnable temp = list[i];
                        list[i] = list[i + 1];
                        list[i + 1] = temp;
                        errorFound = true;
                    }
                }
            } while (errorFound);
            return list;
        }
    }

}
