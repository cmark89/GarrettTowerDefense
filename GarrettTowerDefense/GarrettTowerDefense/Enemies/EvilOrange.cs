using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    class EvilOrange: Enemy
    {
        public EvilOrange(bool carnageMode = false)
        {
            Name = "Evil Orange";
            TextureID = 20;

            BaseHealth = 30;
            //Calculate true health based on the wave number
            Health = BaseHealth * (1 + (.35f * GameScene.waveManager.WaveNumber));
            CurrentHealth = Health;

            Bounty = 8;
            Bounty = (int)Bounty * (1 + (int)(GameScene.waveManager.WaveNumber / 10));

            Damage = 4;

            BaseMovementSpeed = 45;
            MovementSpeed = BaseMovementSpeed;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { 1f, 1f, 1f, 1f, 1f };

            CurrentState = MonsterState.Normal;

            CarnageColor = new Color(.4f, .9f, .7f, 1f);

            if (carnageMode)
            {
                Name = "Carnage Orange";
                Health = 600;
                CurrentHealth = Health;
            }

            //base.Initialize();
        }
    }
}
