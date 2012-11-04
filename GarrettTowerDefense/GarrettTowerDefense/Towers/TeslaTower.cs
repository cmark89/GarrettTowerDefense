using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    public class TeslaTower : Tower
    {
        public static int Cost = 75;

        //Constructor for arrow towers
        public TeslaTower()
        {
            Name = "Tesla Tower";
            TileIndex = 9;
            Health = 100;
            Level = 1;

            DamageType = DamageType.Electrical;
            Damage = 12;
            AttackSpeed = 3f;
            AttackRange = 225;
            ProjectileSpeed = 350;
        }

        public override void LevelUp()
        {
            Damage += 2;
            AttackSpeed -= .25f;
            AttackRange += 25;

            base.LevelUp();
        }

        public override void LaunchAttack(Enemy Target)
        {
            //Fire a projectile at each enemy in range.
            foreach (Enemy e in GameScene.Enemies)
            {
                if (Vector2.Distance(Position, e.Position) <= AttackRange && e.Visible && e.Alive)
                {
                    Projectiles.Add(new Projectile(this, e, ProjectileSpeed, 33));
                }
            }
        }

        public override void OnProjectileHit(Projectile proj, Enemy target)
        {
            Console.WriteLine("The projecile hits!");
            if (target != null)
                target.DamageEnemy(Damage, DamageType);

            DisposeOfProjectile(proj);
        }
    }
}
