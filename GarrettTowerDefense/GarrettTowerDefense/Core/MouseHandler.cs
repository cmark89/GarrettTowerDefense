using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GarrettTowerDefense
{
    static class MouseHandler
    {
        public static MouseState PreviousMouseState;
        public static MouseState CurrentMouseState;

        public static void Update(GameTime gameTime)
        {
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }

        //Returns true when the left mouse button is pressed down
        static public bool Click()
        {
            if (PreviousMouseState.LeftButton == ButtonState.Released && CurrentMouseState.LeftButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        //Returns true when the right mouse button is pressed down
        static public bool RightClick()
        {
            if (PreviousMouseState.RightButton == ButtonState.Released && CurrentMouseState.RightButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        //Returns true when the given rectangle is clicked on
        static public bool ClickOn(Rectangle rect)
        {
            if(MouseInRect(rect) && PreviousMouseState.LeftButton == ButtonState.Released && CurrentMouseState.LeftButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        //Returns true when the given rectangle is right clicked on
        static public bool RightClickOn(Rectangle rect)
        {
            if (MouseInRect(rect) && PreviousMouseState.LeftButton == ButtonState.Released && CurrentMouseState.LeftButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        //Returns true if the given rectangle contains the mouse cursor
        static public bool MouseInRect(Rectangle rect)
        {
            return (CurrentMouseState.X >= rect.X && CurrentMouseState.X <= rect.X + rect.Width && CurrentMouseState.Y >= rect.Y && CurrentMouseState.Y <= rect.Y + rect.Height);
        }

        static public bool MouseOverMap()
        {
            Map currentMap = GameScene.CurrentMap;

            return (CurrentMouseState.X >= 0 && CurrentMouseState.X < currentMap.MapWidth * TileEngine.TileWidth && CurrentMouseState.Y >= 0 && CurrentMouseState.Y < currentMap.MapHeight * TileEngine.TileHeight);
        }
    }
}
