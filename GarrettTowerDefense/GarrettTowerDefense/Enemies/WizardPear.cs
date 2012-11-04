using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GarrettTowerDefense
{
    class WizardPear: Enemy
    {
        public int phase = 1;
        public Color drawColor = Color.White;

        public WizardPear()
        {
            Name = "Wizard Pear";
            TextureID = 29;

            BaseHealth = 1500;
            //Calculate true health based on the wave number
            Health = BaseHealth;
            CurrentHealth = Health;

            Bounty = 150;

            Damage = 20;

            BaseMovementSpeed = 30;
            MovementSpeed = BaseMovementSpeed;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { 1f, 1f, 1f, 1f, 1f };

            CurrentState = MonsterState.Normal;

            //base.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Alive)
            {
                spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle((int)Position.X, (int)Position.Y, TileEngine.TileWidth, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(TextureID), drawColor);
            }
        }

        public override void CheckForDeath()
        {
            if (CurrentHealth <= 0)
            {
                Alive = false;
                GameScene.waveManager.WavePaused = false;
                return;
            }
        }
    }
}
