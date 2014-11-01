using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.InventorySystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxelRPGGame.GameEngine.EnvironmentState;
using VoxelRPGGame.MenuSystem;
using Microsoft.Xna.Framework.Input;
using VoxelRPGGame.GameEngine.UI.Tooltips;

namespace VoxelRPGGame.GameEngine.UI.Inventory
{
    /// <summary>
    /// Object responsible of the display of an inventory item
    /// </summary>
    public class InventoryItemView: UIElement
    {
        public delegate void RequestAddTooltip(InventoryItemView itemView, InventoryItemTooltip tooltip);
        public event RequestAddTooltip RequestAddTooltipEvent;
        public delegate void RequestRemoveTooltip(InventoryItemTooltip tooltip);
        public event RequestRemoveTooltip RequestRemoveTooltipEvent;


        public static InventorySlot TEMPMouseOver = null;
        /// <summary>
        /// The InventoryItem data object that the view is displaying i.e. the "owner" of the view
        /// </summary>
        private InventoryItem _inventoryItem;
        private Vector2 _position;
        private Texture2D _icon;
        /// <summary>
        /// Used to display amount when item has a count
        /// </summary>
        private string _quantity;


        private InventorySlot _owner;
        protected Rectangle _boundingBox;

        protected Vector2 _releasedPosition;
        int differenceX=0;
        int differenceY = 0;


        protected int _mouseHoverCounter = 0;//Counts the number of ticks the mouse has been hovering over element 
        protected bool isBeingDragged = false;

        protected InventoryItemTooltip _tooltip = null;

        public override Vector2 Position
        {
            get
            {
                return _position;
            }

        }

        public override float Width
        {
            get
            {
                return _boundingBox.Width;
            }

        }
        public override float Height
        {
            get
            {
                return _boundingBox.Height;
            }

        }

        public InventoryItemView(InventoryItem item, Vector2 position,InventorySlot owner)
        {
            RequestAddTooltipEvent += GameHUDScreen.GetInstance().AddTooltip;
            RequestRemoveTooltipEvent += GameHUDScreen.GetInstance().RemoveTooltip;

            _owner = owner;
            _position = new Vector2(position.X,position.Y);
            _releasedPosition = new Vector2(_position.X, _position.Y);
            _inventoryItem = item;

            _icon = ScreenManager.GetInstance().ContentManager.Load<Texture2D>(_inventoryItem.IconLocation);
           // _icon = ScreenManager.GetInstance().ContentManager.Load<Texture2D>("Textures\\UI\\TestIcon");

            _boundingBox = new Rectangle((int)_position.X, (int)_position.Y, 40,40);
        }

        public override void Update(GameTime theTime, GameState state)
        {
        }

        //  public virtual void HandleInput(GameTime gameTime, InputState input) { }

        //Handles user input. This is separate to Update, as a screen can still be updated even if it cannot handle input  
        public override void HandleInput(GameTime gameTime, InputState input, GameState state)
        {

            if (_boundingBox.Contains(new Point(input.CurrentMouseState.X, input.CurrentMouseState.Y)) && input.IsMouseVisible)
            {
                if (input.CurrentMouseState.LeftButton == ButtonState.Pressed&&input.PreviousMouseState.LeftButton==ButtonState.Released)
                {
                    isBeingDragged = true;
                }

                if(!isBeingDragged)
                {
                    _mouseHoverCounter++;

                    if (_mouseHoverCounter>30&&_tooltip == null)
                    {
                        _tooltip = new InventoryItemTooltip(_inventoryItem, Position);
                        OnRequestAddTooltip(_tooltip);
                    }
                }
                else
                {
                    _mouseHoverCounter = 0;
                }

            }
            else
            {
                _mouseHoverCounter = 0;

                OnRequestRemoveTooltip(_tooltip);
             
               
            }

            if (input.CurrentMouseState.LeftButton == ButtonState.Released)
            {
                if (isBeingDragged&&TEMPMouseOver != null)
                {
                    ((InventoryView)TEMPMouseOver.Owner).RequestAddAt(this, TEMPMouseOver);
                }

                isBeingDragged = false;
                _releasedPosition = new Vector2(_position.X, _position.Y);
                differenceX = (int)_releasedPosition.X - (int)_owner.PositionAbsolute.X;
                differenceY = (int)_releasedPosition.Y - (int)_owner.PositionAbsolute.Y;

            }

            if (isBeingDragged)
            {
                OnRequestRemoveTooltip(_tooltip);
                Vector2 currentPoint = new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                Vector2 previousPoint = new Vector2(input.PreviousMouseState.X, input.PreviousMouseState.Y);

                _position.X = _position.X + (currentPoint.X - previousPoint.X);
                _position.Y = _position.Y + (currentPoint.Y - previousPoint.Y);
                _boundingBox = new Rectangle((int)_position.X, (int)_position.Y, 40,40);

            }
            else
            {

              
                _position.X -=(int)(differenceX );
                _position.Y -= (int)(differenceY );
                _boundingBox = new Rectangle((int)_position.X, (int)_position.Y,40, 40);

                if (_position == _owner.PositionAbsolute)
                {
                    differenceX = 0;
                    differenceY = 0;
                }
            }


        }


        public override void Draw(SpriteBatch Batch, GameState state)
        {
            Batch.Draw(_icon, _boundingBox, Color.White);

            if(_inventoryItem.IsStackable)
            {

                Vector2 stackCountMeasurement = ScreenManager.GetInstance().DefaultMenuFont.MeasureString("" + _inventoryItem.Stock);
                Batch.DrawString(ScreenManager.GetInstance().DefaultMenuFont, "" + _inventoryItem.Stock, _position + new Vector2(Width - (stackCountMeasurement.X+3), 0), Color.White);
            }
        }


        public InventoryItem InventoryItem
        {
            get
            {
                return _inventoryItem;
            }
        }


        #region Event Handlers

        public void OnRequestAddTooltip(InventoryItemTooltip tooltip)
        {
            if(RequestAddTooltipEvent!=null)
            {
                RequestAddTooltipEvent(this, tooltip);
            }
        }

        public void OnRequestRemoveTooltip(InventoryItemTooltip tooltip)
        {

            if (_tooltip != null)
            {
                //remove tooltip from ui
                if (RequestRemoveTooltipEvent != null)
                {
                    RequestRemoveTooltipEvent(tooltip);
                }
                _tooltip = null;
            }

           
        }

        #endregion
    }
}
