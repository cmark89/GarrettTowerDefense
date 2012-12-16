using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GarrettTowerDefense
{
    public class TitleScene : Scene
    {
        public Texture2D Background { get; private set; }
        public Texture2D TitleText { get; private set; }
        public Texture2D ControlTexture { get; private set; }

        public SpriteFont TextFont { get; private set; }

        private bool fadingIn = false;
        private float opacity = 1f;

        private bool controlsShown = false;

        private string startText = "Press SPACE to Play!";
        private string controlsText = "Press C for Controls and Help!";

        private Color startColor = Color.White;

        private double time = 0f;
        private double startTime = 0f;

        public TitleScene()
        {
            Initialize();
        }

        public override void Initialize()
        {
            AudioManager.PlaySong(7);
        }

        public override void LoadContent(ContentManager Content)
        {
            Background = Content.Load<Texture2D>("TitleScreen");
            TitleText = Content.Load<Texture2D>("TitleScreenText");
            TextFont = Content.Load<SpriteFont>("Fonts/MainMenuFont");
            ControlTexture = Content.Load<Texture2D>("GUI/Controls");
        }

        public override void Update(GameTime gameTime)
        {            
            if(!controlsShown)
            {
                startTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (startTime > 0)
                {
                    startColor = Color.White;

                    if (startTime > .5f)
                        startTime = -.5f;

                    
                }
                else
                {
                    startColor = new Color(0f,0f,0f,0f);
                }
                



                if (fadingIn)
                {
                    // Text is fading in, so apply a positive change to alpha
                    opacity += (float)gameTime.ElapsedGameTime.TotalSeconds / 3f;
                    if (opacity >= 1)
                        fadingIn = false;
                }
                else
                {
                    // Text is fading out, so apply a negative change to alpha
                    opacity -= (float)gameTime.ElapsedGameTime.TotalSeconds / 3f;
                    if (opacity <= .3f)
                        fadingIn = true;
                }

                // Check for input here to end the game or start the game
                if (KeyboardHandler.KeyPress(Keys.Space))
                {
                    GarrettTowerDefense.ChangeScene(new GameScene());
                    GameScene.SetMap((Map)Serializer.Deserialize("map.gtd"));
                }

                if(KeyboardHandler.KeyPress(Keys.C))
                {
                    controlsShown = true;
                }

                time += gameTime.ElapsedGameTime.TotalSeconds;
                if (time > 19.6D)
                {
                    GarrettTowerDefense.ChangeScene(new OpeningMovieScene());
                    AudioManager.StopMusic();
                }
            }
            else
            {
                if(KeyboardHandler.KeyPress(Keys.C) || KeyboardHandler.KeyPress(Keys.Space) || KeyboardHandler.KeyPress(Keys.Escape) || KeyboardHandler.KeyPress(Keys.Enter))
                {
                    controlsShown = false;
                    time = 0D;
                }
            }
            
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Background, Vector2.Zero, Color.White);

            Color titleColor = Color.White * opacity;

            spriteBatch.Draw(TitleText, new Vector2(0,45), titleColor);
            spriteBatch.DrawString(TextFont, startText, new Vector2((GarrettTowerDefense.StaticGraphics.GraphicsDevice.Viewport.Width / 2f) - (TextFont.MeasureString(startText).X / 2), 360), startColor);
            spriteBatch.DrawString(TextFont, controlsText, new Vector2((GarrettTowerDefense.StaticGraphics.GraphicsDevice.Viewport.Width / 2f) - (TextFont.MeasureString(controlsText).X / 2), 430), Color.White);

            if (controlsShown)
                spriteBatch.Draw(ControlTexture, Vector2.Zero, Color.White);
        }
    }
}
