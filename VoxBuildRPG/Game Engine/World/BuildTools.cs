using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.World.Voxels;
using Microsoft.Xna.Framework;
using VoxelRPGGame.GameEngine.World;
using VoxelRPGGame.GameEngine.Physics;
using VoxelRPGGame.GameEngine.World.Building;
using VoxelRPGGame.GameEngine.World.Textures;

namespace VoxelRPGGame.GameEngine.World
{
    public static class BuildTools
    {
        static Vector3? TEMP_firstWallPoint = null;
        static Vector3? TEMP_secondWallPoint = null;
        private static int TEMP_WallHeight = 1;

        public static bool AddBlock(Ray selectionRay,AbstractBlock selectedBlock)
        {
            bool result = false;
            Direction nearestFaceDirection = Direction.NULL;

            CollisionFace face = VoxelRaycastUtility.GetNearestCollisionFace(selectedBlock.GetCollisionFaces(), selectionRay);

            if (face is BlockCollisionFace)
            {
                nearestFaceDirection=(face as BlockCollisionFace).Facing;
            }

            if (nearestFaceDirection != Direction.NULL)
            {
                result = selectedBlock.OnAddBlock(nearestFaceDirection, new DirtBlock(BlockShape.Cube, Direction.North));
            }

            return result;

        }

        public static void RemoveBlock(AbstractBlock block)
        {
            block.OnHit();
        }


        public static void AddWall(AbstractTerrainObject selectedObject, Ray ray)
        {
            if (selectedObject != null)
            {
                Vector3? wallAnchor = VoxelRaycastUtility.GetNearestWallAnchor(VoxelRaycastUtility.GetNearestCollisionFace(selectedObject.GetCollisionFaces(), ray), ray);

                if (wallAnchor != null)
                {
                    Vector3 anchor = (Vector3)wallAnchor;

                    if (TEMP_secondWallPoint == null)
                    {
                        if (TEMP_firstWallPoint == null)//If no existing point
                        {
                            TEMP_firstWallPoint = anchor;
                        }
                        else if ((Vector3)TEMP_firstWallPoint != anchor)//If current point = existing point
                        {
                            //NOTE: Check that second point is within range of first
                            TEMP_secondWallPoint = new Vector3(anchor.X, ((Vector3)TEMP_firstWallPoint).Y, anchor.Z);//Ensure that both points have same Y value
                        }
                    }

                    if (TEMP_secondWallPoint != null)
                    {
                        if (Math.Abs(((Vector3)TEMP_firstWallPoint).X - ((Vector3)anchor).X) <= 1 &&
                           Math.Abs(((Vector3)TEMP_firstWallPoint).Z - ((Vector3)anchor).Z) <= 1 &&
                           ((Vector3)TEMP_firstWallPoint).Y == ((Vector3)anchor).Y)
                        {
                            TEMP_secondWallPoint = anchor;
                        }
                        else
                        {

                            //Check that second point is within range of first
                            if (Math.Abs(((Vector3)TEMP_firstWallPoint).X - ((Vector3)TEMP_secondWallPoint).X) <= 1 &&
                                Math.Abs(((Vector3)TEMP_firstWallPoint).Z - ((Vector3)TEMP_secondWallPoint).Z) <= 1 &&
                                ((Vector3)TEMP_firstWallPoint).Y == ((Vector3)TEMP_secondWallPoint).Y)
                            {
                                for (int i = 1; i <= TEMP_WallHeight; i++)
                                {
                                    ChunkManager.GetInstance().AddWall((Vector3)TEMP_firstWallPoint + new Vector3(0, i - 1, 0), (Vector3)TEMP_secondWallPoint + new Vector3(0, i - 1, 0));
                                }

                            }
                            TEMP_firstWallPoint = TEMP_secondWallPoint;
                            TEMP_secondWallPoint = null;
                        }
                    }
                }
            }

        }

        public static void BuildWalls_Released()
        {
            if (TEMP_firstWallPoint != null && TEMP_secondWallPoint != null)
            {
                //Check that second point is within range of first
                if (Math.Abs(((Vector3)TEMP_firstWallPoint).X - ((Vector3)TEMP_secondWallPoint).X) <= 1 &&
                    Math.Abs(((Vector3)TEMP_firstWallPoint).Z - ((Vector3)TEMP_secondWallPoint).Z) <= 1 &&
                    ((Vector3)TEMP_firstWallPoint).Y == ((Vector3)TEMP_secondWallPoint).Y)
                {
                    for (int i = 1; i <= TEMP_WallHeight; i++)
                    {
                        ChunkManager.GetInstance().AddWall((Vector3)TEMP_firstWallPoint + new Vector3(0, i - 1, 0), (Vector3)TEMP_secondWallPoint + new Vector3(0, i - 1, 0));
                    }

                }
            }
            TEMP_secondWallPoint = null;
            TEMP_firstWallPoint = null;
        }

        public static void RemoveWall(Wall wall)
        {
            wall.OnHit();
        }

        public static void PaintFullFace(AbstractTerrainObject terrainObject, Ray ray)
        {
            if(terrainObject is AbstractBlock)
            {
                (terrainObject as AbstractBlock).BlockTextures[Direction.Up]+=1;
                var greatest = Enum.GetValues(typeof(TextureName)).Cast<TextureName>().Max();
                if ((terrainObject as AbstractBlock).BlockTextures[Direction.Up] > greatest)
                {
                    (terrainObject as AbstractBlock).BlockTextures[Direction.Up] = 0;
                }

                (terrainObject as AbstractBlock).SetFaces();
                (terrainObject as AbstractBlock).TEMP_RequestBuildBuffers();
            }
          
        }

        public static void ToggleWallHeight()
        {
            TEMP_WallHeight++;

            if (TEMP_WallHeight > 3)
            {
                TEMP_WallHeight = 1;
            }
        }
    }
}
