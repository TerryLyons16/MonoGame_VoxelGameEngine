using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.EnvironmentState;
using VoxelRPGGame.MenuSystem;
using VoxelRPGGame.MenuSystem.MenuElements;

namespace VoxelRPGGame.GameEngine.UI.UIComponents
{
    public class QuantitySelector:UIElement
    {
        public delegate void OnQuantityChanged(int quantity);
        public event OnQuantityChanged OnQuantityChangedEvent; 


        protected int _quantity,_minQuantity,_maxQuantity;
        protected bool _hasMinQuantity = false, _hasMaxQuantity = false;

        protected MenuElement _increaseQuantity;
        protected MenuElement _decreaseQuantity;

#region Properties

        public int MinQuantity
        {
            get
            {
                return _minQuantity;
            }

            set
            {
                _hasMinQuantity = true;
                _minQuantity = value;
                if (_minQuantity > _quantity)
                {
                    _quantity = _minQuantity;
                    QuantityChanged();
                }
            }
        }

        public int MaxQuantity
        {
            get
            {
                return _maxQuantity;
            }

            set
            {
                _hasMaxQuantity = true;
                _maxQuantity = value;
                //Don't allow the quantity to be greater than the max quantity
                if(_maxQuantity<_quantity)
                {
                    _quantity = _maxQuantity;
                    QuantityChanged();
                }
            }
        }

        public override Vector2 Position
        {
            get
            {
                return _positionAbsolute;
            }

        }

        public override float Width
        {
            get
            {
                return 0;
            }

        }
        public override float Height
        {
            get
            {
                return 0;
            }

        }

        public int Quantity
        {
            get
            {
                return _quantity;
            }
        }
#endregion
        public QuantitySelector(Vector2 positionAbsolute,int initialQuantity):this(positionAbsolute)
        {
            _quantity = initialQuantity;
        }

        public QuantitySelector(Vector2 positionAbsolute)
        {
            isActive = true;
            hasFocus = true;
            _positionAbsolute = positionAbsolute;

            _decreaseQuantity = new TextElement("-", ScreenManager.GetInstance().DefaultMenuFont, ScreenManager.GetInstance().DefaultMenuFont);
            _decreaseQuantity.OnClickEvent += DecreaseQuantity;
            _decreaseQuantity.IsActive = true;

            _increaseQuantity = new TextElement("+", ScreenManager.GetInstance().DefaultMenuFont, ScreenManager.GetInstance().DefaultMenuFont);
            _increaseQuantity.OnClickEvent += IncreaseQuantity;
            _increaseQuantity.IsActive = true;
        }

        public override void Update(GameTime theTime, GameState state, Vector2 parentPosition)
        {
        }

        public override void Update(GameTime theTime, GameState state)
        {
            _decreaseQuantity.Update(theTime);
            _increaseQuantity.Update(theTime);
        }

        //  public virtual void HandleInput(GameTime gameTime, InputState input) { }

        //Handles user input. This is separate to Update, as a screen can still be updated even if it cannot handle input  
        public override void HandleInput(GameTime gameTime, InputState input, GameState state)
        {
            _decreaseQuantity.HandleInput(gameTime, input);
            _increaseQuantity.HandleInput(gameTime, input);
        }


        public override void Draw(SpriteBatch Batch, GameState state)
        {

           
            Batch.DrawString(ScreenManager.GetInstance().DefaultMenuFont,""+ _quantity, Position, Color.White);

            _decreaseQuantity.Position = Position + new Vector2(20, 0);

            _decreaseQuantity.Draw(Batch);
            _increaseQuantity.Position = _decreaseQuantity.Position + new Vector2(_decreaseQuantity.BoundingBox.Width+5,0);
            _increaseQuantity.Draw(Batch);
        }


#region Event Handlers

        public void IncreaseQuantity(MenuElement sender)
        {
            int prevousQuantity = _quantity;
            _quantity++;

            if(_hasMaxQuantity&&_quantity>=_maxQuantity)
            {
                _quantity = _maxQuantity;
            }

            if(prevousQuantity!=_quantity)
            {
                QuantityChanged();
            }

        }

        public void DecreaseQuantity(MenuElement sender)
        {
            int prevousQuantity = _quantity;
            _quantity--;

            if (_hasMinQuantity && _quantity <= _minQuantity)
            {
                _quantity = _minQuantity;
            }

            if (prevousQuantity != _quantity)
            {
                QuantityChanged();
            }

        }

        public void QuantityChanged()
        {
            if(OnQuantityChangedEvent!=null)
            {
                OnQuantityChangedEvent(_quantity);
            }
        }


#endregion
    }
}
