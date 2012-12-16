using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GarrettTowerDefense
{
    public class Observatory : Tower
    {
        public static int Cost = 45;
        public static float RevealRange = 150;

        //Constructor for arrow towers
        public Observatory()
        {
            Name = "Observatory";
            TileIndex = 13;
            Health = 100;
            Level = 1;

            UpgradeCost = new int[] { 30, 60, 90, 120 };
            BuildCost = Cost;

            DamageType = DamageType.Physical;
            Damage = 0;
            AttackSpeed = 0;
            AttackRange = 700;
        }

        public override void UpdateTooltipText()
        {
            tooltipText = String.Format("Level {0} {1} \n\n\nRange: {2} \n\nNext Upgrade: \n+50 Range\n\nUpgrade Cost: {3}",
                Level, Name, RevealRange, Level < 5 ? UpgradeCost[Level - 1].ToString() : " - ");
        }

        public override void LevelUp()
        {
            RevealRange += 50;
            base.LevelUp();
        }

        public override void LevelDown()
        {
            RevealRange -= 50;

            base.LevelDown();
        }
    }
}
