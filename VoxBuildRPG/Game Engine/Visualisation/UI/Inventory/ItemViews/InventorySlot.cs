using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxelRPGGame.GameEngine.EnvironmentState;
using VoxelRPGGame.MenuSystem;
using VoxelRPGGame.GameEngine.InventorySystem;
using Microsoft.Xna.Framework.Input;
using VoxelRPGGame.GameEngine.InventorySystem.Abilities;

namespace VoxelRPGGame.GameEngine.UI.Inventory
{
    public class InventorySlot: UIElement
    {
        protected InventoryItemView _inventoryItem = null;

        protected Vector2 _positionRelative;//position relative to whatever inventory it is in
        protected Vector2 _positionAbsolute;//Position on screen it is drawn at

        protected Texture2D _slotTexture;
        protected Rectangle _boundingBox;

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
        protected Color TEMPCOLOR = Color.White;

        public InventorySlot(Vector2 positionAbsolute, InventoryView owner)
        {
            _owner = owner;
            _slotTexture = ScreenManager.GetInstance().ContentManager.Load<Texture2D>("Textures\\UI\\InventorySlot");
            _positionAbsolute = new Vector2(positionAbsolute.X, positionAbsolute.Y);
            _boundingBox = new Rectangle((int)_positionAbsolute.X, (int)_positionAbsolute.Y, 40,40);


       //     _inventoryItem = new InventoryItemView(null, _positionAbsolute,this);
        }


        public InventorySlot(Vector2 _positionAbsolute, InventoryItem item, InventoryView owner)
            : this(_positionAbsolute,owner)
        {
            _inventoryItem = new InventoryItemView(item, PositionAbsolute, this);
        }

        public bool AddInventoryItem(InventoryItemView item)
        {
            bool result = false;

            if (_inventoryItem == null)
            {
                _inventoryItem = item;
                result = true;
            }

            //NOTE: Need to handle communication with the underlying inventory Model
            //i.e. add the item to the Model (may be handled at higher level...)
            return result;
        }

        public InventoryItemView RemoveInventoryItem()
        {
            InventoryItemView result = _inventoryItem;

            _inventoryItem = null;

            return result;
        }


        public InventoryItemView InventoryItemView
        {
            get
            {
                return _inventoryItem;
            }
        }


        public InventoryItem InventoryItem
        {
            get
            {
                if (_inventoryItem == null)
                {
                    return null;
                }
                else
                {
                    return _inventoryItem.InventoryItem;
                }
            }
        }

        public bool ContainsItem()
        {
            if (_inventoryItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void Update(GameTime theTime, GameState state)
        {
            if (_inventoryItem != null)
            {
                _inventoryItem.Update(theTime, state);
            }
        }

        //  public virtual void HandleInput(GameTime gameTime, InputState input) { }

        //Handles user input. This is separate to Update, as a screen can still be updated even if it cannot handle input  
        public override void HandleInput(GameTime gameTime, InputState input, GameState state)
        {
            if (_inventoryItem != null)
            {
                _inventoryItem.HandleInput(gameTime, input, state);
            }


            if (_boundingBox.Contains(new Point(input.CurrentMouseState.X, input.CurrentMouseState.Y)) && input.IsMouseVisible)
            {
                if (input.CurrentMouseState.LeftButton == ButtonState.Released && input.PreviousMouseState.LeftButton == ButtonState.Pressed)
                {
                    TEMPCOLOR = Color.Red;
                    //NOTE: Should be elsewhere - currenty an ablity in any inventory could be selected
                  /*  if (Owner is InventoryView && _inventoryItem != null && _inventoryItem.InventoryItem is AbilityInventoryItem)
                    {
                        GameWorldControlState.GetInstance().OnAbilitySelected((Owner as InventoryView).GetSlotPosition(this)[0]+1);
                    }*/
                }
                else
                {
                    TEMPCOLOR = Color.Goldenrod;
                }
                InventoryItemView.TEMPMouseOver = this;
            }
            else
            {
                TEMPCOLOR = Color.White;
            }
        }


        public override void Draw(SpriteBatch Batch, GameState state)
        {
            if (_inventoryItem != null)
            {
                _inventoryItem.Draw(Batch, state);
            }

            Batch.Draw(_slotTexture, _boundingBox, TEMPCOLOR);


        }


        public Vector2 PositionAbsolute
        {
            get
            {
                return _positionAbsolute;
            }
        }
      
    }
}
