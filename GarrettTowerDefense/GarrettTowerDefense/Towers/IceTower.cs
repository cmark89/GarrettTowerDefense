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

        private float altAttackCharge;
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
            BuildCost = Cost;

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

        public override void UpdateTooltipText()
        {
            if(Level == 1)
            {
                tooltipText = String.Format("Level {0} {1} \n\nDamage: {2} \nAttack Speed: {3} \nRange: {4} \n\nNext Upgrade: \n+2 Damage \nGrants Sub-Attack\n\nUpgrade Cost: {5}",
                    Level, Name, Damage, AttackSpeed, AttackRange, Level < 5 ? UpgradeCost[Level - 1].ToString() : " - ");
            }
            else
            {
                   tooltipText = String.Format("Level {0} {1} \n\nDamage: {2} \nAttack Speed: {3} \nRange: {4}\nSub-Attack Speed: {5} \n\nNext Upgrade: \n+2 Damage \nSub-Attack Speed - 1\n\nUpgrade Cost: {6}",
                    Level, Name, Damage, AttackSpeed, AttackRange, altAttackSpeed, Level < 5 ? UpgradeCost[Level - 1].ToString() : " - ");
            }
            
        }

        public override void LevelUp()
        {
            Damage += 2;
            altDamage += 1;
            altAttackSpeed -= 1f;

            base.LevelUp();
        }

        public override void LevelDown()
        {
            Damage -= 2;
            altDamage -= 1;
            altAttackSpeed += 1;

            base.LevelDown();
        }

        public override void Update(GameTime gameTime)
        {
            if (stunned)
            {
                stunAnimation.Update(gameTime);
                unstunTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (unstunTime <= 0)
                {
                    stunned = false;
                    stunAnimation = null;
                }
            }

            AttackCharge += (float)gameTime.ElapsedGameTime.TotalSeconds;
            altAttackCharge += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Only try to attack if the tower has a damage value of greater than 0
            if (Damage > 0 && AttackCharge >= AttackSpeed)
            {
                Attack(gameTime);
            }

            if (Level > 1 && altAttackCharge >= altAttackSpeed)
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

            if (explodeAnimation != null)
            {
                explodeAnimation.Update(gameTime);
            }
        }


        public void AltAttack(GameTime gameTime)
        {
            altAttackCharge = 0f;

            List<Enemy> altAttackTargets = GameScene.Enemies.FindAll(x => Target != x && x.Alive && x.Visible && Vector2.Distance(x.Position, Position) <= AttackRange);
            if(altAttackTargets.Count > 0)
                LaunchSecondaryAttack(altAttackTargets[new Random().Next(0, altAttackTargets.Count)]);
        }


        public override void LaunchAttack(Enemy Target)
        {
            AudioManager.PlaySoundEffect(7);
            //Fire a projectile
            Projectiles.Add(new Projectile(this, Target, ProjectileSpeed, 34));
        }

        public void LaunchSecondaryAttack(Enemy Target)
        {
            AudioManager.PlaySoundEffect(7);
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
