using System.Diagnostics;

namespace Kenshi_DnD
{
    class Hero : ITurnable
    {
        //Name and title, example: "Carlos, the database scholar"
        string name;
        string title;

        //Offensive ints
        //Brute Force is associated with Sheks, skeletons and Hive Soldiers
        //Dexterity is associated with Humans(All types), Hive Princes and Hive Workers 
        HeroInventory personalInventory;
        StatModifier heroStats;
        //Defensive ints
        //Health points
        //Toughness is max health points
        int toughness;
        //Resistance is defense against Brute Force
        //Perception is defense against Dexterity
        //Agility determines how fast the hero interacts with the enemies
        //Race is a key component in the world building of this game. The races are: Human, Shek, Skeleton and Hive
        Race race;
        //Race specifies the exact type of race that the character is. The subraces are:
        //Human: Greenlander, scorchlander
        //Hive: Hive Prince, Hive Worker, Hive Soldier
        //The other races don't have subrace
        Race subrace;
        //Inventory

        //Background story is a semi randomly asigned tale to give the hero a bit of depth. Depending on their circunstances, these will vary.
        //For example: If a Shek is a rebel, their background could tell how they rejected Esata the Stone Golem(Shek Kingdom's Queen) for her untraditionalist ways.
        //If a human is a merchant noble, their background could tell how they inherited their father's slave farm.
        string backgroundStory;

        //Recruitment date is the date when the hero was recruited.
        DateTime recruitmentDate;
        //Level determines roughly how strong the hero is. However, this is not a direct correlation, as the hero's ints are built upon leveling up.
        int level;
        //Experience is the amount of experience the hero has. It determines how much the hero has leveled up.
        int experience;
        //Limbs can be lost and bought in the kenshi universe.
        Limb[] limbs;
        public Hero(string name, string title, int toughness, int bruteForce, int agility, int resistance, int dexterity,
            string backgroundStory, int level, int experience, Race race, Race subrace, Limb[] limbs)
        {
            heroStats = new StatModifier(bruteForce, dexterity, resistance, toughness, agility);
            this.name = name;
            this.title = title;
            this.toughness = toughness + race.GetToughness() + subrace.GetToughness();
            this.SetHp(toughness);
            this.backgroundStory = backgroundStory;
            this.recruitmentDate = DateTime.Now;
            this.level = level;
            this.experience = experience;
            this.race = race;
            this.subrace = subrace;
            this.limbs = limbs;
            personalInventory = new HeroInventory();
        }
        public Hero(string name, string title, int toughness, int bruteForce, int agility, int resistance, int dexterity,
             Race race, Race subrace, Limb[] limbs)
        {
            this.name = name;
            this.title = title;
            heroStats = new StatModifier(bruteForce, dexterity, resistance, toughness, agility);
            this.toughness = toughness + race.GetToughness() + subrace.GetToughness();
            this.SetHp(toughness);
            this.level = 1;
            this.backgroundStory = "";
            this.recruitmentDate = DateTime.Now;
            this.race = race;
            this.subrace = subrace;
            this.limbs = limbs;
            personalInventory = new HeroInventory();
        }
        // 1 = Brute Force, 2 = Dexterity, 3 = HP, 4 = Resistance, 5 = Agility
        public int GetStat(int opt)
        {
            int stat = 0;
            switch (opt)
            {
                case 1:
                    {
                        for (int i = 0; i < limbs.Length; i++)
                        {
                            if (limbs[i] != null)
                            {
                                stat += limbs[i].GetBruteForce();
                            }
                        }
                        stat = heroStats.GetBruteForce() + race.GetBruteForce() + subrace.GetBruteForce() + personalInventory.GetStat(opt);
                        break;
                    }
                case 2:
                    {
                        for (int i = 0; i < limbs.Length; i++)
                        {
                            if (limbs[i] != null)
                            {
                                stat += limbs[i].GetDexterity();
                            }
                        }
                        stat = heroStats.GetDexterity() + race.GetDexterity() + subrace.GetDexterity() + personalInventory.GetStat(opt);
                        break;
                    }
                case 3:
                    {
                        for (int i = 0; i < limbs.Length; i++)
                        {
                            if (limbs[i] != null)
                            {
                                stat += limbs[i].GetHp();
                            }
                        }
                        stat = heroStats.GetHp() + race.GetHp() + subrace.GetHp() + personalInventory.GetStat(opt);
                        break;
                    }
                case 4:
                    {
                        for (int i = 0; i < limbs.Length; i++)
                        {
                            if (limbs[i] != null)
                            {
                                stat += limbs[i].GetResistance();
                            }
                        }
                        stat = heroStats.GetResistance() + race.GetResistance() + subrace.GetResistance() + personalInventory.GetStat(opt);
                        break;
                    }
                case 5:
                    {
                        for (int i = 0; i < limbs.Length; i++)
                        {
                            if (limbs[i] != null)
                            {
                                stat += limbs[i].GetAgility();
                            }
                        }
                        stat = heroStats.GetAgility() + race.GetAgility() + subrace.GetAgility() + personalInventory.GetStat(opt);
                        break;
                    }
            }
            return stat;
        }
        public StatModifier GetAllStats()
        {
            StatModifier allStats;
            int bruteForce = 0;
            int dexterity = 0;
            int hp = 0;
            int resistance = 0;
            int agility = 0;

            bruteForce += this.GetStat(1);
            dexterity += this.GetStat(2);
            hp += this.GetStat(3);
            resistance += this.GetStat(4);
            agility += this.GetStat(5);

            allStats = new StatModifier(bruteForce, dexterity, resistance, hp, agility);
            return allStats;

        }
        public string GetName()
        {
            return name;
        }
        public string GetTitle()
        {
            return title;
        }
        public int GetBruteForce()
        {
            return heroStats.GetBruteForce();
        }
        public int GetDexterity()
        {
            return heroStats.GetDexterity();
        }
        public int GetToughness()
        {
            return toughness;
        }
        public int GetHp()
        {
            return heroStats.GetHp();
        }
        public bool IsAlive()
        {
            return (this.heroStats.GetHp() > 0);
        }
        public int GetResistance()
        {
            return heroStats.GetResistance();
        }
        public int GetAgility()
        {
            return heroStats.GetAgility();
        }
        public string GetBackgroundStory()
        {
            return backgroundStory;
        }
        public DateTime GetRecruitmentDate()
        {
            return recruitmentDate;
        }
        public int GetLevel()
        {
            return level;
        }
        public int GetExperience()
        {
            return experience;
        }
        public void SetName(string name)
        {
            this.name = name;
        }
        public void SetTitle(string title)
        {
            this.title = title;
        }
        public void SetToughness(int toughness)
        {
            this.toughness = toughness;
        }
        public void SetHp(int hp)
        {
            this.heroStats.SetHp(hp);
        }
        public void SetBackgroundStory(string backgroundStory)
        {
            this.backgroundStory = backgroundStory;
        }
        public void SetRecruitmentDate(DateTime recruitmentDate)
        {
            this.recruitmentDate = recruitmentDate;
        }
        public void SetLevel(int level)
        {
            this.level = level;
        }
        public void SetExperience(int experience)
        {
            this.experience = experience;
        }
        public Limb[] GetLimbs()
        {
            return limbs;
        }
        public HeroInventory GetInventory()
        {
            return personalInventory;
        }
        public void AddItemToInventory(Item item)
        {
            if (item.CanUseItem(this))
            {
                Debug.WriteLine(this.name + " can use " + item.GetName());
                item.SetAlreadyUsed(true);
                UseLimbs( item.GetLimbsNeeded());
                personalInventory.AddItem(item);
            }
            else
            {
                Debug.WriteLine(this.name + " can't use " + item.GetName());
            }
        }
        private void FreeLimbs(int num)
        {
            int iterator = 0;
            while (num > 0)
            {
                Debug.WriteLine("Trying limb " + iterator);
                Debug.WriteLine("Limbs to free now: " + num);
                if (limbs[iterator] != null)
                {
                    if (limbs[iterator].GetBeingUsed())
                    {
                        Debug.WriteLine("Limb found");
                        limbs[iterator].SetBeingUsed(false);
                        num -= 1;
                    }
                    else
                    {
                        Debug.WriteLine("Limb already free");
                    }
                }
                else
                {
                    Debug.WriteLine("Limb is missing...");
                }
                iterator += 1;
            }
        }
        private void UseLimbs(int num)
        {
            int iterator = 0;
            while(num > 0)
            {
                Debug.WriteLine("Trying limb " + iterator);
                Debug.WriteLine("Limbs needed now: " + num);
                if (limbs[iterator] != null)
                {
                    if (!limbs[iterator].GetBeingUsed())
                    {
                        Debug.WriteLine("Limb found");
                        limbs[iterator].SetBeingUsed(true);
                        num -= 1;
                    }
                    else
                    {
                        Debug.WriteLine("Limb already used");
                    }
                }
                else
                {
                    Debug.WriteLine("Limb is missing...");
                }
                    iterator += 1;
            }
        }
        public bool IsInInventory(Item item)
        {
            return personalInventory.ContainsItem(item);
        }
        public void RemoveItemFromInventory(Item item)
        {
            item.SetAlreadyUsed(false);
            FreeLimbs(item.GetLimbsNeeded());
            personalInventory.RemoveItem(item);
        }
        public bool AreRangedItems()
        {
            return personalInventory.AreRangedItems();
        }
        public bool AreMeleeItems()
        {
            return personalInventory.AreMeleeItems();
        }
        public void PutLimb(Limb newLimb)
        {
            for (int i = 0; i < limbs.Length; i++)
            {
                if (limbs[i] == null)
                {
                    limbs[i] = newLimb;
                    break;
                }
            }
        }

        public override string ToString()
        {
            return $"- {name}, {title} - \n" + GetAllStats().ToString();
        }
    }
}
