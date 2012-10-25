using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace GarrettTowerDefense
{
    public class GUIButton
    {
        public static Texture2D UpTexture;
        public static Texture2D OverTexture;
        public static Texture2D DownTexture;



        //The location in screen space of this button
        public Rectangle Rect { get; private set; }
        //The ID of this button
        public int ID { get; private set; }
        //The current button state
        public GUIButtonState State { get; private set; }
        //The current texture to use to draw the button
        public Texture2D currentImage { get; private set; }
        //The image to appear on the button
        public int buttonImage {get; private set;}


        public GUIButton(Rectangle thisRect, int id)
        {
            Rect = thisRect;
            State = GUIButtonState.Up;

            buttonImage = id;
        }


        public static void LoadContent(ContentManager Content)
        {
            UpTexture = Content.Load<Texture2D>("GUI/GUIButtonUp");
            OverTexture = Content.Load<Texture2D>("GUI/GUIButtonOver");
            DownTexture = Content.Load<Texture2D>("GUI/GUIButtonDown");
        }

        public void Update(GameTime gameTime)
        {
            if (MouseHandler.MouseInRect(Rect))
            {
                if (MouseHandler.CurrentMouseState.LeftButton == ButtonState.Pressed)
                {
                    State = GUIButtonState.Down;
                }
                else
                {
                    State = GUIButtonState.Over;
                }
            }
            else
            {
                State = GUIButtonState.Up;
            }

            switch (State)
            {
                case GUIButtonState.Up:
                    currentImage = GUIButton.UpTexture;
                    break;
                case GUIButtonState.Over:
                    currentImage = GUIButton.OverTexture;
                    break;
                case GUIButtonState.Down:
                    currentImage = GUIButton.DownTexture;
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(currentImage, Rect, Color.White);
            spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle(Rect.X + 7, Rect.Y + 7, TileEngine.TileWidth, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(buttonImage), Color.White);
        }

        public bool ButtonClicked()
        {
            if (MouseHandler.MouseInRect(Rect) && MouseHandler.Click())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool MouseOver()
        {
            if (MouseHandler.MouseInRect(Rect))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    public enum GUIButtonState
    {
        Up = 0,
        Over,
        Down
    }
}
