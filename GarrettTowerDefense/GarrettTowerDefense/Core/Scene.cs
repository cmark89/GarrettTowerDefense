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


    public class TitleScene : Scene
    {
        public override void LoadContent(ContentManager Content)
        {
        }

        public override void Initialize()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}
