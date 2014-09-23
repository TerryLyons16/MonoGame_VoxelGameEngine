using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VoxelRPGGame.GameEngine.InventorySystem;

namespace VoxelRPGGame.GameEngine.UI.Inventory
{
    public abstract class InventoryView: UIElement
    {
        protected InventorySystem.IInventory _inventoryModel;
        protected Dictionary<InventoryItem, int[]> desiredPositions = new Dictionary<InventoryItem, int[]>();


        public InventoryView(InventorySystem.IInventory inventoryModel)
        {
            _inventoryModel = inventoryModel;
            _inventoryModel.AddSubscriber(OnInventoryModelUpdate);
        }


        protected virtual void OnDestroy()
        {
            if (_inventoryModel != null)
            {
                _inventoryModel.RemoveSubscriber(OnInventoryModelUpdate);
            }
        }

        protected abstract void OnInventoryModelUpdate();

        public virtual void RequestAddAt(InventoryItemView item, InventorySlot slot)
        {
            desiredPositions.Clear();//Clear the desired positions to remove any data of items that were failed to be added
            if(slot!=null&&item!=null&&ContainsSlot(slot))
            {
                if(!slot.ContainsItem())//Slot is empty - standard add
                {
                    if (_inventoryModel is ArrayInventory)
                    {
                        desiredPositions.Add(item.InventoryItem, GetSlotPosition(slot));
                        (_inventoryModel as ArrayInventory).AddItemAt(item.InventoryItem,GetSlotPosition(slot)[0]);//Pass the column number i.e slot position [0]
                    }
                    else
                    {

                        desiredPositions.Add(item.InventoryItem, GetSlotPosition(slot));
                        _inventoryModel.AddItem(item.InventoryItem);
                    }
                }
                else
                {
                    if(slot.InventoryItem.IsStackable)//Attempt to stack item
                    {

                    }
                    else//Not stackable - attempt to swap items
                    {

                    }
                }

            }
        }

        public virtual void OnRequestSwapItems(InventoryItemView localItem,InventoryItemView itemToSwap)
        {

        }

        public abstract int[] GetSlotPosition(InventorySlot slot);

        public abstract bool ContainsSlot(InventorySlot slot);
       // public abstract bool AddAtFirstAvailable(InventoryItemView item);


        public InventorySystem.IInventory InventoryModel
        {
            get
            {
                return _inventoryModel;
            }
        }

        public void ReattachModel(InventorySystem.IInventory inventoryModel)
        {
            OnDestroy();//Remove the model's listener
            _inventoryModel = inventoryModel;
            _inventoryModel.AddSubscriber(OnInventoryModelUpdate);//Add listener to the new model
            OnInventoryModelUpdate();
        }
    }
}
