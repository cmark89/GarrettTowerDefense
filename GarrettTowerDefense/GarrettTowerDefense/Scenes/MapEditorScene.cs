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
            //Player clicks the mouse
            if (MouseHandler.Click())
            {
                //The mouse is over the map.
                
                if (MouseHandler.CurrentMouseState.X < CurrentMap.MapWidth * TileEngine.TileWidth && CurrentMouseAction == MouseAction.PlaceTile)
                {
                    //Get the tile the mouse is over
                    Point curMouseTile = TileEngine.ScreenSpaceToMapSpace(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y));

                    if (storedTile == 4 && castlePlaced)
                    {
                        castleTile.tiles.RemoveAt(1);
                        
                    }

                    PlaceTile(curMouseTile, storedTile, isBaseTile);

                    if (storedTile == 4)
                    {
                        castleTile = CurrentMap.mapCells[curMouseTile.Y, curMouseTile.X];
                        castlePlaced = true;
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

        public static void SelectTile(int index, bool isBaseLayer)
        {
            storedTile = index;
            isBaseTile = isBaseLayer;

            CurrentMouseAction = MouseAction.PlaceTile;
        }

        public static void PlaceTile(Point targetTile, int tileID, bool isBase)
        {
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