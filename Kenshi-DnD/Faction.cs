using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Kenshi_DnD
{
    // Class that represents a faction in the game, which can be in different regions, that monsters can be part of, and player can be less or more hostile to
    [Serializable]
    public class Faction
    {
        // Id of faction
        int factionId;
        // Name of faction
        string factionName;
        // Description of faction
        string factionDescription;
        // Color of faction, to know the exact values, see MainWindow.cs GetBrushByNum()
        int factionColor;
        // Relations towards the player, in kenshi, factions have relations between themselves, but it would not give any value to this game
        int relations;
        // List of what makes you lose or gain relations with this exact faction. I added functionality for factions to have multiple hostilities, however I didn't end up using this
        List<Hostilities.Hostility> hostilities;
        // Decides if you will lose or gain relations if you win against them
        bool respectByFighting;
        
        // Constructor
        public Faction(int factionId, string factionName, string factionDescription, int relations, int factionColor, bool respectByFighting)
        {         
            this.factionId = factionId;
            this.factionName = factionName;
            this.factionDescription = factionDescription;
            this.relations = relations;
            this.factionColor = factionColor;
            this.respectByFighting = respectByFighting;
              hostilities = new List<Hostilities.Hostility>();
        }
        // Getters and setters
        public int GetFactionId()
        {
            return factionId;
        }
        public string GetFactionName()
        {
            return "@" + factionColor + "@" + factionName +"@";
        }
        public string GetFactionDescription()
        {
            return factionDescription;
        }

        public int GetFactionColor()
        {
            return this.factionColor;
        }
        public int GetRelation()
        {
            return relations;
        }
        public bool GetRespectByFighting()
        {
            return respectByFighting;
        }
        // Adds hostility to hostilities
        public void AddHostility(string newHostility)
        {
            hostilities.Add(HostilityParser(newHostility));
        }
        // Parses the string of the hostility into a Hostilities.Hostility
        private Hostilities.Hostility HostilityParser(string newHostility)
        {
            switch (newHostility)
            {
                case "OkranReligion":
                    {
                        return Hostilities.Hostility.OkranReligion;
                    }
                case "CorruptOligarchy":
                    {
                        return Hostilities.Hostility.CorruptOligarchy;
                    }
                case "StrengthTest":
                    {
                        return Hostilities.Hostility.StrengthTest;
                    }
                case "HiveCastOuts":
                    {
                        return Hostilities.Hostility.HiveCastOuts;
                    }
                case "Survival":
                    {
                        return Hostilities.Hostility.Survival;
                    }
                default:
                    {
                        Debug.WriteLine("Something went wrong on hostility parser");
                        return Hostilities.Hostility.Survival;
                    }
            }
            
        }
        // Gains or loses relations depending on hostilities
        public void AffectRelations(Adventure myAdventure, Random rnd)
        {
            int relationsLoss = 0;
            for (int i = 0; i < hostilities.Count; i++)
            {
                switch (hostilities[i])
                {
                    case Hostilities.Hostility.OkranReligion:
                        {
                            Hero[] heroes = myAdventure.GetHeroes();
                            for (int j = 0; j < myAdventure.GetHeroesCount(); j += 1)
                            {
                                if(heroes[j].GetSubrace().GetName() != "Greenlander")
                                {
                                    relationsLoss -= 5;
                                }
                                for(int k = 0; k < heroes[j].GetLimbs().Length; k += 1)
                                {
                                    if (heroes[j].GetLimbs()[k].GetName() != "Extremidad normal")
                                    {
                                        relationsLoss += 5;
                                    }
                                }
                            }
                            if(relationsLoss == 0)
                            {
                                AddOrSubtractRelation(10);
                            }

                            break;
                        }
                    case Hostilities.Hostility.CorruptOligarchy:
                        {
                            if(myAdventure.GetCats() / myAdventure.GetHeroesCount() < 200)
                            {
                                relationsLoss += 5;
                            }
                            else
                            {
                                AddOrSubtractRelation(5);
                            }
                            break;
                        }
                    case Hostilities.Hostility.StrengthTest:
                        {
                            Hero[] heroes = myAdventure.GetHeroes();
                            for (int j = 0; j < myAdventure.GetHeroesCount(); j += 1)
                            {
                                if (heroes[j].GetLevel() > 10)
                                {
                                    relationsLoss -= 5;
                                }
                            }
                            if(relationsLoss == 0)
                            {
                                AddOrSubtractRelation(5);
                            }
                            break;
                        }
                    case Hostilities.Hostility.HiveCastOuts:
                        {
                            Hero[] heroes = myAdventure.GetHeroes();
                            for (int j = 0; j < myAdventure.GetHeroesCount(); j += 1)
                            {
                                if (heroes[j].GetRace().GetName() == "Enjambre")
                                {
                                    relationsLoss -= 10;
                                }
                            }
                            if(relationsLoss == 0)
                            {
                                AddOrSubtractRelation(5);
                            }

                            break;
                        }
                    case Hostilities.Hostility.Survival:
                        {
                            int mightNotSeePlayer = rnd.Next(0, 3);
                            switch (mightNotSeePlayer)
                            {
                                case < 2:
                                    {
                                        relationsLoss -= 5;
                                        break;
                                    }
                                case 2:
                                    {
                                        AddOrSubtractRelation(2);
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            Debug.WriteLine("Something went wrong on hostility affect");
                            break;
                        }
                }
            }
            
            AddOrSubtractRelation(relationsLoss);
           
        }
        // Adds or subtracts an amount from the relations
        public void AddOrSubtractRelation(int amount)
        {
            relations += amount;
            if (relations > 100)
            {
                relations = 100;
            }
            if(relations < 0)
            {
                relations = 0;
            }
        }
        // Returns a string of all the hostilities descripted
        public string HostilityToString()
        {
            string toReturn = "Hostilidades:";
            for (int i = 0; i < hostilities.Count; i++)
            {

                switch (hostilities[i])
                {
                    case Hostilities.Hostility.OkranReligion:
                        {
                            toReturn+= "\n  Atacará a los no Greenlander y a gente con extremidades robóticas.";
                            break;
                        }
                    case Hostilities.Hostility.CorruptOligarchy:
                        {
                            toReturn += "\n Son hostiles con los pobres.";
                            break;
                        }
                    case Hostilities.Hostility.StrengthTest:
                        {
                            toReturn += "\n Quieren enfrentarse a enemigos formidables y ganará respeto hacía quienes les derroten.";
                            break;
                        }
                    case Hostilities.Hostility.HiveCastOuts:
                        {
                            toReturn += "\n Atacan a los desertores del Enjambre.";
                            break;
                        }
                    case Hostilities.Hostility.Survival:
                        {
                            toReturn += "\n Atacará a cualquiera con el propósito de sobrevivir.";
                            break;
                        }
                    default:
                        {
                            Debug.WriteLine("Something went wrong on hostility parser");
                            toReturn += "Something went wrong on hostility parser";
                            break;
                        }
                }
            }
            return toReturn;
        }

    }
}
