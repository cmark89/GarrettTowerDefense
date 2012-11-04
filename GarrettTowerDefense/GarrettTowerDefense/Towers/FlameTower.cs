using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GarrettTowerDefense
{
    public class FlameTower : Tower
    {
        public static int Cost = 70;

        public float burnPercent;
        public float burnDuration;

        public float burnAoE = 75;

        //Constructor for arrow towers
        public FlameTower()
        {
            Name = "Flame Tower";
            TileIndex = 8;
            Health = 100;
            Level = 1;

            DamageType = DamageType.Fire;
            Damage = 15;
            AttackSpeed = 1.6f;
            AttackRange = 200;
            ProjectileSpeed = 180;

            burnDuration = 5f;
            burnPercent = .1f;
        }

        public override void LevelUp()
        {
            Damage += 3;
            AttackSpeed -= .2f;
            burnAoE += 15;

            base.LevelUp();
        }

        public override void LaunchAttack(Enemy Target)
        {
            //Fire a projectile
            Projectiles.Add(new Projectile(this, Target, ProjectileSpeed, 32));
        }

        public override void OnProjectileHit(Projectile proj, Enemy target)
        {
            Console.WriteLine("The burning projecile hits!");
            if (Target != null)
            {
                Target.DamageEnemy(Damage, DamageType);
                Target.BeginBurn(burnPercent, burnDuration);
            }

            DisposeOfProjectile(proj);
        }
    }
}
