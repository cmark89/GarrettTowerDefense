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
    public class VictoryScene : Scene
    {
        public Texture2D Background { get; private set; }
        private float quoteDelay = 2f;
        private float elapsedTime = 0f;
        private bool playingMusic = false;

        public VictoryScene()
        {

        }

        public override void Initialize()
        {
            AudioManager.StopMusic();
            AudioManager.PlaySoundEffect(1, 1f);
        }

        public override void LoadContent(ContentManager Content)
        {
            Background = Content.Load<Texture2D>("Victory");
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsedTime >= quoteDelay && !playingMusic)
            {
                playingMusic = true;
                AudioManager.PlaySong(6);
            }


            // Check for input here to go back to menu
            if (playingMusic && (KeyboardHandler.KeyPress(Keys.Space) || KeyboardHandler.KeyPress(Keys.Escape) || KeyboardHandler.KeyPress(Keys.Enter) || MouseHandler.Click()))
            {
                GarrettTowerDefense.ChangeScene(new TitleScene());
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Background != null)
            {
                spriteBatch.Draw(Background, Vector2.Zero, Color.White);
            }
        }
    }
}
