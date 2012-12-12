using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GarrettTowerDefense
{
    static class KeyboardHandler
    {
        public static KeyboardState PreviousKeyboardState;
        public static KeyboardState CurrentKeyboardState;

        public static void Update(GameTime gameTime)
        {
            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
        }

        //Returns true when the provided key is pressed and let up
        static public bool KeyPress(Keys thisKey)
        {
            if(PreviousKeyboardState.IsKeyDown(thisKey) && CurrentKeyboardState.IsKeyUp(thisKey))
                return true;
            else
                return false;
        }
    }
}
