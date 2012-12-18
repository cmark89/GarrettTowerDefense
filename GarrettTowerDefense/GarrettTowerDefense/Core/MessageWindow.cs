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
    public delegate void ShowMessageWindowEventHandler();
    public delegate void HideMessageWindowEventHandler();

    public class MessageWindow
    {
        private Texture2D windowTexture;

        private string messageText;
        private Rectangle windowRect;
        private Rectangle messageRectangle;

        public event ShowMessageWindowEventHandler ShowMessageWindow;
        public event HideMessageWindowEventHandler HideMessageWindow;

        private SpriteFont font;

        private bool useSlowReveal = true;
        private int charsRevealed = 0;

        // This is the delay between individual characters appearing.
        private const float textRevealTime = .06f;
        private float nextCharRevealTime;

        public bool Visible { get; private set; }

        public MessageWindow(string textureFile, string text = "", bool slowReveal = true, bool isVisible = true)
        {
            // Uses the static ContentManager instance to load the provided texture name, as well as the proper font
            windowTexture = GarrettTowerDefense.StaticContent.Load<Texture2D>("GUI/" + textureFile);
            font = GarrettTowerDefense.StaticContent.Load<SpriteFont>("Fonts/MessageFont");

            windowRect = new Rectangle(0, 0, windowTexture.Width, windowTexture.Height);
            messageRectangle = new Rectangle(144, 38, 340, 69);

            if (!String.IsNullOrEmpty(text))
            {
                SetText(text);
            }

            if (isVisible)
                ShowWindow();
            
            GameScene.messageWindow = this;
        }

        public void ShowWindow()
        {
            Visible = true;
            OnShowMessageWindow();
        }

        public void HideWindow()
        {
            Visible = false;
            OnHideMessageWindow();
        }

        public void SetText(string text)
        {
            messageText = font.GetWrappedString(text, messageRectangle);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(windowTexture, windowRect, Color.White);
                if (!useSlowReveal)
                    spriteBatch.DrawString(font, messageText, new Vector2(messageRectangle.X, messageRectangle.Y), Color.White);
                else
                {
                    string revealedText = messageText.Substring(0, charsRevealed);
                    spriteBatch.DrawString(font, revealedText, new Vector2(messageRectangle.X, messageRectangle.Y), Color.White);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            // Update the typing effect here...
            // Check for input to close the window.

            // This is for really fast testing...
            // Change this to actually detect meaningful input.
            if ((MouseHandler.Click() || KeyboardHandler.KeyPress(Keys.Space) || KeyboardHandler.KeyPress(Keys.Escape) || KeyboardHandler.KeyPress(Keys.Enter)) && Visible)
            {
                if (useSlowReveal && charsRevealed < messageText.Length)
                {
                    charsRevealed = messageText.Length;
                }
                else
                {
                    HideWindow();
                }
            }


            // And this is for the typing effect...
            if (useSlowReveal && gameTime.TotalGameTime.TotalSeconds >= nextCharRevealTime)
            {
                RevealNextChar();
                nextCharRevealTime = (float)gameTime.TotalGameTime.TotalSeconds + textRevealTime;
            }
        }

        public void RevealNextChar()
        {
            if(charsRevealed < messageText.Length)
                charsRevealed++;
        }

        // Raise the event when shown
        public void OnShowMessageWindow()
        {
            if (ShowMessageWindow != null)
                ShowMessageWindow();
        }

        // Raise the event when shown
        public void OnHideMessageWindow()
        {
            if (HideMessageWindow != null)
                HideMessageWindow();
        }
    }
}
