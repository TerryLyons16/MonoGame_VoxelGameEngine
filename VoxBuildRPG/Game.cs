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

using VoxelRPGGame.MenuSystem;

namespace VoxelRPGGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
   //     SpriteBatch spriteBatch;

  //      GameScreen currentScreen;
  //      TitleScreen titleScreen;
  //      GameplayScreen gameplayScreen;
        ScreenManager screenManager;
     

        public Game()
        {
          //  graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 720;

          /*  graphics.PreferredBackBufferWidth = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.75);
            graphics.PreferredBackBufferHeight = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.75);


            graphics.PreferMultiSampling = true;*/

            screenManager = ScreenManager.CreateScreenManager(this);
            Components.Add(screenManager);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            screenManager.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
          /*  spriteBatch = new SpriteBatch(GraphicsDevice);
            
            titleScreen = new TitleScreen(Content);
            titleScreen.Exit += ExitGame;
            titleScreen.Play += PlayGame;

            gameplayScreen = new GameplayScreen(Content,GraphicsDevice);
            gameplayScreen.Exit += ExitGame;
            gameplayScreen.ToMenu += OpenMenu;

            currentScreen = titleScreen;*/
            

            // TODO: use this.Content to load your game content here
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
       /* protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
           /* if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();*/

            // TODO: Add your update logic here
       //     currentScreen.Update(gameTime);

      /*      base.Update(gameTime);
        }
        */
       /* public void PlayGame(object sender, EventArgs e)
        {
            //todo: play game
            currentScreen = gameplayScreen;
        }

        public void ExitGame(object sender, EventArgs e)
        {
            this.Exit();
        }

        public void OpenMenu(object sender, EventArgs e)
        {
            currentScreen = titleScreen;
        }
        */
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);

           /* spriteBatch.Begin();
            currentScreen.Draw(spriteBatch);
            spriteBatch.End();
            */
            base.Draw(gameTime);
        }
    }
}
