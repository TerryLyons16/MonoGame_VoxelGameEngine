using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelRPGGame.GameEngine.InventorySystem
{
    public enum Rarity
    {
        NULL,
        Common,
        Uncommon,
        Rare,
        Epic
    }
    public class InventoryItem
    {
        public delegate InventoryItem RequestInventoryRemoval(InventoryItem item);
        public event RequestInventoryRemoval RemoveFromInventoryRequest;

        protected IInventory _owner;

        protected string _name="";
        protected string _description="";

        protected Rarity _rarity;

        protected string _iconLocation;

        protected bool _isStackable;

        protected int _maxStackSize;
        protected int _stock=1;//The amount of the item - will always be at least 1

        protected bool _isQuestItem;

        public InventoryItem(string iconLocation)
        {
            _iconLocation = iconLocation;
        }

        /// <summary>
        /// If the item is stackable, will attempt to add the amount and return the remainder if the stack becomes full
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int AddQuantity(int amount)
        {
            int result = amount;

            if (_isStackable && _stock < _maxStackSize)
            {
                int amtThatCanBeAdded = _maxStackSize - _stock;

                if (amount > amtThatCanBeAdded)//Add as much as will fit, return remainder
                {
                    _stock += amtThatCanBeAdded;
                    result = amount - amtThatCanBeAdded;
                }
                else //Add the full amount and return 0
                {
                    _stock += amount;
                    result = 0;
                }
                

            }

            return result;
        }

        /// <summary>
        /// If the item is stackable, will attempt to remove the specified amount and returns the actual amount removed
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int RemoveQuantity(int amount)
        {
            int result = 0;
            if (_isStackable && _stock>=0)
            {
                int amtThatCanBeRemoved = _stock;

                if (amount > amtThatCanBeRemoved)//Remove as much as will fit
                {
                    _stock -= amtThatCanBeRemoved;
                    result = amtThatCanBeRemoved;
                }
                else //Remove the full amount
                {
                    _stock -= amount;
                    result = amount;
                }
                

            } //NOTE: If amount ever hits 0, need to fire an event to remove the item from whatever inventory it is in

            if (_isStackable&&_stock <= 0)
            {
                //If no stock, remove the empty item
                OnRemoveRequest();
            }

            return result;
        }

        public void Rename(string newName)
        {
            _name = newName;
        }

        public void SetDescription(string description)
        {
            _description = description;
        }

        public void OnAddToInventory(IInventory owner)
        {
            if (_owner != owner)//Only do this if the owner inventory has changed (i.e. not if its position in the inventory was swapped)
            {
                _owner = owner;
                //Remove from current inventory
                OnRemoveRequest();
                RemoveFromInventoryRequest = null;
                RemoveFromInventoryRequest += _owner.RemoveItem;
            }

        }

        protected void OnRemoveRequest()
        {
            if(RemoveFromInventoryRequest!=null)
            {
                RemoveFromInventoryRequest(this);
            }
        }

        /// <summary>
        /// Splits a stack item into two based on the desiredAmount for the new stack.
        /// If Item is not stackable, or if desired amount is full stack, returns the item
        /// </summary>
        /// <param name="desiredAmount"></param>
        /// <returns></returns>
        public InventoryItem SplitStack(int desiredAmount)
        {
            InventoryItem result = null;
            
            //Only split stack if current item is stackable  
            if(_isStackable)
            {
                //If the desired amount is the full stock, return this item
                if (desiredAmount >= Stock)
                {
                    result = this;
                }
                else
                {
                    //Create a new inventory item with the properties of the current item
                    result = CopyProperties();
                    //Split the stock across the two items
                    result._stock=RemoveQuantity(desiredAmount);
                }
            }
            else
            {
                result = this;
            }
            

            return result;
        }

        /// <summary>
        /// Copies the properties of an inventory item into a new inventory item. Used for splitting stacks.
        /// Override in any subclass that has additional properties
        /// </summary>
        /// <param name="targetItem"></param>
        /// <returns></returns>
        protected virtual InventoryItem CopyProperties()
        {
            InventoryItem result = new InventoryItem(_iconLocation);
            result._name = Name;
            result._description = Description;
            result._isStackable = _isStackable;
            result._maxStackSize = _maxStackSize;
            result._rarity = _rarity;

            return result;
        }
    
        #region Properties
        public bool IsStackable
        {
            get
            {
                return _isStackable;
            }
        }

        public int Stock
        {
            get
            {
                return _stock;
            }

        }

        public string IconLocation
        {
            get
            {
                return _iconLocation;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public Rarity Rarity
        {
            get
            {
               return _rarity;
            }
            set
            {
                _rarity = value;
            }
        }

        public virtual string ItemType
        {
            get
            {
                return "";
            }
        }

        public bool IsFull
        {
            get
            {
                bool result = true;

                if(_isStackable)
                {
                   if(Stock<_maxStackSize)
                   {
                       result = false;
                   }
                }

                return result;
            }
        }

        public int AvailableStockSpace
        {
            get
            {
                int result = 0;

                if(_isStackable)
                {
                    result = _maxStackSize - Stock;
                }

                return result;
            }
        }

        public int MaxStackSize
        {
            get
            {
                return _maxStackSize;
            }
        }


        #endregion


    }
}
