using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    class RangedItem : Item
    {
        protected int buff;
        //0 is hp, 1 is brute force, 2 is skill, 3 is resistance, 4 is agility
        protected int statToModify;
        protected int difficulty;
        public RangedItem(int buff, int statToModify, int difficulty, string name, int value, int resellValue, int limbsNeeded)
            : base(name, value, resellValue, limbsNeeded)
        {
            this.buff = buff;
            this.statToModify = statToModify;
            this.difficulty = difficulty;
        }
        public override void UseItem(Hero hero)
        {
            // Implement the logic for using the ranged item
            // For example, apply the buff to the hero's stats
            if (CanUseItem(hero))
            {

            }
        }
        public override bool CanUseItem(Hero hero)
        {
            // Implement the logic to check if the hero can use the ranged item
            // For example, check if the hero has enough limbs and meets the difficulty requirement
            return base.CanUseItem(hero) && hero.GetSkill() >= difficulty;
        }
    }
}
