using System;
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
        public static Map CurrentMap { get; private set; }
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

        //Used for storing the price of towers efficiently
        public static int LoadedPrice {get; set;}
        public static WaveManager waveManager { get; set; }


        //Stores player related variables
        public static int MaxHealth;
        public static int CurHealth;
        public static int Gold;


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
            Gold = 200;
            
            CurrentMouseAction = MouseAction.None;
        }

        public override void LoadContent(ContentManager Content)
        {
            GUI = new GUI(Content);
            EnemyHealthbar = Content.Load<Texture2D>("GUI/EnemyHealthbar");
        }

        
        public override void Update(GameTime gameTime)
        {
            #region GUI and Input
            //Check for mouse input and handle it accordingly
            HandleMouseInput(gameTime);

            GUI.Update(gameTime);

            //Check for GUI input
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
            
            #endregion

            //Update each tower in turn
            foreach (Tower t in Towers)
            {
                t.Update(gameTime);
            }

            foreach (Enemy e in Enemies)
            {
                if(e.Alive)
                    e.Update(gameTime);
            }

            for (int i = 0; i < Enemies.Count; i++)
            {
                if (!Enemies[i].Alive)
                    Enemies.RemoveAt(i);
            }

            //Update the wave manager.
            waveManager.Update(gameTime);

            
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            //If a tower is currently selected, draw that tower at the cursor position in a faded opacity.
            
            //Draw the map
            if (CurrentMap != null)
                CurrentMap.Draw(spriteBatch);

            //pathfinder.Draw(spriteBatch);

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

            GUI.Draw(spriteBatch);
        }


        public static void SetMap(Map map)
        {
            CurrentMap = map;
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
                            if (Gold >= LoadedPrice) { Gold -= LoadedPrice; }
                            break;
                        case MouseAction.BuildArrowTower:
                            new ArrowTower().Build(curMouseTile);
                            if (Gold >= LoadedPrice) { Gold -= LoadedPrice; }
                            break;
                        case MouseAction.BuildToxicTower:
                            new ToxicTower().Build(curMouseTile);
                            if (Gold >= LoadedPrice) { Gold -= LoadedPrice; }
                            break;
                        case MouseAction.BuildFlameTower:
                            new FlameTower().Build(curMouseTile);
                            if (Gold >= LoadedPrice) { Gold -= LoadedPrice; }
                            break;
                        case MouseAction.BuildTeslaTower:
                            new TeslaTower().Build(curMouseTile);
                            if (Gold >= LoadedPrice) { Gold -= LoadedPrice; }
                            break;
                        case MouseAction.BuildIceTower:
                            new IceTower().Build(curMouseTile);
                            if (Gold >= LoadedPrice) { Gold -= LoadedPrice; }
                            break;
                        case MouseAction.BuildGlaiveTower:
                            new GlaiveTower().Build(curMouseTile);
                            if (Gold >= LoadedPrice) { Gold -= LoadedPrice; }
                            break;
                        case MouseAction.BuildObservatory:
                            new Observatory().Build(curMouseTile);
                            if (Gold >= LoadedPrice) { Gold -= LoadedPrice; }
                            break;
                        case MouseAction.BuildGoldMine:
                            new GoldMine().Build(curMouseTile);
                            if (Gold >= LoadedPrice) { Gold -= LoadedPrice; }
                            break;
                        default:
                            break;
                    }

                    

                    //Handle clicking on the map
                    AudioManager.PlaySoundEffect(1);
                }
                //The mouse is too far to the right to be over the map
                else
                {
                    //Handle clicking over the interface.
                    AudioManager.PlaySoundEffect(0);
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
            Console.WriteLine("Gain " + gold + " gold!  Now have " + (Gold + gold));
            Gold += gold;
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
        Upgrade
    }
}