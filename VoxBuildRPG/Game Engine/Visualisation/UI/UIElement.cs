using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.UI;
using Microsoft.Xna.Framework;
using VoxelRPGGame.GameEngine.EnvironmentState;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelRPGGame.GameEngine.UI
{
    public abstract class UIElement : AbstractHUDElement
    {
        public delegate void MoveToFrontRequest(UIElement element);
        public event MoveToFrontRequest MoveToFrontRequestEvent; 

        protected UIElement _owner;



        protected Vector2 _positionRelative;//position relative to whatever UIElement it is in
        protected Vector2 _positionAbsolute;//Position on screen it is drawn at


        public abstract Vector2 Position
        {
            get;

        }

        public abstract float Width
        {
            get;

        }
        public abstract float Height
        {
            get;

        }
        public override void Update(GameTime theTime, GameState state)
        {
        }

        //  public virtual void HandleInput(GameTime gameTime, InputState input) { }

        //Handles user input. This is separate to Update, as a screen can still be updated even if it cannot handle input  
        public override void HandleInput(GameTime gameTime, InputState input, GameState state)
        { 
        }


        public override void Draw(SpriteBatch Batch, GameState state)
        {
        }


        public UIElement Owner
        {
            get
            {
                return _owner;
            }
        }


        #region Events

        public void RequestMoveToFront()
        {
            if(MoveToFrontRequestEvent!=null)
            {
                MoveToFrontRequestEvent(this);
            }
        }

        #endregion
    }
}
