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
    public class PlayerBackpackGridView : InventoryGridView
    {
        public PlayerBackpackGridView(PlayerInventory inventoryModel, int numCols, Vector2 positionRelative,Vector2 parentPosition)
            : base(inventoryModel, numCols, positionRelative, parentPosition)
        {

        }

        public override void Draw(SpriteBatch Batch, GameState state)
        {
            base.Draw(Batch, state);
            Color nameColor = Color.White;
            if (_inventoryModel is PlayerInventory)
            {
                string money = "" + (_inventoryModel as PlayerInventory).Currency;

                Batch.DrawString(ScreenManager.GetInstance().DefaultMenuFont, money, new Vector2(Position.X , Position.Y+Height+5), nameColor);
            }
        }
    }
}
