using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using VoxelRPGGame.GameEngine.World;
using VoxelRPGGame.GameEngine.World.Voxels;

namespace VoxelRPGGame.GameEngine.InventorySystem.Abilities.Build
{
    public class RemoveBlockAbility: AbilityInventoryItem
    {

        public RemoveBlockAbility()
            : base("Remove Block","Textures\\UI\\TestIcon")
        {
            _isStackable = false; ;
            _maxStackSize = 1;
            _stock = 1;
            _abilityUsageMode = AbilityUsageMode.Sustained;
            AllowedTools.Add(Tools.ToolType.Hammer);
        }

        public override void OnClick(InputState input)
        {
            //Attempt to remove block at clicked position
              Ray? mouseRay = input.GetMouseRay();

              if (mouseRay != null)
              {
                  AbstractWorldObject selectedObject = VoxelRaycastUtility.GetNearestIntersectingWorldObject((Ray)mouseRay);

                  if (selectedObject != null && selectedObject is AbstractBlock)
                  {
                      BuildTools.RemoveBlock(selectedObject as AbstractBlock);
                  }
              }
        }

        public override void OnDrag(InputState input)
        {
            //Could add drag functionality to 
        }

        public override void OnRelease(InputState input)
        {

        }

        public override void OnSelect()
        {
            //Could add toggle to flatten blocks into slopes - or add as separate ability

        }

        public override void OnDeselect()
        {

        }
    }
}
