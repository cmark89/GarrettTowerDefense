using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    class MoroseMushroom: Enemy
    {
        public MoroseMushroom()
        {
            Name = "Morose Mushroom";
            TextureID = 22;

            BaseHealth = 50;
            //Calculate true health based on the wave number
            Health = BaseHealth * (1 + (.35f * GameScene.waveManager.WaveNumber));
            CurrentHealth = Health;

            Bounty = 8;
            Bounty = (int)Bounty * (1 + (int)(GameScene.waveManager.WaveNumber / 10));

            Damage = 4;

            BaseMovementSpeed = 25;
            MovementSpeed = BaseMovementSpeed;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { .5f, 1f, 1.5f, 1f, 1f };

            CurrentState = MonsterState.Normal;

            //base.Initialize();
        }
    }
}
