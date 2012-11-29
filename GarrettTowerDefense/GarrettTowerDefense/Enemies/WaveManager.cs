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

        public static List<CarnageModeBuff> CarnageBuffs;

        public bool WavePaused = false;
        public static bool carnageMode = false;

        public static List<QueuedSpawn> QueuedSpawns { get; set; }

        Random rand;

        public WaveManager()
        {
            QueuedSpawns = new List<QueuedSpawn>();
            CarnageBuffs = new List<CarnageModeBuff>();
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

            if (QueuedSpawns.Count > 0)
            {
                foreach (QueuedSpawn qs in QueuedSpawns)
                {
                    SpawnAt(qs.enemy, qs.spawnPoint);
                }

                QueuedSpawns.Clear();
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
                        //WavePaused = true;
                        //EnemiesLeft = 0;
                        //newEnemy = new AlexanderHamilton();
                        //break;
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

            // If in carnage mode...
            if (carnageMode && WaveNumber < 24)
            {
                //Set the number of buffs for this wave...
                int numberOfBuffs = ((WaveNumber - 12) / 4) + 1;
                GetNextCarnageBuffs(numberOfBuffs);
                EnemiesLeft = 10 + ((WaveNumber - 12) * 2);
            }

            if (WaveNumber == 24)
            {
                AlexanderHamilton.ShowMessage("Worthless underlings.  It seems I will have to dispose of you myself!");
                AudioManager.PlaySong(6);
                AudioManager.SetVolume(.5f);
            }
        }

        public void ReenableSpawn()
        {
            Console.WriteLine("Reenable spawning");
            WavePaused = false;
            Console.WriteLine("WavePaused : " + WavePaused.ToString());
            SpawnEnemy();
        }

        //Spawns a specific enemy at a point (used for bosses)
        public static void SpawnAt(Enemy e, Point spawnAt)
        {
            Vector2 spawnVector = new Vector2(spawnAt.X * TileEngine.TileWidth, spawnAt.Y * TileEngine.TileHeight);
            e.Initialize(spawnVector);
        }

        // This class handles queued orders to spawn enemies so as not to disrupt the enumerations while waiting for the next cycle (used to allow boss spawning)
        public class QueuedSpawn
        {
            public Enemy enemy;
            public Point spawnPoint;

            public QueuedSpawn(Enemy e, Point p)
            {
                enemy = e;
                spawnPoint = p;
            }
        }

        public static void BeginCarnageMode()
        {
            Console.WriteLine("------------------");
            Console.WriteLine("CARNAGE MODE ACTIVATED");
            Console.WriteLine("------------------");
            carnageMode = true;
            GetNextCarnageBuffs();
        }

        public static void GetNextCarnageBuffs(int targetBuffs = 1)
        {
            Random rand = new Random();
            CarnageBuffs.Clear();

            int newBuff = 0;
            // For each buff to add...
            while(CarnageBuffs.Count < targetBuffs)
            {
                // Roll a new buff to add to the wave
                newBuff = rand.Next(0, Enum.GetValues(typeof(CarnageModeBuff)).Length);

                // Check if the buff already exists...
                if (CarnageBuffs.Contains((CarnageModeBuff)newBuff))
                {
                    continue;
                }
                else
                {
                    // Add the buff to the list of buffs for the wave
                    CarnageBuffs.Add((CarnageModeBuff)newBuff);
                }
            }

            Console.WriteLine("------------------");
            Console.WriteLine("THIS WAVE'S CARNAGE:");
            Console.WriteLine("------------------");
            foreach (CarnageModeBuff cmb in CarnageBuffs)
            {
                Console.WriteLine(cmb.ToString());
            }
            Console.WriteLine("------------------");
        }
            
    }
}

