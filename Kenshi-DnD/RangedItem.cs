using System.Windows.Markup;
using System.Xml.Linq;

namespace Kenshi_DnD
{
    [Serializable]
    class RangedItem : Item
    {
        int difficulty;
        int maxAmmo;
        int ammo;
        public RangedItem(string name, string description, int value, int resellValue, int limbsNeeded, int difficulty, int ammo, StatModifier statToModify)
            : base(name, description, value, resellValue, limbsNeeded, statToModify)
        {
            this.maxAmmo = ammo;
            this.ammo = ammo;
            this.difficulty = difficulty;
        }
        public override string AnnounceUse()
        {
            return "¡El héroe coge distancia... y usa " + "@" + GetRarityColor() + "@" + name + "@" + "!";
        }
        public int GetDifficulty()
        {
            return difficulty;
        }
        public bool IsFull()
        {
            return ammo == maxAmmo;
        }
        public int GetAmmo()
        {
            return ammo;
        }
        public string GetAmmoAndMaxAmmo()
        {
            return ammo + "/" + maxAmmo;
        }
        public void FillAmmo()
        {
            this.ammo = maxAmmo; 
        }
        public void ShootAmmo()
        {
            this.ammo -= 1;
        }
        public override void UpgradeRarity(Rarity.Rarities rarity)
        {
            SetRarity(rarity);
            double costMultiplier = 1.0;
            int buff = 0;

            switch (rarity)
            {
                case Rarity.Rarities.Junk:
                    {
                        costMultiplier = 1.0;
                        break;
                    }
                case Rarity.Rarities.RustCovered:
                    {
                        costMultiplier = 1.10;
                        buff = 1;
                        break;
                    }
                case Rarity.Rarities.Catun:
                    {
                        costMultiplier = 1.25;
                        buff = 2;
                        break;
                    }
                case Rarity.Rarities.Mk:
                    {
                        costMultiplier = 1.50;
                        buff = 3;
                        break;
                    }
                case Rarity.Rarities.Edgewalker:
                    {
                        costMultiplier = 1.75;
                        buff = 4;
                        break;
                    }
                case Rarity.Rarities.Meitou:
                    {
                        costMultiplier = 2.0;
                            buff = 5;
                        break;
                    }
            }

            statToModify.UpgradeStat(Stats.Stat.Dexterity, buff);
            difficulty += buff / 2;

            value = (int)(value * costMultiplier);
            resellValue = (int)(value * 0.5);

        }
        public override Item GetCopy()
        {
            StatModifier statCopy = statToModify.GetCopy();

            Item itemCopy = new RangedItem(name,description,value,resellValue,limbsNeeded,difficulty,maxAmmo, statCopy);
            itemCopy.SetRarity(rarity);
            return itemCopy;
        }
        public override string ItemInfo()
        {
            return base.ItemInfo() + "\n" +
                   "Dificultad: " + difficulty + "\n";
        }
    }
}
