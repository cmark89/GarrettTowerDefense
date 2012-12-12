using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GarrettTowerDefense
{
    abstract public class Scene
    {
        public static Map CurrentMap;

        public abstract void LoadContent(ContentManager Content);

        public abstract void Initialize();
        
        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
