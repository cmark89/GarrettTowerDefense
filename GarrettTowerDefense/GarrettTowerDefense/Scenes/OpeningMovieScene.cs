using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace GarrettTowerDefense
{
    public class OpeningMovieScene : Scene
    {
        public Texture2D Texture { get; private set; }
        public Video Intro { get; private set; }

        private static VideoPlayer player;
        private Rectangle drawRect;

        private bool started = false;

        public OpeningMovieScene()
        {
            Initialize();
        }

        public override void Initialize()
        {
            if (player == null)
            {
                player = new VideoPlayer();
                player.IsLooped = false;
            }
        }

        public override void LoadContent(ContentManager Content)
        {
            Intro = Content.Load<Video>("Intro");
        }

        public override void Update(GameTime gameTime)
        {
            if (!started)
            {
                player.Play(Intro);
                started = true;
            }

            drawRect = new Rectangle(0, 0, GarrettTowerDefense.StaticGraphics.GraphicsDevice.Viewport.Width, GarrettTowerDefense.StaticGraphics.GraphicsDevice.Viewport.Height);
           
            // Check for input here to skip and go straight to the title screen
            if ((started && player.State == MediaState.Stopped) || KeyboardHandler.KeyPress(Keys.Space) || KeyboardHandler.KeyPress(Keys.Enter) || KeyboardHandler.KeyPress(Keys.Escape))
            {
                GarrettTowerDefense.ChangeScene(new TitleScene());
                player.Stop();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Video is playing, so draw it.
            if (player.State != MediaState.Stopped)
            {
                Texture = player.GetTexture();

                if (Texture != null)
                {
                    spriteBatch.Draw(Texture, drawRect, Color.White);
                }
            }
            else
            {
                GarrettTowerDefense.StaticGraphics.GraphicsDevice.Clear(Color.Black);
            }
        }
    }
}
