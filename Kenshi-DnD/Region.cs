using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    public class Region
    {
        string name;
        string description;
        List<Faction> factions;
        public Region(string name, string description)
        {
            this.name = name;
            this.description = description;
            factions = new List<Faction>();
        }
        public void AddFaction(Faction faction)
        {
            factions.Add(faction);
        }
        public string GetName()
        {
            return name;
        }
        public string GetDescription()
        {
            return description;
        }
        public List<Faction> GetFactions()
        {
            return factions;
        }
    }
}
