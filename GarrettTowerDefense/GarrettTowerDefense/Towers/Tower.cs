using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GarrettTowerDefense
{

    public class Tower
    {
        //The tile this tower is attached to
        public MapCell ParentCell;
        //The Tower's position in map coordinates, and stores the center of that tile as its Vector2 position.
        public Point MapPosition;
        public Vector2 Position;
        public Vector2 CenterPosition
        {
            get { return new Vector2(Position.X + (TileEngine.TileWidth / 2), Position.Y + (TileEngine.TileHeight / 2)); }
        }

        //The Tower's tile graphic index in the tileset
        public int TileIndex;
        
        public string Name { get; protected set; }
        protected int Health { get; set; }
        public int Level { get; protected set; }

        protected DamageType DamageType {get; set;}
        protected int Damage { get; set; }
        protected float AttackSpeed { get; set; }
        protected float AttackRange { get; set; }
        protected float ProjectileSpeed { get; set; }

        public bool Constructed { get; set; }
        public bool Destroyed { get; set; }

        public Enemy Target { get; protected set; }
        public float AttackCharge { get; protected set; }

        public List<Projectile> Projectiles;
        public List<Projectile> DisabledProjectiles;

        public int[] UpgradeCost = new int[4];
        public int BuildCost;

        protected bool stunned = false;
        protected float unstunTime;
        protected Animation stunAnimation;
        protected Animation explodeAnimation;
        protected float explodeTimeRemaining = 0f;

        // Stores the text displayed in the tooltip.
        public string tooltipText;

        protected int attackSoundEffect;

        public Tower()
        {
            //Empty constructor
        }

        public virtual void UpdateTooltipText()
        {
        }

        //Build the tower on the selected tile
        public virtual void Build(Point point)
        {
            if ((GameScene.CurrentMap.TileIsBuildable(point)  && GameScene.Gold >= GameScene.LoadedPrice))
            {
                //Play sound effect.
                //Create the tower on the given tile
                MapPosition = point;
                Position = new Vector2(point.X * TileEngine.TileWidth, point.Y * TileEngine.TileHeight);
                ParentCell = GameScene.CurrentMap[point.Y, point.X];

                ParentCell.IsWalkable = false;
                ParentCell.IsBuildable = false;
                ParentCell.ContainsTower = true;

                GameScene.Gold -= BuildCost;
                AudioManager.PlaySoundEffect(9);

                GameScene.CurrentMap.SetMovementCost(MapPosition, Pathfinding.Pathfinder.TOWER_COST);

                Initialize();
            }
            else
            {
                //Give error sound
                GameScene.ClearMouseAction();
            }
        }

        public virtual void Attack(GameTime gameTime)
        {
            if (stunned)
                return;

            //Target is out of range
            if (Target != null && Target.Alive && Vector2.Distance(Position, Target.Position) > AttackRange)
            {
                Target = null;
            }

            //First, acquire a new target if there is no target or if the current target has been killed or if the current target is invisible
            if ((Target != null && !Target.Alive) || Target == null || !Target.Visible || Vector2.Distance(Position, Target.Position) > AttackRange)
            {
                Target = AcquireNewTarget(Position, AttackRange);
            }

            //If the target exists
            if (Target != null && Target.Alive)
            {
                //Attack the target normally
                AttackCharge = 0;
                LaunchAttack(Target);
            }
            else
            {
                //There is no target in range.  Set the next attack time up by .4f seconds or so to prevent hard computing
                AttackCharge -= .4f;
                return;
            }
        }

        public virtual void LaunchAttack(Enemy Target)
        {
            //Actually launch an attack here
        }

        public virtual Enemy AcquireNewTarget(Vector2 point, float range)
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

            //For each enemy, determine if it's in range.
            foreach (Enemy e in newEnemiesList)
            {
                if(e.Alive && e.Visible && Vector2.Distance(point, e.Position) <= range)
                {
                    //Set the target to the first enemy found to be within range of the tower.
                    return e;
                }
            }

            //No enemy in range, return null.
            return null;
        }

        public virtual void OnProjectileHit(Projectile proj, Enemy target)
        {
            //Put stuff in here that will obliterate the projectile and thus the enemy.
        }

        public virtual void Upgrade()
        {
            if (Level < 5 && GameScene.Gold >= UpgradeCost[Level - 1])
            {
                GameScene.Gold -= UpgradeCost[Level - 1];
                LevelUp();
                //Play level up sound effect here.  Maybe a sparkly.
            }
        }

        public virtual void LevelUp()
        {
            //Increase the level of the tower
            Level++;
            AudioManager.PlaySoundEffect(11);
            UpdateTooltipText();
        }

        public virtual void LevelDown()
        {
            // Decrease level of tower.
            Level--;
        }

        public virtual void UpdateStats()
        {
            //Recalculate damage, attack speed and range based on the level / upgrades
        }

        //Destroy the tower, either on purpose or as the result of a game effect
        public virtual void Destroy()
        {

            //Free the parent tile from the no building restriction if applicable.
            if (Name != "Gold Mine")
            {
                ParentCell.IsWalkable = true;
                ParentCell.IsBuildable = true;
                GameScene.CurrentMap.SetMovementCost(MapPosition, 1f);
            }

            ParentCell.ContainsTower = false;
            Constructed = false;

            Explode();

            if (Name != "Gold Mine")
            {
                GameScene.RecalculateEnemyPath();
            }
        }

        public virtual void Initialize()
        {
            GameScene.Towers.Add(this);
            Projectiles = new List<Projectile>();
            DisabledProjectiles = new List<Projectile>();
            Constructed = true;
            AttackCharge = 0;
            UpdateTooltipText();

            GameScene.RecalculateEnemyPath();
        }
        
        public virtual void Update(GameTime gameTime)
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

            //Only try to attack if the tower has a damage value of greater than 0
            if (Damage > 0 && AttackCharge >= AttackSpeed)
            {
                Attack(gameTime);
            }

            foreach (Projectile p in Projectiles)
            {
                if(p.Enabled)
                    p.Update(gameTime);
            }

            if (DisabledProjectiles.Count > 0)
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

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Constructed)
            {
                spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle(MapPosition.X * TileEngine.TileWidth, MapPosition.Y * TileEngine.TileHeight, TileEngine.TileWidth, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(TileIndex), Color.White);

                if(Level > 1)
                    spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle(MapPosition.X * TileEngine.TileWidth, MapPosition.Y * TileEngine.TileHeight, TileEngine.TileWidth, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(35 + Level), Color.White);

                if (stunned)
                {
                    spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle(MapPosition.X * TileEngine.TileWidth, MapPosition.Y * TileEngine.TileHeight, TileEngine.TileWidth, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(stunAnimation.CurrentFrameIndex), Color.White);
                }
            }

            foreach (Projectile p in Projectiles)
            {
                if (p.Enabled)
                    p.Draw(spriteBatch);
            }

            if (explodeAnimation != null)
            {
                spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle(MapPosition.X * TileEngine.TileWidth, MapPosition.Y * TileEngine.TileHeight, TileEngine.TileWidth, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(explodeAnimation.CurrentFrameIndex), Color.White);
            }
        }


        public void DisposeOfProjectile(Projectile proj)
        {
            DisabledProjectiles.Add(proj);
            proj.Destroy();
        }

        public void Stun(float duration)
        {
            stunned = true;
            unstunTime = duration;
            stunAnimation = new Animation(new int[] { 46, 47 }, 5);
        }

        public void Sell(float refundPercentage)
        {
            int upgradeTotal = 0;
            if (Level > 1)
            {
                for (int i = 0; i < Level - 1; i++)
                {
                    upgradeTotal += UpgradeCost[i];
                }
            }

            int totalCost = BuildCost + upgradeTotal;
            GameScene.Gold += (int)(totalCost * refundPercentage);

            Destroy();
        }

        // Creates the explosion animation and triggers the countdown to absolute destruction.
        public void Explode()
        {
            explodeAnimation = new Animation(new int[] { 48, 49, 50, 51, 52, 53 }, 12, true);
            explodeAnimation.AnimationFinish += new AnimationFinishEventHandler(FinishExploding);
            AudioManager.PlaySoundEffect(10);
        }

        // Finally, remove the tower from the list, ensuring it will be garbage collected.
        public void FinishExploding()
        {
            Console.WriteLine("Explosion callback completed.");
            explodeAnimation = null;
            Destroyed = true;
        }

        public void RemoveTower()
        {
            GameScene.Towers.Remove(GameScene.Towers.Find(x => x == this));
        }
    }

    public enum DamageType
    {
        Physical = 0,
        Poison,
        Fire,
        Ice,
        Electrical
    }
}
