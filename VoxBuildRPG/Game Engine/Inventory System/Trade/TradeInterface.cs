using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelRPGGame.GameEngine.InventorySystem.Trade
{
    public abstract class TradeInterface
    {

        protected abstract void Trade(ITradeInventory shop, ITradeInventory customer, ITradeableItem item, int desiredQuantity, float unitPrice);

        public abstract void Buy(ITradeInventory shop, ITradeInventory customer, ITradeableItem item, int desiredQuantity);

        public abstract void Sell(ITradeInventory shop, ITradeInventory customer, ITradeableItem item, int desiredQuantity);


    }
}
