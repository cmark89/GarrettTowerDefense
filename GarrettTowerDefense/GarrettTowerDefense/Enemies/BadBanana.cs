using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    class BadBanana: Enemy
    {
        public BadBanana(bool carnageMode = false)
        {
            Name = "Bad Banana";
            TextureID = 21;

            BaseHealth = 25;
            //Calculate true health based on the wave number
            Health = BaseHealth * (1 + (.35f * GameScene.waveManager.WaveNumber));
            CurrentHealth = Health;

            Bounty = 8;
            Bounty = (int)Bounty * (1 + (int)(GameScene.waveManager.WaveNumber / 10));

            Damage = 4;

            BaseMovementSpeed = 70;
            MovementSpeed = BaseMovementSpeed;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { 1f, 1f, 1f, 1f, 1f };

            CurrentState = MonsterState.Normal;

            if (carnageMode)
            {
                Name = "Carnage Banana";
                BaseHealth = 300;
                //Add keywords or something...
            }

            //base.Initialize();
        }
    }
}
