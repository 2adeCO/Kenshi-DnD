using System.Diagnostics;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class Combat
    {
        CombatWindow window;
        Hero[] heroes;
        Monster[] enemies;
        Dice myDice;
        Inventory myInventory;
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
            this.myInventory = myInventory;
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

        public void NextTurn(ITurnable fighterTarget)
        {
            ITurnable attacker = turnOrder[turnIndex];
            window.UpdateLogUI(attacker.GetName() + " medita su acción");
            if (fighterTarget is Hero && attacker is Hero)
            {
                Hero hero = (Hero)attacker;
                hero.SetBuff(new StatModifier(0, 0, 0, 0, 0));
                InteractWithHero(hero, (Hero)fighterTarget);
            }

            if (attacker is Hero)
            {
                Hero hero = (Hero)attacker;
                hero.SetBuff(new StatModifier(0, 0, 0, 0, 0));

                if (fighterTarget is Hero && attacker is Hero)
                {

                    InteractWithHero(hero, (Hero)fighterTarget);
                }
                else
                {

                    HeroAttacks(hero, (Monster)fighterTarget);
                }

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
        private void InteractWithHero(Hero hero, Hero receiver)
        {
            if (receiver.IsAlive())
            {
                if (hero.AreConsumableItems())
                {
                    ConsumableAction(hero, receiver);
                }
            }
            else
            {
                //Loot body
                Debug.WriteLine("Hero is dead");
                bool canRevive = false;
                if (hero.GetInventory().AreConsumableItems())
                {
                    Item[] consumableItems = hero.GetInventory().GetConsumables(2);
                    for (int i = 0; i < consumableItems.Length; i++)
                    {
                        MeleeItem meleeItem = (MeleeItem)consumableItems[i];
                        if (meleeItem.CanRevive())
                        {
                            canRevive = true;
                        }
                    }
                    if (canRevive)
                    {
                        window.UpdateLogUI("¡" + hero.GetName() + " revive a " + receiver.GetName() + "!");
                        ConsumableAction(hero, receiver);
                    }
                }



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
            window.UpdateLogUI(attacker.GetName() + " ataca a " + defender.GetName() + " y le hace " + hits + " puntos de daño");

        }
        public void HeroAttacks(Hero attacker, Monster defender)
        {
            bool noWeapon = true;
            if (attacker.AreConsumableItems())
            {
                window.UpdateLogUI(attacker.GetName() + " se medica...");
                attacker.SetBuff(ConsumableAction(attacker, attacker));
                if (attacker.GetBuff().ToString() != "")
                {
                    window.UpdateCombatStatsUI(attacker.GetName() + " recibe un buff de:\n" + attacker.GetBuff().ToString());
                }
            }
            if (attacker.AreRangedItems())
            {
                noWeapon = false;
                window.UpdateLogUI(attacker.GetName() + " coge distancia... y apunta a " + defender.GetName());
                RangeAttack(attacker, defender);
            }

            if (attacker.AreMeleeItems())
            {
                noWeapon = false;
                window.UpdateLogUI(attacker.GetName() + " arremete contra " + defender.GetName());
                MeleeAttack(attacker, defender);
            }
            if (noWeapon)
            {
                //In case the hero has no weapon, he will use martial arts
                //In Kenshi, Martial Arts are the default attack of having no weapons, and one of the most rewarding skills to train
                //Intented for late game, a good martial artist can kill a whole squad of enemies
                //If the hero is not good at martial arts, he will probably fail miserably
                //I will use limbs to determine the martial arts skill, so there's no other way to get better other than to get limbs torn off
                //and getting better ones. This will cause a struggle on the player, as only he can choose whether to train it or not because
                //there is no reason to really master martial arts other than personal satisfaction.
                window.UpdateLogUI(attacker.GetName() + " se pone a prueba en artes marciales contra " + defender.GetName());
                MartialArtsAttack(attacker, defender);
            }


        }
        private StatModifier ConsumableAction(Hero user, Hero receiver)
        {

            Item[] consumableItems = user.GetInventory().GetConsumables(2);

            int hpBoost = 0;
            int agilityBoost = 0;
            int bruteForceBoost = 0;
            int resistanceBoost = 0;
            int dexterityBoost = 0;
            for (int i = 0; i < consumableItems.Length; i++)
            {
                if (consumableItems[i] != null)
                {
                    window.UpdateLogUI(consumableItems[i].AnnounceUse());
                    hpBoost += consumableItems[i].GetStatToModify().GetHp();
                    agilityBoost += consumableItems[i].GetStatToModify().GetAgility();
                    bruteForceBoost += consumableItems[i].GetStatToModify().GetBruteForce();
                    resistanceBoost += consumableItems[i].GetStatToModify().GetResistance();
                    dexterityBoost += consumableItems[i].GetStatToModify().GetDexterity();
                    myInventory.RemoveItem(consumableItems[i]);
                    user.GetInventory().RemoveItem(consumableItems[i]);
                }
            }
            if (hpBoost > 0)
            {
                if (user != receiver)
                {
                    window.UpdateLogUI(user.GetName() + " cura a " + receiver.GetName() + " " + hpBoost + " puntos de vida");
                }
                else
                {
                    window.UpdateLogUI(user.GetName() + " se cura " + hpBoost + " puntos de vida");
                }
            }

            receiver.Heal(hpBoost);

            StatModifier statBuff = new StatModifier(bruteForceBoost, dexterityBoost, 0, resistanceBoost, agilityBoost);


            return statBuff;
        }
        private void MeleeAttack(Hero attacker, Monster defender)
        {
            int attackerStat;
            int defenderStat;
            int defenderHealth;
            Item[] uncastedMeleeItems = attacker.GetInventory().GetMelee(2);
            for (int i = 0; i < uncastedMeleeItems.Length; i++)
            {
                window.UpdateLogUI(uncastedMeleeItems[i].AnnounceUse());
            }
            attackerStat = attacker.GetStat(1);
            defenderStat = defender.GetResistance();
            defenderHealth = defender.GetHp();



            if (defender.GetImmunity() == 2)
            {
                window.UpdateLogUI(defender.GetName() + " tiene inmunidad a ataques físicos, " + attacker.GetName() + " se replantea sus acciones...");
                return;
            }
            else
            {
                if (defender.GetImmunity() == -3 || defender.GetImmunity() == 1)
                {
                    window.UpdateLogUI(defender.GetName() + " tiene resistencia a ataques físicos, " + attacker.GetName() + " lo intenta igualmente");
                    attackerStat /= 2;
                }
            }
            window.UpdateCombatStatsUI("Fuerza bruta de " + attacker.GetName() + ": " + attackerStat + "\n" +
                "Resistencia de " + defender.GetName() + ": " + defenderStat + "\n" +
                "Golpes necesarios para matar: " + defenderHealth);
            int hits = myDice.PlayDice(attackerStat - defenderStat);
            window.UpdateLogUI(attacker.GetName() + " propina " + hits + " golpes a " + defender.GetName());
            window.UpdateDicesUI(myDice.GetRollHistory());

            if (hits >= defenderHealth)
            {
                window.UpdateLogUI("¡" + attacker.GetName() + " rebana a " + defender.GetName() + "!");
                defender.SetHp(0);
            }
            else
            {
                window.UpdateLogUI(attacker.GetName() + " no logra matar a " + defender.GetName());
            }
        }
        private void RangeAttack(Hero attacker, Monster defender)
        {
            Debug.WriteLine(attacker.GetName() + " attacks " + defender.GetName());
            int attackerStat;
            int defenderStat = defender.GetResistance();
            int defenderHealth = defender.GetHp();
            int defenderAgility = defender.GetAgility();
            attackerStat = attacker.GetStat(2);

            Item[] uncastedRangedItems = attacker.GetInventory().GetRanged(2);

            RangedItem[] rangedItems = new RangedItem[uncastedRangedItems.Length];

            for (int i = 0; i < uncastedRangedItems.Length; i++)
            {
                rangedItems[i] = (RangedItem)uncastedRangedItems[i];
                window.UpdateLogUI(rangedItems[i].AnnounceUse());
            }

            int misses = 0;
            int damage = 0;
            int emptyAmmoWeapons = 0;
            int permittedMisses = myDice.PlayDice(attackerStat / 3);
            window.UpdateDicesUI(myDice.GetRollHistory());
            window.UpdateCombatStatsUI("Fallos permitidos: " + permittedMisses +
                "\nDestreza de " + attacker.GetName() + ": " + attackerStat + "\n" +
                "Resistencia de " + defender.GetName() + ": " + defenderStat + "\n" +
                "Agilidad de " + defender.GetName() + ": " + defenderAgility + "\n" +
                "Dados necesarios para matar: " + defenderHealth);

            do
            {
                emptyAmmoWeapons = 0;
                for (int i = 0; i < rangedItems.Length; i++)
                {
                    if (rangedItems[i].GetAmmo() <= 0)
                    {
                        emptyAmmoWeapons += 1;
                    }
                    else
                    {
                        int diceNum = myDice.PlayDice(attackerStat - (rangedItems[i].GetDifficulty() + (defenderAgility / 2)));
                        window.UpdateDicesUI(myDice.GetRollHistory());

                        if (diceNum >= rangedItems[i].GetDifficulty())
                        {
                            damage += rangedItems[i].GetStatToModify().GetBruteForce();
                            window.UpdateLogUI(attacker.GetName() + " acierta y hace " + rangedItems[i].GetStatToModify().GetBruteForce() + " de daño");
                            rangedItems[i].ShootAmmo();
                        }
                        else
                        {
                            window.UpdateLogUI(defender.GetName() + " logra esquivar");
                            misses += 1;
                            rangedItems[i].ShootAmmo();
                        }
                        if (rangedItems[i].GetAmmo() == 0)
                        {
                            window.UpdateLogUI(rangedItems[i].GetName() + " se queda sin munición...");
                        }
                    }

                }
            } while (misses <= permittedMisses && emptyAmmoWeapons != rangedItems.Length);

            Debug.WriteLine("daño" + damage);

            if (defender.GetImmunity() == -2)
            {
                window.UpdateLogUI(defender.GetName() + " tiene inmunidad a ataques a distancia, " + attacker.GetName() + " derrochó munición...");
                return;
            }
            else
            {
                if (defender.GetImmunity() == -1 || defender.GetImmunity() == 3)
                {
                    window.UpdateLogUI(defender.GetName() + " tiene resistencia a ataques a distancia, " + attacker.GetName() + " lo intenta igualmente");
                    damage /= 2;
                }
            }


            if (myDice.PlayDice(damage - defenderStat) >= defenderHealth)
            {
                window.UpdateDicesUI(myDice.GetRollHistory());
                window.UpdateLogUI("¡" + attacker.GetName() + " deja como un colador a " + defender.GetName() + "!");
                defender.SetHp(0);
            }
            else
            {
                window.UpdateDicesUI(myDice.GetRollHistory());
                window.UpdateLogUI(attacker.GetName() + " no dejó rasguños en " + defender.GetName());
            }
        }
        private void MartialArtsAttack(Hero attacker, Monster defender)
        {
            int attackerStat = attacker.GetMartialArtStat();
            int defenderStat = defender.GetResistance();
            int defenderHealth = defender.GetHp();
            int damage = 0;
            Debug.WriteLine("Martial art: " + attackerStat);
            switch (attackerStat)
            {
                case < 20:
                    {
                        Limb limbUsed = attacker.GetLimbs()[rnd.Next(0, 4)];

                        int limbStat = limbUsed.GetBruteForce() + limbUsed.GetDexterity();

                        int hitChances = attacker.GetStat(5);

                        window.UpdateCombatStatsUI("Miembro usado: " + limbUsed.GetName() + "\n" +
                            "Daño de miembro: " + limbStat + "\n" +
                            "Posibilidades de golpear: " + hitChances + "\n" +
                            "Resistencia de " + defender.GetName() + ": " + defenderStat + "\n" +
                            "Daño necesario para acabar con " + defender.GetName() + ": " + defenderHealth);


                        for (int i = hitChances; i > 0; i -= 1)
                        {
                            int hit = myDice.PlayDice(limbStat - defenderStat);
                            window.UpdateDicesUI(myDice.GetRollHistory());
                            if (hit >= defenderStat)
                            {
                                int currentDamage = rnd.Next(1, limbStat);
                                damage += currentDamage;
                                window.UpdateLogUI(attacker.GetName() + " hace que " + defender.GetName() + " se coma un nudillo con " + currentDamage + " de daño");
                            }
                            else
                            {
                                window.UpdateLogUI(defender.GetName() + " logra esquivar un puñetazo");
                            }
                        }
                        window.UpdateLogUI(attacker.GetName() + " hace un daño total de " + damage);
                        if (damage >= defenderHealth)
                        {
                            window.UpdateLogUI("¡" + attacker.GetName() + " deja a " + defender.GetName() + " en el suelo!");
                            defender.SetHp(0);
                        }
                        else
                        {
                            window.UpdateLogUI(attacker.GetName() + " hace reír a " + defender.GetName());
                        }

                        break;
                    }
                case < 30:
                    {
                        Limb[] limbsUsed = new Limb[2];
                        limbsUsed[0] = attacker.GetLimbs()[rnd.Next(0, 4)];
                        limbsUsed[1] = attacker.GetLimbs()[rnd.Next(0, 4)];


                        int limbStat = 0;

                        for (int i = 0; i < limbsUsed.Length; i++)
                        {
                            limbStat += limbsUsed[i].GetBruteForce() + limbsUsed[i].GetDexterity();
                        }
                        int hitChances = attacker.GetStat(5);

                        string limbsUsedNames = "\n";
                        for (int i = 0; i < limbsUsed.Length; i++)
                        {
                            limbsUsedNames += limbsUsed[i].GetName() + "\n";
                        }


                        window.UpdateCombatStatsUI("Miembros usados: " + limbsUsedNames + "\n" +
                            "Daño de miembros: " + limbStat + "\n" +
                            "Posibilidades de golpear: " + hitChances + "\n" +
                            "Resistencia de " + defender.GetName() + ": " + defenderStat + "\n" +
                            "Daño necesario para acabar con " + defender.GetName() + ": " + defenderHealth);


                        for (int i = hitChances; i > 0; i -= 1)
                        {
                            int hit = myDice.PlayDice(limbStat - defenderStat);
                            window.UpdateDicesUI(myDice.GetRollHistory());
                            if (hit >= defenderStat)
                            {
                                int currentDamage = rnd.Next(1, limbStat);
                                damage += currentDamage;
                                window.UpdateLogUI(attacker.GetName() + " repatea a " + defender.GetName() + " con carrerilla y hace " + currentDamage + " de daño");
                            }
                            else
                            {
                                window.UpdateLogUI(defender.GetName() + " logra esquivar por los pelos");
                            }
                        }
                        window.UpdateLogUI(attacker.GetName() + " hace un daño total de " + damage);
                        if (damage >= defenderHealth)
                        {
                            window.UpdateLogUI("¡" + attacker.GetName() + " deja a " + defender.GetName() + " con la boca partida!");
                            defender.SetHp(0);
                        }
                        else
                        {
                            window.UpdateLogUI(attacker.GetName() + " se cansa de tanto fallar ante " + defender.GetName());
                        }


                        break;
                    }
                case < 50:
                    {
                        Limb[] limbsUsed = new Limb[3];
                        limbsUsed[0] = attacker.GetLimbs()[rnd.Next(0, 4)];
                        limbsUsed[1] = attacker.GetLimbs()[rnd.Next(0, 4)];
                        limbsUsed[2] = attacker.GetLimbs()[rnd.Next(0, 4)];

                        int limbStat = 0;
                        for (int i = 0; i < limbsUsed.Length; i++)
                        {
                            limbStat += limbsUsed[i].GetBruteForce() + limbsUsed[i].GetDexterity();
                        }
                        int hitChances = attacker.GetStat(5);

                        string limbsUsedNames = "\n";
                        for (int i = 0; i < limbsUsed.Length; i++)
                        {
                            limbsUsedNames += limbsUsed[i].GetName() + "\n";
                        }


                        window.UpdateCombatStatsUI("Miembros usados: " + limbsUsedNames + "\n" +
                            "Daño de miembros: " + limbStat + "\n" +
                            "Posibilidades de golpear: " + hitChances + "\n" +
                            "Resistencia de " + defender.GetName() + ": " + defenderStat + "\n" +
                            "Daño necesario para acabar con " + defender.GetName() + ": " + defenderHealth);

                        for (int i = hitChances; i > 0; i -= 1)
                        {
                            int hit = myDice.PlayDice(limbStat - defenderStat);
                            window.UpdateDicesUI(myDice.GetRollHistory());
                            if (hit >= defenderStat)
                            {
                                int currentDamage = rnd.Next(1, limbStat);
                                damage += currentDamage;
                                window.UpdateLogUI(attacker.GetName() + " agarra a " + defender.GetName() + " y lo hace morder el polvo, " + currentDamage + " de daño");
                            }
                            else
                            {
                                window.UpdateLogUI(defender.GetName() + " se zafa del agarre");
                            }
                        }
                        window.UpdateLogUI(attacker.GetName() + " hace un daño total de " + damage);
                        if (damage >= defenderHealth)
                        {
                            window.UpdateLogUI("¡" + attacker.GetName() + " deja a " + defender.GetName() + " un nudo de pescador!");
                            defender.SetHp(0);
                        }
                        else
                        {
                            window.UpdateLogUI(attacker.GetName() + " se aleja de " + defender.GetName() + " tras tanto meneo");
                        }

                        break;
                    }
                default:
                    {
                        Limb[] limbsUsed = new Limb[4];
                        limbsUsed[0] = attacker.GetLimbs()[rnd.Next(0, 4)];
                        limbsUsed[1] = attacker.GetLimbs()[rnd.Next(0, 4)];
                        limbsUsed[2] = attacker.GetLimbs()[rnd.Next(0, 4)];
                        limbsUsed[3] = attacker.GetLimbs()[rnd.Next(0, 4)];

                        int limbStat = 0;
                        for (int i = 0; i < limbsUsed.Length; i++)
                        {
                            limbStat += limbsUsed[i].GetBruteForce() + limbsUsed[i].GetDexterity();
                        }
                        int hitChances = attacker.GetStat(5);

                        string limbsUsedNames = "\n";
                        for (int i = 0; i < limbsUsed.Length; i++)
                        {
                            limbsUsedNames += limbsUsed[i].GetName() + "\n";
                        }


                        window.UpdateCombatStatsUI("Miembros usados: " + limbsUsedNames + "\n" +
                            "Daño de miembros: " + limbStat + "\n" +
                            "Posibilidades de golpear: " + hitChances + "\n" +
                            "Resistencia de " + defender.GetName() + ": " + defenderStat + "\n" +
                            "Daño necesario para acabar con " + defender.GetName() + ": " + defenderHealth);


                        for (int i = hitChances; i > 0; i -= 1)
                        {
                            int hit = myDice.PlayDice(limbStat - defenderStat);
                            window.UpdateDicesUI(myDice.GetRollHistory());
                            if (hit >= defenderStat)
                            {
                                int currentDamage = rnd.Next(1, limbStat);
                                damage += currentDamage;
                                window.UpdateLogUI(attacker.GetName() + " salta y golpea " + currentDamage + " veces en el aire a " + defender.GetName());
                            }
                            else
                            {
                                window.UpdateLogUI(defender.GetName() + " rueda por el suelo y esquiva");
                            }
                        }
                        window.UpdateLogUI(attacker.GetName() + " hace un daño total de " + damage);
                        if (damage >= defenderHealth)
                        {
                            window.UpdateLogUI("¡" + attacker.GetName() + " lanza a " + defender.GetName() + " al aire y lo remata con 50 toques!");
                            defender.SetHp(0);
                        }
                        else
                        {
                            window.UpdateLogUI(attacker.GetName() + " se pregunta que maestro a enseñado artes marciales a " + defender.GetName());
                        }

                        break;
                    }
            }

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
            Hero victim = heroesToAttack[rnd.Next(0, count)];

            window.UpdateLogUI(GetCurrentAttacker().GetName() + " pone los ojos en " + victim.GetName());
            return victim;
        }
        public int GetGameState()
        {
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
