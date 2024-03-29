﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pathfinding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;


namespace GarrettTowerDefense
{
    public class GameScene : Scene
    {
        //Stores the current map
        public static Pathfinder pathfinder;

        //Stores a list of all towers currently in the game
        public static List<Tower> Towers { get; private set; }

        //Stores a list of all enemies currently in the game
        public static List<Enemy> Enemies { get; set; }

        public static MouseAction CurrentMouseAction { get; set; }

        public static Texture2D GUITexture { get; private set; }
        public static Rectangle GUIArea { get; private set; }

        public static Texture2D EnemyHealthbar { get; private set; }

        public static GUI GUI { get; private set; }

        public static MessageWindow messageWindow;
        public static Tooltip tooltip;

        //Used for storing the price of towers efficiently
        public static int LoadedPrice {get; set;}
        public static WaveManager waveManager { get; set; }

        // Used for pausing the game.
        public static bool Paused = false;
        public static bool PlayerPaused = false;


        //Stores player related variables
        public static int MaxHealth;
        public static int CurHealth;
        public static int Gold;

        public static bool showGrid = false;

        public static Tower selectedTower = null;


        public GameScene()
        {
            Initialize();
        }


        public override void Initialize()
        {
            Towers = new List<Tower>();
            Enemies = new List<Enemy>();

            waveManager = new WaveManager();

            MaxHealth = 100;
            CurHealth = MaxHealth;
            Gold = 150;
            
            CurrentMouseAction = MouseAction.None;

            GUI = new GUI(GarrettTowerDefense.StaticContent);
        }

        public override void LoadContent(ContentManager Content)
        {
            GUI = new GUI(Content);
            
            EnemyHealthbar = Content.Load<Texture2D>("GUI/EnemyHealthbar");
        }

        
        public override void Update(GameTime gameTime)
        {
            if (GarrettTowerDefense.loadingScene)
                return;

            #region GUI and Input
            // Check if the game has to pause
            if(KeyboardHandler.KeyPress(Keys.Space))
            {
                PlayerPaused = !PlayerPaused;

                if (PlayerPaused)
                {
                    ClearMouseAction();
                    AudioManager.SetVolume(.25f);
                }
                else
                {
                    AudioManager.SetVolume(1f);
                }
            }

            // Check for mouse input and handle it accordingly
            HandleMouseInput(gameTime);

            if (MouseHandler.MouseOverMap() && CurrentMap[MouseHandler.MapY, MouseHandler.MapX].ContainsTower)
            {
                selectedTower = Towers.Find(x => x.MapPosition.X == MouseHandler.MapX && x.MapPosition.Y == MouseHandler.MapY);
            }
            else if(selectedTower != null)
            {
                selectedTower = null;
            }

            GUI.Update(gameTime);

            //Check for GUI input
            if (!PlayerPaused)
            {
                switch (GUI.currentInput)
                {
                    case GUIInput.BuildBarricade:
                        CurrentMouseAction = MouseAction.BuildBarricade;
                        LoadedPrice = Barricade.Cost;
                        break;
                    case GUIInput.BuildArrowTower:
                        CurrentMouseAction = MouseAction.BuildArrowTower;
                        LoadedPrice = ArrowTower.Cost;
                        break;
                    case GUIInput.BuildToxicTower:
                        CurrentMouseAction = MouseAction.BuildToxicTower;
                        LoadedPrice = ToxicTower.Cost;
                        break;
                    case GUIInput.BuildFlameTower:
                        CurrentMouseAction = MouseAction.BuildFlameTower;
                        LoadedPrice = FlameTower.Cost;
                        break;
                    case GUIInput.BuildTeslaTower:
                        CurrentMouseAction = MouseAction.BuildTeslaTower;
                        LoadedPrice = TeslaTower.Cost;
                        break;
                    case GUIInput.BuildIceTower:
                        CurrentMouseAction = MouseAction.BuildIceTower;
                        LoadedPrice = IceTower.Cost;
                        break;
                    case GUIInput.BuildGlaiveTower:
                        CurrentMouseAction = MouseAction.BuildGlaiveTower;
                        LoadedPrice = GlaiveTower.Cost;
                        break;
                    case GUIInput.BuildObservatory:
                        CurrentMouseAction = MouseAction.BuildObservatory;
                        LoadedPrice = Observatory.Cost;
                        break;
                    case GUIInput.BuildGoldMine:
                        CurrentMouseAction = MouseAction.BuildGoldMine;
                        LoadedPrice = GoldMine.Cost;
                        break;
                    default:
                        break;
                }
            }
            
            #endregion

            //Update each tower in turn
            if (!Paused && !PlayerPaused)
            {
                foreach (Tower t in Towers)
                {
                    t.Update(gameTime);
                }

                foreach (Enemy e in Enemies)
                {
                    if (e.Alive)
                        e.Update(gameTime);
                }

                for (int i = 0; i < Towers.Count; i++)
                {
                    if (Towers[i].Destroyed)
                    {
                        Towers.RemoveAt(i);
                    }
                }

                for (int i = 0; i < Enemies.Count; i++)
                {
                    if (!Enemies[i].Alive)
                        Enemies.RemoveAt(i);
                }
                //Should add an upgrade button and make this require MouseAction.Updrade instead of MouseAction.None.  Otherwise it's dumb.
                if (CurrentMouseAction == MouseAction.None && MouseHandler.MouseOverMap() && MouseHandler.RightClick())
                {
                    Vector2 mousepos = new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y);
                    Point clickedtile = TileEngine.ScreenSpaceToMapSpace(mousepos);

                    Tower t = GameScene.Towers.Find(x => x.MapPosition == clickedtile);
                    if (t != null)
                    {
                        t.Upgrade();
                    }
                }

                //Update the wave manager.
                waveManager.Update(gameTime);
            }

            if (tooltip != null)
                tooltip.Update(gameTime);
            

            if (messageWindow != null)
                messageWindow.Update(gameTime);            
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            if (GarrettTowerDefense.loadingScene)
                return;
            
            //Draw the map
            if (CurrentMap != null)
                CurrentMap.Draw(spriteBatch);

            //pathfinder.Draw(spriteBatch);
            if (Keyboard.GetState().IsKeyDown(Keys.LeftAlt))
            {
                showGrid = true;
            }
            else
            {
                showGrid = false;
            }

            //Draw something cool at the cursor if relevant
            if (CurrentMouseAction != MouseAction.None && MouseHandler.MouseOverMap())
            {
                Point curMouseTile = TileEngine.ScreenSpaceToMapSpace(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y));
                Color showColor = new Color(0f,0f,0f,.4f);
                if ((CurrentMap.mapCells[curMouseTile.Y, curMouseTile.X].IsBuildable && Gold >= LoadedPrice && CurrentMouseAction != MouseAction.BuildGoldMine) || (CurrentMouseAction == MouseAction.BuildGoldMine && CurrentMap.mapCells[curMouseTile.Y, curMouseTile.X].ContainsGold && Gold >= LoadedPrice))
                {
                    showColor = new Color(0f,.7f,0f,.4f);
                }
                else
                {
                    showColor = new Color(1f,0f,0f,.4f);
                }

                switch (CurrentMouseAction)
                {
                    case MouseAction.BuildBarricade:
                        spriteBatch.Draw(CurrentMap.Tileset.Texture, TileEngine.ScreenSpaceToMapVector(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y)), CurrentMap.Tileset.GetSourceRectangle(16), showColor);
                        break;
                    case MouseAction.BuildArrowTower:
                        spriteBatch.Draw(CurrentMap.Tileset.Texture, TileEngine.ScreenSpaceToMapVector(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y)), CurrentMap.Tileset.GetSourceRectangle(6), showColor);
                        break;
                    case MouseAction.BuildToxicTower:
                        spriteBatch.Draw(CurrentMap.Tileset.Texture, TileEngine.ScreenSpaceToMapVector(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y)), CurrentMap.Tileset.GetSourceRectangle(7), showColor);
                        break;
                    case MouseAction.BuildFlameTower:
                        spriteBatch.Draw(CurrentMap.Tileset.Texture, TileEngine.ScreenSpaceToMapVector(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y)), CurrentMap.Tileset.GetSourceRectangle(8), showColor);
                        break;
                    case MouseAction.BuildTeslaTower:
                        spriteBatch.Draw(CurrentMap.Tileset.Texture, TileEngine.ScreenSpaceToMapVector(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y)), CurrentMap.Tileset.GetSourceRectangle(9), showColor);
                        break;
                    case MouseAction.BuildIceTower:
                        spriteBatch.Draw(CurrentMap.Tileset.Texture, TileEngine.ScreenSpaceToMapVector(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y)), CurrentMap.Tileset.GetSourceRectangle(11), showColor);
                        break;
                    case MouseAction.BuildGlaiveTower:
                        spriteBatch.Draw(CurrentMap.Tileset.Texture, TileEngine.ScreenSpaceToMapVector(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y)), CurrentMap.Tileset.GetSourceRectangle(12), showColor);
                        break;
                    case MouseAction.BuildObservatory:
                        spriteBatch.Draw(CurrentMap.Tileset.Texture, TileEngine.ScreenSpaceToMapVector(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y)), CurrentMap.Tileset.GetSourceRectangle(13), showColor);
                        break;
                    case MouseAction.BuildGoldMine:
                        spriteBatch.Draw(CurrentMap.Tileset.Texture, TileEngine.ScreenSpaceToMapVector(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y)), CurrentMap.Tileset.GetSourceRectangle(15), showColor);
                        break;
                    default:
                        break;
                }
            }

            //Draw each tower
            foreach (Tower t in Towers)
            {
                t.Draw(spriteBatch);
            }

            //Draw enemies
            foreach (Enemy e in Enemies)
            {
                e.Draw(spriteBatch);
            }
            
            //Draw projectiles
            foreach (Enemy e in Enemies)
            {
                if (e.Stealthed && !e.Visible)
                    continue;

                float healthPercent = (e.CurrentHealth / e.Health);
                int width = (int)((TileEngine.TileWidth - 1) * healthPercent) + 1;
                Color color;
                if(healthPercent >= .5f)
                {
                    color = new Color((1 - healthPercent)/.5f,1f,0f,1f);
                }
                else
                {
                    color = new Color(1f,healthPercent/.5f,0f,1f);
                }
                
                spriteBatch.Draw(EnemyHealthbar, new Rectangle((int)e.Position.X, (int)e.Position.Y, width, 3), color);
            }

            if (messageWindow != null)
            {
                messageWindow.Draw(spriteBatch);
            }

            GUI.Draw(spriteBatch);
            if (tooltip != null)
            {
                tooltip.Draw(spriteBatch);
            }
        }


        public static void SetMap(Map map)
        {
            map.LoadFromSerialized();
            CurrentMap = map;
            CurrentMap.Initialize();
            pathfinder = new Pathfinder(map);
        }

        #region Mouse Input
        //Handle mouse input during the game.
        public static void HandleMouseInput(GameTime gameTime)
        {
            //Player clicks the mouse
            if (MouseHandler.Click())
            {
                //The mouse is over the map.
                
                if (MouseHandler.CurrentMouseState.X < CurrentMap.MapWidth * TileEngine.TileWidth)
                {
                    //Get the tile the mouse is over
                    Point curMouseTile = TileEngine.ScreenSpaceToMapSpace(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y));
                    
                    //Run a switch on the mouse state to determine what to do
                    switch (CurrentMouseAction)
                    {
                        case MouseAction.BuildBarricade:
                            new Barricade().Build(curMouseTile);
                            break;
                        case MouseAction.BuildArrowTower:
                            new ArrowTower().Build(curMouseTile);
                            break;
                        case MouseAction.BuildToxicTower:
                            new ToxicTower().Build(curMouseTile);
                            break;
                        case MouseAction.BuildFlameTower:
                            new FlameTower().Build(curMouseTile);
                            break;
                        case MouseAction.BuildTeslaTower:
                            new TeslaTower().Build(curMouseTile);
                            break;
                        case MouseAction.BuildIceTower:
                            new IceTower().Build(curMouseTile);
                            break;
                        case MouseAction.BuildGlaiveTower:
                            new GlaiveTower().Build(curMouseTile);
                            break;
                        case MouseAction.BuildObservatory:
                            new Observatory().Build(curMouseTile);
                            break;
                        case MouseAction.BuildGoldMine:
                            new GoldMine().Build(curMouseTile);
                            break;
                        default:
                            break;
                    }
                }
            }

            if (MouseHandler.RightClick())
            {
                //If right clicking, break the current operation.
                CurrentMouseAction = MouseAction.None;
            }
        }
        #endregion

        public static void ClearMouseAction()
        {
            CurrentMouseAction = MouseAction.None;
        }

        //Recalculate the path to the castle whenever a tower is constructed.
        public static void RecalculateEnemyPath()
        {
            foreach (Enemy e in Enemies)
            {
                e.GetPath(CurrentMap.CastleTile);
            }
        }

        public static void GainGold(int gold)
        {
            Gold += gold;
        }

        public static void DamageCastle(int damage)
        {
            AudioManager.PlaySoundEffect(10);
            CurHealth -= damage;

            if (CurHealth <= 0)
            {
                GarrettTowerDefense.ChangeScene(new GameOverScene());
            }
        }
    }

    public enum MouseAction
    {
        None = 0,
        BuildBarricade,
        BuildArrowTower,
        BuildToxicTower,
        BuildFlameTower,
        BuildTeslaTower,
        BuildIceTower,
        BuildGlaiveTower,
        BuildObservatory,
        BuildGoldMine,
        PlaceTile,
        Upgrade
    }
}