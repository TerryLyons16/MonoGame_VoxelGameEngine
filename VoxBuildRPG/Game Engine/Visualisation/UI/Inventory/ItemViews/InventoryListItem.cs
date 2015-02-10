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

        public override float Width
        {
            get
            {
                return base.Width + _namePositionOffset.X + ScreenManager.GetInstance().DefaultMenuFont.MeasureString(_inventoryItem.InventoryItem.Name).X;
            }
        }

        public override float Height
        {
            get
            {
                return base.Height + _namePositionOffset.Y + ScreenManager.GetInstance().DefaultMenuFont.MeasureString(_inventoryItem.InventoryItem.Name).Y;
            }
        }

        public InventoryListItem(Vector2 positionRelative,Vector2 parentPosition, InventoryView owner,InventoryItem item)
            : base(positionRelative, parentPosition, owner)
        {
            _inventoryItem = new Inventory.InventoryItemView(item, Vector2.Zero,_positionAbsolute, this,false);
            _boundingBox = new Rectangle((int)_positionAbsolute.X, (int)_positionAbsolute.Y, 100, 50);
        }


        public override void Draw(SpriteBatch Batch, GameState state)
        {
            if (_inventoryItem != null)
            {
                _inventoryItem.Draw(Batch, state);
            }

            Color nameColor = InventoryItemDisplayUtilities.GetItemRarityColour(_inventoryItem.InventoryItem.Rarity);


          //  Batch.Draw(_slotTexture, _boundingBox, TEMPCOLOR);
            Batch.DrawString(ScreenManager.GetInstance().DefaultMenuFont, "" + _inventoryItem.InventoryItem.Name, new Vector2(Position.X + _inventoryItem.Width + _namePositionOffset.X, Position.Y), nameColor);

        }
 
    }
}
