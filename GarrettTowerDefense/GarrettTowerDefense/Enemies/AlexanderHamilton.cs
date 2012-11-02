using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    class AlexanderHamilton: Enemy
    {
        public AlexanderHamilton()
        {
            Name = "Alexander Hamilton";
            TextureID = 15;

            BaseHealth = 1000000;
            //Calculate true health based on the wave number
            Health = BaseHealth;
            CurrentHealth = Health;

            Bounty = 150;

            Damage = 100;

            BaseMovementSpeed = 50;
            MovementSpeed = BaseMovementSpeed;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { 1f, 1f, 1f, 1f, 1f };

            CurrentState = MonsterState.Normal;

            //base.Initialize();
        }
    }
}
