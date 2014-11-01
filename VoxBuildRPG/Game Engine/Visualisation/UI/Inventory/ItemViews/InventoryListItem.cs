using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.EnvironmentState;
using VoxelRPGGame.GameEngine.InventorySystem;
using VoxelRPGGame.MenuSystem;

namespace VoxelRPGGame.GameEngine.UI.Inventory
{
    public class InventoryListItem : InventorySlot
    {

        protected Vector2 _namePositionOffset = new Vector2(10,0);

        public InventoryListItem(Vector2 positionAbsolute, InventoryView owner,InventoryItem item)
            : base(positionAbsolute, owner)
        {
            _inventoryItem = new Inventory.InventoryItemView(item, _positionAbsolute, this);
            _boundingBox = new Rectangle((int)_positionAbsolute.X, (int)_positionAbsolute.Y, 100, 50);
        }



        public override void Draw(SpriteBatch Batch, GameState state)
        {
            if (_inventoryItem != null)
            {
                _inventoryItem.Draw(Batch, state);
            }

            Color nameColor = Color.White;


          //  Batch.Draw(_slotTexture, _boundingBox, TEMPCOLOR);
            Batch.DrawString(ScreenManager.GetInstance().DefaultMenuFont, "" + _inventoryItem.InventoryItem.Name, new Vector2(Position.X + _inventoryItem.Width + _namePositionOffset.X, Position.Y), nameColor);

        }
 
    }
}
