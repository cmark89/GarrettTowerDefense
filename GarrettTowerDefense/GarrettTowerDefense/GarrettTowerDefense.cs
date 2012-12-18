using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Pathfinding;

namespace GarrettTowerDefense
{
    
    public class GarrettTowerDefense : Microsoft.Xna.Framework.Game
    {
        //my code dont steal

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Viewport viewport;

        public static Scene currentScene;
        public static GraphicsDeviceManager StaticGraphics;

        public static ContentManager StaticContent;

        public static bool loadingScene = false;

        public GarrettTowerDefense()
        {
            graphics = new GraphicsDeviceManager(this);
            this.Window.Title = "Garrett Tower Defense";
            StaticGraphics = graphics;
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            viewport = GraphicsDevice.Viewport;
            TileEngine.InitializeEngine();
            AudioManager.Initialize();

            //Later replace with something else.
            IsMouseVisible = true;

            //Load the tileset here so the map can be properly loaded.
            new Tileset(Content.Load<Texture2D>("levelTileset"));

            //Let the audio manager load all relevant assets itself
            AudioManager.LoadContent(Content);


            // If you want to make changes to the game map, uncomment the following line and then comment out the 
            // two lines that follow it.  Make any changes you like to the map, and then click the save button to
            // automatically write to map.gtd.  Afterwards, swap the commented lines once more.

            //currentScene = new MapEditorScene();

            currentScene = new OpeningMovieScene();
            currentScene.LoadContent(Content);

            

            

            base.Initialize();

            
        }


        protected override void LoadContent()
        {
            StaticContent = Content;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);          
            
            //GameScene.CurrentMap.InitializeTestMap2();
            currentScene.LoadContent(Content);
        }


        protected override void UnloadContent()
        {

        }
        
        protected override void Update(GameTime gameTime)
        {
            MouseHandler.Update(gameTime);
            KeyboardHandler.Update(gameTime);

            currentScene.Update(gameTime);

            base.Update(gameTime);
        }

        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            currentScene.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void ChangeScene(Scene newScene)
        {
            loadingScene = true;

            newScene.LoadContent(StaticContent);
            newScene.Initialize();
            currentScene = newScene;

            loadingScene = false;
        }
    }
}
