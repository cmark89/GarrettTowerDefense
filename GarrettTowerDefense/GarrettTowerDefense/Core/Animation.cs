using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    public delegate void AnimationFinishEventHandler();

    public class Animation
    {
        public int[] AnimationFrames { get; set; }
        public int CurrentFrame { get; set; }
        public int CurrentFrameIndex
        {
            get { return AnimationFrames[CurrentFrame]; }
        }
        
        private int frameRate;
        private float frameTime;
        private float elapsedTime = 0;
        private bool oneShot = false;
        public bool enabled;

        public event AnimationFinishEventHandler AnimationFinish;

        public Animation(int[] frames, int fRate = 20, bool oneshot = false)
        {
            AnimationFrames = frames;
            CurrentFrame = 0;
            frameRate = fRate;

            oneShot = oneshot;

            elapsedTime = 0;
            enabled = true;

            // Calculate the time for each frame to be displayed based on the target FPS
            frameTime = 1.0f / frameRate;
        }

        public void Update(GameTime gameTime)
        {
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsedTime >= frameTime)
            {
                CurrentFrame++;
                elapsedTime = 0;

                if (CurrentFrame >= AnimationFrames.Length)
                {
                    if (!oneShot)
                        CurrentFrame = 0;
                    else
                    {
                        AnimationFrames = new int[] { 43 };
                        enabled = false;
                        OnAnimationFinish();
                    }
                }
            }
        }

        protected void OnAnimationFinish()
        {
            if (AnimationFinish != null)
                AnimationFinish();
        }

        // The animation class does not draw itself.  Each animation is drawn by whatever is responsible for creating it.
    }
}
