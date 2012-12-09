using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pathfinding;

namespace GarrettTowerDefense
{
    public class Enemy
    {
        //Name of the enemy for display purposes
        public string Name { get; protected set; }
        //The position of the enemy in screen space.
        public Vector2 Position { get; protected set; }
        public Vector2 CenterPosition
        {
            get { return new Vector2(Position.X + (TileEngine.TileWidth / 2), Position.Y + (TileEngine.TileHeight / 2)); }
        }

        public Point MapPosition { get; protected set; }
        //The index of the enemy in the sprite sheet
        public int TextureID { get; protected set; }

        public bool Alive { get; set; }

        public float BaseHealth { get; protected set; }
        public float Health { get; protected set; }
        public float CurrentHealth { get; protected set; }

        public int Bounty { get; protected set; }

        public int Damage { get; protected set; }

        public float BaseMovementSpeed { get; protected set; }
        public float MovementSpeed { get; protected set; }
        public bool Stealthed { get; protected set; }
        public bool Visible { get; protected set; }

        public List<Keyword> Keywords { get; protected set; }
        public float[] Weaknesses { get; protected set; }
        public MonsterState CurrentState { get; protected set; }

        public List<Vector2> Waypoints { get; protected set; }

        public Color DrawColor { get; protected set; }
        public Color CarnageColor { get; protected set; }

        protected bool isFrozen = false;
        protected float freezeDuration = 0f;

        protected bool isBurning = false;
        protected float burnDPS = 0f;
        protected float burnDuration = 0f;

        public bool isPoisoned = false;
        protected float poisonDPS = 0f;
        protected float poisonDuration = 0f;

        protected bool _canDestroyTowers = false;

        private bool regenHealth = false;
        private float regenPercent = .025f;


        public virtual void Initialize(Vector2 initialPosition)
        {
            Position = initialPosition;
            Alive = true;
            DrawColor = Color.White;
            GameScene.Enemies.Add(this);
            Console.WriteLine("\nFind path to castle (located at " + GameScene.CurrentMap.CastleTile.X + ", " + GameScene.CurrentMap.CastleTile.Y + ")");
            GetPath(GameScene.CurrentMap.CastleTile);

            if (Stealthed)
                Visible = false;
            else
                Visible = true;

            if (WaveManager.carnageMode)
            {
                DrawColor = CarnageColor;
                ApplyCarnageBuffs();
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            //Move the enemy and shit if not stunned
            if (CurrentState != MonsterState.Stunned)
            {
                //Enemy is frozen; check for freeze expiration
                if (isFrozen)
                {
                    freezeDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (freezeDuration <= 0)
                    {
                        isFrozen = false;
                        MovementSpeed = BaseMovementSpeed;
                    }
                }
                //Enemy is burning; check for burn expiration and damage
                if (isBurning)
                {
                    float burnDamage = (float)(burnDPS * gameTime.ElapsedGameTime.TotalSeconds);
                    CurrentHealth -= burnDamage;

                    burnDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (burnDuration <= 0)
                    {
                        isBurning = false;
                    }
                }

                //Enemy is poisoned; check for poison damage and expiration
                if (isPoisoned)
                {
                    float poisonDamage = (float)(poisonDPS * gameTime.ElapsedGameTime.TotalSeconds);
                    CurrentHealth -= poisonDamage;

                    poisonDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (poisonDuration <= 0)
                    {
                        isPoisoned = false;
                    }
                }

                //Check for death from damage over time effects
                CheckForDeath();

                //If the monster uses stealth...
                if (Stealthed)
                {
                    //Check for an observatory in detection range of the monster
                    Tower observ = GameScene.Towers.Find(x => x.Name == "Observatory" && Vector2.Distance(Position, x.Position) < Observatory.RevealRange);
                    if (observ == null)
                    {
                        //If there is no observatory, it remains hidden
                        Visible = false;
                    }
                    else
                    {
                        //Else, it is visible
                        Visible = true;
                    }
                }

                if (regenHealth && CurrentHealth < Health)
                {
                    float healedAmount = (Health * regenPercent) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    CurrentHealth += healedAmount;
                    if (CurrentHealth > Health)
                    {
                        CurrentHealth = Health;
                    }
                }

                UpdateMovement(gameTime);
                
                //If the enemy is in  the castle tile, destroy it and damage the castle.
                if (TileEngine.ScreenSpaceToMapSpace(Position) == GameScene.CurrentMap.CastleTile)
                {
                    Console.WriteLine("Enemy hit the castle!");
                    Alive = false;
                    GameScene.CurHealth -= Damage;
                }
            }
        }

        public virtual void CheckForDeath()
        {
            if (CurrentHealth <= 0)
            {
                GameScene.GainGold(Bounty);
                Alive = false;
                return;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Alive && !Stealthed)
            {
                spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle((int)Position.X, (int)Position.Y, TileEngine.TileWidth, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(TextureID), DrawColor);
            }
            else if (Alive && Stealthed && Visible)
            {
                spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle((int)Position.X, (int)Position.Y, TileEngine.TileWidth, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(TextureID), DrawColor * .5f);
            }
        }

        //Update the path to the target
        public virtual void GetPath(Point target)
        {
            int pathCost = 0;
            Waypoints = GameScene.pathfinder.FindPath(TileEngine.ScreenSpaceToMapSpace(Position), target, ref  pathCost);
            Console.WriteLine("Position:" + Position);
            Console.WriteLine("Map Position: " + TileEngine.ScreenSpaceToMapSpace(Position));

            if (pathCost < Pathfinder.TOWER_COST)
            {
                //There is a path to the end point that does not go through a tower.
                //Ensure that the monster cannot destroy towers.
                _canDestroyTowers = false;
                //Return.
                return;
            }
            else
            {
                //It's time for some big pain to happen!
                _canDestroyTowers = true;
                return;
            }
        }


        public virtual void UpdateMovement(GameTime gameTime)
        {
            if (Waypoints.Count == 0)
                return;

            //Check if the distance to the next waypoint (Waypoints[0]) is less than or equal to the movement this frame; if so, set the 
            //monster's position to that point, and then remove the first waypoint from the list and set the next.
            float movementThisFrame = MovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Vector2.Distance(Position, Waypoints[0]) <= movementThisFrame)
            {
                Position = Waypoints[0];
                Waypoints.RemoveAt(0);
            }
            else
            {
                //Otherwise, move towards the waypoint
                Vector2 newMovement = Waypoints[0] - Position;
                newMovement.Normalize();

                newMovement *= movementThisFrame;
                Position += newMovement;
            }

            MapPosition = TileEngine.ScreenSpaceToMapSpace(Position);

            if (_canDestroyTowers && GameScene.CurrentMap[MapPosition.Y, MapPosition.X].ContainsTower)
            {
                GameScene.Towers.Find(x => x.MapPosition == MapPosition).Destroy();
            }
        }

        public void SetPosition(Vector2 v2)
        {
            Position = v2;
            Console.WriteLine("Enemy spawn! " + Position);
        }


        public virtual void DamageEnemy(int damage, DamageType type)
        {
            if (!Alive)
                return;

            float totalDamage = damage * Weaknesses[(int)type];
            
            CurrentHealth -= totalDamage;
            CheckForDeath();
        }


        public void BeginFreeze(float slow, float duration)
        {
            if (Weaknesses[(int)DamageType.Ice] <= .3f)
                return;

            MovementSpeed = BaseMovementSpeed * slow;
            isFrozen = true;
            freezeDuration = duration;
        }

        public virtual void BeginBurn(float percent, float duration)
        {
            if (Weaknesses[(int)DamageType.Fire] <= .3f)
                return;

            float damage = (percent * Health) * Weaknesses[(int)DamageType.Fire];
            Console.WriteLine(Name + " will burn for " + damage + " damage (" + damage/duration + " damage per second) over " + duration + " seconds.");
            isBurning = true;
            burnDPS = (Health * percent) / duration;
            burnDuration = duration;
        }

        public void BeginPoison(float damage, float duration)
        {
            if (Weaknesses[(int)DamageType.Poison] <= .3f)
                return;

            poisonDPS = damage * Weaknesses[(int)DamageType.Poison];
            Console.WriteLine(Name + " will be poisoned for " + damage + " damage per second for " + duration + " seconds.");
            isPoisoned = true;
            poisonDuration = duration;
        }

        // Finds all the carnage mode buffs in the current wave and applies them to the monster
        public void ApplyCarnageBuffs()
        {
            if (WaveManager.CarnageBuffs.Contains(CarnageModeBuff.Fast))
            {
                BaseMovementSpeed += 20;
                MovementSpeed = BaseMovementSpeed;
            }

            if (WaveManager.CarnageBuffs.Contains(CarnageModeBuff.FireImmune))
            {
                Weaknesses[(int)DamageType.Fire] = .25f;
            }

            if (WaveManager.CarnageBuffs.Contains(CarnageModeBuff.IceImmune))
            {
                Weaknesses[(int)DamageType.Ice] = .25f;
            }

            if (WaveManager.CarnageBuffs.Contains(CarnageModeBuff.ShockImmune))
            {
                Weaknesses[(int)DamageType.Electrical] = .25f;
            }

            if (WaveManager.CarnageBuffs.Contains(CarnageModeBuff.PoisonImmune))
            {
                Weaknesses[(int)DamageType.Poison] = .25f;
            }

            if(WaveManager.CarnageBuffs.Contains(CarnageModeBuff.Prismatic))
            {
                Weaknesses[(int)DamageType.Poison] = .5f;
                Weaknesses[(int)DamageType.Fire] = .5f;
                Weaknesses[(int)DamageType.Ice] = .5f;
                Weaknesses[(int)DamageType.Electrical] = .5f;
            }

            if (WaveManager.CarnageBuffs.Contains(CarnageModeBuff.Immortal))
            {
                Health *= 1.5f;
                CurrentHealth = Health;
            }

            if (WaveManager.CarnageBuffs.Contains(CarnageModeBuff.Tough))
            {
                Weaknesses[(int)DamageType.Physical] = .3f;
            }

            if (WaveManager.CarnageBuffs.Contains(CarnageModeBuff.Regenerating))
            {
                regenHealth = true;
            }

            if (WaveManager.CarnageBuffs.Contains(CarnageModeBuff.Invisible))
            {
                Stealthed = true;
            }
        }
    }

    public enum Keyword
    {
        Fast = 0,       //Enemy moves at 150% speed
        Tough,          //Enemy takes 30% reduced damage
        Flying,         //Enemy is flying and can fly over terrain
        Invisible,      //Enemy is invisible and cannot be seen without detection
        Immune          //Enemy is immune to slowing / poisoning / burning effects
    }

    public enum MonsterState
    {
        Normal,
        Poisoned,
        Burning,
        Slowed,
        Stunned
    }

    public enum BossPhase
    {
        Walk = 0,
        Summon,
        ShieldWalk,
        AttackWalk
    }

    public enum CarnageModeBuff
    {
        Invisible,      //The spawned enemy will be invisible
        FireImmune,     //The spawned enemy will be immune to fire damage
        IceImmune,      //The spawned enemy will be immune to ice damage
        ShockImmune,    //The spawned enemy will be immune to shock damage
        PoisonImmune,   //The spawned enemy will be immune to poison damage
        Prismatic,      //The spawned enemy will take half damage from all elemental damage
        Regenerating,   //The spawned enemy will regenerate 3% of its maximum health per second.
        Fast,           //The spawned enemy will move with +15 speed.
        Tough,          //The spawned enemy will take half damage from physical attacks
        Immortal        //The spawned enemy will have double health
    }
}
