using System.Diagnostics;

namespace Kenshi_DnD
{
    // Class that manages the combat between heroes and monsters. - Santiago Cabrero
    class Combat
    {
        // CombatWindow is the UI that shows the combat information
        CombatWindow window;
        // Current adventure
        Adventure myAdventure;
        // Heroes and monsters involved in the combat
        Hero[] heroes;
        Monster[] enemies;
        // Dice used for combat rolls
        Dice myDice;
        // Inventory of the player
        Inventory myInventory;
        //Keeps track of every turn progress
        List<Turn> everyTurn;
        //Generates a list of fighters in the order they must attack
        List<ITurnable> turnOrder;
        // Index of the current turn in the turn order
        int turnIndex;
        // Random for dices and other random events
        Random rnd;
        // Combat state: 0 = combat in progress, 1 = combat won, -1 = combat lost
        int combatState;
        // Only constructor, decides the next 24 turns
        public Combat(Hero[] newHeroes, Monster[] newEnemies, Adventure myAdventure, Random random, CombatWindow window)
        {
            this.window = window;
            combatState = 0;
            rnd = random;
            this.myAdventure = myAdventure;
            this.myInventory = myAdventure.GetInventory();
            turnIndex = 0;
            this.heroes = newHeroes;
            this.enemies = newEnemies;
            this.myDice = myAdventure.GetDice();

            everyTurn = new List<Turn>();
            for (int i = 0; i < heroes.Length; i+=1)
            {
                everyTurn.Add(new Turn(heroes[i]));
            }
            for (int i = 0; i < enemies.Length; i+=1)
            {
                everyTurn.Add(new Turn(enemies[i]));
            }
            turnOrder = new List<ITurnable>();
            // Decides next 24 turns
            DecideNextNTurns(24);
            this.window = window;
        }
        // Returns current attacker, Hero or Monster
        public ITurnable GetCurrentAttacker()
        {
            return turnOrder[turnIndex];
        }
        // Decides the next N turns based on the agility of each fighter, the class Turn is used to keep track of the progress of each fighter's turn
        public void DecideNextNTurns(int numOfTurns)
        {
            // Sorts the fighters by agility, and adds them to the turn order
            int numOfAddedTurns = 0;
            List<ITurnable> tempList;
            
            do
            {
                // Makes a new list to store fighters that completed their turn
                tempList = new List<ITurnable>();
                // Advances all the turns
                for (int i = 0; i < everyTurn.Count; i+=1)
                {
                    if (everyTurn[i].GetFighter().IsAlive())
                    {
                        everyTurn[i].AdvanceTurn();
                    }
                }
                // Checks which fighters have completed their turn and adds them to the tempList
                for (int i = 0; i < everyTurn.Count; i+=1)
                {
                    // If a fighter has completed various turns, add them multiple times
                    while (everyTurn[i].IsTurnComplete())
                    {
                        tempList.Add(everyTurn[i].GetFighter());
                        numOfAddedTurns += 1;
                    }
                }
                // Sorts the tempList by agility
                tempList = SortByAgility(tempList);
                // Adds them to the actual fighter turn order
                for (int i = 0; i < tempList.Count; i+=1)
                {
                    turnOrder.Add(tempList[i]);
                }
                //If numOfTurns is 3, and in the same AdvanceTurn, more than 3 complete the attack
                //Every one will pass
            } while (numOfAddedTurns <= numOfTurns);
        }

        // Plays the turn, either a hero or a monster play
        public async Task NextTurn(ITurnable fighterTarget)
        {
            // Gets the current attacker from the turn order
            ITurnable attacker = turnOrder[turnIndex];
            await window.UpdateLogUI(attacker.GetName() + " medita su acción", 400);
            // If the attacker is a hero, it will interact with the target, if it's a monster, it will attack a random hero
            if (attacker is Hero)
            {
                Hero hero = (Hero)attacker;
                // Sets the buff of the hero to 0, as the buffs are temporary and restart when the turn starts
                hero.SetBuff(new StatModifier(0, 0, 0, 0, 0));

                // If the target is a hero, it will interact with the target, else, it will attack the monster
                if (fighterTarget is Hero && attacker is Hero)
                {
                    hero = (Hero)attacker;
                    hero.SetBuff(new StatModifier(0, 0, 0, 0, 0));
                    // Hero applies new buff to target hero
                    await InteractWithHero(hero, (Hero)fighterTarget);
                }
                else
                {
                    // Hero attacks monster
                    await HeroAttacks(hero, (Monster)fighterTarget);
                }

            }
            else
            {
                // Monster decides a victim from alive heroes
                Hero victim = DecideVictim();
                // Victim is only null if there are no heros alive left
                if(victim == null)
                {
                    return;
                }
                await window.UpdateLogUI(GetCurrentAttacker().GetName() + " pone los ojos en " + victim.GetName(), 400);
                // Monster attacks victim
                await MonsterAttacks((Monster)attacker, victim);
            }
            // Check if a group (monsters or heroes) is dead
            UpdateGameState();
            // If game has ended, returns
            if (GetGameState() != 0)
            {
                return;
            }
            // Advances the turn, and if the next turnable is dead, advances the turn
            turnIndex += 1;
            AdvanceTurnIfItsDead();
            // If the are less than 6 turns left, decide next 12 turns
            if (turnOrder.Count - turnIndex < 6)
            {
                DecideNextNTurns(12);
            }
        }
        // Current attacker is hero and interacts with hero target
        private async Task InteractWithHero(Hero hero, Hero receiver)
        {
            // If receiver is alive, and has consumable items, uses consumable items on receiver, else, loots its body
            if (receiver.IsAlive())
            {
                if (hero.AreConsumableItems())
                {
                    await ConsumableAction(hero, receiver);
                }
            }
            else
            {
                // Looks for items able to revive hero, else it loots its body
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
        // Advances the index
        public void AdvanceIndex()
        {
            turnIndex += 1;
            // If there are less than 6 turns left, generates 12 more
            if (turnOrder.Count - turnIndex < 6)
            {
                DecideNextNTurns(12);
            }
        }
        // Hero tries to get items from other hero
        private async Task LootBody(Hero hero, Hero corpse)
        {
            // Hero's looter stat
            int looterStat = hero.GetStat(Stats.Stat.Agility) + (hero.GetStat(Stats.Stat.Dexterity) / 2);

            await window.UpdateCombatStatsUI("@9@Habilidad de robo@ de " + hero.GetName() + ": " + looterStat, 200);
            // If Dice plays more than 3 successful rolls, the items are recovered for all the heroes
            if(myDice.PlayDice(looterStat,rnd) > 3)
            {
                string itemRecovered = corpse.FreeAllItems();
                await window.UpdateLogUI(hero.GetName() + " revolvió el cuerpo inconsciente de " + corpse.GetName() +
                    " y consiguió recuperar: " + (itemRecovered == "" ? " Ninguno" : itemRecovered), 0);

            }
            else
            {
                await window.UpdateLogUI(hero.GetName() + " se tropieza y vuelve a su posición..." ,0);
            }
                await window.UpdateDicesUI(myDice.GetRollHistory(), 1200);

        }
        // Monster attacks hero
        public async Task MonsterAttacks(Monster attacker, Hero defender)
        {
            int attackerStat = attacker.GetStrength();
            int defenderStat = defender.GetStat(Stats.Stat.Resistance);
            await window.UpdateCombatStatsUI("@9@Fuerza@ de " + attacker.GetName() + ": " + attackerStat +"\n" +
                "@9@Resistencia@ de " + defender.GetName() + ": " + defenderStat +
                "\n@9@Vida actual@ de " + defender.GetName() + ": " + defender.GetHp(),0);

            int hits = attackerStat;

            //Defender(hero) throws a dice, protects 1 hp for each pair of dice wins. Example: Hero has 3 resistance,
            //and attacker has 1 strength, so defender throws two dices, if the two are won, he will take 2 damage instead of 3
            int hitsDefense = myDice.PlayDice(defenderStat - attackerStat, rnd) /2;
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
            // Monster hurts the hero, if he levels up, then prints it on log
            if (defender.Hurt(hits)){
                await window.UpdateLogUI(defender.GetName() + " subió a @214@Nivel " + defender.GetLevel() + "@ @116@a base de golpes...@",400);
            }
            // If the attack is stronger than half of the hero's toughness, it will count as a hard hit, and has a chance of cutting a victim's limb
			if (hits > defender.GetToughness() / 2)
			{
                await window.UpdateLogUI("Ha sido muy doloroso", 400);
                // Chance of losing a limb 
				if (myDice.PlayDice(hits / 2, rnd) > 2)
				{
					Limb limbLost = defender.RemoveLimb(rnd.Next(0, 4));
					await window.UpdateLogUI("La " + limbLost.GetName() + " de " + defender.GetName() + " @120@voló por los aires@...",0);
                    await window.UpdateDicesUI(myDice.GetRollHistory(), 1200);
				}
			}

            // Tells if defender is dead or alive
			if (defender.GetHp() <= 0)
            {

                await window.UpdateLogUI(attacker.GetName() + " @120@deja KO@ a " + defender.GetName(), 1200);
                
            }
            else
            {
                await window.UpdateLogUI("¡"+ defender.GetName() + " resiste!", 400);

            }

        }
        // Hero attacks monster
        public async Task HeroAttacks(Hero attacker, Monster defender)
        {
            bool noWeapon = true;
            // Consumable action
            if (attacker.AreConsumableItems())
            {
                await window.UpdateLogUI(attacker.GetName() + " se medica..." , 400);
                attacker.SetBuff(await ConsumableAction(attacker, attacker));
                if (attacker.GetBuff().ToString() != "")
                {
                    await window.UpdateCombatStatsUI("¡" + attacker.GetName() + " está @3@dopado@!",800);
                }
            }
            // Ranged action
            if (attacker.AreRangedItems())
            {
                noWeapon = false;
                await window.UpdateLogUI(attacker.GetName() + " coge distancia... y apunta a " + defender.GetName(), 800);
                await RangeAttack(attacker, defender);
            }
            // Melee action
            if (attacker.AreMeleeItems())
            {
                noWeapon = false;
                await window.UpdateLogUI(attacker.GetName() + " arremete contra " + defender.GetName(), 800);
                await MeleeAttack(attacker, defender);
            }
            // Martial arts action
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
            // Gets rewards if the monster is dead
            if (!defender.IsAlive())
            {
                // If can drop item, it might get an Item
                if (defender.CanDropItem())
                {
                    Item itemDropped = GenerateItem();

                    if (itemDropped != null)
                    {
                        await window.UpdateLogUI("Se encontró @"+ itemDropped.GetRarityColor() + "@" + itemDropped.GetName() + "@ en el cadáver...", 800);
                        myInventory.AddItem(itemDropped);
                    }
                    else
                    {
                        await window.UpdateLogUI("No tiene ninguna pertenencia...", 800);
                    }

                }
                // Gets XP
                if (defender.GetXpDrop() == 0)
                {
                    await window.UpdateLogUI("No se consiguió nada de experiencia", 300);
                }
                else
                {
                    await window.UpdateLogUI("¡" + attacker.GetName() + " ha conseguido @214@" + defender.GetXpDrop() + " puntos de experiencia@!", 800);
                    if (attacker.GainXp(defender.GetXpDrop()))
                    {
                        await window.UpdateLogUI("¡Y sube a @218@NIVEL " + attacker.GetLevel() + "@!", 800);
                    }
                }
                // Gets Cats
                if(defender.GetCats() != 0)
                {
                    int cats = defender.GetPossibleCats(rnd);
                    await window.UpdateLogUI("¡" + attacker.GetName() + " consigue @214@" + cats + " cats@!", 800);
                    myAdventure.GainCats(cats);
                }
			}


        }
        // Adds every consumable item buff to receiver
        private async Task<StatModifier> ConsumableAction(Hero user, Hero receiver)
        {

            Item[] consumableItems = user.GetInventory().GetConsumables(2);

            int hpBoost = 0;
            int agilityBoost = 0;
            int bruteForceBoost = 0;
            int resistanceBoost = 0;
            int dexterityBoost = 0;
            // Gets the boost of every consumable item
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
            // Cures receiver
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
            // Instantiates a new buff
            StatModifier statBuff = new StatModifier(bruteForceBoost, dexterityBoost, 0, resistanceBoost, agilityBoost);
            return statBuff;
        }
        // Hero attacks monster
        private async Task MeleeAttack(Hero attacker, Monster defender)
        {
            Item[] uncastedMeleeItems = attacker.GetInventory().GetMelee(2);
            for (int i = 0; i < uncastedMeleeItems.Length; i+=1)
            {
                await window.UpdateLogUI(uncastedMeleeItems[i].AnnounceUse(),200);
            }
            // Gets the stats that will be used
            int attackerStat = attacker.GetStat(Stats.Stat.BruteForce);
            int defenderStat = defender.GetResistance();
            int defenderHealth = defender.GetHp();

            // If the immunity blocks all the damage, returns
            if (defender.GetImmunity() == Immunities.Immunity.ImmuneToMeleeAndResistantToRanged || defender.GetImmunity() == Immunities.Immunity.ImmuneToMelee)
            {
                await window.UpdateLogUI(defender.GetName() + " tiene inmunidad a ataques físicos, " + attacker.GetName() + " se replantea sus acciones...",800);
                return;
            }
            else
            {
                // If the immunity is resistant only, divides damage in half
                if (defender.GetImmunity() == Immunities.Immunity.ResistantToMelee ||
                    defender.GetImmunity() == Immunities.Immunity.ResistantToBoth ||
                    defender.GetImmunity() == Immunities.Immunity.ImmuneToRangedAndResistantToMelee)
                {
                    await window.UpdateLogUI(defender.GetName() + " tiene resistencia a ataques físicos, " + attacker.GetName() + " lo intenta igualmente",800);
                    attackerStat /= 2;
                }
            }
            await window.UpdateCombatStatsUI("@9@Fuerza bruta@ de " + attacker.GetName() + ": " + attackerStat + "\n" +
                "@9@Resistencia@ de " + defender.GetName() + ": " + defenderStat + "\n" +
                "Golpes necesarios para matar: " + defenderHealth,0);
            // Hits are a roll of ATK - DEF
            int hits = myDice.PlayDice(attackerStat - defenderStat, rnd);
            await window.UpdateLogUI(attacker.GetName() + " propina " + hits + " golpes a " + defender.GetName(), 0);
            await window.UpdateDicesUI(myDice.GetRollHistory() , 1200);

            // If hits are equal or higher than defender's health, it kills the monster
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
        // Hero range attacks monster
        private async Task RangeAttack(Hero attacker, Monster defender)
        {
            // Gets the stats that will be used
            int defenderStat = defender.GetResistance() / 2;
            int defenderHealth = defender.GetHp();
            int defenderAgility = defender.GetAgility();
            int attackerStat = attacker.GetStat(Stats.Stat.Dexterity);

            Item[] uncastedRangedItems = attacker.GetInventory().GetRanged(2);

            RangedItem[] rangedItems = new RangedItem[uncastedRangedItems.Length];
            // Casts the Items into RangedItems
            for (int i = 0; i < uncastedRangedItems.Length; i+=1)
            {
                rangedItems[i] = (RangedItem)uncastedRangedItems[i];
                await window.UpdateLogUI(rangedItems[i].AnnounceUse(), 200);
            }
            // Possible misses
            int misses = 0;
            // Damage done
            int damage = 0;
            // Weapons used and without ammo
            int emptyAmmoWeapons = 0;
            // Permitted misses, successful rolls between 0 and hero's dexterity 
            int permittedMisses = myDice.PlayDice(attackerStat / 3, rnd);
            await window.UpdateDicesUI(myDice.GetRollHistory(), 0);
            await window.UpdateCombatStatsUI("Fallos permitidos: " + permittedMisses +
                "\n@9@Destreza@ de " + attacker.GetName() + ": " + attackerStat + "\n" +
                "@9@Resistencia@ de " + defender.GetName() + ": " + defenderStat + "\n" +
                "@9@Agilidad@ de " + defender.GetName() + ": " + defenderAgility + "\n" +
                "Dados necesarios para matar: " + defenderHealth,400);
            // Repeats the attack cycle if misses are below permitted misses, and weapons are not empty
            do
            {
                emptyAmmoWeapons = 0;
                for (int i = 0; i < rangedItems.Length; i+=1)
                {
                    // If ammo is not empty, it will shoot
                    if (rangedItems[i].GetAmmo() <= 0)
                    {
                        emptyAmmoWeapons += 1;
                    }
                    else
                    {
                        // Hero dexterity - (weapon's dexterity + agility / 4)
                        int diceNum = myDice.PlayDice(attackerStat - (rangedItems[i].GetDifficulty() + (defenderAgility / 4)), rnd);
                        await window.UpdateDicesUI(myDice.GetRollHistory(),0);
                        // If the number is higher than the weapon's difficulty, it counts as a successful shot
                        if (diceNum >= rangedItems[i].GetDifficulty())
                        {
                            damage += rangedItems[i].GetStatToModify().GetBruteForce();
                            await window.UpdateLogUI(attacker.GetName() + " acierta y hace " + rangedItems[i].GetStatToModify().GetBruteForce() + " de daño",400);
                            rangedItems[i].ShootAmmo();
                        }
                        else
                        {
                            // Adds 1 to misses
                            await window.UpdateLogUI(defender.GetName() + " logra esquivar", 400);
                            misses += 1;
                            rangedItems[i].ShootAmmo();
                        }
                        // Announces if the weapon is empty
                        if (rangedItems[i].GetAmmo() == 0)
                        {
                            await window.UpdateLogUI(rangedItems[i].GetName() + " se queda sin munición..." , 400);
                        }
                    }

                }
            } while (misses <= permittedMisses && emptyAmmoWeapons != rangedItems.Length);

            // Announces if all the weapons are empty
            if(emptyAmmoWeapons == rangedItems.Length)
            {
                await window.UpdateLogUI(attacker.GetName() + " gastó la munición de todas sus armas...", 800);

            }
            // Announces if the misses are higher than permitted
            if (misses >= permittedMisses)
            {
                await window.UpdateLogUI(attacker.GetName() + " falló demasiado...", 800);
            }
            // If immunity blocks all the damage, returns
            if (defender.GetImmunity() == Immunities.Immunity.ImmuneToRangedAndResistantToMelee ||
                defender.GetImmunity() == Immunities.Immunity.ImmuneToRanged)
            {
                await window.UpdateLogUI(defender.GetName() + " tiene inmunidad a ataques a distancia, " + attacker.GetName() + " derrochó munición...",1200);
                return;
            }
            else
            {
                // If the immunity is resistant to ranged, then divides the damage in half
                if (defender.GetImmunity() == Immunities.Immunity.ResistantToBoth ||
                defender.GetImmunity() == Immunities.Immunity.ResistantToRanged ||
                defender.GetImmunity() == Immunities.Immunity.ImmuneToMeleeAndResistantToRanged)
                {
                    await window.UpdateLogUI(defender.GetName() + " tiene resistencia a ataques a distancia, " + attacker.GetName() + " lo intenta igualmente",1200);
                    damage /= 2;
                }
            }

            // If rolls are higher than defender health, then hero kills defender
            if (myDice.PlayDice(damage - defenderStat, rnd) >= defenderHealth)
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
        // Hero martial arts attacks monster
        private async Task MartialArtsAttack(Hero attacker, Monster defender)
        {
            // Gets the stats that will be used
            int attackerStat = attacker.GetMartialArtStat();
            int defenderStat = defender.GetResistance();
            int defenderHealth = defender.GetHp();
            int damage = 0;
            // Depending on attackerStat, it will use 1-4 limbs
            switch (attackerStat)
            {
                case < 20:
                    {
                        // Gets the limb used
                        Limb limbUsed = attacker.GetRandomLimb(rnd);
                        // Gets the limb damage
                        int limbStat = limbUsed.GetBruteForce() + limbUsed.GetDexterity() == 0 ?
                            1 : limbUsed.GetBruteForce() + limbUsed.GetDexterity();
                        // Has chance to attack for every agility point
                        int hitChances = attacker.GetStat(Stats.Stat.Agility);

                        await window.UpdateCombatStatsUI("Miembro usado: " + limbUsed.GetName() + "\n" +
                            "Daño de miembro: " + limbStat + "\n" +
                            "Posibilidades de golpear: " + hitChances + "\n" +
                            "@9@Resistencia@ de " + defender.GetName() + ": " + defenderStat + "\n" +
                            "Daño necesario para acabar con " + defender.GetName() + ": " + defenderHealth, 0);

                        // Might hit the defender, and build up damage
                        for (int i = hitChances; i > 0; i -= 1)
                        {
                            int hit = myDice.PlayDice(limbStat - defenderStat, rnd);
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
                        // If the damage is higher than defender's health, kills it
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
                        // Gets the limbs used
                        Limb[] limbsUsed = new Limb[2];
                        limbsUsed[0] = attacker.GetRandomLimb(rnd);
                        limbsUsed[1] = attacker.GetRandomLimb(rnd);
                        int limbStat = 0;
                        // Gets the limb damage
                        for (int i = 0; i < limbsUsed.Length; i+=1)
                        {
                            limbStat += limbsUsed[i].GetBruteForce() + limbsUsed[i].GetDexterity() == 0 ?
                            1 : limbsUsed[i].GetBruteForce() + limbsUsed[i].GetDexterity();
                        }
                        // Has chance to attack for every agility point
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

                        // Might hit the defender, and build up damage
                        for (int i = hitChances; i > 0; i -= 1)
                        {
                            int hit = myDice.PlayDice(limbStat - defenderStat, rnd);
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
                        // If the damage is higher than defender's health, kills it
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
                        // Gets the limbs used
                        Limb[] limbsUsed = new Limb[3];
                        limbsUsed[0] = attacker.GetRandomLimb(rnd);
                        limbsUsed[1] = attacker.GetRandomLimb(rnd);
                        limbsUsed[2] = attacker.GetRandomLimb(rnd);
                        // Gets the limbs damage
                        int limbStat = 0;
                        for (int i = 0; i < limbsUsed.Length; i+=1)
                        {
                            limbStat += limbsUsed[i].GetBruteForce() + limbsUsed[i].GetDexterity() == 0 ?
                            1 : limbsUsed[i].GetBruteForce() + limbsUsed[i].GetDexterity();
                        }
                        // Has chance to attack for every agility point
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
                        // Might hit the defender, and build up damage
                        for (int i = hitChances; i > 0; i -= 1)
                        {
                            int hit = myDice.PlayDice(limbStat - defenderStat, rnd);
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
                        // If the damage is higher than defender's health, kills it
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
                        // Gets the limbs used
                        Limb[] limbsUsed = new Limb[4];
                        limbsUsed[0] = attacker.GetRandomLimb(rnd);
                        limbsUsed[1] = attacker.GetRandomLimb(rnd);
                        limbsUsed[2] = attacker.GetRandomLimb(rnd);
                        limbsUsed[3] = attacker.GetRandomLimb(rnd);

                        // Gets the limbs damage
                        int limbStat = 0;
                        for (int i = 0; i < limbsUsed.Length; i+=1)
                        {
                            limbStat += limbsUsed[i].GetBruteForce() + limbsUsed[i].GetDexterity() == 0 ?
                            1 : limbsUsed[i].GetBruteForce() + limbsUsed[i].GetDexterity();
                        }
                        // Has chance to attack for every agility point
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

                        // Might hit the defender, and build up damage
                        for (int i = hitChances; i > 0; i -= 1)
                        {
                            int hit = myDice.PlayDice(limbStat - defenderStat, rnd);
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
                        // If the damage is higher than defender's health, kills it
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
        // Advances turns of dead iturnables
        private void AdvanceTurnIfItsDead()
        {
            if (!turnOrder[turnIndex].IsAlive())
            {
                do
                {
                    AdvanceIndex();
                } while (!turnOrder[turnIndex].IsAlive());
            }
        }
        // Decides the victim of a hero
        private Hero DecideVictim()
        {
            int count = 0;
            for (int i = 0; i < heroes.Length; i+=1)
            {
                if (heroes[i].IsAlive())
                {
                    count+=1;
                }
            }
            if(count == 0)
            {
                UpdateGameState();
                return null;
            }
            Hero[] heroesToAttack = new Hero[count];
            count = 0;
            for (int i = 0; i < heroes.Length; i+=1)
            {
                if (heroes[i].IsAlive())
                {
                    heroesToAttack[count] = heroes[i];
                    count += 1;
                }
            }
            Hero victim = heroesToAttack[rnd.Next(0, count)];

            return victim;
        }
        // Gets the game state
        public int GetGameState()
        {
            return combatState;
        }
        // Updates game state
        private void UpdateGameState()
        {
            bool lost = true;
            bool won = true;
            // Checks if heroes or monsters are all dead
            for (int i = 0; i < heroes.Length; i+=1)
            {
                if (heroes[i].IsAlive())
                {
                    lost = false;
                    break;
                }
            }
            for (int i = 0; i < enemies.Length; i+=1)
            {
                if (enemies[i].IsAlive())
                {
                    won = false;
                    break;
                }
            }
            // Updates the combat state
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
        // Generates an item for when 
        private Item GenerateItem()
        {
            Item itemToDrop = null;
            int rarityDecider = rnd.Next(0, 20);
            Rarity.Rarities rarity;
            // 45% of not dropping an item
            // 25% of junk item
            // 15% of rust covered item
            // 10% of catun item
            // 5% of mk item
            switch (rarityDecider)
            {
                case <9:
                    {
                        return null;
                    }
                case <14:
                    {
                        rarity = Rarity.Rarities.Junk;
                        break;
                    }
                case <17:
                    {
                        rarity = Rarity.Rarities.RustCovered;
                        break;
                    }
                case <19:
                    {
                        rarity = Rarity.Rarities.Catun;
                        break;
                    }
                case 19:
                    {
                        rarity = Rarity.Rarities.Mk;
                        break;
                    }
                default:
                    {
                        //Shouldn't ever trigger this case, however VS makes me put it. I'll put a Debug.WriteLine just in case
                        Debug.WriteLine("Didn't know what rarity to put");
                        rarity = Rarity.Rarities.Junk;
                        break;
                    }
            }
            //Returns a copy of the item with its rarity
            itemToDrop = myAdventure.GetAllItems()[rnd.Next(0, myAdventure.GetAllItems().Length)];
            itemToDrop = itemToDrop.GetCopy();
            itemToDrop.UpgradeRarity(rarity);
            return itemToDrop;
        }
        // Bubble sorts by agility 
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
        // Gets N turns for the list in CombatWindow
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
                    DecideNextNTurns(12);
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
