using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using VoxelRPGGame.GameEngine.World;
using VoxelRPGGame.GameEngine.World.Voxels;
using VoxelRPGGame.GameEngine.World.Building;

namespace VoxelRPGGame.GameEngine.InventorySystem.Abilities.Build
{
    //NOTE: This may be expanded to remove any non-voxel building component i.e. walls, roof, doors, windows, furniture etc.
    public class RemoveWallAbility: AbilityInventoryItem
    {

        public RemoveWallAbility():base(null)
        {
            _name = "Remove Wall";
            _isStackable = false; ;
            _maxStackSize = 1;
            _stock = 1;
            _abilityUsageMode = AbilityUsageMode.Sustained;

        }

        public override void OnClick(InputState input)
        {
            //Attempt to remove wall at clicked position
              Ray? mouseRay = input.GetMouseRay();

              if (mouseRay != null)
              {
                  AbstractWorldObject selectedObject = VoxelRaycastUtility.GetNearestIntersectingWorldObject((Ray)mouseRay);

                  if (selectedObject != null && selectedObject is Wall)
                  {
                      BuildTools.RemoveWall(selectedObject as Wall);
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
