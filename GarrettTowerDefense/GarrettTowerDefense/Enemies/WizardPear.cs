using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GarrettTowerDefense
{
    class WizardPear: Enemy
    {
        public int lives = 5;

        public BossPhase bossPhase;
        public float summonDPS = 25;    // Damage taken every second while summoning.
        
        public float summonInterval;
        public float nextSummonTime;
        public GameTime currentGameTime;

        public WizardPear()
        {
            Name = "Wizard Pear";
            TextureID = 29;

            bossPhase = BossPhase.Walk;

            //BaseHealth = 100;
            BaseHealth = 2000;
            
            //Calculate true health based on the wave number
            Health = BaseHealth;
            CurrentHealth = Health;

            Bounty = 150;

            Damage = 20;

            BaseMovementSpeed = 30;
            MovementSpeed = BaseMovementSpeed;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { 1f, 1f, 1f, 1f, 1f };

            CurrentState = MonsterState.Normal;

            //base.Initialize();
        }

        #region Enemy overrides
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Alive)
            {
                Color drawColor = Color.White;
                if (bossPhase == BossPhase.Summon)
                    drawColor = new Color(.3f, .8f, .2f, .5f);

                spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle((int)Position.X, (int)Position.Y, TileEngine.TileWidth, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(TextureID), drawColor);
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Cache the game time
            currentGameTime = gameTime;

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
                        UpdateSummoning(gameTime);
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

        public override void CheckForDeath()
        {
            if (CurrentHealth <= 0)
            {
                if (lives == 1)
                {
                    GameScene.GainGold(Bounty);
                    Console.WriteLine("Wizard Pear dies!");
                    GameScene.waveManager.WavePaused = false;
                    GameScene.waveManager.ReenableSpawn();
                    WaveManager.BeginCarnageMode();
                    Alive = false;
                    return;
                }
                else
                {
                    CurrentHealth = Health;
                    Console.WriteLine("Wizard Pear is acting strangely!");
                    lives--;
                    NextPhase(currentGameTime);
                }
            }
        }
        #endregion

        public void UpdateSummoning(GameTime gameTime)
        {
            if(gameTime.TotalGameTime.TotalSeconds >= nextSummonTime)
            {
                SummonEnemy();
                nextSummonTime = (float)gameTime.TotalGameTime.TotalSeconds + summonInterval;
            }

            CurrentHealth -= summonDPS * (float)gameTime.ElapsedGameTime.TotalSeconds;
            CheckForDeath();
        }

        public void SummonEnemy()
        {
            Random rand = new Random();
            Enemy enemy = null;
            Point point;

            // Get the number of legitimate spawn points on the map and store it
            int normalSpawns = GameScene.CurrentMap.SpawnPoints.Count;
            
            // Roll between 0 and 4 to determine the enemy spawned
            switch (rand.Next(0,5))
            {
                case 0:
                    enemy = new LimeyLemon();
                    break;
                case 1:
                    enemy = new MoroseMushroom();
                    break;
                case 2:
                    enemy = new BadBanana();
                    break;
                case 3:
                    enemy = new SmellyLettuce();
                    break;
                case 4:
                    enemy = new AngryApple();
                    break;
                default:
                    break;
            }

            int rolledPoint = rand.Next(0, normalSpawns + 1);
            if (rolledPoint < normalSpawns)
            {
                point = GameScene.CurrentMap.SpawnPoints[rolledPoint];
            }
            else
            {
                point = MapPosition;
            }
            WaveManager.QueuedSpawns.Add(new WaveManager.QueuedSpawn(enemy, point));
            //WaveManager.SpawnAt(enemy, point);
            //Play a cool graphic here
        }

        

        public void NextPhase(GameTime gameTime)
        {
            switch (lives)
            {
                case 4:
                    bossPhase = BossPhase.Summon;
                    summonInterval = 2f;
                    nextSummonTime = (float)gameTime.TotalGameTime.TotalSeconds + summonInterval;
                    break;
                case 3:
                    bossPhase = BossPhase.Walk;
                    BaseMovementSpeed += 10;
                    MovementSpeed = BaseMovementSpeed;
                    TeleportToSpawn();
                    break;
                case 2:
                    bossPhase = BossPhase.Summon;
                    summonInterval = 1f;
                    nextSummonTime = (float)gameTime.TotalGameTime.TotalSeconds + summonInterval;
                    break;
                case 1:
                    bossPhase = BossPhase.Walk;
                    BaseMovementSpeed += 10;
                    MovementSpeed = BaseMovementSpeed;
                    TeleportToSpawn();
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
    }
}
