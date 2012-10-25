using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    public class GoldMine : Tower
    {
        public static int Cost = 75;

        public double LastGoldTime;
        public float GoldInterval = 10f;
        public int GoldBonus = 6;

        //Constructor for arrow towers
        public GoldMine()
        {
            Name = "Gold Mine";
            TileIndex = 15;
            Health = 100;
            Level = 1;

            DamageType = DamageType.Physical;
            Damage = 0;
            AttackSpeed = 0f;
            AttackRange = 0;
            
        }


        public override void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalSeconds - LastGoldTime >= GoldInterval)
            {
                LastGoldTime = gameTime.TotalGameTime.TotalSeconds;
                ProduceGold();
            }
        }

        public void ProduceGold()
        {
            GameScene.Gold += GoldBonus;
        }

        //Build the tower on the selected tile
        public override void Build(Point point)
        {
            if (GameScene.CurrentMap.TileContainsGold(point) && GameScene.Gold >= GameScene.LoadedPrice)
            {
                //Play sound effect.
                //Create the tower on the given tile
                MapPosition = point;
                Position = new Vector2(point.X * TileEngine.TileWidth, point.Y * TileEngine.TileHeight);
                GameScene.CurrentMap.SetTileGoldability(point, false);

                Initialize();
            }
            else
            {
                //Give error sound
                GameScene.ClearMouseAction();
            }
        }
    }
}
