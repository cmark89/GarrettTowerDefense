using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    class CruelCarrot : Enemy
    {
        public CruelCarrot(bool carnageMode = false)
        {
            Name = "Cruel Carrot";
            TextureID = 18;

            BaseHealth = 15;
            //Calculate true health based on the wave number
            Health = BaseHealth * (1 + (.35f * GameScene.waveManager.WaveNumber));
            CurrentHealth = Health;

            Bounty = 5;
            Bounty = (int)Bounty * (1 + (int)(GameScene.waveManager.WaveNumber / 10));

            Damage = 3;

            BaseMovementSpeed = 45;
            MovementSpeed = BaseMovementSpeed;

            Keywords = new List<Keyword>();
            Weaknesses = new float[] { 1f, 1f, 1f, 1f, 1f };

            CurrentState = MonsterState.Normal;
            CarnageColor = new Color(.7f, .3f, .7f, 1f);

            if (carnageMode)
            {
                Name = "Carnage Carrot";
                
                //Increased health
                Health = 500;
                CurrentHealth = Health;
            }

            //base.Initialize();
        }
    }
}
