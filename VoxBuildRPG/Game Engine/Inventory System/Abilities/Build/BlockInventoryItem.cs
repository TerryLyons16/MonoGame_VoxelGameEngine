using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.World.Voxels;
using VoxelRPGGame.GameEngine.World;
using Microsoft.Xna.Framework;

namespace VoxelRPGGame.GameEngine.InventorySystem.Abilities.Build
{
    /// <summary>
    /// Any terrain block inventory item.
    /// Acts an ability which is used to place the block in the world
    /// </summary>
    public class BlockInventoryItem: AbilityInventoryItem
    {
        protected MaterialType _blockType;


        public BlockInventoryItem(MaterialType blockType, string description, int quantity)
            : base("Textures\\UI\\TestIconBlock")
        {
            _name = blockType+" Block";
            _description = description;
            _isStackable = true;
            _stock = quantity;
            _numUses = _stock;//Any time quantity changes, _numUses must change
            _abilityUsageMode = AbilityUsageMode.Multi;
            AllowedTools.Add(Tools.ToolType.Hammer);
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
    }
}
