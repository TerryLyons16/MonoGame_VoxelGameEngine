using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace VoxelRPGGame.GameEngine.UI
{
    /// <summary>
    /// Any drawable object of the engine, including Menus, menu elements, and the visualisation of engine components 
    /// </summary>
    public abstract class AbstractDrawableGameObject
    {

        protected bool isDrawable = true;
        protected bool hasFocus = true;

        /// <summary>
        /// NOTE: If owned by a gameComponent, the component will set the Drawable object's state
        /// NOTE: This update is reserved for graphical update only, and should be called by the appropriate
        /// render system.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// NOTE: Only for user graphical input.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="input"></param>
        public abstract void HandleInput(GameTime gameTime, InputState input);

        public abstract void Draw(SpriteBatch Batch);


        public bool IsDrawable
        {
            get
            {
                return isDrawable;
            }

            set
            {
                isDrawable = value;
            }
        }

        public bool HasFocus
        {
            get
            {
                return hasFocus;
            }
        }
    }
}
