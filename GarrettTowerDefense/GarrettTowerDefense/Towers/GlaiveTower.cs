using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    public class GlaiveTower : Tower
    {
        public static int Cost = 90;
        int MaxBounces = 4;
        int MaxBounceRange = 125;
        int MaxGlaives = 2;

        //Constructor for arrow towers
        public GlaiveTower()
        {
            Name = "Glaive Tower";
            TileIndex = 12;
            Health = 100;
            Level = 1;

            DamageType = DamageType.Physical;
            Damage = 10;
            AttackSpeed = 1.8f;
            AttackRange = 300;
            ProjectileSpeed = 240;
        }

        public override void LaunchAttack(Enemy Target)
        {
            if(Projectiles.Count <= MaxGlaives)
            {
                //Fire a projectile
                Projectiles.Add(new Projectile(this, Target, ProjectileSpeed, 35));
            }
        }

        public override void OnProjectileHit(Projectile proj, Enemy target)
        {
            Console.WriteLine("The projecile hits!");
            if (target != null)
            {
                int finalDamage = Damage - (2 * proj.Bounces);
                target.DamageEnemy(Damage, DamageType);
                proj.Bounces++;
                proj.HitEnemies.Add(target);

                
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
