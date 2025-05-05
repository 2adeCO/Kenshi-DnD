using System.Security.Cryptography.X509Certificates;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Kenshi_DnD
{
    abstract class Item
    {
        protected string name;
        protected int value;
        protected int resellValue;
        protected int limbsNeeded;
        protected int rarity;
        protected bool alreadyUsed;
        protected StatModifier statToModify;

        public Item(string name, int value, int resellValue, int limbsNeeded, StatModifier statToModify, int rarity)
        {
            this.name = name;
            this.value = value;
            this.resellValue = resellValue;
            this.limbsNeeded = limbsNeeded;
            alreadyUsed = false;
            this.statToModify = statToModify;
            this.rarity = rarity;
        }

        public abstract string AnnounceUse();
        public override string ToString()
        {
            return
                   "Valor: " + value + "\n" +
                   "Valor de reventa: " + resellValue + "\n" +
                   "Peso (Cantidad de miembros necesitados): " + limbsNeeded + "\n" +
                   "Rareza: " + RarityToString();
        }
        public void UnUse()
        {
            this.alreadyUsed = false;
        }
        public void SetAlreadyUsed(bool alreadyUsed)
        {
            this.alreadyUsed = alreadyUsed;
        }
        
        public int GetRarity()
        {
            return rarity;
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
        public StatModifier GetStatToModify()
        {
            return statToModify;
        }
        public int GetLimbsNeeded()
        {
            return limbsNeeded;
        }
        public bool IsUsed()
        {
            return alreadyUsed;
        }
        public string RarityToString()
        {
            //Kenshi's actual weapon tiers go like this:
            //Rusted Junk, Rusting Blade, Mid-Grade Salvage, Old Re-Fitted Blade, Re-fitted Blade,
            //Catun No. 1, Catun No. 2, Catun No. 3,
            //Mk I, Mk II, Mk III,
            //Edge Type 1, Edge Type 2, Edge Type 3, Meitou
            //For obvious reasons I kept it simpler.
            //Also, in Kenshi anything below Edge is garbage, I won't make it so harsh in this game haha
            switch (rarity)
            {
                case 0:
                    {
                        return "@8@Chatarra@";
                    }
                case 1:
                    {
                        return "@7@Llena de óxido@";
                    }
                case 2:
                    {
                        return "@6@Catun No. 1@";
                    }
                case 3:
                    {
                        return "@2@Mk I@";
                    }
                case 4:
                    {
                        return "@3@Edgewalker I@";
                    }
                case 5:
                    {
                        return "@6@Meitu@";
                    }
                default:
                    {
                        return "Rareza no puesta";
                    }

            }
        }
        public int GetRarityColor()
        {
            switch (rarity)
            {
                case 0:
                    {
                        return 8;
                    }
                case 1:
                    {
                        return 7;
                    }
                case 2:
                    {
                        return 6;
                    }
                case 3:
                    {
                        return 2;
                    }
                case 4:
                    {
                        return 3;
                    }
                case 5:
                    {
                        return 6;
                    }
                default:
                    {
                        return 9;
                    }
            }
        }
    }
}
