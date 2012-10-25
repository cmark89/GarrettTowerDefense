﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    class CruelCarrot : Enemy
    {
        public CruelCarrot()
        {
            Name = "Cruel Carrot";
            TextureID = 18;

            BaseHealth = 15;
            //Calculate true health based on the wave number
            Health = BaseHealth * (1 + (.35f * GameScene.waveManager.WaveNumber));
            CurrentHealth = Health;

            Damage = 3;

            BaseMovementSpeed = 45;
            MovementSpeed = BaseMovementSpeed;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { 1f, 1f, 1f, 1f, 1f };

            CurrentState = MonsterState.Normal;

            //base.Initialize();
        }
    }
}
