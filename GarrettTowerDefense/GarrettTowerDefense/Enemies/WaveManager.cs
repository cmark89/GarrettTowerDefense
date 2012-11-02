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

        Random rand;

        public WaveManager()
        {
            rand = new Random();

            WaveNumber = 0;
            EnemiesLeft = 0;
            WaveTime = 0f;
            SpawnRate = 1.4f;
            NextSpawn = 0;
        }

        public void Update(GameTime gameTime)
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
                EnemiesLeft--;
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
                    default:
                        newEnemy = null;
                        break;
                }

                if (newEnemy == null)
                    return;

                Point spawnAt = (GameScene.CurrentMap.SpawnPoints[rand.Next(0, GameScene.CurrentMap.SpawnPoints.Count)]);
                Vector2 spawnVector = new Vector2(spawnAt.X * TileEngine.TileWidth, spawnAt.Y * TileEngine.TileHeight);

                newEnemy.Initialize();
                newEnemy.SetPosition(spawnVector);
            }
        }

        public void NextWave()
        {
            Console.WriteLine("Advance to next wave!");
            WaveNumber++;
            WaveTime = -10f;
            EnemiesLeft = 7 + WaveNumber * 2;

            NextSpawn = 0 + SpawnRate;
        }
            
    }
}

