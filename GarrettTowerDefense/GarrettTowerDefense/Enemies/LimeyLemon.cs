using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    class LimeyLemon: Enemy
    {
        public LimeyLemon(bool carnageMode = false)
        {
            Name = "Limey Lemon";
            TextureID = 28;

            BaseHealth = 90;
            //Calculate true health based on the wave number
            Health = BaseHealth * (1 + (.35f * GameScene.waveManager.WaveNumber));
            CurrentHealth = Health;

            Bounty = 14;
            Bounty = (int)Bounty * (1 + (int)(GameScene.waveManager.WaveNumber / 10));

            Damage = 4;

            BaseMovementSpeed = 45;
            MovementSpeed = BaseMovementSpeed;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { 1f, 1f, 1f, 1f, 1f };

            CurrentState = MonsterState.Normal;
            CarnageColor = new Color(.8f, .3f, .7f, 1f);

            if (carnageMode)
            {
                Name = "Carnage Lemon";
                Health = 1000;
                CurrentHealth = Health;
                //Add keywords or something...
            }

            //base.Initialize();
        }
    }
}
