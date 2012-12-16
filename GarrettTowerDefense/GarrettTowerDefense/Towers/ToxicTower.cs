using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    public class ToxicTower : Tower
    {
        public static int Cost = 45;

        private float poisonDuration;

        //Constructor for arrow towers
        public ToxicTower()
        {
            Name = "Toxic Tower";
            TileIndex = 7;
            Health = 100;
            Level = 1;

            UpgradeCost = new int[] { 30, 60, 90, 120 };
            BuildCost = Cost;

            DamageType = DamageType.Poison;
            Damage = 5;
            AttackSpeed = 1.4f;
            AttackRange = 210;
            ProjectileSpeed = 170;

            poisonDuration = 10f;
        }

        public override void UpdateTooltipText()
        {
            tooltipText = String.Format("Level {0} {1} \n\nDamage: {2} \nAttack Speed: {3} \nRange: {4} \n\nNext Upgrade: \n+5 Damage\n\nUpgrade Cost: {5}",
                Level, Name, Damage, AttackSpeed, AttackRange, Level < 5 ? UpgradeCost[Level - 1].ToString() : " - ");
        }

        public override void LevelUp()
        {
            //Add in toxic pool effect?
            Damage += 5;

            base.LevelUp();
        }

        public override void LevelDown()
        {
            Damage -= 5;

            base.LevelDown();
        }

        public override void Attack(GameTime gameTime)
        {
            if (stunned)
                return;

            //Attack a target

            Target = AcquireNewTarget(Position, AttackRange);
            
            //If the target exists
            if (Target != null && Target.Alive)
            {
                //Attack the target normally
                AttackCharge = 0f;
                LaunchAttack(Target);
            }
            else
            {
                //There is no target in range.  Set the next attack time up by .4f seconds or so to prevent hard computing
                AttackCharge -= .4f;
                return;
            }
        }

        public override Enemy AcquireNewTarget(Vector2 point, float range)
        {
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

            //Now that that list has been cloned, find a target in range that is not poisoned.

            List<Enemy> unpoisonedList = newEnemiesList.FindAll(x => !x.isPoisoned && x.Alive && x.Visible && Vector2.Distance(point, x.Position) <= range);
            Console.WriteLine("Unpoisoned targets: " + unpoisonedList.Count);
            if (unpoisonedList.Count > 0)
            {
                return unpoisonedList[rand.Next(0, unpoisonedList.Count)];
            }
            else
            {
                //For each enemy, determine if it's in range.
                foreach (Enemy e in newEnemiesList)
                {
                    if (e.Alive && e.Visible && Vector2.Distance(point, e.Position) <= range)
                    {
                        //Set the target to the first enemy found to be within range of the tower.
                        return e;
                    }
                }

                //No enemy in range, return null.
                return null;
            }
        }


        public override void LaunchAttack(Enemy Target)
        {
            AudioManager.PlaySoundEffect(12);
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
