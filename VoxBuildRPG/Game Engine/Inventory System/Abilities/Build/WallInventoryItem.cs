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
   
     public class WallInventoryItem: AbilityInventoryItem
    {
        protected MaterialType _wallType;


        public WallInventoryItem(MaterialType wallType, string description, int quantity):base(null)
        {
           _name = wallType+" Wall";
            _description = description;
            _stock = quantity;
            _numUses = _stock;//Any time quantity changes, _numUses must change
            _abilityUsageMode = AbilityUsageMode.Multi;

        }

        public override void OnClick(InputState input)
        {
            //Attempt to place the first point of the wall
            Ray? mouseRay = input.GetMouseRay();

            if (mouseRay != null)
            {
                AbstractWorldObject selectedObject = VoxelRaycastUtility.GetNearestIntersectingWorldObject((Ray)mouseRay);

                if (selectedObject != null)
                {
                    Vector3? wallAnchor = VoxelRaycastUtility.GetNearestWallAnchor(VoxelRaycastUtility.GetNearestCollisionFace(selectedObject.GetCollisionFaces(), (Ray)mouseRay), (Ray)mouseRay);

                    if (wallAnchor != null)
                    {
                        Vector3 anchor = (Vector3)wallAnchor;


                        //Add anchor to start of collection of wall points...





                    }
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
