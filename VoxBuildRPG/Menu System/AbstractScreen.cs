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
    public abstract class AbstractScreen
    {
        protected bool hasFocus = false; //Denotes whether the screen has focus and can be allowed to handle input
        protected bool isActive = false; //Denotes whether the screen is active and can be allowed to update
        protected bool isVisible = true; //Denotes whether the screen is visible and can be drawn


        public abstract void Update(GameTime theTime);

      //  public virtual void HandleInput(GameTime gameTime, InputState input) { }

        //Handles user input. This is separate to Update, as a screen can still be updated even if it cannot handle input  
        public abstract void HandleInput(GameTime gameTime, InputState input);
 

        public abstract void Draw(SpriteBatch Batch);
 


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
