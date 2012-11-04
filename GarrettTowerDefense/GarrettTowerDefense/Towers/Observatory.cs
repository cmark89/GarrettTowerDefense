﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GarrettTowerDefense
{
    public class Observatory : Tower
    {
        public static int Cost = 150;
        public static float RevealRange = 100;

        //Constructor for arrow towers
        public Observatory()
        {
            Name = "Observatory";
            TileIndex = 13;
            Health = 100;
            Level = 1;

            DamageType = DamageType.Physical;
            Damage = 0;
            AttackSpeed = 0;
            AttackRange = 700;
        }

        public override void LevelUp()
        {
            RevealRange += 50;
            base.LevelUp();
        }
    }
}
