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
        

        public Faction(int factionId, string factionName, string factionDescription, int relations, int factionColor) 
        { 
            this.factionId = factionId;
            this.factionName = factionName;
            this.factionDescription = factionDescription;
            this.relations = relations;
            this.factionColor = factionColor;
            
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
