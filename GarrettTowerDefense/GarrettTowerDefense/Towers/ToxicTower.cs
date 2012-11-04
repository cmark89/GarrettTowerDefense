using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GarrettTowerDefense
{
    public class ToxicTower : Tower
    {
        public static int Cost = 60;

        private float poisonDuration;

        //Constructor for arrow towers
        public ToxicTower()
        {
            Name = "Toxic Tower";
            TileIndex = 7;
            Health = 100;
            Level = 1;

            UpgradeCost = new int[] { 30, 60, 90, 120 };

            DamageType = DamageType.Poison;
            Damage = 5;
            AttackSpeed = 1.4f;
            AttackRange = 270;
            ProjectileSpeed = 170;

            poisonDuration = 10f;
        }

        public override void LevelUp()
        {
            //Add in toxic pool effect?
            Damage += 5;

            base.LevelUp();
        }

        public override void LaunchAttack(Enemy Target)
        {
            //Fire a projectile
            Projectiles.Add(new Projectile(this, Target, ProjectileSpeed, 31));
        }

        public override void OnProjectileHit(Projectile proj, Enemy target)
        {
            Console.WriteLine("The toxic projecile hits!");
            if (Target != null)
            {
                Target.DamageEnemy(Damage, DamageType);

                //Start the target a-chokin'!
                //Use this formula to calculate the damage done.
                float poisonDPS = (Damage * 4) / poisonDuration;
                Target.BeginPoison(poisonDPS, poisonDuration);
            }

            DisposeOfProjectile(proj);
        }
    }
}
