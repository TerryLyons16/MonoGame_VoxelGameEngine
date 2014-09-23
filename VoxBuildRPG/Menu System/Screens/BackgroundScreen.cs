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

namespace VoxelRPGGame.MenuSystem.Screens
{
    public class BackgroundScreen: AbstractScreen
    {
        Texture2D BackgroundImage;

        public BackgroundScreen()
        {
            //Load the background texture for the screen
            //BackgroundImage = Content.Load<Texture2D>("BackgroundScreen");

                BackgroundImage = null;

        }


        public override void Update(GameTime theTime)
        {
           
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            
        }

        public override void Draw(SpriteBatch Batch)
        {

            int screenWidth = ScreenManager.GetInstance().GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = ScreenManager.GetInstance().GraphicsDevice.PresentationParameters.BackBufferHeight;

            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);

            if (BackgroundImage != null)
            {
                Batch.Draw(BackgroundImage, screenRectangle, Color.White);
            }
           
        }
    }
}
