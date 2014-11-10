using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.World.Voxels;
using VoxelRPGGame.GameEngine.World;
using Microsoft.Xna.Framework;
using VoxelRPGGame.GameEngine.InventorySystem.Trade;

namespace VoxelRPGGame.GameEngine.InventorySystem.Abilities.Build
{
    /// <summary>
    /// Any terrain block inventory item.
    /// Acts an ability which is used to place the block in the world
    /// </summary>
    public class BlockInventoryItem: AbilityInventoryItem, ITradeableItem
    {
        protected MaterialType _blockType;


        public BlockInventoryItem(MaterialType blockType, string description, int quantity)
            : base( blockType+" Block","Textures\\UI\\TestIconBlock")
        {
            _description = description;
            _maxStackSize = 250;
            _isStackable = true;
            _stock = quantity;
            _numUses = _stock;//Any time quantity changes, _numUses must change
            _abilityUsageMode = AbilityUsageMode.Multi;
            AllowedTools.Add(Tools.ToolType.Hammer);

            CustomerBuyPrice = 1;
            CustomerSellPrice = 1;
            BaseValue = 1;
        }


        public override void OnClick(InputState input) 
        { 
            //Attempt to add block at clicked position
            Ray? mouseRay = input.GetMouseRay();

            if (mouseRay != null)
            {
                  AbstractWorldObject selectedObject = VoxelRaycastUtility.GetNearestIntersectingWorldObject((Ray)mouseRay);

                  if (selectedObject != null)
                  {
                      //Need a way to determine what type of block to create based on BlockType
                      bool blockAdded = BuildTools.AddBlock((Ray)mouseRay, selectedObject as AbstractBlock);

                      if(blockAdded)
                      {
                          RemoveQuantity(1);
                      }
                  }
            }
            

            //If successful, decrement _quantity and _numUses 
        }

        public override void OnDrag(InputState input) 
        {
            //Could add drag functionality to add multiple 
        }

        public override void OnRelease(InputState input) 
        { 

        }

        public override void OnSelect()
        { 

        }

        public override void OnDeselect()
        {

        }


        /// <summary>
        /// Copies the properties of an inventory item into a new inventory item. Used for splitting stacks.
        /// Override in any subclass that has additional properties
        /// </summary>
        /// <param name="targetItem"></param>
        /// <returns></returns>
        protected override InventoryItem CopyProperties()
        {
            BlockInventoryItem result = new BlockInventoryItem(_blockType, _description,0);
            result._description = Description;
            result._isStackable = _isStackable;
            result._maxStackSize = _maxStackSize;
            result._rarity = _rarity;

            return result;
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
                if (_isQuestItem)
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
