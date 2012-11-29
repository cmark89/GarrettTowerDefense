using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GarrettTowerDefense;

namespace GarrettTowerDefense
{
    public class GUI
    {
        public List<GUIButton> Buttons { get; private set; }
        public Texture2D GUITexture { get; private set; }
        public Texture2D HealthbarTexture { get; private set; }
        public GUIInput currentInput { get; private set; }

        public SpriteFont normalFont { get; private set; }
        public SpriteFont goldFont { get; private set; }

        public Vector2 tooltipText1Start { get; private set; }
        public string tooltipText0 { get; private set; }
        public string tooltipText1 { get; private set; }
        

        private static Rectangle GUIArea;

        public GUI(ContentManager Content)
        {
            GUITexture = Content.Load<Texture2D>("GUI/MainGUI");
            HealthbarTexture = Content.Load<Texture2D>("GUI/Healthbar");
            normalFont = Content.Load<SpriteFont>("Fonts/NormalFont");
            goldFont = Content.Load<SpriteFont>("Fonts/GoldFont");


            GUIButton.LoadContent(Content);
            Buttons = new List<GUIButton>();
            currentInput = GUIInput.None;

            SetupGUI();
        }

        public void SetupGUI()
        {
            int x = GarrettTowerDefense.viewport.Width - GUITexture.Width;

            tooltipText1Start = new Vector2(x + 5, 325);

            GUIArea = new Rectangle(x, 0, GUITexture.Width, GUITexture.Height);
            Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 23, GUIArea.Y + 104, 47, 47), 16));
            Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 74, GUIArea.Y + 104, 47, 47), 6));
            Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 125, GUIArea.Y + 104, 47, 47), 7));

            Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 23, GUIArea.Y + 156, 47, 47), 8));
            Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 74, GUIArea.Y + 156, 47, 47), 9)); 
            Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 125, GUIArea.Y + 156, 47, 47), 11));

            Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 23, GUIArea.Y + 208, 47, 47), 12));
            Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 74, GUIArea.Y + 208, 47, 47), 13));
            Buttons.Add(new GUIButton(new Rectangle(GUIArea.X + 125, GUIArea.Y + 208, 47, 47), 15));
        }


        public void Update(GameTime gameTime)
        {
            foreach (GUIButton b in Buttons)
            {
                b.Update(gameTime);
            }

            //Check for GUI hovering
            Rectangle tooltipRect = new Rectangle((int)tooltipText1Start.X, (int)tooltipText1Start.Y, 145, 182);

            // Note: Refactor this to a BuildStrings() method called on construction, to prevent calling GetWrappedString every frame.
            if (Buttons[0].MouseOver())
            {
                tooltipText0 = "Barricade";
                tooltipText1 = normalFont.GetWrappedString("Barricades are simple constructions of metal and several less sturdy materials.  While they lack any true offensive capabilities, they are cheap and can be used to block enemy pathing.", tooltipRect);
                            
            }
            else if (Buttons[1].MouseOver())
            {
                tooltipText0 = "Arrow Tower";
                tooltipText1 = normalFont.GetWrappedString("Arrow Towers are the workhorse of Garrett's fortifications.  While they do not possess any special effects or attributes, their cheap cost is admired far and wide.", tooltipRect);
            }
            else if (Buttons[2].MouseOver())
            {
                tooltipText0 = "Toxic Tower";
                tooltipText1 = normalFont.GetWrappedString("Toxic Towers are vile constructions born of filth and disease.  Its attacks cause enemies to lose health over time.", tooltipRect);
            }
            else if (Buttons[3].MouseOver())
            {
                tooltipText0 = "Flame Tower";
                tooltipText1 = normalFont.GetWrappedString("The Flame Tower is a powerful furnace built from the bones of Garrett's enemies.  While it attacks slowly, its attacks deal damage in an area and cause targets to burn for a percentage of their health over time.", tooltipRect);
            }
            else if (Buttons[4].MouseOver())
            {
                tooltipText0 = "Tesla Tower";
                tooltipText1 = normalFont.GetWrappedString("The Tesla Tower is Garrett's most maddening invention.  The Tesla Tower has a slow attack speed, but its attacks will hit all enemies within a large radius around it.", tooltipRect);
            }
            else if (Buttons[5].MouseOver())
            {
                tooltipText0 = "Ice Tower";
                tooltipText1 = normalFont.GetWrappedString("The Ice Towers are menacing obelisks made of unbreakable ice crystals.  Any enemy unfortunate enough to be hit by this tower will have its movement speed slowed.", tooltipRect);
            }
            else if (Buttons[6].MouseOver())
            {
                tooltipText0 = "Glaive Tower";
                tooltipText1 = normalFont.GetWrappedString("The Glaive Tower is a powerful and deadly device that shoots spinning blades of carnage at its opponents.  These blades will bounce to additional targets, allowing it to cut through swarms of enemies.", tooltipRect);
            }
            else if (Buttons[7].MouseOver())
            {
                tooltipText0 = "Observatory";
                tooltipText1 = normalFont.GetWrappedString("The Observatory, normally a non-violent retreat for intellectuals, has been upgraded to reveal stealthed enemy units.", tooltipRect);
            }
            else if (Buttons[8].MouseOver())
            {
                tooltipText0 = "Gold Mine";
                tooltipText1 = normalFont.GetWrappedString("Gold is the fuel of the engine of commerce.  Constructs a Gold Mine on top of a gold vein, providing passive income.", tooltipRect);
            }
            else
            {
                tooltipText0 = "";
                tooltipText1 = "";
            }

            //Check for GUI input
            currentInput = GUIInput.None;

            if (Buttons[0].ButtonClicked())
            {
                currentInput = GUIInput.BuildBarricade;
            }
            if (Buttons[1].ButtonClicked())
            {
                currentInput = GUIInput.BuildArrowTower;
            }
            if (Buttons[2].ButtonClicked())
            {
                currentInput = GUIInput.BuildToxicTower;
            }
            if (Buttons[3].ButtonClicked())
            {
                currentInput = GUIInput.BuildFlameTower;
            }
            if (Buttons[4].ButtonClicked())
            {
                currentInput = GUIInput.BuildTeslaTower;
            }
            if (Buttons[5].ButtonClicked())
            {
                currentInput = GUIInput.BuildIceTower;
            }
            if (Buttons[6].ButtonClicked())
            {
                currentInput = GUIInput.BuildGlaiveTower;
            }
            if (Buttons[7].ButtonClicked())
            {
                currentInput = GUIInput.BuildObservatory;
            }
            if (Buttons[8].ButtonClicked())
            {
                currentInput = GUIInput.BuildGoldMine;
            }
            
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GUITexture, GUIArea, Color.White);
            spriteBatch.Draw(HealthbarTexture, new Rectangle(GUIArea.X + 7, GUIArea.Y+7, (int)(HealthbarTexture.Width * ((float)GameScene.CurHealth/(float)GameScene.MaxHealth)),24), Color.White);
            
            spriteBatch.DrawString(goldFont, GameScene.Gold.ToString(), new Vector2(GUIArea.X + 75, 37), Color.Orange);
            foreach (GUIButton b in Buttons)
            {
                b.Draw(spriteBatch);
            }

            if (tooltipText1 != null)
            {
                spriteBatch.DrawString(normalFont, tooltipText1, tooltipText1Start + new Vector2(6,0), Color.LightYellow);
            }
        }
    }

    public enum GUIInput
    {
        None = 0,
        BuildBarricade,
        BuildArrowTower,
        BuildToxicTower,
        BuildFlameTower,
        BuildTeslaTower,
        BuildIceTower,
        BuildGlaiveTower,
        BuildObservatory,
        BuildGoldMine,
        UpgradeTower,
        DestroyTower,
        PlaceTile
    }
}
