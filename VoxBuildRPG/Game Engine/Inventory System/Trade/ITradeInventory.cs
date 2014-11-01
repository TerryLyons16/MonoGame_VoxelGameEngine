
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelRPGGame.GameEngine.InventorySystem.Trade
{
    public interface ITradeInventory : IInventory
    {

         float Currency
        {
            get;
            set;
        }
    }
}
