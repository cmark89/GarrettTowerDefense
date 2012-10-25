using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

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
            if (Buttons[0].MouseOver())
            {
                tooltipText0 = "Barricade";
                tooltipText1 = "Barricades are simple constructions \nof metal and several less sturdy \nmaterials.  While they lack any true \noffensive capabilities, they are cheap \nand can be used to block enemy \npathing.";
            }
            else if (Buttons[1].MouseOver())
            {
                tooltipText0 = "Arrow Tower";
                tooltipText1 = "Arrow Towers are the workhorse of \nGarrett's fortifications.  While \nthey do not possess any special \neffects or attributes, their cheap \ncost is admired far and wide.";
            }
            else if (Buttons[2].MouseOver())
            {
                tooltipText0 = "Toxic Tower";
                tooltipText1 = "Toxic Towers are vile constructions \nborn of filth and disease.  Its attacks \ncause enemies to lose health \nover time.";
            }
            else if (Buttons[3].MouseOver())
            {
                tooltipText0 = "Flame Tower";
                tooltipText1 = "The Flame Tower is a powerful \nfurnace built from the bones of \nGarrett's enemies.  While it attacks \nslowly, its attacks deal damage in an \narea and cause targets to burn for a \npercentage of their health over \ntime.";
            }
            else if (Buttons[4].MouseOver())
            {
                tooltipText0 = "Tesla Tower";
                tooltipText1 = "The Tesla Tower is Garrett's most \nmaddening invention.  The Tesla \nTower has a slow attack speed, but its \nattacks will hit all enemies within a \nlarge radius around it.";
            }
            else if (Buttons[5].MouseOver())
            {
                tooltipText0 = "Ice Tower";
                tooltipText1 = "The Ice Towers are menacing obelisks \nmade of unbreakable ice crystals.  \nAny enemy unfortunate enough to \nbe hit by this tower will have its \nmovement speed slowed.";
            }
            else if (Buttons[6].MouseOver())
            {
                tooltipText0 = "Glaive Tower";
                tooltipText1 = "The Glaive Tower is a powerful and \ndeadly device that shoots spinning \nblades of carnage at its opponents.  \nThese blades will bounce to \nadditional targets, allowing it to cut \nthrough swarms of enemies.";
            }
            else if (Buttons[7].MouseOver())
            {
                tooltipText0 = "Observatory";
                tooltipText1 = "The Observatory, normally a \nnon-violent retreat for intellectuals, \nhas been upgraded to reveal \nstealthed enemy units.";
            }
            else if (Buttons[8].MouseOver())
            {
                tooltipText0 = "Gold Mine";
                tooltipText1 = "Gold is the fuel of the engine of \ncommerce.  Constructs a Gold Mine \non top of a gold vein, providing \npassive income.";
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
                spriteBatch.DrawString(normalFont, tooltipText1, tooltipText1Start, Color.LightYellow);
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
        DestroyTower
    }
}
