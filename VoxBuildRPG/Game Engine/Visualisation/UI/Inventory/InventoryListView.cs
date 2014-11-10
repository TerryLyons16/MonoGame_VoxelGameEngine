using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxelRPGGame.GameEngine.EnvironmentState;
using Microsoft.Xna.Framework.Input;
using VoxelRPGGame.GameEngine.InventorySystem;

namespace VoxelRPGGame.GameEngine.UI.Inventory
{
    public class InventoryListView : InventoryView
    {

        protected List<InventoryListItem> _inventoryList;

        public override float Height
        {
            get {

                float result = 0;

                if (_inventoryList.Count>0)
                {
                    //Height is the Y position of the right-most slot-position.Y 
                    result = _inventoryList[_inventoryList.Count - 1].Height + _inventoryList[_inventoryList.Count - 1].Position.Y - Position.Y;
                }
                return result;
            }
        }

        public override float Width
        {
            get
            {
                float result = 0;

                if (_inventoryList.Count > 0)
                {
                    //Width is the x position of the right-most slot-position.X 
                    result = _inventoryList[_inventoryList.Count - 1].Width + _inventoryList[_inventoryList.Count - 1].Position.X - Position.X;
                }
                return result;
            }
        }

        public override Vector2 Position
        {
            get
            {
                return _positionAbsolute;
            }
        }

        public InventoryListView(InventorySystem.IInventory inventoryModel, Vector2 positionAbsolute)
            : base(inventoryModel)
        {
            _inventoryList = new List<InventoryListItem>();
            _positionAbsolute = positionAbsolute;
            isActive = true;
            OnInventoryModelUpdate();
        }


        public override void HandleInput(GameTime gameTime, InputState input, GameState state)
        {
            foreach (InventoryListItem i in _inventoryList)
            {
                if (i != null)
                {
                    i.HandleInput(gameTime, input, state);
                }
            }

        }

        public override void Update(GameTime theTime, GameState state)
        {
            foreach (InventoryListItem i in _inventoryList)
            {
                if (i != null)
                {
                    i.Update(theTime, state);
                }
            }
        }

        public override void Draw(SpriteBatch Batch, GameState state)
        {
            foreach (InventoryListItem i in _inventoryList)
            {
                if (i != null)
                {
                    i.Draw(Batch, state);
                }
            }
        }





        public override bool ContainsSlot(InventorySlot slot)
        {
                throw new NotImplementedException();
        }

        public override int[] GetSlotPosition(InventorySlot slot)
        {
            throw new NotImplementedException();
        }

        protected override void OnInventoryModelUpdate()
        {
            _inventoryList = new List<InventoryListItem>();

            Vector2 _itemPosition = Position;

            foreach (InventoryItem item in _inventoryModel.Items)
            {
                InventoryListItem listItem = new InventoryListItem(_itemPosition,this,item);

                _inventoryList.Add(listItem);
                _itemPosition.Y += listItem.Height;
            }
        }
    }
}
