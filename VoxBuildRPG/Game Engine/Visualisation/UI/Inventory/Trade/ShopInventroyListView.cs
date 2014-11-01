using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.EnvironmentState;
using VoxelRPGGame.GameEngine.InventorySystem;
using VoxelRPGGame.GameEngine.InventorySystem.Trade;
using VoxelRPGGame.MenuSystem;
using VoxelRPGGame.MenuSystem.MenuElements;

namespace VoxelRPGGame.GameEngine.UI.Inventory.Trade
{
    public class ShopInventroyListView : InventoryListView
    {


        protected ShopTradeInterface tradeInterface;

        protected ITradeInventory _buyerInventoryModel;

        protected TradeType _tradeType;

        public ShopInventroyListView(ITradeInventory sellerInventoryModel, ITradeInventory buyerInventoryModel, Vector2 positionAbsolute,TradeType tradeType)
            : base(sellerInventoryModel, positionAbsolute)
        {
            _buyerInventoryModel = buyerInventoryModel;

            tradeInterface = new ShopTradeInterface();

            _tradeType = tradeType;

            OnInventoryModelUpdate();
        }

         protected override void OnInventoryModelUpdate()
         {
             _inventoryList = new List<InventoryListItem>();

             Vector2 _itemPosition = Position;

             foreach (InventoryItem item in _inventoryModel.Items)
             {
                 if (item is ITradeableItem)
                 {
                     ShopInventoryListItem listItem = new ShopInventoryListItem(_itemPosition, this, item, _tradeType);

                     if (_tradeType == TradeType.Buy)
                     {
                         listItem.OnRequestTradeEvent += Buy;
                     }
                     else if (_tradeType == TradeType.Sell)
                     {
                         listItem.OnRequestTradeEvent += Sell;
                     }


                     _inventoryList.Add(listItem);
                     _itemPosition.Y += listItem.Height;
                 }
             }
         }

         public override void Draw(SpriteBatch Batch, GameState state)
         {
             base.Draw(Batch, state);
             if (_buyerInventoryModel is ITradeInventory)
             {
                 Color nameColor = Color.White;
                 string money = "" + (_buyerInventoryModel as ITradeInventory).Currency;
                 Batch.DrawString(ScreenManager.GetInstance().DefaultMenuFont, money, new Vector2(Position.X, Position.Y + Height + 5), nameColor);
             }
         }


        #region Event Handlers

         public void Buy(ITradeableItem item, int desiredQuantity)
         {
             if (_inventoryModel is ITradeInventory)
             {
                 tradeInterface.Buy((_inventoryModel as ITradeInventory), _buyerInventoryModel, item, desiredQuantity);
             }
         }

         public void Sell(ITradeableItem item, int desiredQuantity)
         {
             if (_inventoryModel is ITradeInventory)
             {
                
                 tradeInterface.Sell((_inventoryModel as ITradeInventory),_buyerInventoryModel, item, desiredQuantity);
             }
         }

        #endregion
    }
}
