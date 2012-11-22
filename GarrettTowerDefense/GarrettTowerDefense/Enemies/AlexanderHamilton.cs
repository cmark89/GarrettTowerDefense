using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GarrettTowerDefense
{
    class AlexanderHamilton : Enemy
    {
        public int currentPhase;
        public BossPhase bossPhase;

        // True when the damage shield activates
        public bool shieldActive = false;
        // This is the type of damage to ignore
        public DamageType shieldType;

        public GameTime currentGameTime;
        public float healthPercentage;

        // Special attack variables
        private Animation beamAnimation;
        private Animation orbAnimation;

        private Tower beamTarget;
        private float beamLength;
        private float beamElapsedTime;
        private float beamExtendTime = .3f;
        private float beamVisibleTime = .45f;
        private float beamRange = 140;

        private float stunDuration = 9f;
        private float orbCooldown = 14f;
        private Tower orbTarget;
        private float orbRange = 350f;
        private Vector2 orbPosition;
        private float nextOrbTime;
        private float orbSpeed = 130f;

        private float beamCooldown = 1.3f;
        private float nextBeamTime;



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
            bossPhase = BossPhase.AttackWalk;

            BaseMovementSpeed = 25;
            MovementSpeed = BaseMovementSpeed;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { 1f, 1f, 1f, 1f, 1f };

            CarnageColor = Color.White;

            CurrentState = MonsterState.Normal;


            //TEST THIS SHIT!
            //In actuality, have this set the time to a couple seconds after spawning
            nextOrbTime = 13f;

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

                    spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle((int)CenterPosition.X, (int)CenterPosition.Y, (int)beamLength, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(beamAnimation.CurrentFrameIndex), new Color(1f,1f,1f,.5f), Vector2Helper.FindAngle(beamTarget.CenterPosition, CenterPosition), new Vector2(0, TileEngine.TileHeight / 2), SpriteEffects.None, 0f);
                }

                if (orbAnimation != null)
                {
                    // Draw the orb in a very standard manner
                    spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle((int)orbPosition.X, (int)orbPosition.Y, TileEngine.TileWidth, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(orbAnimation.CurrentFrameIndex), new Color(1f,1f,1f,.5f));
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
        public void CheckHealth()
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
            if (beamAnimation == null && nextBeamTime <= gameTime.TotalGameTime.TotalSeconds)
            {
                // Fire a beam
                FireBeam();
            }


            if (orbAnimation == null && nextOrbTime <= gameTime.TotalGameTime.TotalSeconds)
            {
                // Shoot an orb
                FireOrb();
            }


            if (beamAnimation != null)
            {
                beamAnimation.Update(gameTime);

                beamElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                // This becomes 1 once the extend time has completed.
                beamLength = (float)Math.Min((beamElapsedTime / beamExtendTime), 1);
                beamLength *= Vector2.Distance(Position, beamTarget.Position);

                if (beamElapsedTime > beamVisibleTime)
                {
                    beamAnimation = null;
                    beamElapsedTime = 0f;
                    nextBeamTime = (float)gameTime.TotalGameTime.TotalSeconds + beamCooldown;
                    BeamHit(beamTarget);
                }
            }


            if (orbAnimation != null)
            {
                float distanceThisFrame = (orbSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);

                if (Vector2.Distance(orbTarget.Position, orbPosition) <= distanceThisFrame)
                {
                    OrbHit(orbTarget);
                }
                else
                {
                    Vector2 movement = orbTarget.Position - orbPosition;
                    movement.Normalize();
                    movement *= distanceThisFrame;
                    orbPosition += movement;
                }
            }
        }

        // <summary>
        // Launches a beam at a random tower.
        // </summary>
        public void FireBeam()
        {
            List<Tower> validTargets = GameScene.Towers.FindAll(x => Vector2.Distance(Position, x.Position) <= beamRange);
            if (validTargets.Count <= 0)
                return;

            Random rand = new Random();

            beamTarget = validTargets[rand.Next(0, validTargets.Count)];
            beamAnimation = new Animation(new int[] { 42, 43, 44 });
        }

        public void BeamHit(Tower target)
        {
            //KABLAMMO!

            target.LevelDown();
            if (target.Level <= 0)
            {
                target.Destroy();
            }
        }

        public void FireOrb()
        {
            List<Tower> viableTargets = GameScene.Towers.FindAll(x => Vector2.Distance(x.Position, Position) <= orbRange);
            if (viableTargets.Count <= 0)
                return;
            
            Random rand = new Random();

            // Create the orb
            orbAnimation = new Animation(new int[] { 45 }, 0);
            orbPosition = Position;
            orbTarget = viableTargets[rand.Next(0, viableTargets.Count)];
        }

        public void OrbHit(Tower target)
        {
            orbAnimation = null;

            List<Tower> affectedTowers = GameScene.Towers.FindAll(x => x.Name == target.Name);
            foreach (Tower t in affectedTowers)
            {
                t.Stun(stunDuration);
            }

            nextOrbTime = (float)currentGameTime.TotalGameTime.TotalSeconds + orbCooldown;
        }
    }
}
