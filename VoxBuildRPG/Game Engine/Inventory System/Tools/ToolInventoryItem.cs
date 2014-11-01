using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.InventorySystem.Trade;

namespace VoxelRPGGame.GameEngine.InventorySystem.Tools
{
    /// <summary>
    /// Stores all types of tools and weapons
    /// </summary>
    public enum ToolType
    {
        NULL,
        /// <summary>
        /// For buildings components-walls, roofs, doors, furniture etc. 
        /// </summary>
        Hammer,
        Pickaxe,
        OneHandedSword,
        Shield,
        Bow
    }

    /// <summary>
    /// Determines how the tool is equipped and wielded
    /// </summary>
    public enum EquipConstraint
    {
        NULL,
        None,
        /// <summary>
        /// Can only be equipped in the main hand
        /// </summary>
        Primary,
        /// <summary>
        /// Can only be equipped in the secondary hand
        /// </summary>
        Secondary,
        /// <summary>
        /// Takes up both hand slots
        /// </summary>
        TwoHanded
    }
    /// <summary>
    /// Any Item that can be wielded by the player
    /// </summary>
    public class ToolInventoryItem: InventoryItem,ITradeableItem 
    {
        protected ToolType _toolType;
        protected EquipConstraint _equipConstraint;

        protected int _minDamage;
        protected int _maxDamage;

        public ToolInventoryItem(ToolType type,string iconLocation,EquipConstraint tempEquipConstraint)
            : base(iconLocation)
        {
            _equipConstraint = tempEquipConstraint;
            _toolType = type;
            //Tools/Weapons cannot be stacked
            _isStackable = false;
            _maxStackSize = 1;
            _stock = 1;
        }

        public virtual void OnEquip() { }
        public virtual void OnUnequip() { }



        public ToolType ToolType
        { 
        get
            {
                return _toolType;
            }
        }

        public override string ItemType
        {
            get
            {
                return ToolType.ToString();
            }
        }

        public EquipConstraint EquipConstraint
        {
            get
            {
                return _equipConstraint;
            }
        }

        #region Trade
        public float BaseValue
        {
            get;
            set;
        }

        /// <summary>
        /// The buy price for the customer (i.e. the player).
        /// </summary>
        public float CustomerBuyPrice
        {
            get;
            set;
        }

        /// <summary>
        /// The sell price for the customer (i.e. the player).
        /// </summary>
        public float CustomerSellPrice
        {
            get;
            set;
        }


        public InventoryItem InventoryItem
        {
            get
            {
                return this;
            }

        }


        public bool IsTradeable
        {
            get
            {
                if(_isQuestItem)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
              
        }
        #endregion
    }
}
