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

        protected bool isFrozen = false;
        protected float freezeDuration = 0f;

        protected bool isBurning = false;
        protected float burnDPS = 0f;
        protected float burnDuration = 0f;

        protected bool isPoisoned = false;
        protected float poisonDPS = 0f;
        protected float poisonDuration = 0f;


        public virtual void Initialize()
        {
            Alive = true;
            GameScene.Enemies.Add(this);
            Console.WriteLine("\nFind path to castle (located at " + GameScene.CurrentMap.CastleTile.X + ", " + GameScene.CurrentMap.CastleTile.Y + ")");
            GetPath(GameScene.CurrentMap.CastleTile);

            if (Stealthed)
                Visible = false;
            else
                Visible = true;
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
                spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle((int)Position.X, (int)Position.Y, TileEngine.TileWidth, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(TextureID), Color.White);
            }
            else if (Alive && Stealthed && Visible)
            {
                spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle((int)Position.X, (int)Position.Y, TileEngine.TileWidth, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(TextureID), Color.White * .5f);
            }
        }

        //Update the path to the target
        public virtual void GetPath(Point target)
        {
            Waypoints = GameScene.pathfinder.FindPath(TileEngine.ScreenSpaceToMapSpace(Position), target);
            if (Waypoints.Count > 0)
            {
                //There is a path to the end point.  Terminate.
                return;
            }
            else
            {
                //It's time for some big pain to happen!
                TextureID = 2;
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
        }

        public void SetPosition(Vector2 v2)
        {
            Position = v2;
        }


        public void DamageEnemy(int damage, DamageType type)
        {
            if (!Alive)
                return;

            float totalDamage = damage * Weaknesses[(int)type];
            
            CurrentHealth -= totalDamage;
            CheckForDeath();
        }


        public void BeginFreeze(float slow, float duration)
        {
            MovementSpeed = BaseMovementSpeed * slow;
            isFrozen = true;
            freezeDuration = duration;
        }

        public void BeginBurn(float percent, float duration)
        {
            float damage = (percent * Health) * Weaknesses[(int)DamageType.Fire];
            Console.WriteLine(Name + " will burn for " + damage + " damage (" + damage/duration + " damage per second) over " + duration + " seconds.");
            isBurning = true;
            burnDPS = (Health * percent) / duration;
            burnDuration = duration;
        }

        public void BeginPoison(float damage, float duration)
        {
            poisonDPS = damage * Weaknesses[(int)DamageType.Poison];
            Console.WriteLine(Name + " will be poisoned for " + damage + " damage per second for " + duration + " seconds.");
            isPoisoned = true;
            poisonDuration = duration;
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
}
