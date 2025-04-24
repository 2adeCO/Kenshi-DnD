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
        protected bool isRare;
        protected bool alreadyUsed;
        protected StatModifier statToModify;

        public Item(string name, int value, int resellValue, int limbsNeeded, StatModifier statToModify,bool isRare)
        {
            this.name = name;
            this.value = value;
            this.resellValue = resellValue;
            this.limbsNeeded = limbsNeeded;
            alreadyUsed = false;
            this.statToModify = statToModify;
            this.isRare = isRare;
        }

        public abstract StatModifier UseItem(Hero hero);
        public override string ToString()
        {
            return "Nombre: " + name + "\n" +
                   "Valor: " + value + "\n" +
                   "Valor de reventa: " + resellValue + "\n" +
                   "Peso (Cantidad de miembros necesitados): " + limbsNeeded + "\n" +
                   (isRare?("Objeto raro"):(""));
        }
        public void UnUse()
        {
            this.alreadyUsed = false;
        }
        public bool CanUseItem(Hero hero)
        {
            if(!alreadyUsed)
            {
                int limbsAvailable = 0;
                for(int i = 0; i < hero.GetLimbs().Length; i += 1)
                {
                    if (!hero.GetLimbs()[i].GetBeingUsed())
                    {
                        limbsAvailable += 1;
                    }
                }
                if (limbsAvailable >= limbsNeeded)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsRare()
        {
            return isRare;
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
        public bool IsUsed()
        {
            return alreadyUsed;
        }

    }
}
