using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    class Animation
    {
        public int[] AnimationFrames { get; set; }
        public int CurrentFrame { get; set; }
        
        private int frameRate;
        private float frameTime;
        private float elapsedTime = 0;

        public Animation(int[] frames, int fRate = 30)
        {
            AnimationFrames = frames;

            // Calculate the time for each frame to be displayed based on the target FPS
            frameTime = 1.0f / fRate;
        }

        public void Update(GameTime gameTime)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsedTime >= frameTime)
            {
                CurrentFrame++;
                if (CurrentFrame > AnimationFrames.Length)
                {
                    CurrentFrame = 0;
                }
            }
        }

        // The animation class does not draw itself.  Each animation is drawn by whatever is responsible for creating it.
    }
}
