using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GarrettTowerDefense
{
    public class Tooltip
    {
        public static SpriteFont headerFont;
        public static SpriteFont bodyFont;
        public static SpriteFont goldFont;
        
        int posX;
        int posY;

        string headerText;
        string bodyText;
        string goldText;

        public Tooltip(string[] text)
        {
            headerText = text[0];
            bodyText = text[1];
            goldText = text[2];

            GameScene.tooltip = this;
        }

        public void Update(GameTime gameTime)
        {
            posX = MouseHandler.CurrentMouseState.X - GUI.TooltipTexture.Width;
            posY = MouseHandler.CurrentMouseState.Y - GUI.TooltipTexture.Height/2;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // First, draw the texture
            spriteBatch.Draw(GUI.TooltipTexture, new Vector2(posX, posY), Color.White);

            // Next, draw the header text
            spriteBatch.DrawString(headerFont, headerText, new Vector2(posX + 79 - (headerFont.MeasureString(headerText).X / 2), posY + 18), Color.White);

            // Now the body text
            string newBodyString = bodyFont.GetWrappedString(bodyText, new Rectangle(posX + 16, posY + 33, 115, 88));
            spriteBatch.DrawString(bodyFont, newBodyString, new Vector2(posX + 5 + 79 - (bodyFont.MeasureString(newBodyString).X / 2), posY + 33), Color.Black);

            // Finally the gold text
            spriteBatch.DrawString(goldFont, goldText, new Vector2(posX + 38, posY + 128), Color.Gold);
        }
    }
}
