using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.InventorySystem;

namespace VoxelRPGGame.GameEngine.InventorySystem
{
    public class ArrayInventory: IInventory
    {
      
        protected event OnUpdate OnInventoryUpdate;

        protected InventoryItem[] _items;

        public int MaxCapacity
        {
            get
            {
                return _items.Length;
            }
        }
        public bool HasMaxCapacity
        {
            //An array always has a max capacity
            get
            {
                return true;
            }
        }

        public ArrayInventory(int capacity)
        {
            _items=new InventoryItem[capacity];
        }

        public virtual InventoryItem AddItemAt(InventoryItem item, int position)
        {
            InventoryItem result = null;

            if(position<_items.Length)
            {
                if(_items.Contains(item))
                {
                    //If already in inventory, remove from current position
                    for (int i = 0; i < _items.Length; i++)
                    {
                        if (_items[i] == item)
                        {
                            _items[i] = null;
                            result = item;
                        }
                    }
                    _items[position] = item;
                    //Don't call OnAddToInventory, as it hasn't been added - just moved
                    result = null;
                }
                if(_items[position]!=null)
                {
                    //Swap item
                }

                else
                {
                    _items[position] = item;
                    item.OnAddToInventory(this);
                    result=null;
                }
                UpdateViews();
            }

            return result;
        }

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
                if (!IsFull && item != null)
                {
                    int ? pos = GetFirstAvailablePosition(_items);
                    if (pos != null)
                    {
                        _items[(int)pos] = item;
                        item.OnAddToInventory(this);
                        result = null;//Don't return anything as item has been added
                    }
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

        public int? GetFirstAvailablePosition(InventoryItem[] grid)
        {
            int ? result =null;

            for (int i = 0; i < grid.Length; i++)
            {
                    if (grid[i]==null)
                    {
                        //break from inner loop
                        result = i;
                        break;
                    }
                }
            return result;
        }

        public virtual InventoryItem RemoveItem(InventoryItem item)
        {
            InventoryItem result = null;
            if (_items.Contains(item))
            {
                for(int i=0;i<_items.Length;i++)
                {
                    if(_items[i]==item)
                    {
                        _items[i] = null;
                        result = item;
                    }
                }

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
            return _items.Where(item => item!=null&&item.GetType() == type).ToList<InventoryItem>();
        }

        public int QuantityCanAdd(InventoryItem item, int count)
        {
            int result = 0;

            //If amount can be added as a single stack and there is space for that stack
            if (!IsFull && count <= item.MaxStackSize)
            {
                result = count;
            }

            else if (item.IsStackable && ContainsItemOfType(item.GetType()))
            {
                Queue<InventoryItem> applicableStacks = new Queue<InventoryItem>(ItemsOfType(item.GetType()));
                //Remove the amount that can be added too each existing stack from the total

                int amountToFit = count;
                while (applicableStacks.Count > 0)
                {
                    InventoryItem itemInInventory = applicableStacks.Dequeue();
                    if (!itemInInventory.IsFull)
                    {
                        amountToFit -= itemInInventory.AvailableStockSpace;
                    }
                }

                //If there is space for new stacks 
                if ((HasMaxCapacity || MaxCapacity > 0) && FreeSpace>0)
                {
                    int freeSpace = FreeSpace;
                    amountToFit -= (item.MaxStackSize * freeSpace);
                }


                if (amountToFit > 0)
                {
                    result = count - amountToFit;
                }
                else
                {
                    result = count;
                }

            }

            return result;
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
                bool result = true;

                for(int i=0;i<_items.Length;i++)
                {
                    if(_items[i]==null)
                    {
                        //If any space is null, then inventory not full
                        result = false;
                        break;
                    }
                }
                return result;

            }
        }

        protected int FreeSpace
        {
            get
            {
                int result = 0;

                for (int i = 0; i < _items.Length; i++)
                {
                    if (_items[i] == null)
                    {
                        //If any space is null, then inventory not full
                        result++;
                    }
                }
                return result;
            }
        }
        #endregion
    }
}
