using System.Windows.Markup;
using System.Xml.Linq;

namespace Kenshi_DnD
{
    [Serializable]
    class RangedItem : Item
    {
        int difficulty;
        int ammo;
        public RangedItem(string name, string description, int value, int resellValue, int limbsNeeded, int difficulty, int ammo, StatModifier statToModify)
            : base(name, description, value, resellValue, limbsNeeded, statToModify)
        {
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
        public void AddAmmo(int ammo)
        {
            this.ammo += ammo;
        }
        public int GetAmmo()
        {
            return ammo;
        }
        public void ShootAmmo()
        {
            this.ammo -= 1;
        }
        public override void SetRarity(Rarity.Rarities rarity)
        {
            this.rarity = rarity;
            int costIncrease = 0;
            int buff = 0;

            switch (rarity)
            {
                case Rarity.Rarities.Junk:
                    {
                        buff = 1;
                        costIncrease = 0;
                        break;
                    }
                case Rarity.Rarities.RustCovered:
                    {
                        costIncrease = 100;
                        buff = 2; break;
                    }
                case Rarity.Rarities.Catun:
                    {
                        costIncrease = 500;
                        buff = 3; break;
                    }
                case Rarity.Rarities.Mk:
                    {
                        costIncrease = 750;
                        buff = 4; break;
                    }
                case Rarity.Rarities.Edgewalker:
                    {
                        costIncrease = 1500;
                        buff = 5; break;
                    }
                case Rarity.Rarities.Meitou:
                    {
                        costIncrease = 2500;
                        buff = 6; break;
                    }
            }
            
            statToModify.UpgradeStat(Stats.Stat.Dexterity, buff);
            difficulty = difficulty + buff;
            value = value + costIncrease;
            resellValue = resellValue + costIncrease / 2;

        }
        public override Item GetCopy()
        {
            StatModifier statCopy = statToModify.GetCopy();

            Item copy = new RangedItem(name,description,value,resellValue,limbsNeeded,difficulty,ammo, statCopy);
            return copy;
        }
        public override string ToString()
        {
            return base.ToString() + "\n" +
                   "Dificultad: " + difficulty + "\n";
        }
    }
}
