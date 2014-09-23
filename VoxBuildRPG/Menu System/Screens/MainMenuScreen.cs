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

using VoxelRPGGame.MenuSystem.MenuElements;

using VoxelRPGGame.GameEngine;


namespace VoxelRPGGame.MenuSystem.Screens
{
    public class MainMenuScreen: MenuScreen
    {
        Texture2D blankTexture;
        SpriteFont font;

        //string menuTitle = "Main Menu";

        //private List<MenuElement> elements = new List<MenuElement>();

        public event EventHandler<EventArgs> Unpause;
        public event EventHandler<EventArgs> ToMenu;
        public event EventHandler<EventArgs> Exit;
        

        public MainMenuScreen()
            : base("Main Menu")
        {
           
            blankTexture = ScreenManager.GetInstance().ContentManager.Load<Texture2D>("blank");
            //blankTexture = new Texture2D(graphics, 1, 1, false, SurfaceFormat.Color);
            //blankTexture.SetData<Color>(new Color[] { Color.White });

          


                font = ScreenManager.GetInstance().ContentManager.Load<SpriteFont>("Fonts\\SimulatorFont");
                TEMP_SetSimulatorMenuElements();
            
          

            hasFocus = true;
            isActive = true;

            OnTransitionIn();
        }

        private void TEMP_SetSimulatorMenuElements()
        {
            TextElement playGameButton = new TextElement("New Game");
            playGameButton.IsActive = true;
            playGameButton.Selected += NewGame;
          //  playGameButton.ElementText="New Simulation";

            elements.Add(playGameButton);

         /*   ButtonElement mapEditorButton = new ButtonElement();
            mapEditorButton.IsActive = true;
            mapEditorButton.Selected += MapEditor;
            mapEditorButton.ElementText = "Map Editor";

            elements.Add(mapEditorButton);*/

            TextElement OptionMenuButton = new TextElement("Options");
            OptionMenuButton.IsActive = true;
            OptionMenuButton.Selected += OptionMenu;
          //  OptionMenuButton.ElementText = "Options";


            elements.Add(OptionMenuButton);

            TextElement exitGameButton = new TextElement("Exit");
            exitGameButton.IsActive = true;
            exitGameButton.Selected += ExitGame;
        //    exitGameButton.ElementText = "Exit";

            elements.Add(exitGameButton);


            foreach (MenuElement e in elements)
            {
                e.Font = font;
                e.TextColour = Color.White;
                e.Alpha = 1f;
            }

          //  setElementPositions_Centre(10);
            setElementPositions_Left(100,10);
        }

        private void SetMenuElements()
        {
            TextElement playGameButton = new TextElement("New Game");
            playGameButton.IsActive = true;
            playGameButton.Selected += NewGame;

            elements.Add(playGameButton);

            TextElement OptionMenuButton = new TextElement("Options");
            OptionMenuButton.IsActive = true;
            OptionMenuButton.Selected += OptionMenu;
            

            elements.Add(OptionMenuButton);

            TextElement exitGameButton = new TextElement("Exit");
            exitGameButton.IsActive = true;
            exitGameButton.Selected += ExitGame;
            
       

            elements.Add(exitGameButton);

            setElementPositions_Centre(10);

      
     

        }

        public override void Draw(SpriteBatch Batch)
        {
            //Adds opacity for screen
            float alpha = 0.0f;

            Batch.Draw(blankTexture, ScreenManager.GetInstance().GraphicsDeviceManager.GraphicsDevice.Viewport.Bounds, Color.Black * alpha);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(ScreenManager.GetInstance().GraphicsDeviceManager.GraphicsDevice.Viewport.Width / 2, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            //Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            //titlePosition.Y -= transitionOffset * 100;

            Batch.DrawString(font, menuTitle, titlePosition, Color.White * screenAlpha, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            foreach (MenuElement element in elements)
            {
                element.Draw(Batch);
            }
        }

#region Event Handlers
        public void NewGame(object sender, EventArgs e)
        {
            OnTransitionOut();
            ScreenManager.GetInstance().Game.IsMouseVisible = true;

            ScreenManager.GetInstance().Screens.Clear();

            ScreenManager.GetInstance().addScreen(new LoadingScreen());

            //   GameManager g = new GameManager(content, graphics, ScreenManager);

            //    ScreenManager.addScreen(g);
            //   ScreenManager.addScreen(new GameplayScreen(content, graphics, ScreenManager));
            ScreenManager.GetInstance().addScreen(new GameScreen(false));

        }

        public void MapEditor(object sender, EventArgs e)
        {
            OnTransitionOut();
            ScreenManager.GetInstance().Game.IsMouseVisible = true;

            ScreenManager.GetInstance().Screens.Clear();

            ScreenManager.GetInstance().addScreen(new LoadingScreen());


            ScreenManager.GetInstance().addScreen(new GameScreen(true));
        }


     /*   public void TestGame(object sender, EventArgs e)
        {
            OnTransitionOut();
            ScreenManager.GetInstance().Game.IsMouseVisible = true;

            ScreenManager.GetInstance().Screens.Clear();

            ScreenManager.GetInstance().addScreen(new LoadingScreen());

         //   GameManager g = new GameManager(content, graphics, ScreenManager);
          
        //    ScreenManager.addScreen(g);
         //   ScreenManager.addScreen(new GameplayScreen(content, graphics, ScreenManager));
            ScreenManager.GetInstance().addScreen(new SimulationScreen());
            
        }*/

        public void OptionMenu(object sender, EventArgs e)
        {
            OnTransitionOut();
            ScreenManager.GetInstance().Game.IsMouseVisible = true;
            //ScreenManager.Screens.Clear();

            isVisible = false;
            hasFocus = false;

            OptionsScreen options = new OptionsScreen();
            options.ExitMenu += ExitMenu;

            ScreenManager.GetInstance().addScreen(options);
          
        }

        public void ExitMenu(object sender, EventArgs e)
        {
            OnTransitionOut();
            ScreenManager.GetInstance().removeScreen((AbstractScreen)sender);



            for (int i = 0; i < ScreenManager.GetInstance().Screens.Count; i++)
            {
                //NOTE: Will need to change this, as currently exiting the options menu will set ALL screens in screenManager
                //to visible and hasFocus, not just the ones that were in that state before entering the options menu
                ScreenManager.GetInstance().Screens[i].IsVisible = true;
                if (ScreenManager.GetInstance().Screens[i] is MenuScreen)
                {
                    (ScreenManager.GetInstance().Screens[i] as MenuScreen).OnTransitionIn();

                }
                ScreenManager.GetInstance().Screens[i].HasFocus = true;
            }


        }


        public void ExitGame(object sender, EventArgs e)
        {
            ScreenManager.GetInstance().Game.Exit();
        }
#endregion 
    }
    
}
