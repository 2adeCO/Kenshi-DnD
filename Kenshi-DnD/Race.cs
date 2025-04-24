using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class Race : StatModifier
    {
        string name;
        int toughness;
        public Race(string name, int bruteForce, int dexterity, int toughness, int hp, int resistance, int agility) 
            : base(bruteForce, dexterity, hp, resistance, agility)
        {
            this.name = name;
			this.toughness = toughness;
		}
        public string GetName()
        {
            return name;
        }
        public void SetName(string name)
        {
            this.name = name;
        }
        public void SetToughness(int toughness)
        {
            this.toughness = toughness;
        }
        public int GetToughness()
        {
            return toughness;
        }
    }
}
