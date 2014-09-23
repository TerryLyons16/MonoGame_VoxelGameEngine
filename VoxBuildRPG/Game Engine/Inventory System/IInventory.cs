using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelRPGGame.GameEngine.InventorySystem
{
     public delegate void OnUpdate();
     
    public interface IInventory
    {

      
         
         InventoryItem AddItem(InventoryItem item);
         InventoryItem RemoveItem(InventoryItem item);
         InventoryItem AddItemToStack(InventoryItem itemToAdd, InventoryItem itemInInventory);
         void AddSubscriber(OnUpdate subscriberListener);

         void RemoveSubscriber(OnUpdate subscriberListener);

     
        /// <summary>
        /// Used to inform an Inventory's View(s) that it has updated
        /// </summary>
         void UpdateViews();


         bool ContainsItemOfType(Type type);


        /// <summary>
        /// Returns all items of the type in the inventory as a list of generic InventoryItems
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
         List<InventoryItem> ItemsOfType(Type type);

        #region Properties

         List<InventoryItem> Items { get; }
         bool HasMaxCapacity { get; }
         int MaxCapacity { get; }

         bool IsFull { get; }
        #endregion

    }
}
