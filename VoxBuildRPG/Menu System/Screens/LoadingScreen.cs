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
    public class LoadingScreen: AbstractScreen
    {
        //Note: Need to make mouse invisible in loading screen

        private Texture2D LoadingScreenBackground;
        protected SpriteFont font;
        private string loadingText="Loading";
        private static string loadingDetailText = "";
        Vector2 textOrigin;
        private int loadingTimer = 0,timerDirection=1;

         public LoadingScreen()
            : base()
        {
           
            //Load the background texture for the screen


                LoadingScreenBackground = null;
                font = ScreenManager.GetInstance().ContentManager.Load<SpriteFont>("Fonts\\SimulatorFont");

           

            textOrigin = font.MeasureString(loadingText) / 2;
        }


        public override void Update(GameTime theTime)
        {
           
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {

        }

        public override void Draw(SpriteBatch Batch)
        {
            ScreenManager.GetInstance().Game.IsMouseVisible = false;

            if (loadingTimer <= 0)
            {
                timerDirection = 1;
            }

            else if (loadingTimer >= 60)
            {
                timerDirection = -1;
            }

            loadingTimer+=1*timerDirection;

            int screenWidth = ScreenManager.GetInstance().GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = ScreenManager.GetInstance().GraphicsDevice.PresentationParameters.BackBufferHeight;

            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);

            if (loadingTimer % 20 == 0)
            {
                if (timerDirection == 1)
                {
                    loadingText += ".";
                }
                else
                {
                    loadingText=loadingText.Remove(loadingText.Length-1);
                }
            }

            if (LoadingScreenBackground != null)
            {
                Batch.Draw(LoadingScreenBackground, screenRectangle, Color.White);
            }

            Vector2 textPosition= new Vector2(screenWidth/2,screenHeight/2);
             
            Vector2 textScale = new Vector2(1,1);
          //  textOrigin = font.MeasureString(loadingText) / 2;


            Batch.DrawString(font, loadingText, textPosition, Color.White, 0,
                                   textOrigin, textScale, SpriteEffects.None, 0);

            textPosition.Y += 30;
            textScale = new Vector2(0.8f, 0.8f);

            Vector2 loadingTextOrigin = font.MeasureString(loadingDetailText) / 2;
            Batch.DrawString(font, loadingDetailText, textPosition, Color.White, 0,
                                   loadingTextOrigin, textScale, SpriteEffects.None, 0);
          //  base.Draw(Batch);
        }


        public static void SetLoadingDetailText(string text)
        {
            loadingDetailText = text;
        }
    
    }
}
