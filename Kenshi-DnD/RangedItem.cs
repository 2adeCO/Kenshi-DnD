namespace Kenshi_DnD
{
    // Item which is ranged. - Santiago Cabrero
    [Serializable]
    class RangedItem : Item
    {
        // Difficulty when the item is shot
        int difficulty;
        // Max ammo capacity of the item
        int maxAmmo;
        // Current ammo
        int ammo;
        // Constructor
        public RangedItem(string name, string description, int value, int resellValue, int limbsNeeded, int difficulty, int ammo, StatModifier statToModify)
            : base(name, description, value, resellValue, limbsNeeded, statToModify)
        {
            this.maxAmmo = ammo;
            this.ammo = ammo;
            this.difficulty = difficulty;
        }
        // Abstract override that returns a string that announces the use of the item
        public override string AnnounceUse()
        {
            return "¡El héroe coge distancia... y usa " + "@" + GetRarityColor() + "@" + name + "@" + "!";
        }
        // Getters and setters
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
        // Loses one ammo
        public void ShootAmmo()
        {
            this.ammo -= 1;
        }
        // Ranged item rarity upgrade affects on dexterity
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
        // Returns copy of the item
        public override Item GetCopy()
        {
            StatModifier statCopy = statToModify.GetCopy();

            Item itemCopy = new RangedItem(name, description, value, resellValue, limbsNeeded, difficulty, maxAmmo, statCopy);
            itemCopy.SetRarity(rarity);
            return itemCopy;
        }
        // Virtual override that returns info about the item
        public override string ItemInfo()
        {
            return base.ItemInfo() + "\n" +
                   "Dificultad: " + difficulty + "\n";
        }
    }
}
