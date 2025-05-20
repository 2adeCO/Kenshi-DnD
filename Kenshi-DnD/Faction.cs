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
        List<Hostilities.Hostility> hostilities;
        

        public Faction(int factionId, string factionName, string factionDescription, int relations, int factionColor) 
        
            this.factionId = factionId;
            this.factionName = factionName;
            this.factionDescription = factionDescription;
            this.relations = relations;
            this.factionColor = factionColor;
            this.respectByFighting = respectByFighting;

            hostilities = new List<Hostilities.Hostility>();
        }
        
        public void SetFactionName(string factionName)
        {
            this.factionName = factionName;
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
