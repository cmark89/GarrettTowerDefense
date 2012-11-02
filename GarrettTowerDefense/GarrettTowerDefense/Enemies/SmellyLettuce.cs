using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    class SmellyLettuce: Enemy
    {
        public SmellyLettuce(bool carnageMode = false)
        {
            Name = "Smelly Lettuce";
            TextureID = 27;

            BaseHealth = 70;
            //Calculate true health based on the wave number
            Health = BaseHealth * (1 + (.35f * GameScene.waveManager.WaveNumber));
            CurrentHealth = Health;

            Bounty = 13;
            Bounty = (int)Bounty * (1 + (int)(GameScene.waveManager.WaveNumber / 10));

            Damage = 4;

            BaseMovementSpeed = 50;
            MovementSpeed = BaseMovementSpeed;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { .75f, .5f, 1f, 1.5f, 1.2f };

            CurrentState = MonsterState.Normal;

            if (carnageMode)
            {
                Name = "Carnage Lettuce";
                BaseHealth = 300;
                //Add keywords or something...
            }

            //base.Initialize();
        }
    }
}
