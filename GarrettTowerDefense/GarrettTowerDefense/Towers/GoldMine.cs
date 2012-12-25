using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GarrettTowerDefense
{
    public class GoldMine : Tower
    {
        public static int Cost = 50;

        public double GoldChargeTime;
        public float GoldInterval = 10f;
        public int GoldBonus = 6;

        //Constructor for arrow towers
        public GoldMine()
        {
            Name = "Gold Mine";
            TileIndex = 15;
            Health = 100;
            Level = 1;

            UpgradeCost = new int[] { 50, 100, 150, 200 };
            BuildCost = Cost;

            DamageType = DamageType.Physical;
            Damage = 0;
            AttackSpeed = 0f;
            AttackRange = 0;
        }

        public override void UpdateTooltipText()
        {
            tooltipText = String.Format("Level {0} {1} \n\nGold Income: {2}\nIncome Interval: {3} seconds \n\nNext Upgrade: \n+2 Gold Income\n-1 Income Interval\n\nUpgrade Cost: {4}",
                Level, Name, GoldBonus, GoldInterval, Level < 5 ? UpgradeCost[Level - 1].ToString() : " - ");
        }

        public override void LevelUp()
        {
            GoldBonus += 2;
            GoldInterval -= 1;

            base.LevelUp();
        }

        public override void LevelDown()
        {
            GoldBonus -= 2;
            GoldInterval += 1;

            base.LevelDown();
        }


        public override void Update(GameTime gameTime)
        {
            if (Constructed && !GameScene.Paused && !GameScene.PlayerPaused)
            {
                GoldChargeTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (GoldChargeTime >= GoldInterval)
                {
                    GoldChargeTime = 0;
                    ProduceGold();
                }
            }
        }

        public void ProduceGold()
        {
            AudioManager.PlaySoundEffect(6);
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
                ParentCell = GameScene.CurrentMap[point.Y, point.X];

                ParentCell.IsWalkable = false;
                ParentCell.IsBuildable = false;
                ParentCell.ContainsTower = true;

                GameScene.Gold -= BuildCost;

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
