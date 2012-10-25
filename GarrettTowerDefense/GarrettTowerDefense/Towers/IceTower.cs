using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    public class IceTower : Tower
    {
        public static int Cost = 70;

        private float slowAmount;
        private float freezeDuration;

        //Constructor for arrow towers
        public IceTower()
        {
            Name = "Ice Tower";
            TileIndex = 11;
            Health = 100;
            Level = 1;

            DamageType = DamageType.Ice;
            Damage = 10;
            AttackSpeed = 1.4f;
            AttackRange = 200;
            ProjectileSpeed = 160;

            slowAmount = .5f;
            freezeDuration = 3f;
        }

        public override void LaunchAttack(Enemy Target)
        {
            //Fire a projectile
            Projectiles.Add(new Projectile(this, Target, ProjectileSpeed, 34));
        }

        public override void OnProjectileHit(Projectile proj, Enemy target)
        {
            Console.WriteLine("The projecile hits!");
            if (Target != null)
            {
                Target.BeginFreeze(slowAmount, freezeDuration);
                Target.DamageEnemy(Damage, DamageType);
            }

            DisposeOfProjectile(proj);
        }
    }
}
