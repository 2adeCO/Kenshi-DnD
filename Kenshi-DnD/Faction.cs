using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class Faction
    {
        int factionId;
        string factionName;
        string factionDescription;

        public Faction(int factionId, string factionName, string factionDescription)
        {
            this.factionId = factionId;
            this.factionName = factionName;

            this.factionDescription = factionDescription;
        }

        public void SetFactionId(int factionId)
        {
            this.factionId = factionId;
        }
        public int GetFactionId()
        {
            return factionId;
        }
        public void SetFactionName(string factionName)
        {
            this.factionName = factionName;
        }
        public string GetFactionName()
        {
            return factionName;
        }
        public void SetFactionDescription(string factionDescription)
        {
            this.factionDescription = factionDescription;
        }
        public string GetFactionDescription()
        {
            return factionDescription;
        }
    }
}
