using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.InventorySystem.Trade;

namespace VoxelRPGGame.GameEngine.InventorySystem
{
    public class PlayerInventory : Inventory, ITradeInventory
    {
        protected float _currency;

        public PlayerInventory(int? maxCapacity):base(maxCapacity)
        {
            _currency = 0;
        }
        public float Currency
        {
            get
            {
                return _currency;
            }
            set
            {
                _currency = value;
            }
        }
    }
}
