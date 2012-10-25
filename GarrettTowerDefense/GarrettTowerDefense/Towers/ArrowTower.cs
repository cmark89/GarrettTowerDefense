using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GarrettTowerDefense
{
    public class ArrowTower : Tower
    {
        public static int Cost = 40;

        //Constructor for arrow towers
        public ArrowTower()
        {
            Name = "Arrow Tower";
            TileIndex = 6;
            Health = 100;
            Level = 1;

            DamageType = DamageType.Physical;
            Damage = 8;
            AttackSpeed = 1.5f;
            AttackRange = 150;
            ProjectileSpeed = 250;
        }

        public override void LaunchAttack(Enemy Target)
        {
            //Fire a projectile
            Projectiles.Add(new Projectile(this, Target, ProjectileSpeed, 30));
        }

        public override void OnProjectileHit(Projectile proj, Enemy target)
        {
            Console.WriteLine("The projecile hits!");
            if(Target != null)
                Target.DamageEnemy(Damage, DamageType);

            DisposeOfProjectile(proj);
        }
    }
}
