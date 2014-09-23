using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;

using VoxelRPGGame.MenuSystem.MenuElements;
using VoxelRPGGame.GameEngine;
using VoxelRPGGame.GameEngine.UI;




namespace VoxelRPGGame.MenuSystem.Screens
{
    /// <summary>
    /// Part of the Menu system hierarchy, class handles the running of the simulation engine and rendering of the model
    /// </summary>
    

    public class GameScreen:AbstractScreen
    {
        private GameplayScreen gameplayScreen = null;
 

        //----For Loading-------------------------------
        Thread loadingThread;
        int loadDisplayTime=500;
        bool setManagerActive = false;//used to set the manager and all screens active when loading is complete
        //----------------------------------------------


        public GameScreen(bool useMapEditor)
        {
  
            IsVisible = false;
            isActive = false;
            HasFocus = false;
            loadingThread = new Thread(new ThreadStart(LoadEngine));
            loadingThread.IsBackground = true;//Thread will terminate if running when application is closed
            ScreenManager.GetInstance().registerThread(loadingThread);
            loadingThread.Start();
        }


        public override void Update(GameTime theTime)
        {

            //Call the Engine's Update
            //if (engine != null)
            //{
            //    engine.Update();


            //Renderer can only render if there is an engine...

            //NOTE: Should the renderer have an update?
            //Call the Game Renderer's Update
            if (gameplayScreen != null)
            {
                gameplayScreen.Update(theTime);
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (gameplayScreen != null)
            {
                gameplayScreen.HandleInput(gameTime, input);
            }

        }

        public override void Draw(SpriteBatch Batch)
        {
            //Call the Renderer's Draw

            //NOTE: The renderer will be in charge of rendering 2 screens: the 2D HUD and the 3D World
            //It will be passed the game state from the engine and decide how to draw each component
            //It will also be in charge of sending input from the visualisation to the engine

            if (gameplayScreen != null)
            {
                gameplayScreen.Draw(Batch);
            }

           
        }

        public void LoadEngine()
        {
            //    ModelMap map = new ModelMap();

            //Initialise Game Engine
            LoadingScreen.SetLoadingDetailText("Loading Engine");
            //  engine = new Engine();
            //   engine.OnExit += OnExit;
            //Load Model into the engine

            //if (!isMapEditor)
            //{
            //    engine.Initialise();
            //}
            //else
            //{
            // //   engine.InitialiseMapEditor(map);
            //}

            Thread.Sleep(loadDisplayTime);


            LoadingScreen.SetLoadingDetailText("Loading Renderer");
            //Initialise Graphics Renderer
            gameplayScreen = new GameplayScreen();
            //  renderer = map.LoadRenderer();
            gameplayScreen.Pause += OnPause;
            gameplayScreen.Play += OnPlay;
            gameplayScreen.Initialise();
            Thread.Sleep(loadDisplayTime);



            //Load graphics into the renderer


            //Put the thread to sleep so that anything still being initialised is ready when it exits and fires OnLoad event
            Thread.Sleep(loadDisplayTime);
            LoadingComplete(this, new EventArgs());
            // OnLoad(this, new EventArgs());

        }
        //{
        //    //------------Initialise Simulator---------------

        //    //workplaceManager = new WorkplaceManager(this);
        //    //  resourceManager = new ResourceManager(this);
        //    //agentManager = new AgentManager(this);

        //    //Envent handler to set agent skills when a skill is created
        //    // workplaceManager.OnSkillCreated += agentManager.SetAllAgentSkills;

        //    //    agentManager.OnAgentCreated += workplaceManager.SetSkillsForAgent;

        //    //-----------------------------------------------



        //    //-----------Create Gameplay Screen--------------


        //    //NOTE: All controllers present on load should be created during loading

        //    //NOTE: All entities present on load should be passed to the GameplayScreen before it starts loading

        //    GameplayScreen s = new GameplayScreen(content, graphics, screenManager, TEMP_type);
        //    s.Pause += PauseSimulation;
        //    s.Play += PlaySimulation;
        //    gameplayScreens.Add(s);
        //    currentGameplayScreenIndex = gameplayScreens.IndexOf(s);

        //    //---------------------------------------------------------------------------------------------------------

        //    //NOTE: might need to initialise managers here
        //    //agentManager.Initialise();
        //    //resourceManager.InitialiseResources();
        //    //  workplaceManager.InitialiseTasks();
        //    //---------------------------------------------------------------------------------------------------------

        //    //Load the game

        //    gameplayScreens[currentGameplayScreenIndex].Load3DWorld();

        //    //NOTE: Only using Update to ensure gameWorld has a valid gameState for other managers to use to load
        //    //Entities. Should do this in separate loading method for each screen/manager being initialised here
        //    Update(null);
        //    //-----------------------------------------------
        //    this.isVisible = true;


        //    //Add code to create the simulator and initial Gameplay Screen


        //    //NOTE: Initialisation of Task/Resource managers has been moved to onLoad, so as not to disrupt loading of
        //    //3D elements. Need to look into order/coordination of loading so that they may be correclty initialised here.


        //    ScreenManager.unregisterThread(loadingThread);//Unregister thread on termination
        //    //Put the thread to sleep so that anything still being initialised is ready when it exits and fires OnLoad event
        //    Thread.Sleep(500);
        //    LoadingComplete(this, new EventArgs());
        //    // OnLoad(this, new EventArgs());
        //}


        public void LoadingComplete(object sender, EventArgs e)
        {
            ScreenManager.GetInstance().UnregisterThread(loadingThread);//Unregister thread on termination


            ScreenManager.GetInstance().LoadComplete(this, new EventArgs());
            IsVisible = true;
            HasFocus = true;
            isActive = true;
            gameplayScreen.LoadComplete();
          /*  if (gameplayScreen != null)
            {
                gameplayScreen.SetScreensActive();
            }*/
        }
       


#region Event Handlers
        public void OnPause(object sender, EventArgs e)
        {
            //engine.OnPause();
            gameplayScreen.OnPause();
        }

        public void OnPlay(object sender, EventArgs e)
        {
           // engine.OnPlay();
            gameplayScreen.OnPlay();
        }


        public void OnExit()
        {
          
                ScreenManager.GetInstance().Screens.Clear();

                ScreenManager.GetInstance().addScreen(new BackgroundScreen());
                ScreenManager.GetInstance().addScreen(new MainMenuScreen());

        }

#endregion
    }
}
