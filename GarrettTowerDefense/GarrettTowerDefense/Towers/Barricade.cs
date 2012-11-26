using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GarrettTowerDefense
{
    public class Barricade : Tower
    {
        public static int Cost = 25;

        //Constructor for arrow towers
        public Barricade()
        {
            Name = "Barricade";
            TileIndex = 16;
            Health = 200;
            Level = 1;

            UpgradeCost = new int[] { 0 };
            BuildCost = Cost;

            DamageType = DamageType.Physical;
            Damage = 0;
            AttackSpeed = 0;
            AttackRange = 0;
        }

        public override void LevelUp()
        {
            return;
        }
    }
}
