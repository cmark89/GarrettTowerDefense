using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    public class GlaiveTower : Tower
    {
        public static int Cost = 75;
        public int MaxBounces = 4;
        public int MaxBounceRange = 125;
        public int MaxGlaives = 2;

        //Constructor for arrow towers
        public GlaiveTower()
        {
            Name = "Glaive Tower";
            TileIndex = 12;
            Health = 100;
            Level = 1;

            UpgradeCost = new int[4] { 60, 90, 135, 200 };
            BuildCost = Cost;

            DamageType = DamageType.Physical;
            Damage = 15;
            AttackSpeed = 1.7f;
            AttackRange = 300;
            ProjectileSpeed = 240;
        }

        public override void UpdateTooltipText()
        {
            tooltipText = String.Format("Level {0} {1} \n\nDamage: {2} \nAttack Speed: {3} \nRange: {4} \nBounces: {5}\nBounce Range: {6}\n\nNext Upgrade: \n+4 Damage \n+1 Bounce\n+40 Bounce Range\n\nUpgrade Cost: {7}",
                Level, Name, Damage, AttackSpeed, AttackRange, MaxBounces, MaxBounceRange, Level < 5 ? UpgradeCost[Level - 1].ToString() : " - ");
        }

        public override void LevelUp()
        {
            Damage += 4;
            MaxBounces += 1;
            MaxGlaives += 1;
            MaxBounceRange += 40;

            base.LevelUp();
        }

        public override void LevelDown()
        {
            Damage -= 4;
            MaxBounces -= 1;
            MaxGlaives -= 1;
            MaxBounceRange -= 40;

            base.LevelDown();
        }

        public override void LaunchAttack(Enemy Target)
        {
            if(Projectiles.Count <= MaxGlaives)
            {
                AudioManager.PlaySoundEffect(4);
                //Fire a projectile
                Projectiles.Add(new Projectile(this, Target, ProjectileSpeed, 35));
            }
        }

        public override void OnProjectileHit(Projectile proj, Enemy target)
        {
            if (target != null)
            {
                int finalDamage = Damage - (2 * proj.Bounces);
                target.DamageEnemy(finalDamage, DamageType);
                proj.Bounces++;
                proj.HitEnemies.Add(target);
                AudioManager.PlaySoundEffect(5);

                
                if (proj.Bounces > MaxBounces)
                    DisposeOfProjectile(proj);
                else
                {
                    //Find the next target to bounce to.
                    Random rand = new Random();
                    Enemy thisEnemy;

                    List<Enemy> oldEnemiesList = new List<Enemy>(GameScene.Enemies);
                    List<Enemy> newEnemiesList = new List<Enemy>();
                    while (oldEnemiesList.Count > 0)
                    {
                        thisEnemy = oldEnemiesList[rand.Next(0, oldEnemiesList.Count)];
                        oldEnemiesList.Remove(thisEnemy);
                        newEnemiesList.Add(thisEnemy);
                    }

                    //For each enemy, determine if it's in range.
                    foreach (Enemy e in newEnemiesList)
                    {
                        if (!proj.HitEnemies.Contains(e) && e.Alive && e.Visible && Vector2.Distance(proj.Position, e.Position) <= MaxBounceRange)
                        {
                            //Set the target to the first enemy found to be within range of the tower.
                            proj.SetNewTarget(e);
                            break;
                        }
                    }

                    //Since it's possible for the previous loop to not change the target, dispose of it if the target is the same as before.
                    if(proj.HitEnemies.Contains(proj.Target))
                        DisposeOfProjectile(proj);
                }
            }

            
        }
    }
}
