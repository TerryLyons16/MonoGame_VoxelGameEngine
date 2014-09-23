using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelRPGGame.GameEngine.InventorySystem
{
    public class Inventory: IInventory
    {
        protected event OnUpdate OnInventoryUpdate;

        protected LinkedList<InventoryItem> _items = new LinkedList<InventoryItem>();
        protected bool _hasMaxCapacity;
        public bool HasMaxCapacity
        {
            get
            {
                return _hasMaxCapacity;
            }
        }

        protected int _maxCapacity;
        public int MaxCapacity
        {
            get
            {
                return _maxCapacity;
            }
        }

        public Inventory(int ? maxCapacity)
        {
            if (maxCapacity != null)
            {
                _hasMaxCapacity = true;
                _maxCapacity = (int)maxCapacity;
            }
            else
            {
                _hasMaxCapacity = false;
            }
        }

        /// <summary>
        /// Attempts to add an item to the inventory.
        /// Returns the item if it cannot be added.
        /// Returns an item with a count if it can add a quantity of the item, but not all
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual InventoryItem AddItem(InventoryItem item)
        {
            //NOTE: Could add a list of allowed Item types for each inventory rather than using subclasses to control access

            InventoryItem result = item;//Default behaviour is to return item.If the item is added, return null
            if (!_items.Contains(item))
            {

                if (item.IsStackable && ContainsItemOfType(item.GetType()))
               {
                    //Attempt to add to an existing stack
                    //Get the first suitable stack and attempt to add it to that

                   Queue<InventoryItem> applicableStacks = new Queue<InventoryItem>(ItemsOfType(item.GetType()));
                   
                   while (applicableStacks.Count > 0)
                   {
                       //Attempt to add the stack of the item across all stacks of that item type in the inventory
                       item = AddItemToStack(item, applicableStacks.Dequeue());
                       if (item == null)
                       {
                           break;
                       }
                   }
               }

                //If stackable item still has an amount in it after attempting to split it into existing stacks, add it as a new item.
                //If Item is not stackable, attempt to add it as a new item
                if(!IsFull&&item!=null)
               {
                   _items.AddLast(item);
                   item.OnAddToInventory(this);
                   result=null;//Don't return anything as item has been added
               }
            }
            UpdateViews();
            return result;
        }

        /// <summary>
        /// Will attempt to add the item to a specified stack in the inventory
        /// </summary>
        /// <param name="itemToAdd"></param>
        /// <param name="itemInInventory"></param>
        /// <returns></returns>
        public virtual InventoryItem AddItemToStack(InventoryItem itemToAdd, InventoryItem itemInInventory)
        {
            InventoryItem result = itemToAdd;

            if (itemToAdd.IsStackable && itemInInventory.IsStackable && _items.Contains(itemInInventory))
            {
                int amountToAdd = itemToAdd.Stock;
                int remainder = itemInInventory.AddQuantity(amountToAdd);
                int amountRemoved = amountToAdd - remainder;
                itemToAdd.RemoveQuantity(amountRemoved);

                if (itemToAdd.Stock > 0)
                {
                    result = itemToAdd;
                }
                else
                {
                    result = null;
                }

            }


            return result;
        }

        public virtual InventoryItem RemoveItem(InventoryItem item)
        {
            InventoryItem result = null;
            if(_items.Contains(item))
            {
                _items.Remove(item);
                result = item;
            }

            UpdateViews();
            return result;
        }

     /*   public virtual InventoryItem SwapItem(InventoryItem inInventory, InventoryItem toSwap)
        {
            return null;
        }*/


        public void AddSubscriber(OnUpdate subscriberListener)
        {
            OnInventoryUpdate += subscriberListener;
        }

        public void RemoveSubscriber(OnUpdate subscriberListener)
        {
            OnInventoryUpdate -= subscriberListener;
        }


        /// <summary>
        /// Used to inform an Inventory's View(s) that it has updated
        /// </summary>
        public void UpdateViews()
        {
            if (OnInventoryUpdate != null)
            {
                OnInventoryUpdate();
            }
        }


        public bool ContainsItemOfType(Type type)
        {
            bool result = false;
            List<InventoryItem> itemsOfType = ItemsOfType(type);

            if (itemsOfType.Count > 0)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Returns all items of the type in the inventory as a list of generic InventoryItems
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<InventoryItem> ItemsOfType(Type type)
        {
           return _items.Where(item => item.GetType() == type).ToList<InventoryItem>();
        }

     
#region Properties

        public List<InventoryItem> Items
        {
            get
            {
                return _items.ToList<InventoryItem>();
            }
        }


        public bool IsFull
        {
            get
            {
                if (!HasMaxCapacity || _maxCapacity == null)
                {
                    //Can never full if does not have a max capacity
                    return false;
                }
                else
                {
                    if (_items.Count < _maxCapacity)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

            }
        }

#endregion
    }
}
