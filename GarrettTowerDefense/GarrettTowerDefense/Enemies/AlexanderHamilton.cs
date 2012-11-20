using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GarrettTowerDefense
{
    class AlexanderHamilton: Enemy
    {
        public int currentPhase;
        public BossPhase bossPhase;

        // True when the damage shield activates
        public bool shieldActive = false;
        // This is the type of damage to ignore
        public DamageType shieldType;

        public GameTime currentGameTime;
        public float healthPercentage;

        private Animation beamAnimation;
        private Animation projectileAnimation;
        private List<Animation> paralyzeAnimations;

        public AlexanderHamilton()
        {
            Name = "Alexander Hamilton";
            TextureID = 17;

            BaseHealth = 30000;
            //Calculate true health based on the wave number
            Health = BaseHealth;
            CurrentHealth = Health;

            Bounty = 150;

            Damage = 100;

            currentPhase = 1;
            bossPhase = BossPhase.Walk;

            BaseMovementSpeed = 35;
            MovementSpeed = BaseMovementSpeed;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { 1f, 1f, 1f, 1f, 1f };

            CarnageColor = Color.White;

            CurrentState = MonsterState.Normal;

            //base.Initialize();
        }

        #region Overrides
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Alive)
            {
                Color drawColor = Color.White;
                if (bossPhase == BossPhase.Summon)
                    drawColor = new Color(.5f, .5f, .5f, .5f);

                spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle((int)Position.X, (int)Position.Y, TileEngine.TileWidth, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(TextureID), drawColor);

                if (beamAnimation != null)
                {
                    // Draw the beam here, angled between Hamilton and its target.  
                }

                foreach (Animation a in paralyzeAnimations)
                {
                    // Draw each paralysis animation.
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Cache the game time
            currentGameTime = gameTime;

            Console.WriteLine(CurrentHealth);

            //Move the enemy and shit if not stunned
            if (CurrentState != MonsterState.Stunned)
            {
                //Enemy is frozen; check for freeze expiration
                if (isFrozen)
                {
                    freezeDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds * 2f;
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

                    burnDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds * 2f;
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

                    poisonDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds * 2f;
                    if (poisonDuration <= 0)
                    {
                        isPoisoned = false;
                    }
                }

                //Check for death from damage over time effects
                CheckForDeath();

                healthPercentage = CurrentHealth / Health;
                CheckHealth();

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

                switch (bossPhase)
                {
                    case BossPhase.Walk:
                        UpdateMovement(gameTime);
                        break;
                    case BossPhase.Summon:
                        //UpdateSummoning(gameTime);
                        break;
                    case BossPhase.ShieldWalk:
                        UpdateMovement(gameTime);
                        break;
                    case BossPhase.AttackWalk:
                        UpdateAttack(gameTime);
                        UpdateMovement(gameTime);
                        break;
                    default:
                        break;
                }


                //If the enemy is in  the castle tile, destroy it and damage the castle.
                if (TileEngine.ScreenSpaceToMapSpace(Position) == GameScene.CurrentMap.CastleTile)
                {
                    Console.WriteLine("Enemy hit the castle!");
                    Alive = false;
                    GameScene.CurHealth -= Damage;
                }
            }
        }

        // Checks the boss's health to see if it needs to enter the next phase
        public override void CheckHealth()
        {
            if (healthPercentage < .85f && currentPhase == 1)
            {
                // Go to phase 2
                //bossPhase = 
            }
        }

        #endregion

        public void UpdateAttack(GameTime gameTime)
        {
            // Here, update the various forms of attack Hamilton uses.  Shoot a beam at a random tower and BOOM it!
        }
    }
}
