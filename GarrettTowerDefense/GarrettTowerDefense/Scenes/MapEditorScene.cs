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
        public static Map CurrentMap { get; private set; }

        public static MouseAction CurrentMouseAction { get; set; }

        public static Texture2D GUITexture { get; private set; }
        public static Rectangle GUIArea { get; private set; }

        public static GUI GUI { get; private set; }
        
        public static int storedTile;


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
            GUI = new GUI(Content);
        }

        
        public override void Update(GameTime gameTime)
        {
            //Handle drawing and shit.
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw the map
            if (CurrentMap != null)
                CurrentMap.Draw(spriteBatch);

            if (CurrentMouseAction != MouseAction.None && MouseHandler.MouseOverMap())
            {
                Point curMouseTile = TileEngine.ScreenSpaceToMapSpace(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y));
                Color showColor = new Color(0f, .7f, 0f, .4f);
                
                //Replace this with trees and shit
                switch (CurrentMouseAction)
                {
                    //case MouseAction.BuildBarricade:
                        //spriteBatch.Draw(CurrentMap.Tileset.Texture, TileEngine.ScreenSpaceToMapVector(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y)), CurrentMap.Tileset.GetSourceRectangle(16), showColor);
                        //break;
                    default:
                        break;
                }
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
                
                if (MouseHandler.CurrentMouseState.X < CurrentMap.MapWidth * TileEngine.TileWidth)
                {
                    //Get the tile the mouse is over
                    Point curMouseTile = TileEngine.ScreenSpaceToMapSpace(new Vector2(MouseHandler.CurrentMouseState.X, MouseHandler.CurrentMouseState.Y));
                    
                    //Run a switch on the mouse state to determine what to do
                    switch (CurrentMouseAction)
                    {
                        //case MouseAction.BuildBarricade:
                            //new Barricade().Build(curMouseTile);
                            //if (Gold >= LoadedPrice) { Gold -= LoadedPrice; }
                            //break;
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
    }
}