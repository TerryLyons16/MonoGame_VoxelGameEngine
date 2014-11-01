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
    public class MenuElement
    {
        public delegate void OnClick(MenuElement element);
        public event OnClick OnClickEvent; 

        protected Vector2 position;
        protected bool hasFocus = true;
        protected bool isActive = true;

        protected Rectangle boundingBox;
        protected SpriteFont textFont;

        protected Color textColor = Color.White;
        protected float alpha = 1.0f;

        protected string elementText = "";

        protected bool isHighlighted = false;
        protected bool mouseOver = false; //true if the mouse is over the element

      //  public event EventHandler<EventArgs> Highlighted;
    //    public event EventHandler<EventArgs> UnHighlighted;

        public MenuElement()
        {
            
        }

        public virtual void Update(GameTime theTime)
        {
        }

        //Handles user input. This is separate to Update, as a screen can still be updated even if it cannot handle input  
        public virtual void HandleInput(GameTime gameTime, InputState input)
        {
        }
        
        public virtual void Draw(SpriteBatch Batch)
        {
        }

        protected void OnElementClick()
        {
            if(OnClickEvent!=null)
            {
                OnClickEvent(this);
            }
         
        }

     /*   protected void ElementHighlighted(object sender, EventArgs e)
        {
            Highlighted(sender, e);
        }*/

      /*  protected void ElementUnHighlighted(object sender, EventArgs e)
        {
            UnHighlighted(sender, e);
        }*/

        public virtual void SetComponentScale(float scaleX, float scaleY)
        {
           
        }

        public virtual void setHighlighted()
        {
        }

        public virtual void setIdle()
        {
        }

#region Element Effects
        public virtual void HoverScale()
        {

        }

        public virtual void HoverIndent()
        {
        }

        public virtual void IdleResetScale()
        {
        }
#endregion
#region Accessors

        public Vector2 Position
        {
            set
            {
                position = value;
                boundingBox.X = (int)position.X-boundingBox.Width/2;
                boundingBox.Y = (int)position.Y;
                SetDetectionBoxPosition();
            }

            get
            {
                return position;
            }
        }

        public bool IsActive
        {
            set
            {
                isActive = value;
            }

            get
            {
                return isActive;
            }
        }

        public bool HasFocus
        {
            set
            {
                hasFocus = value;
            }

            get
            {
                return hasFocus;
            }
        }

        public Rectangle BoundingBox
        {
            set
            {
                boundingBox = value;
            }

            get
            {
                return boundingBox;
            }
        }

        public bool IsHighlighted
        {
            get
            {
                return isHighlighted;
            }

            set
            {
                isHighlighted = value;
            }
        }

        public bool MouseOver
        {
            get
            {
                return mouseOver;
            }

            set
            {
                mouseOver = value;
            }
        }

        public SpriteFont Font
        {
            get
            {
                return textFont;
            }

            set
            {
                textFont = value;
            }
        }

        public Color TextColour
        {
            get
            {
                return textColor;
            }

            set
            {
                textColor = value;
            }
        }

        public string ElementText
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
        }

        public void UpdateBoundingBox()
        {
            boundingBox.Width = (int)textFont.MeasureString(elementText).X;
            boundingBox.Height = (int)textFont.MeasureString(elementText).Y;
        }

        public virtual void SetDetectionBoxPosition()
        {

        }

#endregion
    }
}
