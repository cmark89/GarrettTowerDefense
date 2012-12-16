using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    class PoopyPotato: Enemy
    {
        public PoopyPotato(bool carnageMode = false)
        {
            Name = "Poopy Potato";
            TextureID = 23;

            BaseHealth = 40;
            //Calculate true health based on the wave number
            Health = BaseHealth * (1 + (.35f * GameScene.waveManager.WaveNumber));
            CurrentHealth = Health;

            Bounty = (GameScene.waveManager.WaveNumber) + 4;

            Damage = 4;

            BaseMovementSpeed = 50;
            MovementSpeed = BaseMovementSpeed;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { .7f, 1f, 1f, 1.3f, 1f };

            CurrentState = MonsterState.Normal;
            CarnageColor = new Color(.3f, .3f, .5f, 1f);

            if (carnageMode)
            {
                Name = "Carnage Potato";
                Health = 750;
                CurrentHealth = Health;
            }

            //base.Initialize();
        }
    }
}
