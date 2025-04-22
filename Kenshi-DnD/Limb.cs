using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class Limb : StatModifier
    {
        string name;
        public Limb(string name, int bruteForce, int skill, int toughness, int hp, int resistance, int agility) 
            : base(bruteForce, skill, toughness, hp, resistance, agility)
        {
            this.name = name;
        }
    }
}