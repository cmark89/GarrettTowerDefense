using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    public class WaveManager
    {
        public int WaveNumber { get; private set; }
        public float WaveTime { get; private set; }
        public float SpawnRate { get; private set; }
        public float NextSpawn { get; private set; }
        public int EnemiesLeft { get; private set; }

        public bool WavePaused = false;

        Random rand;

        public WaveManager()
        {
            rand = new Random();

            WaveNumber = 0;
            EnemiesLeft = 0;
            WaveTime = 0f;
            SpawnRate = 1f;
            NextSpawn = 0;
        }

        public void Update(GameTime gameTime)
        {
            if (!WavePaused)
            {
                WaveTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (WaveTime >= NextSpawn)
                {
                    if (WaveNumber == 0)
                    {
                        Console.WriteLine("Begin the game at wave 1!");
                        NextWave();
                        return;
                    }

                    NextSpawn = WaveTime + SpawnRate;
                    SpawnEnemy();
                }
            }
        }

        public void SpawnEnemy()
        {
            if (EnemiesLeft <= 0)
            {
                NextWave();
                return;
            }
            else
            {
                //Minus one enemy left.
                EnemiesLeft--;

                Console.WriteLine("Spawn an enemy!");
                Enemy newEnemy;
                switch (WaveNumber)
                {
                    case (1):
                        newEnemy = new CruelCarrot();
                        break;
                    case (2):
                        newEnemy = new GrumpyGrape();
                        break;
                    case (3):
                        newEnemy = new EvilOrange();
                        break;
                    case (4):
                        newEnemy = new BadBanana();
                        break;
                    case (5):
                        newEnemy = new MoroseMushroom();
                        break;
                    case (6):
                        newEnemy = new PoopyPotato();
                        break;
                    case (7):
                        newEnemy = new AngryApple();
                        break;
                    case (8):
                        newEnemy = new GhostFruit();
                        break;
                    case (9):
                        newEnemy = new CarnivorousCorn();
                        break;
                    case (10):
                        newEnemy = new SmellyLettuce();
                        break;
                    case (11):
                        newEnemy = new LimeyLemon();
                        break;
                    case (12):
                        WavePaused = true;
                        EnemiesLeft = 0;
                        newEnemy = new WizardPear();
                        break;
                    case (13):
                        newEnemy = new CruelCarrot(true);
                        break;
                    case (14):
                        newEnemy = new GrumpyGrape(true);
                        break;
                    case (15):
                        newEnemy = new EvilOrange(true);
                        break;
                    case (16):
                        newEnemy = new BadBanana(true);
                        break;
                    case (17):
                        newEnemy = new MoroseMushroom(true);
                        break;
                    case (18):
                        newEnemy = new PoopyPotato(true);
                        break;
                    case (19):
                        newEnemy = new AngryApple(true);
                        break;
                    case (20):
                        newEnemy = new GhostFruit(true);
                        break;
                    case (21):
                        newEnemy = new CarnivorousCorn(true);
                        break;
                    case (22):
                        newEnemy = new SmellyLettuce(true);
                        break;
                    case (23):
                        newEnemy = new LimeyLemon(true);
                        break;
                    case (24):
                        WavePaused = true;
                        EnemiesLeft = 0;
                        newEnemy = new AlexanderHamilton();
                        break;
                    default:
                        newEnemy = null;
                        break;
                }

                if (newEnemy == null)
                    return;

                int spawnPointIndex = rand.Next(0, GameScene.CurrentMap.SpawnPoints.Count);
                Point spawnAt = (GameScene.CurrentMap.SpawnPoints[spawnPointIndex]);

                Console.WriteLine(GameScene.CurrentMap.SpawnPoints.Count + " spawn points found.  Spawn at index " + spawnPointIndex);
                Console.WriteLine("Spawn at point: " + spawnAt);

                Vector2 spawnVector = new Vector2(spawnAt.X * TileEngine.TileWidth, spawnAt.Y * TileEngine.TileHeight);

                newEnemy.Initialize(spawnVector);
            }
        }

        public void NextWave()
        {
            Console.WriteLine("Advance to next wave!");
            if (WavePaused)
                WavePaused = false;
            WaveNumber++;
            WaveTime = -10f;
            EnemiesLeft = 7 + WaveNumber * 2;

            NextSpawn = 0 + SpawnRate;
        }

        public void ReenableSpawn()
        {
            Console.WriteLine("Reenable spawning");
            WavePaused = false;
            Console.WriteLine("WavePaused : " + WavePaused.ToString());
            SpawnEnemy();
        }

        //Spawns a specific enemy at a point (used for bosses)
        public void SpawnAt(Enemy e, Point spawnAt)
        {
            Vector2 spawnVector = new Vector2(spawnAt.X * TileEngine.TileWidth, spawnAt.Y * TileEngine.TileHeight);

            e.Initialize(spawnVector);
        }
            
    }
}

