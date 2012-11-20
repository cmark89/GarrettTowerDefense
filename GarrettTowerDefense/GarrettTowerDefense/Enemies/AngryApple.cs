using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    class AngryApple: Enemy
    {
        public AngryApple(bool carnageMode = false)
        {
            Name = "Angry Apple";
            TextureID = 24;

            BaseHealth = 50;
            //Calculate true health based on the wave number
            Health = BaseHealth * (1 + (.35f * GameScene.waveManager.WaveNumber));
            CurrentHealth = Health;

            Bounty = 12;
            Bounty = (int)Bounty * (1 + (int)(GameScene.waveManager.WaveNumber / 10));

            Damage = 4;

            BaseMovementSpeed = 60;
            MovementSpeed = BaseMovementSpeed;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { 1f, 1f, 1.5f, 1f, 1f };

            CurrentState = MonsterState.Normal;
            CarnageColor = new Color(.8f, .7f, .3f, 1f);

            if (carnageMode)
            {
                Name = "Carnage Apple";
                Health = 750;
                CurrentHealth = Health;
            }

            //base.Initialize();
        }
    }
}
