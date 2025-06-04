using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Kenshi_DnD
{
    [Serializable]
    public abstract class Item
    {
        protected string name;
        protected string description;
        protected int value;
        protected int resellValue;
        protected int limbsNeeded;
        protected Rarity.Rarities rarity;
        protected bool alreadyUsed;
        protected StatModifier statToModify;

        public Item(string name,string description, int value, int resellValue, int limbsNeeded, StatModifier statToModify)
        {
            this.name = name;
            this.description = description;
            this.value = value;
            this.resellValue = resellValue;
            this.limbsNeeded = limbsNeeded;
            alreadyUsed = false;
            this.statToModify = statToModify;
            this.rarity = Rarity.Rarities.Junk;
        }
        
        public abstract string AnnounceUse();
        public virtual string ItemInfo()
        {
            return
                   description + "\n" +
                   "Valor: " + value + "\n" +
                   "Valor de reventa: " + resellValue + "\n" +
                   "Peso (Cantidad de miembros necesitados): " + limbsNeeded + "\n" +
                   "Rareza: " + RarityToString() + "\n" +
                   statToModify.ToString();
        }
        public abstract void UpgradeRarity(Rarity.Rarities rarity);
        public Rarity.Rarities GetRarity()
        {
            return rarity;
        }
        public abstract Item GetCopy();
        public void SetAlreadyUsed(bool alreadyUsed)
        {
            this.alreadyUsed = alreadyUsed;
        }
        public string GetName()
        {
            return name;
        }
        public int GetValue()
        {
            return value;
        }
        public int GetResellValue()
        {
            return resellValue;
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
                case Rarity.Rarities.Junk:
                    {
                        return "@8@Chatarra@";
                    }
                case Rarity.Rarities.RustCovered:
                    {
                        return "@7@Llena de óxido@";
                    }
                case Rarity.Rarities.Catun:
                    {
                        return "@6@Catun No. 1@";
                    }
                case Rarity.Rarities.Mk:
                    {
                        return "@2@Mk I@";
                    }
                case Rarity.Rarities.Edgewalker:
                    {
                        return "@3@Edgewalker I@";
                    }
                case Rarity.Rarities.Meitou:
                    {
                        return "@5@Meitu@";
                    }
                default:
                    {
                        return "Rareza no puesta";
                    }

            }
        }
        public void SetRarity(Rarity.Rarities rarity)
        {
            this.rarity = rarity;
        }
        public int GetRarityColor()
        {
            switch (rarity)
            {
                case Rarity.Rarities.Junk:
                    {
                        return 8;
                    }
                case Rarity.Rarities.RustCovered:
                    {
                        return 7;
                    }
                case Rarity.Rarities.Catun:
                    {
                        return 6;
                    }
                case Rarity.Rarities.Mk:
                    {
                        return 2;
                    }
                case Rarity.Rarities.Edgewalker:
                    {
                        return 3;
                    }
                case Rarity.Rarities.Meitou:
                    {
                        return 5;
                    }
                default:
                    {
                        return 9;
                    }
            }
        }
    }
}
