using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace VoxelRPGGame.MenuSystem.Screens
{
    public class TitleScreen: MenuScreen
    {
        Texture2D mTitleScreenBackground;

        public event EventHandler<EventArgs> Exit;
        public event EventHandler<EventArgs> Play;
        public event EventHandler<EventArgs> Pause;
       


        public TitleScreen()
            : base("Splash Screen")
        {
            //Load the background texture for the screen
            mTitleScreenBackground = ScreenManager.GetInstance().ContentManager.Load<Texture2D>("SplashScreen");
            
        }


        public override void Update(GameTime theTime)
        {
           
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input.CurrentKeyboardState.IsKeyDown(Keys.P)&&input.PreviousKeyboardState.IsKeyUp(Keys.P))
            {
                Pause(this, new EventArgs());
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.Enter))
            {
                Play(this, new EventArgs());
            }

            else if (input.CurrentKeyboardState.IsKeyDown(Keys.Escape))
            {
                Exit(this, new EventArgs());
            }
        }

        public override void Draw(SpriteBatch Batch)
        {

            int screenWidth = ScreenManager.GetInstance().GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = ScreenManager.GetInstance().GraphicsDevice.PresentationParameters.BackBufferHeight;

            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);

            Batch.Draw(mTitleScreenBackground, screenRectangle, Color.White);
            base.Draw(Batch);
        }
    }
}
