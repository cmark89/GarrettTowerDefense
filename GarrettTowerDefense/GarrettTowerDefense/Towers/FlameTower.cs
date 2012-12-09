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

            UpgradeCost = new int[] { 50, 75, 125, 175 };
            BuildCost = Cost;

            DamageType = DamageType.Fire;
            Damage = 15;
            AttackSpeed = 1.6f;
            AttackRange = 200;
            ProjectileSpeed = 180;

            burnDuration = 8f;
            burnPercent = .07f;
        }

        public override void UpdateTooltipText()
        {
            tooltipText = String.Format("Level {0} {1} \n\nDamage: {2} \nAttack Speed: {3} \nRange: {4} \nBurn Damage: {5}% \n\nNext Upgrade: \n+3 Damage \n+.2 Attack Speed \n+15 AoE\n\nUpgrade Cost: {6}",
                Level, Name, Damage, AttackSpeed, AttackRange, burnPercent*100, Level < 5 ? UpgradeCost[Level - 1].ToString() : " - ");
        }

        public override void LevelUp()
        {
            Damage += 3;
            AttackSpeed -= .2f;
            burnAoE += 15;

            base.LevelUp();
        }

        public override void LevelDown()
        {
            Damage -= 3;
            AttackSpeed += .2f;
            burnAoE -= 15;

            base.LevelDown();
        }

        public override void LaunchAttack(Enemy Target)
        {
            AudioManager.PlaySoundEffect(3);
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
