using System.Diagnostics;
using System.Security.RightsManagement;

namespace Kenshi_DnD
{
    [Serializable]
    public class Hero : ITurnable
    {
        //Name and title, example: "Carlos, the database scholar"
        string name;
        string title;

        //Offensive ints
        //Brute Force is associated with Sheks, skeletons and Hive Soldiers
        //Dexterity is associated with Humans(All types), Hive Princes and Hive Workers 
        HeroInventory personalInventory;
        StatModifier heroStats;
        StatModifier buff;
        //Defensive ints
        //Health points
        //Toughness is max health points
        int hp;
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
        //XP points to spend in stat upgrades
        int xpPoints;
        //Limbs can be lost and bought in the kenshi universe.
        Limb[] limbs;
        Competency.StartCompetency startCompetency;
        bool hired;
        //In Kenshi the best way to level up at the start is to get beaten a lot
        const int PART_DAMAGE_AS_XP = 6;
        public Hero(string name, string title,string backgroundStory,int bruteForce ,int dexterity, int resistance, int agility,
             int level, Race race, Race subrace, Limb[] limbs, Competency.StartCompetency startCompetency)
        {
            hired = false;
            toughness = 7;
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
        public Hero(string name, string title, int bruteForce, int agility, int resistance, int dexterity,
             Race race, Race subrace, Limb[] limbs)
        {
            toughness = 7;
            this.name = name;
            this.title = title;
            heroStats = new StatModifier(bruteForce, dexterity, resistance, toughness, agility);
            SetToughnessAtConstructor(toughness, race, subrace, limbs);
            this.level = 1;
            this.xpPoints = 0;
            this.backgroundStory = "";
            this.recruitmentDate = DateTime.Now;
            this.race = race;
            this.subrace = subrace;
            this.limbs = limbs;
            personalInventory = new HeroInventory();
            buff = new StatModifier(0, 0, 0, 0, 0);
        }
        // 1 = Brute Force, 2 = Dexterity, 3 = HP, 4 = Resistance, 5 = Agility
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
            Debug.WriteLine(stat);
            Debug.WriteLine(race.GetHp());
            Debug.WriteLine(subrace.GetHp());
            SetToughness(toughness + race.GetHp() + subrace.GetHp() + stat);
            this.hp = this.toughness;   
        }
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
        public int GetAgility()
        {
            return GetStat(Stats.Stat.Agility);
        }
        public string GetName()
        {
            return name;
        }
        public string GetTitle()
        {
            return title;
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
            Debug.WriteLine(toughness);
            this.toughness = toughness;
        }
        public void SetHp(int hp)
        {
            this.hp = hp;
        }
        
        public void SetRecruitmentDate(DateTime recruitmentDate)
        {
            this.recruitmentDate = recruitmentDate;
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
        public void AddItemToInventory(Item item)
        {
            
            Debug.WriteLine(this.name + " can use " + item.GetName());
            item.SetAlreadyUsed(true);
            UseLimbs(item.GetLimbsNeeded());
            personalInventory.AddItem(item);
            
            
        }
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
        public void SetBuff(StatModifier buff)
        {
            this.buff = buff;
        }
        public StatModifier GetBuff()
        {
            return buff;
        }
        private void UseLimbs(int num)
        {
            int iterator = 0;
            while (num > 0)
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
        public bool GainXp(int newXp)
        {
            bool leveledUp = false;
            this.experience += newXp;
            while (experience >= (10 * ((level + 1) * (level + 1))))
            {
                leveledUp = true;
                Debug.WriteLine("Level up");
                level += 1;
                xpPoints += 1;
            }

            return leveledUp;
        }
        public bool IsHired()
        {
            Debug.WriteLine("Hired? : " + hired);
            return hired;
        }
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
        public int GetXpPoints()
        {
            return xpPoints;
        }
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
        public bool Hurt(int hurtHp)
        {
            //
            if(hurtHp < 0)
            {
                hurtHp = 0;
            }
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
        public Competency.StartCompetency GetCompetency()
        {
            return startCompetency;
        }
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
        public Limb GetRandomLimb(Random rnd)
        {
            bool atLeastOne = false;

            for (int i = 0; i < limbs.Length; i += 1)
            {
                if (limbs[i] != null)
                {
                    Debug.WriteLine("Has Limb: " + limbs[i].GetName());
                    atLeastOne = true;
                }
            }
            Limb randomLimb = null;
			if (atLeastOne)
            {
                do
                {
                    int randomIndex = rnd.Next(0, limbs.Length);
                    Debug.WriteLine("Random limb index of "+ limbs.Length + " : "+ randomIndex);
                    if (limbs[randomIndex]!= null)
                    {
                        randomLimb = limbs[randomIndex];
                    }
                } while (randomLimb == null);
            }

            return randomLimb == null? new Limb("No tiene miembros..",0,0,0,0,0,0):randomLimb;
        }
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
        public bool HasPointsToSpend()
        {
            return xpPoints > 0;
        }
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