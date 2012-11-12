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
        //The Tower's tile graphic index in the tileset
        public int TileIndex;
        
        public string Name { get; protected set; }
        protected int Health { get; set; }
        protected int Level { get; set; }

        protected DamageType DamageType {get; set;}
        protected int Damage { get; set; }
        protected float AttackSpeed { get; set; }
        protected float AttackRange { get; set; }
        protected float ProjectileSpeed { get; set; }

        protected bool Constructed { get; set; }

        public Enemy Target { get; protected set; }
        public float NextAttackTime { get; private set; }

        public List<Projectile> Projectiles;
        public List<Projectile> DisabledProjectiles;

        public int[] UpgradeCost = new int[4];


        

        public Tower()
        {
            //Empty constructor
        }

        //Build the tower on the selected tile
        public virtual void Build(Point point)
        {
            if (GameScene.CurrentMap.TileIsBuildable(point) && GameScene.Gold >= GameScene.LoadedPrice)
            {
                //Play sound effect.
                //Create the tower on the given tile
                MapPosition = point;
                Position = new Vector2(point.X * TileEngine.TileWidth, point.Y * TileEngine.TileHeight);
                ParentCell = GameScene.CurrentMap[point.Y, point.X];

                ParentCell.IsWalkable = false;
                ParentCell.IsBuildable = false;
                ParentCell.ContainsTower = true;

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
            //Attack a target

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
                NextAttackTime = (float)gameTime.TotalGameTime.TotalSeconds + AttackSpeed;
                LaunchAttack(Target);
            }
            else
            {
                //There is no target in range.  Set the next attack time up by .4f seconds or so to prevent hard computing
                NextAttackTime = (float)gameTime.TotalGameTime.TotalSeconds + .4f;
                return;
            }
        }

        public virtual void LaunchAttack(Enemy Target)
        {
            //Actually launch an attack here

            //Fire a projectile

            //For testing purposes, just deal damage to the target here

            Console.WriteLine("The tower strikes!");
            Target.DamageEnemy(Damage, DamageType);
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
                Console.WriteLine("Upgrade " + Name + "!");
                GameScene.Gold -= UpgradeCost[Level - 1];
                LevelUp();
                //Play level up sound effect here.  Maybe a sparkly.
            }
        }

        public virtual void LevelUp()
        {
            //Increase the level of the tower
            Level++;
        }

        public virtual void UpdateStats()
        {
            //Recalculate damage, attack speed and range based on the level / upgrades
        }

        //Destroy the tower, either on purpose or as the result of a game effect
        public virtual void Destroy()
        {
            //KABLAMO!!!

            //Free the parent tile from the no building restriction if applicable.
            if(Name != "Gold Mine")
                ParentCell.IsWalkable = true;

            ParentCell.IsBuildable = true;
            ParentCell.ContainsTower = false;

            GameScene.CurrentMap.SetMovementCost(MapPosition, 1f);

            Constructed = false;
            //Projectiles.Clear();
            //DisabledProjectiles.Clear();
            GameScene.Towers.Remove(GameScene.Towers.Find(x => x == this));

            GameScene.RecalculateEnemyPath();
        }

        public virtual void Initialize()
        {
            GameScene.Towers.Add(this);
            Projectiles = new List<Projectile>();
            DisabledProjectiles = new List<Projectile>();
            Constructed = true;
            NextAttackTime = 0;

            GameScene.RecalculateEnemyPath();
        }
        
        public virtual void Update(GameTime gameTime)
        {
            //Only try to attack if the tower has a damage value of greater than 0
            if (Damage > 0 && gameTime.TotalGameTime.TotalSeconds >= NextAttackTime)
            {
                Attack(gameTime);
            }

            foreach (Projectile p in Projectiles)
            {
                if(p.Enabled)
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

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Constructed)
            {
                spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle(MapPosition.X * TileEngine.TileWidth, MapPosition.Y * TileEngine.TileHeight, TileEngine.TileWidth, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(TileIndex), Color.White);

                if(Level > 1)
                    spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle(MapPosition.X * TileEngine.TileWidth, MapPosition.Y * TileEngine.TileHeight, TileEngine.TileWidth, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(35 + Level), Color.White);
            }

            foreach (Projectile p in Projectiles)
            {
                if (p.Enabled)
                    p.Draw(spriteBatch);
            }
        }


        public void DisposeOfProjectile(Projectile proj)
        {
            DisabledProjectiles.Add(proj);
            proj.Destroy();
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
