using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GarrettTowerDefense
{
    public class ArrowTower : Tower
    {
        public static int Cost = 30;

        //Constructor for arrow towers
        public ArrowTower()
        {
            Name = "Arrow Tower";
            TileIndex = 6;
            Health = 100;
            Level = 1;

            UpgradeCost = new int[]{25,50,100,150};
            BuildCost = Cost;

            DamageType = DamageType.Physical;
            Damage = 8;
            AttackSpeed = 1.3f;
            AttackRange = 150;
            ProjectileSpeed = 250;
        }

        public override void UpdateTooltipText()
        {
            tooltipText = String.Format("Level {0} {1} \n\nDamage: {2} \nAttack Speed: {3} \nRange: {4} \n\nNext Upgrade: \n+3 Damage \n+35 Range\n -.1 Attack Speed\n\nUpgrade Cost: {5}",
                Level, Name, Damage, AttackSpeed, AttackRange, Level < 5 ? UpgradeCost[Level - 1].ToString() : " - ");
        }

        public override void LevelUp()
        {
            Damage += 3;
            AttackRange += 35;
            AttackSpeed -= .1f;

            base.LevelUp();
            UpdateTooltipText();
        }

        public override void LevelDown()
        {
            Damage -= 3;
            AttackRange -= 35;
            AttackSpeed += .1f;

            base.LevelDown();
        }

        public override void LaunchAttack(Enemy Target)
        {
            //Fire a projectile
            AudioManager.PlaySoundEffect(2);
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
