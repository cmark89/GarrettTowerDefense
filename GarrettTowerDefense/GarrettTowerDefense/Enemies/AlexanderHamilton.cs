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
        public Color shieldColor;
        public float nextShieldTime;
        public float shieldChangeCooldown = 10f;

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
        private float orbCooldown = 11f;
        private Tower orbTarget;
        private float orbRange = 350f;
        private Vector2 orbPosition;
        private float nextOrbTime;
        private float orbSpeed = 130f;

        private float beamCooldown = 1.2f;
        private float nextBeamTime;

        private bool stunned = false;
        private float unstunTime;

        private Animation destroyTowersAnimation;
        private List<Tower> builtTowers;
        private bool destroyingTowers = false;
        private const float towerDestroyInterval = .25f;
        private int destroyTowerDistance = 0;
        private float nextTowerDestroyTime;
        private float destructionWaveWidth;
        private float destructionWaveHeight;
        private Color destructionWaveColor;

        private bool eventActive = false;



        public AlexanderHamilton()
        {
            Name = "Alexander Hamilton";
            TextureID = 17;

            //BaseHealth = 200;
            BaseHealth = 35000;
            //Calculate true health based on the wave number
            Health = BaseHealth;
            CurrentHealth = Health;

            Bounty = 150;

            Damage = 100;

            currentPhase = 1;
            bossPhase = BossPhase.Walk;

            BaseMovementSpeed = 25;
            MovementSpeed = BaseMovementSpeed;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { 1f, 1f, 1f, 1f, 1f };

            CarnageColor = Color.White;

            CurrentState = MonsterState.Normal;


            //TEST THIS SHIT!
            //In actuality, have this set the time to a couple seconds after spawning
            nextOrbTime = 2f;

            ShowMessage("Here I come, Garrett!");

            //base.Initialize();
        }

        #region Overrides
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Alive)
            {
                if (destroyTowersAnimation != null)
                {
                    // First, draw the explosion wave to make sure it happens below hamilton himself.  
                    int startX = (int)(CenterPosition.X - (destructionWaveWidth / 2f));
                    int startY = (int)(CenterPosition.Y - (destructionWaveHeight / 2f));


                    spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle(startX, startY, (int)destructionWaveWidth, (int)destructionWaveHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(destroyTowersAnimation.CurrentFrameIndex), destructionWaveColor);
                }
                

                Color drawColor = Color.White;

                if (stunned)
                    drawColor = new Color(.5f, .7f, .5f, .5f);

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

                if (shieldActive)
                {
                    spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle((int)Position.X, (int)Position.Y, TileEngine.TileWidth, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(45), shieldColor);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Cache the game time
            currentGameTime = gameTime;

            if (eventActive)
            {
                return;
            }

            if (stunned)
            {
                if (currentGameTime.TotalGameTime.TotalSeconds >= unstunTime)
                {
                    stunned = false;

                    Weaknesses = new float[] { 1f, 1f, 1f, 1f, 1f };
                }
            }

            //Console.WriteLine(CurrentHealth);

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
                        UpdateAttack(gameTime);
                        break;
                    case BossPhase.AttackWalk:
                        UpdateAttack(gameTime);
                        UpdateMovement(gameTime);
                        break;
                    default:
                        break;
                }

                if (destroyingTowers)
                {
                    if (builtTowers.Count > 0)
                    {
                        UpdateTowerDestruction(gameTime);
                    }                    
                }


                //If the enemy is in  the castle tile, destroy it and damage the castle.
                if (CenterPosition == TileEngine.MapPointToVector(GameScene.CurrentMap.CastleTile) + new Vector2(TileEngine.TileWidth / 2, TileEngine.TileHeight / 2))
                {
                    Console.WriteLine("Enemy hit the castle!");
                    Alive = false;
                    GameScene.DamageCastle(1000);
                }
            }
        }

        // Checks the boss's health to see if it needs to enter the next phase
        public void CheckHealth()
        {
            if (healthPercentage < .75f && currentPhase == 1)
            {
                Console.WriteLine("-------------");
                Console.WriteLine("PHASE 2 BEGIN");
                Console.WriteLine("-------------");
                // Go to phase 2
                currentPhase = 2;

                TeleportToSpawn();
                GetPath(GameScene.CurrentMap.CastleTile);
                shieldActive = true;
                bossPhase = BossPhase.ShieldWalk;

                isFrozen = false;
                isBurning = false;
                isPoisoned = false;

                SetShieldImmunity();
                ShowMessage("Hide behind your walls all you please.  We'll see what happens when your weapons are rendered useless!");
            }

            if (healthPercentage < .5f && currentPhase == 2)
            {
                Console.WriteLine("-------------");
                Console.WriteLine("PHASE 3 BEGIN");
                Console.WriteLine("-------------");
                currentPhase = 3;

                shieldActive = false;

                isFrozen = false;
                isBurning = false;
                isPoisoned = false;

                TeleportToSpawn();
                GetPath(GameScene.CurrentMap.CastleTile);

                bossPhase = BossPhase.AttackWalk;
                nextOrbTime = (float)currentGameTime.TotalGameTime.TotalSeconds + 4f;
                ShowMessage("I shall raze your stronghold to the ground!  Behold the ruins of your ambition!");
            }

            if (healthPercentage < .3f && currentPhase == 3)
            {
                Console.WriteLine("-------------");
                Console.WriteLine("PHASE 4 BEGIN");
                Console.WriteLine("-------------");
                currentPhase = 4;

                TeleportToSpawn();
                GetPath(GameScene.CurrentMap.CastleTile);

                isFrozen = false;
                isBurning = false;
                isPoisoned = false;

                Visible = true;

                bossPhase = BossPhase.Walk;

                // Fade out the music, instead of just stopping it
                AudioManager.StopMusic();

                ShowFinalPhaseMessage1();
            }
        }


        public override void CheckForDeath()
        {
            if (CurrentHealth <= 1)
            {
                GarrettTowerDefense.ChangeScene(new VictoryScene());
            }
        }


        public override void UpdateMovement(GameTime gameTime)
        {
            if (Waypoints.Count == 0 || stunned)
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
        #endregion

        public void UpdateAttack(GameTime gameTime)
        {
            // Here, update the various forms of attack Hamilton uses.  Shoot a beam at a random tower and BOOM it!
            if (bossPhase == BossPhase.AttackWalk && beamAnimation == null && nextBeamTime <= gameTime.TotalGameTime.TotalSeconds)
            {
                // Fire a beam
                FireBeam();
            }

            if (shieldActive)
            {
                if (gameTime.TotalGameTime.TotalSeconds > nextShieldTime)
                {
                    SetShieldImmunity();
                    nextShieldTime = (float)gameTime.TotalGameTime.TotalSeconds + shieldChangeCooldown;
                }
            }


            if (bossPhase == BossPhase.AttackWalk && orbAnimation == null && nextOrbTime <= gameTime.TotalGameTime.TotalSeconds)
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
        
        
        public void FireBeam()
        {
            AudioManager.PlaySoundEffect(13, .5f);
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
            AudioManager.PlaySoundEffect(15, .5f);
            List<Tower> viableTargets = GameScene.Towers.FindAll(x => Vector2.Distance(x.Position, Position) <= orbRange);
            if (viableTargets.Count <= 0)
            {

                nextOrbTime = (float)currentGameTime.TotalGameTime.TotalSeconds + 4f;
                return;
            }
            
            Random rand = new Random();

            // Create the orb
            orbAnimation = new Animation(new int[] { 45 }, 0);
            orbPosition = Position;
            orbTarget = viableTargets[rand.Next(0, viableTargets.Count)];
        }


        public void OrbHit(Tower target)
        {
            AudioManager.PlaySoundEffect(16, .5f);

            orbAnimation = null;

            List<Tower> affectedTowers = GameScene.Towers.FindAll(x => x.Name == target.Name);
            foreach (Tower t in affectedTowers)
            {
                t.Stun(stunDuration);
            }

            nextOrbTime = (float)currentGameTime.TotalGameTime.TotalSeconds + orbCooldown;
        }


        public void SetShieldImmunity()
        {
            AudioManager.PlaySoundEffect(14, .5f);

            if (!shieldActive)
            {
                shieldActive = true;
            }

            Random rand = new Random();
            int immunity = rand.Next(0, Enum.GetValues(typeof(DamageType)).Length);
            shieldType = (DamageType)immunity;
            Weaknesses = new float[] { 1f, 1f, 1f, 1f, 1f };
            Weaknesses[immunity] = 0f;

            switch (shieldType) 
            {
                case (DamageType.Physical):
                    shieldColor = new Color(.5f, .5f, .5f, .3f);
                    break;
                case (DamageType.Fire):
                    shieldColor = new Color(.8f, 0f, 0f, .3f);
                    break;
                case (DamageType.Ice):
                    shieldColor = new Color(0f, .65f, .65f, .3f);
                    break;
                case (DamageType.Electrical):
                    shieldColor = new Color(0f, .3f, .65f, .3f);
                    break;
                case (DamageType.Poison):
                    shieldColor = new Color(0f, .65f, .15f, .3f);
                    break;
                default:
                    break;
            }
        }


        public void TeleportToSpawn()
        {
            Random rand = new Random();

            Position = TileEngine.MapPointToVector(GameScene.CurrentMap.SpawnPoints[rand.Next(0, GameScene.CurrentMap.SpawnPoints.Count)]);
            GetPath(GameScene.CurrentMap.CastleTile);
            MapPosition = TileEngine.ScreenSpaceToMapSpace(Position);
        }


        public void DestroyTowers(Tower[] towers)
        {
            foreach (Tower t in towers)
            {
                Console.WriteLine("Destroying a specific tower: " + t.Name);
                t.Sell(1f);
                builtTowers.Remove(t);
            }
        }

        public void UpdateTowerDestruction(GameTime gameTime)
        {
            // Set the animation properties here.
            destructionWaveWidth += ((TileEngine.TileWidth * 2) / towerDestroyInterval) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            destructionWaveHeight += ((TileEngine.TileHeight * 2) / towerDestroyInterval) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Change the color
            destructionWaveColor = new Color(.5f, .5f, .5f, .2f);
            

            if (currentGameTime.TotalGameTime.TotalSeconds >= nextTowerDestroyTime)
            {
                Console.WriteLine("Towers remaining: " + builtTowers.Count); 
                Console.WriteLine("Destroy all towers at " + destroyTowerDistance);
                DestroyTowers(GetTowersAtDistance(destroyTowerDistance));
                destroyTowerDistance++;

                nextTowerDestroyTime = (float)currentGameTime.TotalGameTime.TotalSeconds + towerDestroyInterval;

                if (builtTowers.Count == 0)
                {
                    destroyTowersAnimation = null;
                    Console.WriteLine("Towers destroyed!");
                    destroyingTowers = false;
                    Weaknesses = new float[] { 0f, 0f, 0f, 0f, 0f };
                }
            }
        }


        public void StartFinalPhase()
        {
            AudioManager.PlaySong(5);
            Console.WriteLine("Final phase begin!");
            Weaknesses = new float[] { 0f, 0f, 0f, 0f, 0f };
            isBurning = false;
            isPoisoned = false;
            isFrozen = false; 

            beamAnimation = null;
            orbAnimation = null;

            // Here Hamilton will mock the player.

            // He then destroys towers in a ring outward from wherever he respawns.

            Console.WriteLine("Begin destroying all towers!");
            builtTowers = new List<Tower>(GameScene.Towers.Where(x => x.Constructed));
            destroyTowersAnimation = new Animation(new int[] { 45 }, 0);

            destroyingTowers = true;
            
            stunned = true;
            unstunTime = (float)currentGameTime.TotalGameTime.TotalSeconds + 30f;
            GameScene.GUI.countdownActive = true;
            GameScene.GUI.countdownTime = 30f;
        }
        

        public Tower[] GetTowersAtDistance(int range)
        {
            List<Tower> towersFound = new List<Tower>();

            int xDist;
            int yDist;
            int totalDist;

            foreach (Tower t in builtTowers)
            {
                if (!t.Constructed)
                    continue;

                xDist = Math.Abs(t.MapPosition.X - MapPosition.X);
                yDist = Math.Abs(t.MapPosition.Y - MapPosition.Y);

                totalDist = xDist + yDist;

                Console.WriteLine("Tower is at " + totalDist);

                //if (totalDist <= range || xDist)
                if(xDist <= range && yDist <= range)
                {
                    towersFound.Add(t);
                }
            }

            Console.WriteLine("Towers to destroy: " + towersFound.Count);
            return towersFound.ToArray();
        }


        public static void ShowMessage(string message)
        {
            new MessageWindow("hamiltonMessage", text: message, isVisible: false);
            GameScene.messageWindow.ShowMessageWindow += delegate
            {
                GameScene.Paused = true;
            };
            GameScene.messageWindow.HideMessageWindow += delegate
            {
                GameScene.Paused = false;
            };

            GameScene.messageWindow.ShowWindow();
        }

        public void ShowFinalPhaseMessage1()
        {
            new MessageWindow("hamiltonMessage", "You are stronger than I gave you credit for...but you are a mote of dust before the cosmic powers I command.", isVisible: false);
            GameScene.messageWindow.ShowMessageWindow += delegate
            {
                GameScene.Paused = true;
            };
            GameScene.messageWindow.HideMessageWindow += delegate
            {
                ShowFinalPhaseMessage2();
            };

            GameScene.messageWindow.ShowWindow();
        }

        public void ShowFinalPhaseMessage2()
        {
            new MessageWindow("hamiltonMessage", "Because you have fought so hard, I shall tell you a secret...", isVisible: false);
            GameScene.messageWindow.ShowMessageWindow += delegate
            {
                return;
            };
            GameScene.messageWindow.HideMessageWindow += delegate
            {
                ShowFinalPhaseMessage3();
            };

            GameScene.messageWindow.ShowWindow();
        }

        public void ShowFinalPhaseMessage3()
        {
            new MessageWindow("hamiltonMessage", "I have been using a mere one percent of my nigh unlimited power!", isVisible: false);
            GameScene.messageWindow.ShowMessageWindow += delegate
            {
                return;
            };
            GameScene.messageWindow.HideMessageWindow += delegate
            {
                ShowFinalPhaseMessage4();
            };

            GameScene.messageWindow.ShowWindow();
        }

        public void ShowFinalPhaseMessage4()
        {
            new MessageWindow("hamiltonMessage", "That's right, Garrett, hear the truth and despair!  You have only come so far because I have allowed you to do so.", isVisible: false);
            GameScene.messageWindow.ShowMessageWindow += delegate
            {
                return;
            };
            GameScene.messageWindow.HideMessageWindow += delegate
            {
                ShowFinalPhaseMessage5();
            };

            GameScene.messageWindow.ShowWindow();
        }

        public void ShowFinalPhaseMessage5()
        {
            new MessageWindow("hamiltonMessage", "Only by clinging desperately to an illusion of hope could your defeat be absolute.  Only then could it be perfect.", isVisible: false);
            GameScene.messageWindow.ShowMessageWindow += delegate
            {
                return;
            };
            GameScene.messageWindow.HideMessageWindow += delegate
            {
                ShowFinalPhaseMessage6();
            };

            GameScene.messageWindow.ShowWindow();
        }

        public void ShowFinalPhaseMessage6()
        {
            new MessageWindow("hamiltonMessage", "That is ABSOLUTE DESPAIR!", isVisible: false);
            GameScene.messageWindow.ShowMessageWindow += delegate
            {
                return;
            };
            GameScene.messageWindow.HideMessageWindow += delegate
            {
                ShowFinalPhaseMessage7();
            };

            GameScene.messageWindow.ShowWindow();
        }

        public void ShowFinalPhaseMessage7()
        {
            new MessageWindow("hamiltonMessage", "Now, Garrett, open your eyes and drink in the full extent of your helplessness!  Know that I am Hamilton, the ruler of all the cosmos!", isVisible: false);
            GameScene.messageWindow.ShowMessageWindow += delegate
            {
                return;
            };
            GameScene.messageWindow.HideMessageWindow += delegate
            {
                ShowFinalPhaseMessage8();
            };

            GameScene.messageWindow.ShowWindow();
        }

        public void ShowFinalPhaseMessage8()
        {
            new MessageWindow("hamiltonMessage", "Bask in the full extent of my power!  Bask in your undoing!  Bask in ABSOLUTE DESPAIR!!", isVisible: false);
            GameScene.messageWindow.ShowMessageWindow += delegate
            {
                return;
            };
            GameScene.messageWindow.HideMessageWindow += delegate
            {
                ShowFinalPhaseMessage9();
            };

            GameScene.messageWindow.ShowWindow();
        }

        public void ShowFinalPhaseMessage9()
        {
            new MessageWindow("hamiltonMessage", "Behold...the end of everything!!!", isVisible: false);
            GameScene.messageWindow.ShowMessageWindow += delegate
            {
                return;
            };
            GameScene.messageWindow.HideMessageWindow += delegate
            {
                StartFinalPhase();
                GameScene.Paused = false;
            };

            GameScene.messageWindow.ShowWindow();
        }

        public override void BeginBurn(float percent, float duration)
        {
            if (percent > .005f)
                percent = .005f;

            if (Weaknesses[(int)DamageType.Fire] <= .3f)
                return;

            float damage = (percent * Health) * Weaknesses[(int)DamageType.Fire];
            Console.WriteLine(Name + " will burn for " + damage + " damage (" + damage / duration + " damage per second) over " + duration + " seconds.");
            isBurning = true;
            burnDPS = (Health * percent) / duration;
            burnDuration = duration;
        }
    }
}