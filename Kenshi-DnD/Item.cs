using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenshi_DnD
{
    abstract class Item
    {
        protected string name;
        protected int value;
        protected int resellValue;
        protected int limbsNeeded;
        protected bool alreadyUsed;

        public Item(string name, int value, int resellValue, int limbsNeeded)
        {
            this.name = name;
            this.value = value;
            this.resellValue = resellValue;
            this.limbsNeeded = limbsNeeded;
            alreadyUsed = false;
        }

        public abstract void UseItem(Hero hero);
        public virtual bool CanUseItem(Hero hero)
        {
            // Check if the hero has enough limbs to use the item
            return hero.GetLimbs().Length >= limbsNeeded;
        }
        public string GetName()
        {
            return name;
        }
        public void SetName(string name)
        {
            this.name = name;
        }
        public int GetValue()
        {
            return value;
        }
        public void SetValue(int value)
        {
            this.value = value;
        }
        public int GetResellValue()
        {
            return resellValue;
        }
        public void SetResellValue(int resellValue)
        {
            this.resellValue = resellValue;
        }
        public int GetLimbsNeeded()
        {
            return limbsNeeded;
        }

    }
}
