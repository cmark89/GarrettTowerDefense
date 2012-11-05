using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GarrettTowerDefense
{
    public class EditorGUI
    {
        public List<GUIButton> Buttons { get; private set; }
        public Texture2D GUITexture { get; private set; }
        public GUIInput currentInput { get; private set; }

        public SpriteFont normalFont { get; private set; }

        private List<int> baseTiles;
        

        private static Rectangle GUIArea;

        public EditorGUI(ContentManager Content)
        {
            GUITexture = Content.Load<Texture2D>("GUI/MainGUI");
            normalFont = Content.Load<SpriteFont>("Fonts/NormalFont");

            baseTiles = new List<int>()
            {
                0, 1
            };

            GUIButton.LoadContent(Content);
            Buttons = new List<GUIButton>();
            currentInput = GUIInput.None;

            SetupGUI();
        }

        public void SetupGUI()
        {
            int x = GarrettTowerDefense.viewport.Width - GUITexture.Width;
            
            GUIArea = new Rectangle(x, 0, GUITexture.Width, GUITexture.Height);
            Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 23, GUIArea.Y + 104, 47, 47), 0));
            Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 74, GUIArea.Y + 104, 47, 47), 1));
            Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 125, GUIArea.Y + 104, 47, 47), 2));

            Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 23, GUIArea.Y + 156, 47, 47), 3));
            Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 74, GUIArea.Y + 156, 47, 47), 4)); 
            Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 125, GUIArea.Y + 156, 47, 47), 5));

            Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 23, GUIArea.Y + 208, 47, 47), 14));
            //Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 74, GUIArea.Y + 208, 47, 47)));
            //Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 125, GUIArea.Y + 208, 47, 47)));
        }


        public void Update(GameTime gameTime)
        {
            foreach (GUIButton b in Buttons)
            {
                b.Update(gameTime);
            }

            //Check for GUI input
            currentInput = GUIInput.None;

            foreach (GUIButton b in Buttons)
            {
                if (b.ButtonClicked())
                {
                    Console.WriteLine("Clicked a button!  (Index: " + b.buttonImage.ToString() + ")");
                    currentInput = GUIInput.PlaceTile;
                    MapEditorScene.SelectTile(b.buttonImage, baseTiles.Contains(b.buttonImage));
                }
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GUITexture, GUIArea, Color.White);
            
            foreach (GUIButton b in Buttons)
            {
                b.Draw(spriteBatch);
            }
        }
    }
}
