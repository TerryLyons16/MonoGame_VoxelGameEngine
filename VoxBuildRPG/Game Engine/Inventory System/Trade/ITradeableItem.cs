using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.InventorySystem;

namespace VoxelRPGGame.GameEngine.InventorySystem.Trade
{
    public interface ITradeableItem
    {

         float BaseValue
        {
            get;
            set;
        }

        /// <summary>
        /// The buy price for the customer (i.e. the player).
        /// </summary>
         float CustomerBuyPrice
         {
             get;
             set;
         }

        /// <summary>
        /// The sell price for the customer (i.e. the player).
        /// </summary>
         float CustomerSellPrice
         {
             get;
             set;
         }


         InventoryItem InventoryItem
         {
             get;
         }


         bool IsTradeable
         {
             get;
         }

    }
}
