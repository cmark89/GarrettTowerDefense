﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GarrettTowerDefense
{
    public class GameOverScene : Scene
    {
        public Texture2D Background { get; private set; }
        private float elapsedTime = 0f;
        private float laughDelay = 1.4f;
        private bool playingMusic = false;

        public GameOverScene()
        {

        }

        public override void Initialize()
        {
            AudioManager.StopMusic();
            AudioManager.PlaySoundEffect(0, 1f);
        }

        public override void LoadContent(ContentManager Content)
        {
            Background = Content.Load<Texture2D>("GameOver");
        }

        public override void Update(GameTime gameTime)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsedTime >= laughDelay && !playingMusic)
            {
                playingMusic = true;
                AudioManager.PlaySong(8);
            }


            // Check for input here to go back to menu
            if (playingMusic && KeyboardHandler.KeyPress(Keys.Space))
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