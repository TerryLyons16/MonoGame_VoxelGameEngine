using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using VoxelRPGGame.MenuSystem.Screens;
using VoxelRPGGame.MenuSystem;
using VoxelRPGGame.GameEngine.UI;
using VoxelRPGGame.GameEngine.EnvironmentState;
using VoxBuildRPG.GameEngine.Rendering;
using VoxelRPGGame.MenuSystem.MenuElements;

namespace VoxelRPGGame.GameEngine.UI
{
    /// <summary>
    /// Class is passed the game state by the model,
    /// and renders all components in the state to be visualised
    /// </summary>
    public class GameplayScreen
    {


#region Event Handlers
        public delegate void ObjectCreated(AbstractDrawableGameObject gameObject);
        public event ObjectCreated OnCreateDrawableObject;

        public delegate void ObjectDestroyed(AbstractDrawableGameObject gameObject);
        public event ObjectDestroyed OnDestroyDrawableObject;


        public event EventHandler<EventArgs> Pause;
        public event EventHandler<EventArgs> Play;


#endregion

        protected bool hasFocus = false; //Denotes whether the screen has focus and can be allowed to handle input
        protected bool isActive = false; //Denotes whether the screen is active and can be allowed to update
        protected bool isVisible = false; //Denotes whether the screen is visible and can be drawn

        private GameHUDScreen _hudScreen;
        private Renderer  _renderer;
        private GameState _gameState;
      //  private GameWorldScreen worldScreen;


    

        private bool isOptionsMenuOpen = false;
        private bool isPaused =false;

       // private bool useRenderer;

        public GameplayScreen()
        {
           // this.useRenderer = true;
        //    worldScreen = new GameWorldScreen();
            _hudScreen = GameHUDScreen.GetInstance(); //new GameHUDScreen();
            _gameState = new GameState(null, null, null);
            _renderer = new Renderer(ScreenManager.GetInstance().GraphicsDevice);
        
            _hudScreen.InitializeElements();

            _hudScreen.IsVisible = false;
        }

       // public GameplayScreen(GameHUDScreen hud/*, GameWorldScreen world, bool useRenderer*/)
       // {
       //  //   this.useRenderer = useRenderer;
       //        _gameState = new GameState(null, null, null);
       //     _hudScreen = hud;
       //     _renderer = new Renderer(ScreenManager.GetInstance().GraphicsDevice);
       //   /*  if (useRenderer)
       //     {
       //         worldScreen = world;
       //     }*/
       // }

        public void Initialise()
        {

            //   worldScreen.Initialise(_gameState);

        }

        /// <summary>
        /// Updates the HUD and game state
        /// </summary>
        /// <param name="theTime"></param>
        public void Update(GameTime theTime)
        {
            if (isActive)
            {

                _gameState.Update();

                if (_hudScreen.IsActive)
                {
                    _hudScreen.Update(theTime, _gameState);
                }
            }
            //if (useRenderer)
            //{
            //    if (worldScreen.IsActive)
            //    {
            //        worldScreen.Update(theTime, _gameState);
            //    }
            //}
           
        }

        /// <summary>
        /// Handles input for the gamestate and the HUD.
        /// Updates the ControlState (NOTE: all input will be handled by this in future).
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="input"></param>
        public void HandleInput(GameTime gameTime, InputState input)
        {
          //  ScreenManager.GetInstance().Game.IsMouseVisible = true;
            GameWorldControlState.GetInstance().HandleInput(input);

            if (input.CurrentKeyboardState.IsKeyDown(Keys.Escape) && input.PreviousKeyboardState.IsKeyUp(Keys.Escape))
            {
                // Pause(this, new EventArgs());
                if (!isOptionsMenuOpen)
                {
                    OpenOptionsMenu();
                }
                else
                {
                    PlayGame(null);
                }

            }
            if (!isOptionsMenuOpen)
            {
                if (input.CurrentKeyboardState.IsKeyDown(Keys.P) && input.PreviousKeyboardState.IsKeyUp(Keys.P))
                {
                    if (!isPaused)
                    {
                        PauseGame(this, new EventArgs());
                        isPaused = true;
                    }
                    else
                    {
                        PlayGame(null);
                        isPaused = false;
                    }
                }
            }
            
                //Handle Input for its screens
                //NOTE: Will need to determine which element has focus
            if (hasFocus)
            {
                _gameState.HandleInput(gameTime, input);

                if (_hudScreen.HasFocus)
                {
                    _hudScreen.HandleInput(gameTime, input, _gameState);
                }
               
                    //NOTE: Currently, handling input for UI and world is mutually exclusive. This is incorrect. i.e. can be clicking on UI while moving in 3D
                //Mouse input can only be applied to one at a time, but both should be able to handle input overall
               /* if (useRenderer&&worldScreen.HasFocus)
                {
                    worldScreen.HandleInput(gameTime, input, _gameState);
                }*/
            }

        }


        /// <summary>
        /// Draws the HUD and sends the current game state to the renderer
        /// </summary>
        /// <param name="Batch"></param>
        public void Draw(SpriteBatch Batch)
        {

            if (isVisible)
            {
                //NOTE: Will need to split drawing into DrawRenderTargets and DrawObjects...

                /*  if (useRenderer&&worldScreen != null && worldScreen.IsVisible)
                  {
                      worldScreen.Draw(Batch, _gameState);
            
                  }*/

                _renderer.Render(Batch, _gameState.ActiveCamera,_gameState.GetRenderState());

                if (_hudScreen != null && _hudScreen.IsVisible)
                {
                    _hudScreen.Draw(Batch, _gameState);
                }
            }
        }


        #region Event Handlers


        public void OpenOptionsMenu()
        {
            PauseGame(this, new EventArgs());
            isOptionsMenuOpen = true;

            PauseScreen PauseScreen = new PauseScreen(PlayGame);

            ScreenManager.GetInstance().addScreen(PauseScreen);
            hasFocus = false;
        }

  
        public void PlayGame(MenuElement element)
        {
            foreach (AbstractScreen s in ScreenManager.GetInstance().Screens)
            {
                if (s is PauseScreen)
                {
                    ScreenManager.GetInstance().removeScreen(s);
                    break;
                }

            }
            
            hasFocus = true;


            if (isOptionsMenuOpen)
            {
                if (!isPaused)
                {
                    Play(this, new EventArgs());
                }
            }
            else
            {
                Play(this, new EventArgs());
            }


            isOptionsMenuOpen = false;
        }


        public void LoadComplete()
        {
            IsActive = true;
            HasFocus = true;
            isVisible = true;
            _gameState.LoadComplete();
            OnPlay();
        }

        public void OnPause()
        {
            IsActive = false;
        }


        public void OnPlay()
        {
            IsActive = true;
        }

        public void PauseGame(object sender, EventArgs e)
        {
            //NOTE: Will need to fire a Pause event up to the engine
            Pause(this, new EventArgs());
           
        }



#endregion

        public void SetScreensActive()
        {
            isActive = true;
            hasFocus = true;
            _hudScreen.IsActive = true;
        }

#region Properties
        public bool HasFocus
        {
            get
            {
                return hasFocus;
            }

            set
            {
                hasFocus = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return isActive;
            }

            set
            {
                isActive = value;
            }

        }

        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                isVisible = value;
            }
        }


#endregion
    }
}
