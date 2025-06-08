namespace Kenshi_DnD
{
    // Class that represents a hero that the player can hire and fight with. - Santiago Cabrero
    [Serializable]
    public class Hero : ITurnable
    {
        //Name and title, example: "Carlos, the database scholar"
        string name;
        string title;

        // Inventory of the hero
        HeroInventory personalInventory;
        // Base stats of hero
        StatModifier heroStats;
        // Buff stats of hero
        StatModifier buff;

        //Health points
        //Toughness is max health points
        int hp;
        int toughness;

        //Race is a key component in the world building of this game. The races are: Human, Shek, Skeleton and Hive
        Race race;
        //Race specifies the exact type of race that the character is. The subraces are:
        //Human: Greenlander, scorchlander
        //Hive: Hive Prince, Hive Worker, Hive Soldier
        //The other races don't have subrace
        Race subrace;

        //Background story is a randomly asigned tale to give the hero a bit of depth.
        string backgroundStory;

        //Recruitment date is the date when the hero was recruited.
        DateTime recruitmentDate;

        //Level determines roughly how strong the hero is. However, this is not a direct correlation, as the hero's ints are built upon leveling up.
        int level;
        //Experience is the amount of experience the hero has. It determines how much the hero has leveled up.
        int experience;
        //XP points to spend in stat upgrades
        int xpPoints;

        //Limbs can be lost and bought in the kenshi universe. They affect stats
        Limb[] limbs;
        // Level of competency when the hero was generated
        Competency.StartCompetency startCompetency;
        // For showing the hero as not hired or hired in the bar
        bool hired;

        //In Kenshi the best way to level up at the start is to get beaten a lot, in this game, it's not the best, but it's an option
        const int PART_DAMAGE_AS_XP = 2;
        // Base toughness
        const int BASE_TOUGHNESS = 7;
        // Constructor that generates a whole hero
        public Hero(string name, string title,string backgroundStory,int bruteForce ,int dexterity, int resistance, int agility,
             int level, Race race, Race subrace, Limb[] limbs, Competency.StartCompetency startCompetency)
        {
            hired = false;
            toughness = BASE_TOUGHNESS;
            heroStats = new StatModifier(bruteForce, dexterity, resistance, toughness, agility);
            this.name = name;
            this.title = title;
            SetToughnessAtConstructor(toughness, race, subrace, limbs);
            this.xpPoints = 0;
            this.backgroundStory = backgroundStory;
            this.recruitmentDate = DateTime.Now;
            this.level = level;
            experience = (10 * ((level) * (level)));
            this.race = race;
            this.subrace = subrace;
            this.limbs = limbs;
            personalInventory = new HeroInventory();
            buff = new StatModifier(0, 0, 0, 0, 0);
            this.startCompetency = startCompetency;
        }
        // Constructor of partial hero, only used to represent the starting character's stats 
        public Hero(int bruteForce, int dexterity, int resistance, int agility, Race race, Race subrace, Limb[] limbs) 
        {
            heroStats = new StatModifier(bruteForce,dexterity,0,resistance,agility);
            SetToughnessAtConstructor(7,race, subrace, limbs);
            this.race = race;
            this.subrace = subrace;
            this.limbs = limbs;
            personalInventory = new HeroInventory();
            buff = new StatModifier(0, 0, 0, 0, 0);
        }
        // Gets a selected stat
        public int GetStat(Stats.Stat opt)
        {
            int stat = 0;
            switch (opt)
            {
                case Stats.Stat.BruteForce:
                    {
                        for (int i = 0; i < limbs.Length; i+=1)
                        {
                            if (limbs[i] != null)
                            {
                                stat += limbs[i].GetBruteForce();
                            }
                        }
                        stat += heroStats.GetBruteForce() + race.GetBruteForce() + subrace.GetBruteForce() + personalInventory.GetStat(opt) + buff.GetBruteForce();
                        break;
                    }
                case Stats.Stat.Dexterity:
                    {
                        for (int i = 0; i < limbs.Length; i+=1)
                        {
                            if (limbs[i] != null)
                            {
                                stat += limbs[i].GetDexterity();
                            }
                        }
                        stat += heroStats.GetDexterity() + race.GetDexterity() + subrace.GetDexterity() + personalInventory.GetStat(opt) + buff.GetDexterity();
                        break;
                    }
                case Stats.Stat.HP:
                    {
                        stat = this.GetToughness();
                        break;
                    }
                case Stats.Stat.Resistance:
                    {
                        for (int i = 0; i < limbs.Length; i+=1)
                        {
                            if (limbs[i] != null)
                            {
                                stat += limbs[i].GetResistance();
                            }
                        }
                        stat += heroStats.GetResistance() + race.GetResistance() + subrace.GetResistance() + personalInventory.GetStat(opt) + buff.GetResistance();
                        break;
                    }
                case Stats.Stat.Agility:
                    {
                        for (int i = 0; i < limbs.Length; i+=1)
                        {
                            if (limbs[i] != null)
                            {
                                stat += limbs[i].GetAgility();
                            }
                        }
                        stat += heroStats.GetAgility() + race.GetAgility() + subrace.GetAgility() + personalInventory.GetStat(opt) + buff.GetAgility();
                        break;
                    }
            }
            return stat;
        }
        // Sets the toughness, taking into account the race, subrace, and limbs
        private void SetToughnessAtConstructor(int toughness, Race race, Race subrace, Limb[] limbs)
        {
            int stat = 0;
            for (int i = 0; i < limbs.Length; i+=1)
            {
                if (limbs[i] != null)
                {
                    stat += limbs[i].GetHp();
                }
            }
            SetToughness(toughness + race.GetHp() + subrace.GetHp() + stat);
            this.hp = this.toughness;   
        }
        // Gets a statmodifier copy of all their stats
        public StatModifier GetAllStats()
        {
            StatModifier allStats;
            int bruteForce = 0;
            int dexterity = 0;
            int hp = 0;
            int resistance = 0;
            int agility = 0;

            bruteForce += this.GetStat(Stats.Stat.BruteForce);
            dexterity += this.GetStat(Stats.Stat.Dexterity);
            hp += GetHp();
            resistance += this.GetStat(Stats.Stat.Resistance);
            agility += this.GetStat(Stats.Stat.Agility);

            allStats = new StatModifier(bruteForce, dexterity, hp, resistance, agility);
            return allStats;

        }
        // Get agility is a required method for the Iturnable interface
        public int GetAgility()
        {
            return GetStat(Stats.Stat.Agility);
        }
        // Getters and setters
        public string GetName()
        {
            return name;
        }
        public int GetToughness()
        {
            return toughness;
        }
        public int GetHp()
        {
            return this.hp;
        }
        public bool IsAlive()
        {
            return (this.GetHp() > 0);
        }
        public DateTime GetRecruitmentDate()
        {
            return recruitmentDate;
        }
        public int GetLevel()
        {
            return level;
        }

        public Race GetRace()
        {
            return race;
        }
        public Race GetSubrace()
        {
            return subrace;
        }
        public void SetToughness(int toughness)
        {
            this.toughness = toughness;
        }
        public void SetHp(int hp)
        {
            this.hp = hp;
        }
        public void Hire()
        {
            hired = true;
        }
        public Limb[] GetLimbs()
        {
            return limbs;
        }
        public HeroInventory GetInventory()
        {
            return personalInventory;
        }
        public void SetBuff(StatModifier buff)
        {
            this.buff = buff;
        }
        public StatModifier GetBuff()
        {
            return buff;
        }
        public bool IsHired()
        {
            return hired;
        }
        public int GetXpPoints()
        {
            return xpPoints;
        }
        // Adds the item to the inventory and uses needed limbs
        public void AddItemToInventory(Item item)
        {
            item.SetAlreadyUsed(true);
            UseLimbs(item.GetLimbsNeeded());
            personalInventory.AddItem(item);
        }
        // Returns if the hero has any missing limbs
        public bool IsAmputee()
        {
            for (int i = 0; i < limbs.Length; i += 1)
            {
                if (limbs[i] == null)
                {
                    return true;
                }
            }
            return false;
        }
        // Returns if the hero has enough free limbs to use an item
        public bool CanUseItem(Item item)
        {
            int limbsAvailable = 0;
            for (int i = 0; i < GetLimbs().Length; i += 1)
            {
                if (GetLimbs()[i] != null)
                {
					if (!GetLimbs()[i].GetBeingUsed())
					{
						limbsAvailable += 1;
					}
				}
                
            }
            if (limbsAvailable >= item.GetLimbsNeeded())
            {
                return true;
            }

            return false;
        }
        // Frees limbs from being used
        private void FreeLimbs(int num)
        {
            int iterator = 0;
            while (num > 0)
            {
                if (limbs[iterator] != null)
                {
                    if (limbs[iterator].GetBeingUsed())
                    {
                        limbs[iterator].SetBeingUsed(false);
                        num -= 1;
                    }
                }
                iterator += 1;
            }
        }
        // Uses a number of limbs
        private void UseLimbs(int num)
        {
            int iterator = 0;
            while (num > 0)
            {
                if (limbs[iterator] != null)
                {
                    if (!limbs[iterator].GetBeingUsed())
                    {
                        limbs[iterator].SetBeingUsed(true);
                        num -= 1;
                    }
                }
                iterator += 1;
            }
        }
        // Returns the martial art stat, counted by their limb's stats
        public int GetMartialArtStat()
        {
            int martialArtPower = 0;
            for (int i = 0; i < limbs.Length; i+=1)
            {
                if (limbs[i] != null)
                {
                    martialArtPower += (limbs[i].GetDexterity() + limbs[i].GetBruteForce()) + 
                        (limbs[i].GetAgility() == 0 ? 0 : limbs[i].GetAgility() / 2);
                }
            }
            martialArtPower += (GetStat(Stats.Stat.Agility) == 0 ? 0 : GetStat(Stats.Stat.Agility) / 2);
            return martialArtPower;
        }
        // Gains Xp and levels up if threshold is reached, also returns if the hero has leveled up
        public bool GainXp(int newXp)
        {
            bool leveledUp = false;
            this.experience += newXp;
            while (experience >= (10 * ((level + 1) * (level + 1))))
            {
                leveledUp = true;
                level += 1;
                xpPoints += 1;
            }

            return leveledUp;
        }
        // Returns a string representing the Competency of the hero
        public string CompetencyToString()
        {
            switch (startCompetency)
            {
                case Competency.StartCompetency.Apprentice:
                    {
                        return "Aprendiz";
                    }
                case Competency.StartCompetency.Intermediate:
                    {
                        return "Discípulo";
                    }
                case Competency.StartCompetency.Master:
                    {
                        return "Maestro";
                    }
                default:
                    {
                        return "Error en la competencia";
                    }
            }
        }
       // Heals the hero
        public void Heal(int healHp)
        {
            if (GetHp() + healHp > GetToughness())
            {
                this.SetHp(GetToughness());
            }
            else
            {
                this.SetHp(GetHp() + healHp);
            }
        }
        // Hurts the hero
        public bool Hurt(int hurtHp)
        {
            
            if(hurtHp < 0)
            {
                hurtHp = 0;
            }
            //In kenshi, the best way to level up is to get beaten a lot... In my game, it mostly helps at the start if anything
            bool leveledUp = GainXp(hurtHp / PART_DAMAGE_AS_XP);
            if(GetHp() - hurtHp < 0)
            {
                this.SetHp(0);
            }
            else
            {
                this.SetHp(GetHp() - hurtHp);
            }
            return leveledUp;
        }
        // Removes item from inventory and frees limbs
        public void RemoveItemFromInventory(Item item)
        {
            item.SetAlreadyUsed(false);
            FreeLimbs(item.GetLimbsNeeded());
            personalInventory.RemoveItem(item);
        }
        // Returns if said items are in the hero's inventory
        public bool AreConsumableItems()
        {
            return personalInventory.AreConsumableItems();
        }
        public bool AreRangedItems()
        {
            return personalInventory.AreRangedItems();
        }
        public bool AreMeleeItems()
        {
            return personalInventory.AreMeleeItems();
        }
        // Removes a limb, if the limb is not found, removes first found
        public Limb RemoveLimb(int limbIndex)
        {
            if (limbs[limbIndex] != null)
            {
                Limb limbToReturn = limbs[limbIndex];

				limbs[limbIndex] = null;
                return limbToReturn;
            }
            else
            {
                for (int i = 0; i < limbs.Length; i++)
                {
                    if (limbs[i] != null)
                    {
                        Limb limbToReturn = limbs[i];

						limbs[i] = null;
                        return limbToReturn;
                    }

                }
            }
            return null;
        }
        // Puts a limb in an empty limb space
        public void PutLimb(Limb newLimb)
        {
            for (int i = 0; i < limbs.Length; i += 1)
            {
                if (limbs[i] == null)
                {
                    limbs[i] = newLimb;
                    break;
                }
            }
        }
        // Gets a random limb
        public Limb GetRandomLimb(Random rnd)
        {
            bool atLeastOne = false;

            for (int i = 0; i < limbs.Length; i += 1)
            {
                if (limbs[i] != null)
                {
                    atLeastOne = true;
                }
            }
            Limb randomLimb = null;
			if (atLeastOne)
            {
                do
                {
                    int randomIndex = rnd.Next(0, limbs.Length);
                    if (limbs[randomIndex]!= null)
                    {
                        randomLimb = limbs[randomIndex];
                    }
                } while (randomLimb == null);
            }

            return randomLimb == null? new Limb("No tiene miembros..",0,0,0,0,0,0):randomLimb;
        }
        // Removes all items from inventory, and returns the string of which items
        public string FreeAllItems()
        {
            int count = 0;
            for(int i = 0; i < limbs.Length; i += 1)
            {
                if(limbs[i] != null)
                {
                    if (limbs[i].GetBeingUsed())
                    {
                        count++;
                    }
                }
            }
            FreeLimbs(count);
            return personalInventory.MakeAllItemsDisponible(this);
        }
        // Upgrades a stat
        public void UpgradeStat(Stats.Stat statToUpgrade)
        {
            xpPoints -= 1;
            heroStats.UpgradeStat(statToUpgrade, 1);
            if(statToUpgrade == Stats.Stat.HP)
            {
                toughness += 1;
                Heal(1);
            }
        }
        // Returns if the hero has points to spend
        public bool HasPointsToSpend()
        {
            return xpPoints > 0;
        }
        // Info of hero
        public string GetNameAndTitle()
        {
            return name + ", " + title;
        }
        public override string ToString()
        {
            return
                $"Nivel: {level}\n" +
                $"Experiencia: {experience}\n" +
                $"Raza: {race.GetName()}\n" +
                $"{(subrace.GetName() != "Puro" ? $"Subraza: {subrace.GetName()}\n" : "")}"
                + ((IsAmputee() ) ? ("Le faltan miembros... Acude al mecánico de extremidades...\n") : (""))
                + GetAllStats().ToString();
        }
        public string Meet()
        {
            return $"{backgroundStory}" 
                + "\n-------------------\n" + 
                ToString() 
                + "\n-------------------\n" + 
                "Me uniré a tu aventura por " +  GetCompetencyCost();
        }
        // Returns the cost of the hero depending on its competency
        public int GetCompetencyCost()
        {
          
            int cost = 0;

            switch (startCompetency)
            {
                case Competency.StartCompetency.Apprentice:
                    {
                        cost += 200;
                        break;
                    }
                case Competency.StartCompetency.Intermediate:
                    {
                        cost += 1000;
                        break;
                    }
                case Competency.StartCompetency.Master:
                    {
                        cost += 2000;
                        break;
                    }
            }
            return cost;
        }
    }
}