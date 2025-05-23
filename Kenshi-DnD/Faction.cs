using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Kenshi_DnD
{
    [Serializable]
    public class Faction
    {
        int factionId;
        string factionName;
        string factionDescription;
        int factionColor;
        int relations;
        List<Hostilities.Hostility> hostilities;
        bool respectByFighting;

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
        public int GetFactionId()
        {
            return factionId;
        }
        public string GetFactionName()
        {
            return "@" + factionColor + "@" + factionName +"@";
        }
        public void SetFactionDescription(string factionDescription)
        {
            this.factionDescription = factionDescription;
        }
        public void AddHostility(string newHostility)
        {
            Debug.WriteLine("Adding hostility: " + newHostility);
            hostilities.Add(HostilityParser(newHostility));
        }
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
        public void AffectRelations(Adventure myAdventure)
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
                                    relationsLoss += 5;
                                }
                                for(int k = 0; k < heroes[j].GetLimbs().Length; k += 1)
                                {
                                    if (heroes[j].GetLimbs()[k].GetName() != "Extremidad normal")
                                    {
                                        relationsLoss += 5;
                                    }
                                }
                            }
                            break;
                        }
                    case Hostilities.Hostility.CorruptOligarchy:
                        {
                            if(myAdventure.GetCats() / myAdventure.GetHeroesCount() < 200)
                            {
                                relationsLoss += 5;
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
                                    relationsLoss += 5;
                                }
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
                                    relationsLoss += 10;
                                }
                            }
                            break;
                        }
                    case Hostilities.Hostility.Survival:
                        {
                            relationsLoss += 5;
                            break;
                        }
                    default:
                        {
                            Debug.WriteLine("Something went wrong on hostility affect");
                            break;
                        }
                }
            }
            
            relations -= relationsLoss;
            if (relations < 0)
            {
                relations = 0;
            }
        }
        public void ImproveRelations(int amount)
        {
            relations += amount;
            if (relations > 100)
            {
                relations = 100;
            }
        }
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
        public string GetFactionDescription()
        {
            return factionDescription;
        }
       
        public void SetFactionColor(int factionColor)
        {
            this.factionColor = factionColor;
        }
        public int GetFactionColor()
        {
            return this.factionColor;
        }
        public int GetRelation()
        {
            return relations;
        }
    }
}
