using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    public class TeslaTower : Tower
    {
        public static int Cost = 90;

        //Constructor for arrow towers
        public TeslaTower()
        {
            Name = "Tesla Tower";
            TileIndex = 9;
            Health = 100;
            Level = 1;

            UpgradeCost = new int[] { 50, 75, 125, 175 };
            BuildCost = Cost;

            DamageType = DamageType.Electrical;
            Damage = 15;
            AttackSpeed = 3f;
            AttackRange = 225;
            ProjectileSpeed = 350;
        }

        public override void UpdateTooltipText()
        {
            tooltipText = String.Format("Level {0} {1} \n\nDamage: {2} \nAttack Speed: {3} \nRange: {4} \n\nNext Upgrade: \n+2 Damage \n-.25 Attack Speed\n+25 Range\n\nUpgrade Cost: {5}",
                Level, Name, Damage, AttackSpeed, AttackRange, Level < 5 ? UpgradeCost[Level - 1].ToString() : " - ");
        }

        public override void LevelUp()
        {
            Damage += 4;
            AttackSpeed -= .25f;
            AttackRange += 25;

            base.LevelUp();
        }

        public override void LevelDown()
        {
            Damage -= 4;
            AttackSpeed += .25f;
            AttackRange -= 25;

            base.LevelDown();
        }

        public override void LaunchAttack(Enemy Target)
        {
            AudioManager.PlaySoundEffect(8);

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
            if (target != null)
                target.DamageEnemy(Damage, DamageType);

            DisposeOfProjectile(proj);
        }
    }
}
