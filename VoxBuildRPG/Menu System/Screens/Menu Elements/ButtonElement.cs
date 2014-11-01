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

namespace VoxelRPGGame.MenuSystem.MenuElements
{
    public class ButtonElement: MenuElement
    {
        protected Texture2D ButtonIdle, ButtonClicked, ButtonHover,currentButton;
        
        protected int buttonPressedOffset = 0;

        private int clickedTimer=0, clickedCountdown=5;//Countdown is number of frames button is held down for
        bool buttonClicked = false;

        private bool TEMP_scaleSet = false;

        private Rectangle detectionBox;//Used for mouse detection

        public ButtonElement()
        {
  

            ButtonIdle =  ScreenManager.GetInstance().ContentManager.Load<Texture2D>(ScreenManager.buttonIcons[0]);//Content.Load<Texture2D>("Images\\Menus\\Menu Elements\\DefaultIdleButton");
            ButtonClicked = ScreenManager.GetInstance().ContentManager.Load<Texture2D>(ScreenManager.buttonIcons[1]); //Content.Load<Texture2D>("Images\\Menus\\Menu Elements\\DefaultClickedButton");
            ButtonHover = ScreenManager.GetInstance().ContentManager.Load<Texture2D>(ScreenManager.buttonIcons[2]);//Content.Load<Texture2D>("Images\\Menus\\Menu Elements\\DefaultHoverButton");
            currentButton = ButtonIdle;

           // textFont = content.Load<SpriteFont>("Fonts\\MenuFont");

            boundingBox = new Rectangle((int)position.X, (int)position.Y, ButtonIdle.Width, ButtonIdle.Height);
            detectionBox=new Rectangle((int)position.X, (int)position.Y, ButtonIdle.Width, ButtonIdle.Height);
        }

        public ButtonElement( string idleButtonLocation)
        {
            ButtonIdle = ScreenManager.GetInstance().ContentManager.Load<Texture2D>(idleButtonLocation);
        }



        public override void Update(GameTime theTime)
        {

            if (buttonClicked && clickedTimer == 0)
            {

                buttonClicked = false;
                OnElementClick();

                //currentButton = ButtonIdle;
              
              //  isHighlighted = false;

             //   buttonPressedOffset = 0;
                
              
            }

            if (/*!buttonClicked &&*/ isHighlighted)
            {

               HoverIndent();
                currentButton = ButtonHover;
            }

            else if(!mouseOver && !isHighlighted)
            {
                IdleResetScale();
                currentButton = ButtonIdle;
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            //Button Clicked
            if (((input.CurrentMouseState.LeftButton == ButtonState.Pressed && input.PreviousMouseState.LeftButton == ButtonState.Released) && detectionBox.Contains(new Point(input.CurrentMouseState.X, input.CurrentMouseState.Y))) || (isHighlighted &&( input.CurrentKeyboardState.IsKeyDown(Keys.Enter)&& input.PreviousKeyboardState.IsKeyUp(Keys.Enter))))
            {
                currentButton = ButtonClicked;
                clickedTimer = clickedCountdown;
                buttonClicked = true;

              //  buttonPressedOffset = 2;
            }


            //Mouse is hovering over Button
            else if (input.CurrentMouseState.LeftButton == ButtonState.Released&&clickedTimer == 0 && detectionBox.Contains(new Point(input.CurrentMouseState.X, input.CurrentMouseState.Y)) && input.IsMouseVisible)
            {
              //  currentButton = ButtonHover;
                isHighlighted = true;
                mouseOver = true;
            }

            else if (!input.IsMouseVisible||!isHighlighted)
            {
                mouseOver = false;
                isHighlighted = false;
            }


            //Mouse is outside Button
            else if (clickedTimer == 0 && !detectionBox.Contains(new Point(input.CurrentMouseState.X, input.CurrentMouseState.Y)))
            {
                setIdle();
            }

        }

        public override void Draw(SpriteBatch Batch)
        {
            if(clickedTimer>0)
            {
                clickedTimer--;
            }

            Batch.Draw(currentButton, boundingBox,Color.White * alpha);


            if (elementText != "")
            {
                Vector2 textOrigin = new Vector2(Font.MeasureString(elementText).X / 2, Font.MeasureString(elementText).Y / 2.5f);
                Vector2 textScale =  new Vector2((float)boundingBox.Width/currentButton.Width, (float)boundingBox.Height/currentButton.Height);
                Vector2 textPosition = new Vector2(boundingBox.X+(boundingBox.Width/2),boundingBox.Y+(boundingBox.Height/2));


                Batch.DrawString(Font, elementText, new Vector2(textPosition.X+buttonPressedOffset,textPosition.Y+buttonPressedOffset), textColor, 0,
                           textOrigin, textScale, SpriteEffects.None, 0);
            }

        }

        public override void SetComponentScale(float scaleX, float scaleY)
        {
            boundingBox.Width = (int)(boundingBox.Width * scaleX);
            boundingBox.Height = (int)(boundingBox.Height * scaleY);
            detectionBox.Width = (int)(boundingBox.Width);
            detectionBox.Height = (int)(boundingBox.Height);

        }

        public override void SetDetectionBoxPosition()
        {
            detectionBox.X = boundingBox.X;
            detectionBox.Y = boundingBox.Y;
        }

        public override void setHighlighted()
        {
            if (!buttonClicked)
            {
               // currentButton = ButtonHover;
                isHighlighted = true;
            }
        }

        public override void setIdle()
        {
            clickedTimer = 0;
            isHighlighted = false;
            mouseOver = false;
        }

#region Element Effects
        public override void HoverScale()
        {
            if (!TEMP_scaleSet)
            {
              
                boundingBox.Width += 10;
                boundingBox.Height += 10;
                TEMP_scaleSet = true;
            }
        }
        public override void HoverIndent()
        {
            if (!TEMP_scaleSet)
            {

                boundingBox.X += 5*detectionBox.Width/ButtonIdle.Width;//TEMP way to scale indent: should store component scale and multiply by that
                boundingBox.Y += 5 * detectionBox.Height / ButtonIdle.Height;
                TEMP_scaleSet = true;
            }
        }

        public override void IdleResetScale()
        {
            if (TEMP_scaleSet)
            {
                boundingBox.X -= 5 * detectionBox.Width / ButtonIdle.Width; ;
                boundingBox.Y -= 5 *detectionBox.Height / ButtonIdle.Height;
             //   boundingBox.Width -= 10;
              //  boundingBox.Height -= 10;
                TEMP_scaleSet = false;
            }
        }

        #endregion

#region Accessors



        public string ButtonText
        {
            set
            {
                elementText = value;
            }
        }

        public float Alpha
        {
            set
            {
                alpha = value;
            }

            get
            {
                return alpha;
            }
        }

#endregion
    }
}
