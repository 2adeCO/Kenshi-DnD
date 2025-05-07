using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;

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
        public Combat(Hero[] newHeroes, Monster[] newEnemies, Dice myDice, Inventory myInventory, Random random, CombatWindow window)
        {
            this.window = window;
            combatState = 0;
            rnd = random;
            this.myInventory = myInventory;
            turnIndex = 0;
            this.heroes = newHeroes;
            this.enemies = newEnemies;
            this.myDice = myDice;

            everyTurn = new List<Turn>();
            for (int i = 0; i < heroes.Length; i+=1)
            {
                everyTurn.Add(new Turn(heroes[i]));
            }
            for (int i = 0; i < enemies.Length; i+=1)
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
                for (int i = 0; i < everyTurn.Count; i+=1)
                {
                    if (everyTurn[i].GetFighter().IsAlive())
                    {
                        everyTurn[i].AdvanceTurn();
                    }
                }
                for (int i = 0; i < everyTurn.Count; i+=1)
                {
                    while (everyTurn[i].IsTurnComplete())
                    {
                        tempList.Add(everyTurn[i].GetFighter());
                        numOfAddedTurns += 1;
                    }
                }
                tempList = SortByAgility(tempList);
                for (int i = 0; i < tempList.Count; i+=1)
                {
                    Debug.WriteLine("Added to turnorder: " + tempList[i].GetName() + debugCounter);
                    turnOrder.Add(tempList[i]);
                }
                //If numOfTurns is 3, and in the same AdvanceTurn, more than 3 complete the attack
                //Every one will pass
            } while (numOfAddedTurns <= numOfTurns);
            Debug.WriteLine("Turn order decided for " + numOfAddedTurns + " turns");
        }

        public async Task NextTurn(ITurnable fighterTarget)
        {
            ITurnable attacker = turnOrder[turnIndex];
            await window.UpdateLogUI(attacker.GetName() + " medita su acción", 400);
            
            

            if (attacker is Hero)
            {
                Hero hero = (Hero)attacker;
                hero.SetBuff(new StatModifier(0, 0, 0, 0, 0));

                if (fighterTarget is Hero && attacker is Hero)
                {
                    hero = (Hero)attacker;
                    hero.SetBuff(new StatModifier(0, 0, 0, 0, 0));
                    await InteractWithHero(hero, (Hero)fighterTarget);
                }
                else
                {

                    await HeroAttacks(hero, (Monster)fighterTarget);
                }

            }
            else
            {
                Hero victim = DecideVictim();
                await window.UpdateLogUI(GetCurrentAttacker().GetName() + " pone los ojos en " + victim.GetName(), 400);
                await MonsterAttacks((Monster)attacker, victim);
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
        private async Task InteractWithHero(Hero hero, Hero receiver)
        {
            if (receiver.IsAlive())
            {
                if (hero.AreConsumableItems())
                {
                    await ConsumableAction(hero, receiver);
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
                    for (int i = 0; i < consumableItems.Length; i+=1)
                    {
                        MeleeItem meleeItem = (MeleeItem)consumableItems[i];
                        if (meleeItem.CanRevive())
                        {
                            canRevive = true;
                        }
                    }
                    if (canRevive)
                    {
                        await window.UpdateLogUI("@920@¡" + hero.GetName() + " @@220@revive@ @920@a " + receiver.GetName() + "!@",1200);
                        await ConsumableAction(hero, receiver);
                    }
                }
                else
                {
                    LootBody(hero, receiver);
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
        private async Task LootBody(Hero hero, Hero corpse)
        {
            int looterStat = hero.GetStat(Stats.Stat.Agility) + (hero.GetStat(Stats.Stat.Dexterity) / 2);

            await window.UpdateCombatStatsUI("@9@Habilidad de robo@ de " + hero.GetName() + ": " + looterStat, 0);

            if(myDice.PlayDice(looterStat) > 3)
            {
                corpse.FreeAllItems();
                await window.UpdateLogUI(hero.GetName() + " revolvió el cuerpo inconsciente de " + corpse.GetName() +
                    " y consiguió recuperar: " + corpse.FreeAllItems(), 0);


            }
            else
            {
                await window.UpdateLogUI(hero.GetName() + " se tropieza y vuelve a su posición..." ,0);
            }
                await window.UpdateDicesUI(myDice.GetRollHistory(), 1200);

        }
        public async Task MonsterAttacks(Monster attacker, Hero defender)
        {


            int attackerStat;
            int defenderStat;

            attackerStat = attacker.GetStrength();
            defenderStat = defender.GetStat(Stats.Stat.Resistance);
            await window.UpdateCombatStatsUI("@9@Fuerza@ de " + attacker.GetName() + ": " + attackerStat +"\n" +
                "@9@Resistencia@ de " + defender.GetName() + ": " + defenderStat +
                "\n@9@Vida actual@ de " + defender.GetName() + ": " + defender.GetHp(),0);

            int hits = attackerStat;

            //Defender(hero) throws a dice, protects 1 hp for each pair of dice wins. Example: Hero has 3 resistance,
            //and attacker has 1 strength, so defender throws two dices, if the two are won, he will take 2 damage instead of 3
            int hitsDefense = myDice.PlayDice(defenderStat - attackerStat) /2;

            await window.UpdateDicesUI(myDice.GetRollHistory(), 0);
            if (hitsDefense == 0)
            {
                await window.UpdateLogUI(defender.GetName() + " no es capaz de parar ningún golpe", 400);

            }
            else
            {
                await window.UpdateLogUI( defender.GetName() + " consigue bloquear " + hitsDefense + " de daño", 400);
            }

            hits = hits - hitsDefense;

            await window.UpdateLogUI(attacker.GetName() + " golpea y hace " + hits + " de daño a " + defender.GetName(), 400);

            defender.Hurt(hits);

			Debug.WriteLine("toughness/2: " + defender.GetToughness() / 2);
            Debug.WriteLine("hits/4: " +hits / 4);
			if (hits > defender.GetToughness() / 2)
			{
                Debug.WriteLine("Might lose an arm");
				if (myDice.PlayDice(hits / 4) > 2)
				{
					Limb limbLost = defender.RemoveLimb(rnd.Next(0, 5));
					await window.UpdateLogUI("El " + limbLost.GetName() + " de " + defender.GetName() + " @120@voló por los aires@...",0);
                    await window.UpdateDicesUI(myDice.GetRollHistory(), 1200);
				}
			}


			if (defender.GetHp() <= 0)
            {

                await window.UpdateLogUI(attacker.GetName() + " @120@deja KO@ a " + defender.GetName(), 1200);
                
            }
            else
            {
                await window.UpdateLogUI("¡"+ defender.GetName() + " resiste!", 400);

            }

        }
        public async Task HeroAttacks(Hero attacker, Monster defender)
        {
            bool noWeapon = true;
            if (attacker.AreConsumableItems())
            {
                await window.UpdateLogUI(attacker.GetName() + " se medica..." , 400);
                attacker.SetBuff(await ConsumableAction(attacker, attacker));
                if (attacker.GetBuff().ToString() != "")
                {
                    await window.UpdateCombatStatsUI("¡" + attacker.GetName() + " está @3@dopado@!",800);
                }
            }
            if (attacker.AreRangedItems())
            {
                noWeapon = false;
                await window.UpdateLogUI(attacker.GetName() + " coge distancia... y apunta a " + defender.GetName(), 800);
                await RangeAttack(attacker, defender);
            }

            if (attacker.AreMeleeItems())
            {
                noWeapon = false;
                await window.UpdateLogUI(attacker.GetName() + " arremete contra " + defender.GetName(), 800);
                await MeleeAttack(attacker, defender);
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
                await window.UpdateLogUI(attacker.GetName() + " se pone a prueba en artes marciales contra " + defender.GetName(), 800);
                await MartialArtsAttack(attacker, defender);
            }

            if (!defender.IsAlive())
            {
                if(defender.GetItemDrop() != null)
                {
                    await window.UpdateLogUI("Se encontró " + defender.GetItemDrop().GetName() + " en el cadáver...",800);
                    myInventory.AddItem(defender.GetItemDrop());
                }
                if(defender.GetXpDrop() == 0)
                {
					await window.UpdateLogUI("No se consiguió nada de experiencia", 300);
				}
				await window.UpdateLogUI("¡" + attacker.GetName() + " ha conseguido @214@" + defender.GetXpDrop() + " puntos de experiencia@!", 800);
                if (attacker.GainXp(defender.GetXpDrop()))
                {
                    await window.UpdateLogUI("¡Y sube a @218@NIVEL " + attacker.GetLevel()+"@!",800);
                }
			}


        }
        private async Task<StatModifier> ConsumableAction(Hero user, Hero receiver)
        {

            Item[] consumableItems = user.GetInventory().GetConsumables(2);

            int hpBoost = 0;
            int agilityBoost = 0;
            int bruteForceBoost = 0;
            int resistanceBoost = 0;
            int dexterityBoost = 0;
            for (int i = 0; i < consumableItems.Length; i+=1)
            {
                if (consumableItems[i] != null)
                {
                    await window.UpdateLogUI(consumableItems[i].AnnounceUse() ,200);
                    hpBoost += consumableItems[i].GetStatToModify().GetHp();
                    agilityBoost += consumableItems[i].GetStatToModify().GetAgility();
                    bruteForceBoost += consumableItems[i].GetStatToModify().GetBruteForce();
                    resistanceBoost += consumableItems[i].GetStatToModify().GetResistance();
                    dexterityBoost += consumableItems[i].GetStatToModify().GetDexterity();

                    myInventory.RemoveItem(consumableItems[i]);
                    user.RemoveItemFromInventory(consumableItems[i]);
                }
            }
            if (hpBoost > 0)
            {
                if (user != receiver)
                {
                    await window.UpdateLogUI(user.GetName() + " cura a " + receiver.GetName() + " " + hpBoost + " puntos de vida" ,400);
                }
                else
                {
                    await window.UpdateLogUI(user.GetName() + " se cura " + hpBoost + " puntos de vida", 400);
                }
            }

            receiver.Heal(hpBoost);

            StatModifier statBuff = new StatModifier(bruteForceBoost, dexterityBoost, 0, resistanceBoost, agilityBoost);


            return statBuff;
        }
        private async Task MeleeAttack(Hero attacker, Monster defender)
        {
            int attackerStat;
            int defenderStat;
            int defenderHealth;
            Item[] uncastedMeleeItems = attacker.GetInventory().GetMelee(2);
            for (int i = 0; i < uncastedMeleeItems.Length; i+=1)
            {
                await window.UpdateLogUI(uncastedMeleeItems[i].AnnounceUse(),200);
            }
            attackerStat = attacker.GetStat(Stats.Stat.BruteForce);
            defenderStat = defender.GetResistance();
            defenderHealth = defender.GetHp();



            if (defender.GetImmunity() == Immunities.Immunity.ImmuneToMeleeAndResistantToRanged || defender.GetImmunity() == Immunities.Immunity.ImmuneToMelee)
            {
                await window.UpdateLogUI(defender.GetName() + " tiene inmunidad a ataques físicos, " + attacker.GetName() + " se replantea sus acciones...",800);
                return;
            }
            else
            {
                if (defender.GetImmunity() == Immunities.Immunity.ResistantToMelee ||
                    defender.GetImmunity() == Immunities.Immunity.ResistantToBoth ||
                    defender.GetImmunity() == Immunities.Immunity.ImmunteToRangedAndResistantToMelee)
                {
                    await window.UpdateLogUI(defender.GetName() + " tiene resistencia a ataques físicos, " + attacker.GetName() + " lo intenta igualmente",800);
                    attackerStat /= 2;
                }
            }
            await window.UpdateCombatStatsUI("@9@Fuerza bruta@ de " + attacker.GetName() + ": " + attackerStat + "\n" +
                "@9@Resistencia@ de " + defender.GetName() + ": " + defenderStat + "\n" +
                "Golpes necesarios para matar: " + defenderHealth,0);
            
            int hits = myDice.PlayDice(attackerStat - defenderStat);
            await window.UpdateLogUI(attacker.GetName() + " propina " + hits + " golpes a " + defender.GetName(), 0);
            await window.UpdateDicesUI(myDice.GetRollHistory() , 1200);

            if (hits >= defenderHealth)
            {
                await window.UpdateLogUI("¡" + attacker.GetName() + " @120@rebana a@ " + defender.GetName() + "!", 1200);
                defender.SetHp(0);
            }
            else
            {
                await window.UpdateLogUI(attacker.GetName() + " no logra matar a " + defender.GetName(), 800);
            }
        }
        private async Task RangeAttack(Hero attacker, Monster defender)
        {
            Debug.WriteLine(attacker.GetName() + " attacks " + defender.GetName());
            int defenderStat = defender.GetResistance();
            int defenderHealth = defender.GetHp();
            int defenderAgility = defender.GetAgility();
            int attackerStat = attacker.GetStat(Stats.Stat.Dexterity);

            Item[] uncastedRangedItems = attacker.GetInventory().GetRanged(2);

            RangedItem[] rangedItems = new RangedItem[uncastedRangedItems.Length];

            for (int i = 0; i < uncastedRangedItems.Length; i+=1)
            {
                rangedItems[i] = (RangedItem)uncastedRangedItems[i];
                await window.UpdateLogUI(rangedItems[i].AnnounceUse(), 200);
            }

            int misses = 0;
            int damage = 0;
            int emptyAmmoWeapons = 0;
            int permittedMisses = myDice.PlayDice(attackerStat / 3);
            await window.UpdateDicesUI(myDice.GetRollHistory(), 0);
            await window.UpdateCombatStatsUI("Fallos permitidos: " + permittedMisses +
                "\n@9@Destreza@ de " + attacker.GetName() + ": " + attackerStat + "\n" +
                "@9@Resistencia@ de " + defender.GetName() + ": " + defenderStat + "\n" +
                "@9@Agilidad@ de " + defender.GetName() + ": " + defenderAgility + "\n" +
                "Dados necesarios para matar: " + defenderHealth,400);

            do
            {
                emptyAmmoWeapons = 0;
                for (int i = 0; i < rangedItems.Length; i+=1)
                {
                    if (rangedItems[i].GetAmmo() <= 0)
                    {
                        emptyAmmoWeapons += 1;
                    }
                    else
                    {
                        int diceNum = myDice.PlayDice(attackerStat - (rangedItems[i].GetDifficulty() + (defenderAgility / 2)));
                        await window.UpdateDicesUI(myDice.GetRollHistory(),0);

                        if (diceNum >= rangedItems[i].GetDifficulty())
                        {
                            damage += rangedItems[i].GetStatToModify().GetBruteForce();
                            await window.UpdateLogUI(attacker.GetName() + " acierta y hace " + rangedItems[i].GetStatToModify().GetBruteForce() + " de daño",400);
                            rangedItems[i].ShootAmmo();
                        }
                        else
                        {
                            await window.UpdateLogUI(defender.GetName() + " logra esquivar", 400);
                            misses += 1;
                            rangedItems[i].ShootAmmo();
                        }
                        if (rangedItems[i].GetAmmo() == 0)
                        {
                            await window.UpdateLogUI(rangedItems[i].GetName() + " se queda sin munición..." , 400);
                        }
                    }

                }
            } while (misses <= permittedMisses && emptyAmmoWeapons != rangedItems.Length);

            Debug.WriteLine("daño" + damage);
            if(emptyAmmoWeapons == rangedItems.Length)
            {
                await window.UpdateLogUI(attacker.GetName() + " gastó la munición de todas sus armas...", 800);

            }
            if (misses >= permittedMisses)
            {
                await window.UpdateLogUI(attacker.GetName() + " falló demasiado...", 800);
            }

            if (defender.GetImmunity() == Immunities.Immunity.ImmunteToRangedAndResistantToMelee ||
                defender.GetImmunity() == Immunities.Immunity.ImmuneToRanged)
            {
                await window.UpdateLogUI(defender.GetName() + " tiene inmunidad a ataques a distancia, " + attacker.GetName() + " derrochó munición...",1200);
                return;
            }
            else
            {
                if (defender.GetImmunity() == Immunities.Immunity.ResistantToBoth ||
                defender.GetImmunity() == Immunities.Immunity.ResistantToRanged ||
                defender.GetImmunity() == Immunities.Immunity.ImmuneToMeleeAndResistantToRanged)
                {
                    await window.UpdateLogUI(defender.GetName() + " tiene resistencia a ataques a distancia, " + attacker.GetName() + " lo intenta igualmente",1200);
                    damage /= 2;
                }
            }


            if (myDice.PlayDice(damage - defenderStat) >= defenderHealth)
            {
                await window.UpdateDicesUI(myDice.GetRollHistory(), 0);
                await window.UpdateLogUI("¡" + attacker.GetName() + " @120@deja como un colador a@ " + defender.GetName() + "!", 1200);
                defender.SetHp(0);
            }
            else
            {
                await window.UpdateDicesUI(myDice.GetRollHistory(), 0);
                await window.UpdateLogUI(attacker.GetName() + " no dejó rasguños en " + defender.GetName(), 800);
            }
        }
        private async Task MartialArtsAttack(Hero attacker, Monster defender)
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
                        Limb limbUsed = attacker.GetRandomLimb(rnd);

                        int limbStat = limbUsed.GetBruteForce() + limbUsed.GetDexterity();

                        int hitChances = attacker.GetStat(Stats.Stat.Agility);

                        await window.UpdateCombatStatsUI("Miembro usado: " + limbUsed.GetName() + "\n" +
                            "Daño de miembro: " + limbStat + "\n" +
                            "Posibilidades de golpear: " + hitChances + "\n" +
                            "@9@Resistencia@ de " + defender.GetName() + ": " + defenderStat + "\n" +
                            "Daño necesario para acabar con " + defender.GetName() + ": " + defenderHealth, 0);


                        for (int i = hitChances; i > 0; i -= 1)
                        {
                            int hit = myDice.PlayDice(limbStat - defenderStat);
                            await window.UpdateDicesUI(myDice.GetRollHistory(), 0);
                            if (hit >= defenderStat)
                            {
                                int currentDamage = rnd.Next(1, limbStat);
                                damage += currentDamage;
                                await window.UpdateLogUI(attacker.GetName() + " hace que " + defender.GetName() + " se coma un nudillo con " + currentDamage + " de daño", 400);
                            }
                            else
                            {
                                await window.UpdateLogUI(defender.GetName() + " logra esquivar un puñetazo", 400);
                            }
                        }
                        await window.UpdateLogUI(attacker.GetName() + " hace un daño total de " + damage, 800);
                        if (damage >= defenderHealth)
                        {
                            await window.UpdateLogUI("¡" + attacker.GetName() + " @120@deja a@ " + defender.GetName() + " @120@en el suelo@!", 1200);
                            defender.SetHp(0);
                        }
                        else
                        {
                            await window.UpdateLogUI(attacker.GetName() + " hace reír a " + defender.GetName(), 800);
                        }

                        break;
                    }
                case < 30:
                    {
                        Limb[] limbsUsed = new Limb[2];
                        limbsUsed[0] = attacker.GetRandomLimb(rnd);
                        limbsUsed[1] = attacker.GetRandomLimb(rnd);


                        int limbStat = 0;

                        for (int i = 0; i < limbsUsed.Length; i+=1)
                        {
                            limbStat += limbsUsed[i].GetBruteForce() + limbsUsed[i].GetDexterity();
                        }
                        int hitChances = attacker.GetStat(Stats.Stat.Agility);

                        string limbsUsedNames = "\n";
                        for (int i = 0; i < limbsUsed.Length; i+=1)
                        {
                            limbsUsedNames += limbsUsed[i].GetName() + "\n";
                        }


                        await window.UpdateCombatStatsUI("Miembros usados: " + limbsUsedNames + "\n" +
                            "Daño de miembros: " + limbStat + "\n" +
                            "Posibilidades de golpear: " + hitChances + "\n" +
                            "@9@Resistencia@ de " + defender.GetName() + ": " + defenderStat + "\n" +
                            "Daño necesario para acabar con " + defender.GetName() + ": " + defenderHealth,0);


                        for (int i = hitChances; i > 0; i -= 1)
                        {
                            int hit = myDice.PlayDice(limbStat - defenderStat);
                            await window.UpdateDicesUI(myDice.GetRollHistory(), 0);
                            if (hit >= defenderStat)
                            {
                                int currentDamage = rnd.Next(1, limbStat);
                                damage += currentDamage;
                                await window.UpdateLogUI(attacker.GetName() + " repatea a " + defender.GetName() + 
                                    " con carrerilla y hace " + currentDamage + " de daño" , 400);
                            }
                            else
                            {
                                await window.UpdateLogUI(defender.GetName() + " logra esquivar por los pelos", 400);
                            }
                        }
                        await window.UpdateLogUI(attacker.GetName() + " hace un daño total de " + damage,800);
                        if (damage >= defenderHealth)
                        {
                            await window.UpdateLogUI("¡" + attacker.GetName() + " @120@deja a@ " + defender.GetName() + " @120@con la boca partida@!",1200);
                            defender.SetHp(0);
                        }
                        else
                        {
                            await window.UpdateLogUI(attacker.GetName() + " se cansa de tanto fallar ante " + defender.GetName(), 800);
                        }


                        break;
                    }
                case < 50:
                    {
                        Limb[] limbsUsed = new Limb[3];
                        limbsUsed[0] = attacker.GetRandomLimb(rnd);
                        limbsUsed[1] = attacker.GetRandomLimb(rnd);
                        limbsUsed[2] = attacker.GetRandomLimb(rnd);

                        int limbStat = 0;
                        for (int i = 0; i < limbsUsed.Length; i+=1)
                        {
                            limbStat += limbsUsed[i].GetBruteForce() + limbsUsed[i].GetDexterity();
                        }
                        int hitChances = attacker.GetStat(Stats.Stat.Agility);

                        string limbsUsedNames = "\n";
                        for (int i = 0; i < limbsUsed.Length; i+=1)
                        {
                            limbsUsedNames += limbsUsed[i].GetName() + "\n";
                        }


                        await window.UpdateCombatStatsUI("Miembros usados: " + limbsUsedNames + "\n" +
                            "Daño de miembros: " + limbStat + "\n" +
                            "Posibilidades de golpear: " + hitChances + "\n" +
                            "@9@Resistencia@ de " + defender.GetName() + ": " + defenderStat + "\n" +
                            "Daño necesario para acabar con " + defender.GetName() + ": " + defenderHealth,0);

                        for (int i = hitChances; i > 0; i -= 1)
                        {
                            int hit = myDice.PlayDice(limbStat - defenderStat);
                            await window.UpdateDicesUI(myDice.GetRollHistory(),0);
                            if (hit >= defenderStat)
                            {
                                int currentDamage = rnd.Next(1, limbStat);
                                damage += currentDamage;
                                await window.UpdateLogUI(attacker.GetName() + " agarra a " + defender.GetName() +
                                    " y lo hace morder el polvo, " + currentDamage + " de daño" , 400);
                            }
                            else
                            {
                                await window.UpdateLogUI(defender.GetName() + " se zafa del agarre", 400);
                            }
                        }
                        await window.UpdateLogUI(attacker.GetName() + " hace un daño total de " + damage,800);
                        if (damage >= defenderHealth)
                        {
                            await window.UpdateLogUI("¡" + attacker.GetName() + " @120@deja a@ " + defender.GetName() + " @120@un nudo de pescador@!",1200);
                            defender.SetHp(0);
                        }
                        else
                        {
                            await window.UpdateLogUI(attacker.GetName() + " se aleja de " + defender.GetName() + " tras tanto meneo", 800);
                        }

                        break;
                    }
                default:
                    {
                        Limb[] limbsUsed = new Limb[4];
                        limbsUsed[0] = attacker.GetRandomLimb(rnd);
                        limbsUsed[1] = attacker.GetRandomLimb(rnd);
                        limbsUsed[2] = attacker.GetRandomLimb(rnd);
                        limbsUsed[3] = attacker.GetRandomLimb(rnd);

                        int limbStat = 0;
                        for (int i = 0; i < limbsUsed.Length; i+=1)
                        {
                            limbStat += limbsUsed[i].GetBruteForce() + limbsUsed[i].GetDexterity();
                        }
                        int hitChances = attacker.GetStat(Stats.Stat.Agility);

                        string limbsUsedNames = "\n";
                        for (int i = 0; i < limbsUsed.Length; i+=1)
                        {
                            limbsUsedNames += limbsUsed[i].GetName() + "\n";
                        }


                        await window.UpdateCombatStatsUI("Miembros usados: " + limbsUsedNames + "\n" +
                            "Daño de miembros: " + limbStat + "\n" +
                            "Posibilidades de golpear: " + hitChances + "\n" +
                            "@9@Resistencia@ de " + defender.GetName() + ": " + defenderStat + "\n" +
                            "Daño necesario para acabar con " + defender.GetName() + ": " + defenderHealth, 0);


                        for (int i = hitChances; i > 0; i -= 1)
                        {
                            int hit = myDice.PlayDice(limbStat - defenderStat);
                            await window.UpdateDicesUI(myDice.GetRollHistory(), 0);
                            if (hit >= defenderStat)
                            {
                                int currentDamage = rnd.Next(1, limbStat);
                                damage += currentDamage;
                                await window.UpdateLogUI(attacker.GetName() + " salta y golpea " + currentDamage + " veces en el aire a " + defender.GetName(),400);
                            }
                            else
                            {
                                await window.UpdateLogUI(defender.GetName() + " rueda por el suelo y esquiva", 400);
                            }
                        }
                        await window.UpdateLogUI(attacker.GetName() + " hace un daño total de " + damage,800);
                        if (damage >= defenderHealth)
                        {
                            await window.UpdateLogUI("¡" + attacker.GetName() + " @120@lanza a@ " + defender.GetName() + " @120@al aire y lo remata con 50 toques@!", 1200);
                            defender.SetHp(0);
                        }
                        else
                        {
                            await window.UpdateLogUI(attacker.GetName() + " se pregunta que maestro a enseñado artes marciales a " + defender.GetName(), 800);
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
            for (int i = 0; i < heroes.Length; i+=1)
            {
                if (heroes[i].IsAlive())
                {
                    Debug.WriteLine("Victim counted " + i);
                    count+=1;
                }
            }
            Hero[] heroesToAttack = new Hero[count];
            count = 0;
            for (int i = 0; i < heroes.Length; i+=1)
            {
                if (heroes[i].IsAlive())
                {
                    Debug.WriteLine("People alive:" + i);
                    heroesToAttack[count] = heroes[i];
                    count += 1;
                }
            }
            Hero victim = heroesToAttack[rnd.Next(0, count)];

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

            for (int i = 0; i < heroes.Length; i+=1)
            {
                if (heroes[i].IsAlive())
                {
                    lost = false;
                }
            }
            for (int i = 0; i < enemies.Length; i+=1)
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
                for (int i = 0; i < list.Count - 1; i+=1)
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
        public ITurnable[] GetNTurns(int num)
        {
            ITurnable[] nextTurns = new ITurnable[num];
            int turnsAdded = 0;
            int i = 0;
            do
            {
                //If we look for fighters in the last 6 positions, generate more turns
                if ((turnOrder.Count - (turnIndex + i)) < 6)
                {
                    DecideNextNTurns(12, false);
                }
                //Add fighter if they are alive
                if (turnOrder[i + turnIndex].IsAlive())
                {
                    nextTurns[turnsAdded] = turnOrder[i + turnIndex];
                    turnsAdded += 1 ;
                }
                //Add value to iterator
                i += 1;
                

            } while (turnsAdded < num);
            
            return nextTurns;
        }
        
    }

}
