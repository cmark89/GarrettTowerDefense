using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    class GrumpyGrape: Enemy
    {
        public GrumpyGrape(bool carnageMode = false)
        {
            Name = "Grumpy Grape";
            TextureID = 19;

            BaseHealth = 25;
            //Calculate true health based on the wave number
            Health = BaseHealth * (1 + (.35f * GameScene.waveManager.WaveNumber));
            CurrentHealth = Health;

            Bounty = 6;
            Bounty = (int)Bounty * (1 + (int)(GameScene.waveManager.WaveNumber / 10));

            Damage = 4;

            BaseMovementSpeed = 40;
            MovementSpeed = BaseMovementSpeed;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { 1f, 1f, 1f, 1f, 1f };

            CurrentState = MonsterState.Normal;
            CarnageColor = new Color(.7f, .9f, .2f, 1f);

            if (carnageMode)
            {
                Name = "Carnage Grape";
                Health = 550;
                CurrentHealth = Health;
            }

            //base.Initialize();
        }
    }
}
