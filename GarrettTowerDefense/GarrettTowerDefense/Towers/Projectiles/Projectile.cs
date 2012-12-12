﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GarrettTowerDefense
{
    public class Projectile
    {
        public string Name { get; private set; }

        public int SpriteIndex { get; private set; }
        public float Speed { get; private set; }
        public Vector2 Position { get; private set; }

        public Tower Parent { get; private set; }
        public Enemy Target { get; private set; }

        public bool Enabled { get; private set; }

        public List<Enemy> HitEnemies { get; set; }
        public int Bounces { get; set; }
        private bool isGlaive = false;
        private float rotationSpeed = .4f;
        private float curRotation = -90f;

        //Stores the offset to the center of a tile
        private float _offset = TileEngine.TileWidth / 2;


        public Projectile(Tower parent, Enemy target, float speed) : this(parent, target, speed, 13) { }

        public Projectile(Tower parent, Enemy target, float speed, int index, string name = "")
        {
            if (name != "")
                Name = name;

            Parent = parent;
            Target = target;
            Speed = speed;

            Position = parent.Position;

            //TEST
            SpriteIndex = index;

            Enabled = true;

            if (parent.Name == "Glaive Tower")
            {
                HitEnemies = new List<Enemy>();
                isGlaive = true;
            }

            


            //WORK ON THIS
            //curRotation = (float)Math.Atan2((double)(Target.Position.Y - Position.Y), (double)(Target.Position.X - Position.X));
            //curRotation *= (180f / (float)Math.PI);
            curRotation += 90;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            if (isGlaive)
                curRotation += rotationSpeed;
            else if(Name == "Icicle")
                curRotation = Vector2Helper.FindAngle(Target.Position, Position) - (float)(Math.PI/2);


            //Handle the movement of the projectile
            float movementThisFrame = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Vector2.Distance(Position, Target.Position) <= movementThisFrame)
            {
                Position = Target.Position;
                Parent.OnProjectileHit(this, Target);
            }
            else
            {
                Vector2 newMovement = Target.Position - Position;
                newMovement.Normalize();

                Position += (newMovement * movementThisFrame);
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (Enabled)
                spriteBatch.Draw(GameScene.CurrentMap.Tileset.Texture, new Rectangle((int)Position.X + TileEngine.TileWidth / 2, (int)Position.Y + TileEngine.TileHeight / 2, TileEngine.TileWidth, TileEngine.TileHeight), GameScene.CurrentMap.Tileset.GetSourceRectangle(SpriteIndex), Color.White, curRotation, new Vector2(15, 15), SpriteEffects.None, 0);
        }


        public void Destroy()
        {
            Enabled = false;
        }

        public void SetNewTarget(Enemy e)
        {
            Target = e;
        }
    }
}
