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
using VoxelRPGGame.GameEngine.World.Voxels;
using VoxelRPGGame.GameEngine.UI;


namespace VoxelRPGGame.MenuSystem.Screens
{
    public class PauseScreen : MenuScreen
    {
        Texture2D blankTexture;
        SpriteFont font;

       // string menuTitle;

       // private List<MenuElement> elements = new List<MenuElement>();

        public event EventHandler<EventArgs> ToMenu;
        public event EventHandler<EventArgs> Exit;


        public PauseScreen(EventHandler<EventArgs> unpause)
            : base("Game Paused")
        {
  
                menuTitle = "Simulation Paused";
            

            //menuTitle = "Paused";
            //For Pause Screen


            blankTexture = ScreenManager.GetInstance().ContentManager.Load<Texture2D>("blank");
            //blankTexture = new Texture2D(graphics, 1, 1, false, SurfaceFormat.Color);
            //blankTexture.SetData<Color>(new Color[] { Color.White });

                font = ScreenManager.GetInstance().ContentManager.Load<SpriteFont>("Fonts\\SimulatorFont");
                TEMP_SetSimulatorMenuElements(unpause);


      

         

            isActive = true;
            hasFocus = true;

            OnTransitionIn();
        }


        private void SetMenuElements(EventHandler<EventArgs> unpause)
        {
            TextElement Play = new TextElement("Play Game");
            // testButton.Position = new Vector2(graphics.Viewport.Width / 2, 150);
            Play.IsActive = true;
            Play.Selected += unpause;


            elements.Add(Play);

            TextElement OptionMenuButton = new TextElement("Options");
            OptionMenuButton.IsActive = true;
            OptionMenuButton.Selected += OptionMenu;

            elements.Add(OptionMenuButton);



            TextElement exitGameButton = new TextElement("Exit Game");
            exitGameButton.IsActive = true;
            exitGameButton.Selected += ExitGame;

            elements.Add(exitGameButton);

            setElementPositions_Centre(10);
        }

        private void TEMP_SetSimulatorMenuElements(EventHandler<EventArgs> unpause)
        {
            ButtonElement Play = new ButtonElement();
            // testButton.Position = new Vector2(graphics.Viewport.Width / 2, 150);
            Play.IsActive = true;
            Play.Selected += unpause;
            Play.ElementText = "Resume Simulation";


            elements.Add(Play);

            ButtonElement OptionMenuButton = new ButtonElement();
            OptionMenuButton.IsActive = true;
            OptionMenuButton.Selected += OptionMenu;
            OptionMenuButton.ElementText = "Options";

            elements.Add(OptionMenuButton);



            ButtonElement exitGameButton = new ButtonElement();
            exitGameButton.IsActive = true;
            exitGameButton.Selected += ExitGame;
            exitGameButton.ElementText = "Exit";

            elements.Add(exitGameButton);

            setElementPositions_Centre(20);

            foreach (MenuElement e in elements)
            {
                e.Font = font;
                e.TextColour = Color.White;
                e.Alpha = 1f;
            }

        }

    /*    public void setPlayButton(ContentManager Content, GraphicsDevice device)
        {

        }
            */
        public override void HandleInput(GameTime gameTime, InputState input)
        {
          /*  if(input.CurrentKeyboardState.IsKeyDown(Keys.Escape)&&input.PreviousKeyboardState.IsKeyUp(Keys.Escape))
            {
                Unpause(this, new EventArgs());
            }*/
            base.HandleInput(gameTime, input);
        }



        public override void Draw(SpriteBatch Batch)
        {

            //Adds opacity for pause screen
           /* Batch.Draw(blankTexture,
                new Rectangle(0, 0, graphics.PresentationParameters.BackBufferWidth, graphics.PresentationParameters.BackBufferHeight),  
                new Color(0.25f, 0.25f, 0.25f, 0.5f));*/
            float alpha = 0.5f;

            Batch.Draw(blankTexture, ScreenManager.GetInstance().GraphicsDevice.Viewport.Bounds, Color.Black * alpha);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(ScreenManager.GetInstance().GraphicsDevice.Viewport.Width / 2, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            //Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            //titlePosition.Y -= transitionOffset * 100;

            Batch.DrawString(font, menuTitle, titlePosition, Color.White, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);


            foreach (MenuElement element in elements)
            {
                element.Draw(Batch);
            }

        }


#region Event Handlers
    /*    public void PlayGame(object sender, EventArgs e)
        {
            ScreenManager.Game.IsMouseVisible = true;
            ScreenManager.removeScreen(this);
            foreach (GameScreen screen in screenManager.Screens)
            {
                if (screen is GameplayScreen)
                {
                    screen.HasFocus = true;
                }
            }
        }*/
        public void OptionMenu(object sender, EventArgs e)
        {
            OnTransitionOut();
            ScreenManager.GetInstance().Game.IsMouseVisible = true;
            //ScreenManager.Screens.Clear();

            for (int i = 0; i < ScreenManager.GetInstance().Screens.Count; i++)
            {
                //NOTE: Will need to change this, as currently exiting the options menu will set ALL screens in screenManager
                //to visible and hasFocus, not just the ones that were in that state before entering the options menu
                ScreenManager.GetInstance().Screens[i].IsVisible = false;
                ScreenManager.GetInstance().Screens[i].HasFocus = false;
            }
            ScreenManager.GetInstance().addScreen(new BackgroundScreen());
            OptionsScreen options = new OptionsScreen();
            
            options.ExitMenu += ExitMenu;
            ScreenManager.GetInstance().addScreen(options);

        }

        public void ExitMenu(object sender, EventArgs e)
        {
            ScreenManager.GetInstance().removeScreen((AbstractScreen)sender);
            OnTransitionIn();

            int backgroundScreenIndex=-1;
            for (int i = 0; i < ScreenManager.GetInstance().Screens.Count; i++)
            {
                //NOTE: Will need to change this, as currently exiting the options menu will set ALL screens in screenManager
                //to visible and hasFocus, not just the ones that were in that state before entering the options menu

                ScreenManager.GetInstance().Screens[i].IsVisible = true;
                ScreenManager.GetInstance().Screens[i].HasFocus = true;


                if (ScreenManager.GetInstance().Screens[i] is BackgroundScreen)
                {
                    backgroundScreenIndex = i;
                }

            }

            if (backgroundScreenIndex > -1)
            {
                ScreenManager.GetInstance().removeScreen(ScreenManager.GetInstance().Screens[backgroundScreenIndex]);
            }

        }


        public void ExitGame(object sender, EventArgs e)
        {
            //TEMP:Clear the chunk manager - will need to save on exit
            ChunkManager.Dispose();
            GameHUDScreen.Dispose();


            ScreenManager.GetInstance().Screens.Clear();

            //NOTE: This is temporary, will need to wire the exit event to the GameManager to properly handle exiting
            DebugScreen.GetInstance().RemoveDebugListing("Graph Size: ");

            ScreenManager.GetInstance().addScreen(new BackgroundScreen());
            ScreenManager.GetInstance().addScreen(new MainMenuScreen());

        }

#endregion
    }
}
