using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GarrettTowerDefense
{
    public static class Utility
    {
        public static string GetWrappedString(this SpriteFont sfont, string text, Rectangle drawRectangle)
        {
            // The actual string to draw
            string stringToDraw = "";

            // Stores each word in the list to determine wrapping.
            List<string> wordsInString = new List<string>(text.Split(' '));

            // Stores the current X coordinate of the string, for calculating wrapping.
            int currentX = 0;

            // Stores the length of the next word.
            int wordWidth = 0;
            string currentWord;

            while (wordsInString.Count > 0)
            {
                currentWord = wordsInString[0].Trim();
                wordWidth = (int)sfont.MeasureString(currentWord).X;

                if (currentX + wordWidth >= drawRectangle.Width)
                {
                    currentWord = "\n" + currentWord;
                    currentX = 0;
                }

                currentWord = currentWord + " ";
                currentX += wordWidth;

                stringToDraw = String.Concat(stringToDraw, currentWord);
                wordsInString.RemoveAt(0);
            }

            return stringToDraw;
        }
    }

    class Vector2Helper
    {
        public static float FindAngle(Vector2 to, Vector2 from)
        {
            float y = to.Y - from.Y;
            float x = to.X - from.X;

            return (float)Math.Atan2(y, x);
        }
    }
}
