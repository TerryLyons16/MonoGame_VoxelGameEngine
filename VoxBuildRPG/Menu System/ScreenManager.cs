using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


using System.Windows.Forms;

using VoxelRPGGame.MenuSystem;
using VoxelRPGGame.MenuSystem.Screens;
using VoxelRPGGame.GameEngine.Rendering;


namespace VoxelRPGGame.MenuSystem
{

    /// <summary>
    /// Singleton class that controls the screen hierarchy
    /// </summary>
    public class ScreenManager : DrawableGameComponent
     {
        private SpriteFont font;

        private static ScreenManager screenManager = null;

        public static string[] buttonIcons = new string[3];
        

        SpriteBatch spriteBatch;
        GraphicsDeviceManager graphics;
        ContentManager content;
        BasicEffect defaultEffect; //NOTE: Creation of basicEffects is slow, use shared effect

        InputState input=new InputState();

        private Form gameForm;
        bool isFullscreen = true;

        private List<Thread> activeThreads = new List<Thread>();
        private List<AbstractScreen> screens = new List<AbstractScreen>();
        private List<AbstractScreen> tempScreensList = new List<AbstractScreen>();



        private ScreenManager(Game game):base(game)
        {
            buttonIcons[0] = "Images\\Menus\\Menu Elements\\SimulatorIdleButton";
            buttonIcons[1] = "Images\\Menus\\Menu Elements\\SimulatorHoverButton";
            buttonIcons[2] = "Images\\Menus\\Menu Elements\\SimulatorHoverButton";
  
            //graphics = new GraphicsDeviceManager(this);
            //Content.RootDirectory = "Content";
            content = Game.Content;


            graphics = new GraphicsDeviceManager(game);

            //Set up the window as a borderless form
            Application.EnableVisualStyles();
            gameForm = (Form)Form.FromHandle(game.Window.Handle);
            ToggleFullscreen();
          /*  gameForm.FormBorderStyle = FormBorderStyle.None;
            gameForm.Left = 0;
            gameForm.Top = 0;
            */

        //Form MyGameForm = (Form)Form.FromHandle(game.Window.Handle);
        // MyGameForm.FormBorderStyle = FormBorderStyle.None;

        //    graphics.PreferredBackBufferWidth = GraphicsDevice.Viewport.Width;//1280;
         //   graphics.PreferredBackBufferHeight = GraphicsDevice.Viewport.Height;//720;
         //   graphics.PreferredBackBufferWidth = 1920; //GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;//1280;
        //    graphics.PreferredBackBufferHeight = 1080;// GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;//720;
            //NOTE: Multisampling and Fullscreen toggle do not work
           graphics.PreferMultiSampling = true;
     //      graphics.IsFullScreen = isFullscreen;
           graphics.ApplyChanges();

           game.Exiting += GameExiting;

          DebugScreen.GetInstance();

        }


        /// <summary>
        /// Creates a single instance of the ScreenManager Object
        /// </summary>
        /// <param name="game">The game being run</param>
        /// <returns></returns>
        public static ScreenManager CreateScreenManager(Game game)
        {
            if(screenManager==null)
            {
                screenManager=new ScreenManager(game);
            }

            return screenManager;
        }

        public static ScreenManager GetInstance()
        {
            ScreenManager result = null;
            if(screenManager!=null)
            {
                result = screenManager;
            }
        
            return result;
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 
        public override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            defaultEffect = new BasicEffect(GraphicsDevice);
            ShaderManager.CreateShaderManager(GraphicsDevice);
            graphics.ApplyChanges();

         

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = content.Load<SpriteFont>("Fonts/Font");


            screens.Add(new BackgroundScreen());
            
            screens.Add(new MainMenuScreen());

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
        public override void Update(GameTime gameTime)
        {
            // Allows the game to exit
           /* if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();*/

            if (DebugScreen.GetInstance().IsActive)
            {
                DebugScreen.GetInstance().HandleInput(gameTime, input);
                DebugScreen.GetInstance().Update(gameTime);
            }

         

            input.Update(this.Game.IsMouseVisible);



            tempScreensList.Clear();

            foreach (AbstractScreen screen in screens)
            {
                tempScreensList.Add(screen);
            }

            foreach (AbstractScreen screen in tempScreensList)
            {
                if (screen.HasFocus)
                {
                    screen.HandleInput(gameTime, input);
                }

                if (screen.IsActive)
                {
                    screen.Update(gameTime);
                }
            }


            //Any controls that are accessible to all screens will go here (i.e. toggle fullscreen)

            if (input.CurrentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F3) && !input.PreviousKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F3))
            {
                DebugScreen.GetInstance().IsVisible = !DebugScreen.GetInstance().IsVisible;
            }
          /*  if (input.CurrentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Back))
            {
                ToggleFullscreen();
                //graphics.ToggleFullScreen(); 
            }*/
            
        
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {



            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();          /*  spriteBatch.Begin(SpriteSortMode.Immediate,
                    BlendState.NonPremultiplied,
                    SamplerState.LinearClamp,
                    DepthStencilState.Default,
                    RasterizerState.CullCounterClockwise,
                    null, Matrix.Identity);*/

           

            //NOTE: Should create and loop through tempScreenList here in case a screen is added or removed while drawing
            foreach (AbstractScreen screen in screens)
            {
                if (screen.IsVisible)
                {
                    screen.Draw(spriteBatch);
                }
            }

            if (DebugScreen.GetInstance().IsVisible)
            {
                DebugScreen.GetInstance().Draw(spriteBatch);
            }
           
            spriteBatch.End();

            base.Draw(gameTime);
        }


#region EventHandlers

        public void LoadComplete(object sender, EventArgs e)
        {
            List<AbstractScreen> tempScreens = new List<AbstractScreen>();
            for (int i = 0; i < Screens.Count; i++)
            {
                tempScreens.Add(Screens[i]);
            }

            foreach (AbstractScreen screen in tempScreens)
            {
                if (screen is LoadingScreen)
                {
                    removeScreen(screen);
                }
            }

        }

        public void GameExiting(object sender, EventArgs e)
        {
            //Code to run when game is exiting

           /* foreach (Thread t in activeThreads)
            {
                //Probably need safer way to stop threads
                t.Abort();
            }*/
        }
        

   /*     public void PlayGame(object sender, EventArgs e)
        {
            //todo: play game
            foreach (GameScreen s in screens)
            {
                s.IsActive = false;
                s.HasFocus = false;
            }

            screens.Clear();

              if(!screens.Contains(gameplayScreen))
              {
                  screens.Add(gameplayScreen);
                  gameplayScreen.HasFocus = true;
                  gameplayScreen.IsActive = true;
              }
              

        }

        public void ExitGame(object sender, EventArgs e)
        {
            this.Game.Exit();
        }

        public void PauseGame(object sender, EventArgs e)
        {


            foreach (GameScreen s in screens)
            {
                s.IsActive = false;
                s.HasFocus = false;
            }

              if (!screens.Contains(pauseScreen))
              {
                  screens.Add(pauseScreen);
                  pauseScreen.HasFocus = true;
                  pauseScreen.IsActive = true;
              }
        }

        public void UnpauseGame(object sender, EventArgs e)
        {

               if (screens.Contains(pauseScreen))
               {
                   screens.Remove(pauseScreen);
                   pauseScreen.HasFocus = false;
                   pauseScreen.IsActive = false;
               }

            foreach (GameScreen s in screens)
            {
                //Note: Need more control over setting which screens are active/have focus onunpause
                s.IsActive = true;
                s.HasFocus = true;
            }
        }

        public void OpenMenu(object sender, EventArgs e)
        {



            foreach (GameScreen s in screens)
            {
                s.IsActive = false;
                s.HasFocus = false;
            }

            screens.Clear();

              if (!screens.Contains(titleScreen))
               {
                   screens.Add(titleScreen);
                   titleScreen.HasFocus = true;
                   titleScreen.IsActive = true;
               }
        }
        */

#endregion


        public bool registerThread(Thread thread)
        {
            bool result = false;
            if (!activeThreads.Contains(thread))
            {
                activeThreads.Add(thread);
                result = true;
            }

            return result;
        }

        public bool UnregisterThread(Thread thread)
        {
            bool result = false;
            if (activeThreads.Contains(thread))
            {
                activeThreads.Remove(thread);
                result = true;
            }

            return result;
        }

        public void addScreen(AbstractScreen screen)
        {
            screens.Add(screen);
        }

        public void removeScreen(AbstractScreen screen)
        {
            if(screens.Contains(screen))
            {
                screens.Remove(screen);
            }
        }

        public void ToggleFullscreen()
        {
        /*    graphics.ToggleFullScreen();
         //   graphics.IsFullScreen = !isFullscreen;
            graphics.ApplyChanges();

            isFullscreen = graphics.IsFullScreen;*/

            if(!isFullscreen)
            {
                gameForm.FormBorderStyle = FormBorderStyle.None;
                graphics.PreferredBackBufferWidth = 1920; //GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;//1280;
                graphics.PreferredBackBufferHeight = 1080;// GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;//720;
                graphics.ApplyChanges();
                gameForm.Left = 0;
                gameForm.Top = 0;
                isFullscreen = !isFullscreen;
            }
            else
            {
               
                gameForm.FormBorderStyle = FormBorderStyle.FixedSingle;
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 720;
                graphics.ApplyChanges();
                gameForm.Left = 0;
                gameForm.Top = 0;
                isFullscreen = !isFullscreen;
            }

        }

#region Accessors


        public List<AbstractScreen> Screens
        {
            get
            {
                return screens;
            }
        }

        public SpriteBatch SpriteBatch
        {
            get
            {
                return spriteBatch;
            }
        }

        public bool IsFullscreen
        {
            get
            {
                return isFullscreen;
            }
        }


        public ContentManager ContentManager
        {
            get
            {
                return content;
            }
        }

        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get
            {
                return graphics;
            }
        }

        public SpriteFont DefaultMenuFont
        {
            get
            {
                return font;
            }
        }

        public BasicEffect DefaultEffect
        {
            get
            {
                //NOTE: May reset effect to default settings whenever it is requested
                return defaultEffect;
            }
        }
#endregion
     }
}

