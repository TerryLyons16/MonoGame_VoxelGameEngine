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
    public class TextElement : MenuElement
    { 
        //protected Texture2D ButtonIdle, ButtonClicked, ButtonHover,currentButton;
   //     protected string elementText="";

        protected enum TextAlign
        {
            Centre,
            Left
        }

        protected TextAlign textAlignment;

        private int clickedTimer=0, clickedCountdown=5;//Countdown is number of frames button is held down for
        bool buttonClicked = false;


        private SpriteFont _idleFont,_highlightedFont;

        protected Vector2 textScale = new Vector2(1, 1);

        private float alpha = 0.0f,defaultFadeAmount=0.01f,currentFadeAmount=0;//Fade amount determines how much Text fades per frame


        public TextElement(string text)
        {
            textAlignment = TextAlign.Centre;


            alpha = 1;
            _idleFont =  ScreenManager.GetInstance().ContentManager.Load<SpriteFont>("Fonts\\SimulatorFont");
            _highlightedFont = ScreenManager.GetInstance().ContentManager.Load<SpriteFont>("Fonts\\SimulatorFont");
            textFont = _idleFont; 
            elementText = text;
            isActive = false;
            boundingBox = new Rectangle((int)position.X, (int)position.Y, (int)textFont.MeasureString(elementText).X, (int)textFont.MeasureString(elementText).Y);
        }

        public TextElement(string text, SpriteFont idleFont, SpriteFont highlightedFont)
        {
           _idleFont = idleFont;
            textFont = idleFont;
            _highlightedFont = highlightedFont;

            textAlignment = TextAlign.Centre;


            alpha = 1;
           
            textFont = idleFont;
            elementText = text;
            isActive = false;
            boundingBox = new Rectangle((int)position.X, (int)position.Y, (int)textFont.MeasureString(elementText).X, (int)textFont.MeasureString(elementText).Y);
        }

        public TextElement(string text,string idleFontName, string highlightedFontName)
        {

            _idleFont = ScreenManager.GetInstance().ContentManager.Load<SpriteFont>(idleFontName);
            _highlightedFont = ScreenManager.GetInstance().ContentManager.Load<SpriteFont>(highlightedFontName);
            textFont = _idleFont;
            elementText = text;
            isActive = false;
            boundingBox = new Rectangle((int)position.X, (int)position.Y, (int)textFont.MeasureString(elementText).X, (int)textFont.MeasureString(elementText).Y);
        }

      /*  public TextElement(ref string text):this(ref text)
        {
            elementText=text;
        }
        */

        public override void Update(GameTime theTime)
        {
            if (alpha >= 0 && alpha < 1)
            {
                alpha += currentFadeAmount;
                //textScale.X += currentFadeAmount;
               // textScale.Y += currentFadeAmount;
            }

            else
            {
                StopFade();
            }

            if (alpha > 0.5)
            {
                //Allow input if element is mostly visible
                hasFocus = true;
            }

  

            if (buttonClicked && clickedTimer == 0)
            {
                OnElementClick();
                buttonClicked = false;
               // currentButton = ButtonIdle;
            }

            if (!buttonClicked&&isHighlighted)
            {
                textColor = Color.White;
                textFont = _highlightedFont;
               // SetComponentScale(1.2f, 1.2f);
            }

            else if (!buttonClicked && !isHighlighted)
            {
                textColor = Color.White;
                textFont = _idleFont;
               // SetComponentScale(1f, 1f);
            }
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {

            if (((input.CurrentMouseState.LeftButton == ButtonState.Pressed && input.PreviousMouseState.LeftButton == ButtonState.Released) && boundingBox.Contains(new Point(input.CurrentMouseState.X, input.CurrentMouseState.Y)))||(isHighlighted&&input.CurrentKeyboardState.IsKeyDown(Keys.Enter)))
            {
                clickedTimer = clickedCountdown;
                buttonClicked = true;
            }

            else if (clickedTimer == 0 && boundingBox.Contains(new Point(input.CurrentMouseState.X, input.CurrentMouseState.Y)) && input.IsMouseVisible)//Mouse Hover
            {
                isHighlighted = true;
                mouseOver = true;
               // textColor = Color.Gold;
            }

            else if (!input.IsMouseVisible)
            {
                mouseOver = false;
                isHighlighted = false;
            }

            else if (clickedTimer == 0 && !boundingBox.Contains(new Point(input.CurrentMouseState.X, input.CurrentMouseState.Y)))//Mouse outside element
            {
                isHighlighted = false;
              //  textColor = Color.White;
                clickedTimer = 0;
                mouseOver = false;
             //   buttonClicked = false;
            }

        }

        public override void Draw(SpriteBatch Batch)
        {
            //FadeIn();
         //   boundingBox.Width = (int)textFont.MeasureString(elementText).X;

            if(clickedTimer>0)
            {
                clickedTimer--;
            }


            //Batch.Draw(currentButton, boundingBox, Color.Silver);


            if (elementText != "")
            {
                Vector2 textOrigin;
                //Vector2 textScale = new Vector2(1f, 1);
                Vector2 textPosition;
                switch (textAlignment)
                {
                    case TextAlign.Centre:
                        {
                            textOrigin = new Vector2(textFont.MeasureString(elementText).X / 2, textFont.MeasureString(elementText).Y / 2.5f);
                            textPosition = new Vector2(boundingBox.X + (boundingBox.Width / 2), boundingBox.Y + (boundingBox.Height / 2));
                            break;
                        }
                    case TextAlign.Left:
                        {
                            textOrigin = new Vector2(0, textFont.MeasureString(elementText).Y / 2.5f);
                            textPosition = new Vector2(boundingBox.X, boundingBox.Y);
                            break;
                        }
                    default:
                        {
                            textOrigin = Vector2.Zero;
                            textPosition = Vector2.Zero;
                            break;
                        }
                }


                Batch.DrawString(textFont, elementText, textPosition, textColor * alpha, 0,
                           textOrigin, textScale, SpriteEffects.None, 0);
            }

        }

        public override void SetComponentScale(float scaleX, float scaleY)
        {

            boundingBox = new Rectangle((int)boundingBox.X, (int)boundingBox.Y, (int)(textFont.MeasureString(elementText).X*scaleX), (int)(textFont.MeasureString(elementText).Y*scaleY));

           // boundingBox.Width = (int)(boundingBox.Width * scaleX);
           // boundingBox.Height = (int)(boundingBox.Height * scaleY);

            textScale = new Vector2(scaleX, scaleY);
        }

        public override void setHighlighted()
        {
            if (!buttonClicked)
            {
             //   textColor = Color.Gold;
                isHighlighted = true;
            }
        }


#region Effects
        public void FadeIn()
        {
            currentFadeAmount = Math.Abs(defaultFadeAmount);

        }

        public void FadeOut()
        {
            currentFadeAmount = -Math.Abs(defaultFadeAmount);
            hasFocus = false;
        }

        public void StopFade()
        {
            currentFadeAmount = 0;
            if (alpha > 1)
            {
                alpha = 1;
            }

            else if (alpha < 0)
            {
                alpha = 0;
            }

            if (alpha == 1)
            {
                hasFocus = true;
            }

            else
            {
                hasFocus = false;
            }
        }




#endregion
#region Text Alignment


        public void AlignTextCentre()
        {
            textAlignment = TextAlign.Centre;
        }

        public void AlignTextLeft()
        {
            textAlignment = TextAlign.Left;
        }



#endregion
        #region Accessors
        public float Alpha
        {
            set
            {
                alpha = value;
            }
        }




        #endregion
    }
}
