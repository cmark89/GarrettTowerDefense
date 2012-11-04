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

        private float nextAltAttackTime;
        private float altAttackSpeed;

        public int altDamage;

        //Constructor for arrow towers
        public IceTower()
        {
            Name = "Ice Tower";
            TileIndex = 11;
            Health = 100;
            Level = 1;

            UpgradeCost = new int[] { 40, 70, 120, 170 };

            DamageType = DamageType.Ice;
            Damage = 10;
            AttackSpeed = 1.4f;
            AttackRange = 200;
            ProjectileSpeed = 160;

            slowAmount = .5f;
            freezeDuration = 3f;

            altAttackSpeed = 5f;
            altDamage = 0;
        }

        public override void LevelUp()
        {
            Damage += 2;
            altDamage += 1;
            altAttackSpeed -= 1f;

            base.LevelUp();
        }

        public override void Update(GameTime gameTime)
        {
            //Only try to attack if the tower has a damage value of greater than 0
            if (Damage > 0 && gameTime.TotalGameTime.TotalSeconds >= NextAttackTime)
            {
                Attack(gameTime);
            }

            if (Level > 1 && gameTime.TotalGameTime.TotalSeconds >= nextAltAttackTime)
            {
                AltAttack(gameTime);
            }

            foreach (Projectile p in Projectiles)
            {
                if (p.Enabled)
                    p.Update(gameTime);
            }

            if (DisabledProjectiles.Count == 0)
                return;
            else
            {
                foreach (Projectile p in DisabledProjectiles)
                {
                    Projectiles.Remove(p);
                }
            }
        }


        public void AltAttack(GameTime gameTime)
        {
            nextAltAttackTime = (float)gameTime.TotalGameTime.TotalSeconds + altAttackSpeed;

            List<Enemy> altAttackTargets = GameScene.Enemies.FindAll(x => Target != x && x.Alive && x.Visible && Vector2.Distance(x.Position, Position) <= AttackRange);
            if(altAttackTargets.Count > 0)
                LaunchSecondaryAttack(altAttackTargets[new Random().Next(0, altAttackTargets.Count)]);
        }


        public override void LaunchAttack(Enemy Target)
        {
            //Fire a projectile
            Projectiles.Add(new Projectile(this, Target, ProjectileSpeed, 34));
        }

        public void LaunchSecondaryAttack(Enemy Target)
        {
            //Fire a new projectile
            Projectiles.Add(new Projectile(this, Target, ProjectileSpeed, 36, "Icicle"));
        }

        public override void OnProjectileHit(Projectile proj, Enemy target)
        {
            Console.WriteLine("The projecile hits!");
            if (target != null)
            {
                target.BeginFreeze(slowAmount, freezeDuration);

                if (proj.Name != "Icicle")
                    target.DamageEnemy(Damage, DamageType);
                else if (proj.Name == "Icicle" && altDamage > 0)
                    target.DamageEnemy(altDamage, DamageType);
                    
            }

            DisposeOfProjectile(proj);
        }
    }
}
