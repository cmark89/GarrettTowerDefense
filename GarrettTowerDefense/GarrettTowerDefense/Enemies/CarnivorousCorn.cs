﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    class CarnivorousCorn: Enemy
    {
        public CarnivorousCorn(bool carnageMode = false)
        {
            Name = "Carnivorous Corn";
            TextureID = 26;

            BaseHealth = 60;
            //Calculate true health based on the wave number
            Health = BaseHealth * (1 + (.35f * GameScene.waveManager.WaveNumber));
            CurrentHealth = Health;

            Bounty = (GameScene.waveManager.WaveNumber) + 4;

            Damage = 4;

            BaseMovementSpeed = 60;
            MovementSpeed = BaseMovementSpeed;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { .75f, 1.3f, 1f, 1.3f, 1f };

            CurrentState = MonsterState.Normal;
            CarnageColor = new Color(.7f, .4f, .5f, 1f);

            if (carnageMode)
            {
                Name = "Carnage Corn";
                Health = 850;
                CurrentHealth = Health;
                //Add keywords or something...
            }

            //base.Initialize();
        }
    }
}
