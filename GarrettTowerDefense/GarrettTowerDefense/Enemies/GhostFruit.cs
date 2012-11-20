using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    class GhostFruit: Enemy
    {
        public GhostFruit(bool carnageMode = false)
        {
            Name = "Ghost Fruit";
            TextureID = 25;

            BaseHealth = 40;
            //Calculate true health based on the wave number
            Health = BaseHealth * (1 + (.35f * GameScene.waveManager.WaveNumber));
            CurrentHealth = Health;

            Bounty = 10;
            Bounty = (int)Bounty * (1 + (int)(GameScene.waveManager.WaveNumber / 10));

            Damage = 4;

            BaseMovementSpeed = 60;
            MovementSpeed = BaseMovementSpeed;
            Stealthed = true;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { 1f, 1f, 1.5f, 1f, 1f };

            CurrentState = MonsterState.Normal;
            CarnageColor = new Color(.2f, .4f, .2f, 1f);

            if (carnageMode)
            {
                Name = "Carnage Ghost Fruit";
                Health = 700;
                CurrentHealth = Health;
                //Add keywords or something...
            }

            //base.Initialize();
        }
    }
}
