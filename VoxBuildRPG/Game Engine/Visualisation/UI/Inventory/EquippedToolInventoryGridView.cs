using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.InventorySystem;
using VoxelRPGGame.GameEngine.InventorySystem.Tools;

namespace VoxelRPGGame.GameEngine.UI.Inventory
{
    public class EquippedToolInventoryGridView : InventoryGridView
    {

        public EquippedToolInventoryGridView(EquippedToolInventory inventoryModel, int numCols, Vector2 positionAbsolute)
            : base(inventoryModel,numCols,positionAbsolute)
        {
        }

        public override void RequestAddAt(InventoryItemView item, InventorySlot slot)
        {
            desiredPositions.Clear();//Clear the desired positions to remove any data of items that were failed to be added
            if (slot != null && item != null && ContainsSlot(slot))
            {
                if (!slot.ContainsItem())//Slot is empty - standard add
                {
                    //inventoryModel should always be an EquippedToolInventory
                    if (_inventoryModel is EquippedToolInventory && (_inventoryModel as EquippedToolInventory).Owner!=null)
                    {
                        //Route control of adding tools to the EquippedItemsInventory that owns the EquippedToolInventory
                        if (item.InventoryItem is ToolInventoryItem)
                        {
                            (_inventoryModel as EquippedToolInventory).Owner.AddToolAt((item.InventoryItem as ToolInventoryItem), (_inventoryModel as EquippedToolInventory));
                        }
                    }

                   /* if (_inventoryModel is ArrayInventory)
                    {
                        desiredPositions.Add(item.InventoryItem, GetSlotPosition(slot));
                        (_inventoryModel as ArrayInventory).AddItemAt(item.InventoryItem, GetSlotPosition(slot)[0]);//Pass the column number i.e slot position [0]
                    }
                    else
                    {

                        desiredPositions.Add(item.InventoryItem, GetSlotPosition(slot));
                        _inventoryModel.AddItem(item.InventoryItem);
                    }*/
                }
                else
                {
                    if (slot.InventoryItem.IsStackable)//Attempt to stack item
                    {

                    }
                    else//Not stackable - attempt to swap items
                    {

                    }
                }

            }
        }
    }
}
