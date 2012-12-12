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

        private bool fadingIn = false;
        private float opacity = 1f;

        private double time = 0f;

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
        }

        public override void Update(GameTime gameTime)
        {
            time += gameTime.ElapsedGameTime.TotalSeconds;

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
            else if (time > 19.6D)
            {
                GarrettTowerDefense.ChangeScene(new OpeningMovieScene());
                AudioManager.StopMusic();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Background, Vector2.Zero, Color.White);

            Color titleColor = Color.White * opacity;
            spriteBatch.Draw(TitleText, new Vector2(0,45), titleColor);
        }
    }
}
