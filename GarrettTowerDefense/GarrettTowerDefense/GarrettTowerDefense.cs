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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GarrettTowerDefense : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Viewport viewport;

        public static Scene currentScene;
        public static GraphicsDeviceManager StaticGraphics;

        // Good idea, or no?  Time will tell.
        public static ContentManager StaticContent;

        public static bool loadingScene = false;

        public GarrettTowerDefense()
        {
            graphics = new GraphicsDeviceManager(this);
            this.Window.Title = "Garrett Tower Defense";
            StaticGraphics = graphics;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
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


            currentScene = new OpeningMovieScene();
            currentScene.LoadContent(Content);

            //currentScene = new GameScene();
            //GameScene.SetMap((Map)Serializer.Deserialize("map.gtd"));
            //Scene.CurrentMap.LoadFromSerialized();
            //Scene.CurrentMap.Initialize();

            //currentScene = new MapEditorScene();

            

            base.Initialize();

            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            StaticContent = Content;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);          

            //Set the test map.
            
            //GameScene.CurrentMap.InitializeTestMap2();
            currentScene.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            MouseHandler.Update(gameTime);
            KeyboardHandler.Update(gameTime);

            currentScene.Update(gameTime);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
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
