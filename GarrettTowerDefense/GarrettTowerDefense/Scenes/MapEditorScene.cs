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
    public class MapEditorScene : Scene
    {
        //Stores the current map
        //public static Map CurrentMap { get; private set; }

        public static MouseAction CurrentMouseAction { get; set; }

        public static Texture2D GUITexture { get; private set; }
        public static Rectangle GUIArea { get; private set; }

        public static EditorGUI GUI { get; private set; }
        
        public static int storedTile;
        public static bool isBaseTile;

        public static bool castlePlaced;
        public static MapCell castleTile;


        public MapEditorScene()
        {
            Initialize();
        }


        public override void Initialize()
        {
            CurrentMap = new Map();
            
            CurrentMouseAction = MouseAction.None;
        }


        public override void LoadContent(ContentManager Content)
        {
            GUI = new EditorGUI(Content);
        }

        
        public override void Update(GameTime gameTime)
        {
            GUI.Update(gameTime);
            HandleMouseInput(gameTime);
            //Handle drawing and shit.
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw the map
            if (CurrentMap != null)
                CurrentMap.Draw(spriteBatch);

            if (CurrentMouseAction != MouseAction.None && MouseHandler.MouseOverMap() && !isBaseTile)
            {
                Point curMouseTile = TileEngine.ScreenSpaceToMapSpace(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y));
                Color showColor = new Color(0f, .7f, 0f, .4f);
                
                spriteBatch.Draw(CurrentMap.Tileset.Texture, TileEngine.ScreenSpaceToMapVector(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y)), CurrentMap.Tileset.GetSourceRectangle(storedTile), showColor);
                
            }

            GUI.Draw(spriteBatch);
        }


        public static void SetMap(Map map)
        {
            CurrentMap = map;
        }

        #region Mouse Input
        //Handle mouse input during the game.
        public static void HandleMouseInput(GameTime gameTime)
        {
            Point curMouseTile = TileEngine.ScreenSpaceToMapSpace(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y));
            //Player clicks the mouse
            if (MouseHandler.Click())
            {
                //The mouse is over the map.   
                if (MouseHandler.CurrentMouseState.X < CurrentMap.MapWidth * TileEngine.TileWidth && CurrentMouseAction == MouseAction.PlaceTile)
                {
                    if (!isBaseTile)
                    {
                        if (storedTile == 4 && castlePlaced)
                        {
                            for (int i = 0; i < castleTile.tiles.Count; i++)
                            {
                                if (castleTile.tiles[i].TileID == 4)
                                {
                                    castleTile.tiles.RemoveAt(i);
                                    break;
                                }
                            }
                        }

                        if (storedTile == 4)
                        {
                            castleTile = CurrentMap.mapCells[curMouseTile.Y, curMouseTile.X];
                            castlePlaced = true;
                        }

                        if (storedTile == 5)
                        {
                            Scene.CurrentMap.SpawnPoints.Add(new Point(curMouseTile.X, curMouseTile.Y));
                        }
                    }

                    if(!isBaseTile)
                        ClearNonBaseTiles(curMouseTile);

                    PlaceTile(curMouseTile, storedTile, isBaseTile);
                }
            }
            if (MouseHandler.RightClick())
            {
                //If right clicking, break the current operation.
                if (CurrentMouseAction != MouseAction.None)
                    CurrentMouseAction = MouseAction.None;
                else if (MouseHandler.CurrentMouseState.X < CurrentMap.MapWidth * TileEngine.TileWidth && CurrentMouseAction == MouseAction.None)
                {
                    ClearNonBaseTiles(new Point(curMouseTile.X, curMouseTile.Y));
                }
            }
        }
        #endregion

        public static void ClearNonBaseTiles(Point point)
        {
            if (CurrentMap.mapCells[point.Y, point.X].tiles.Count > 1)
            {
                int i;

                //If there is a spawn point here, ensure that it is properly disposed of from the map's list.
                if (CurrentMap.mapCells[point.Y, point.X].tiles[1].TileID == 5)
                {
                    CurrentMap.SpawnPoints.Remove(CurrentMap.SpawnPoints.Find(x => x.X == point.X && x.Y == point.Y));
                }

                for (i = 1; i < CurrentMap.mapCells[point.Y, point.X].tiles.Count; i++)
                {
                    
                    CurrentMap.mapCells[point.Y, point.X].tiles.RemoveAt(i);
                }
            }
            else
                return;
        }

        public static void ClearMouseAction()
        {
            CurrentMouseAction = MouseAction.None;
        }

        public static void SelectTile(int index, bool isBaseLayer)
        {
            storedTile = index;
            isBaseTile = isBaseLayer;

            CurrentMouseAction = MouseAction.PlaceTile;
        }

        public static void PlaceTile(Point targetTile, int tileID, bool isBase)
        {
            if (!MouseHandler.MouseOverMap())
                return;

            int y = targetTile.Y;
            int x = targetTile.X;

            int? tempIndex = null;

            if(isBase)
            {
                if (CurrentMap.mapCells[y, x].tiles.Count > 1)
                {
                    tempIndex = CurrentMap.mapCells[y, x].tiles[1].TileID;
                }

                CurrentMap.mapCells[y, x] = new MapCell();
            }

            CurrentMap.mapCells[y, x].AddTile(tileID);

            if (tempIndex != null)
                CurrentMap.mapCells[y, x].AddTile((int)tempIndex);
        }
    }
}