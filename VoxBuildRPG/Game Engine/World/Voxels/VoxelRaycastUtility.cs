using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using VoxelRPGGame.GameEngine.Physics;
using VoxelRPGGame.MenuSystem;

namespace VoxelRPGGame.GameEngine.World.Voxels
{
    public static class VoxelRaycastUtility
    {

        /// <summary>
        /// For Debugging, returns all grid positions that the ray passes through
        /// 
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public static List<AbstractWorldObject>/*AbstractWorldObject*/ VoxelIntersectionGrid(Ray ray)
        {
            List<AbstractWorldObject> result = new List<AbstractWorldObject>();

            Vector3? gridLowerBound = ChunkManager.GetInstance().GetMinimumBounds();
            Vector3? gridUpperBound = ChunkManager.GetInstance().GetMaximumBounds();

            if (gridLowerBound == null || gridUpperBound == null)//Will be null if chunk manager has no chunks
            {
                return null;
            }

            float gridLowerX = ((Vector3)gridLowerBound).X, gridUpperX = ((Vector3)gridUpperBound).X;
            float gridLowerY = ((Vector3)gridLowerBound).Y, gridUpperY = ((Vector3)gridUpperBound).Y;
            float gridLowerZ = ((Vector3)gridLowerBound).Z, gridUpperZ = ((Vector3)gridUpperBound).Z;

            float gridWidth = gridUpperX - gridLowerX;
            float gridHeight = gridUpperY - gridLowerY;
            float gridBreadth = gridUpperZ - gridLowerZ;

            Vector3? gridIntersectionPoint =GetNearestGridIntersectionPoint(ray, (Vector3)gridLowerBound, gridWidth, gridHeight, gridBreadth);

            Vector3 rayPosition =  ray.Position;

            //Direction vector.
            float dx = ray.Direction.X;
            float dy = ray.Direction.Y;
            float dz = ray.Direction.Z;

         /*   if (gridIntersectionPoint != null)
            {
                rayPosition = ((Vector3)gridIntersectionPoint) + new Vector3(-dx, -dy, -dz);
            }*/

            //Lower position of block containing origin point.
            float x = GetVoxelCoordinates(rayPosition.X);// GetVoxelLowerCorner(rayPosition.X);// GetVoxelCoordinates(rayPosition.X, 1);
            float y = GetVoxelCoordinates(rayPosition.Y);// GetVoxelLowerCorner(rayPosition.Y);//(float)Math.Floor(rayPosition.Y) - 0.5f;// GetVoxelCoordinates(rayPosition.Y,1);
            float z = GetVoxelCoordinates(rayPosition.Z); //GetVoxelLowerCorner(rayPosition.Z);//(float)Math.Floor(rayPosition.Z) - 0.5f;// GetVoxelCoordinates(rayPosition.Z,1);


         
            float stepX = 1 * signNum(dx);
            float stepY = 1 * signNum(dy);
            float stepZ = 1 * signNum(dz);


            float tMaxX = initMaxT(ray.Position.X, dx);//stepX > 0 ? (x+stepX)-rayPosition.X :x-rayPosition.X;//
            float tMaxY = initMaxT(ray.Position.Y, dy);
            float tMaxZ = initMaxT(ray.Position.Z, dz);

            // The change in t when taking a step (always positive).
            float tDeltaX = stepX / dx;
            float tDeltaY = stepY / dy;
            float tDeltaZ = stepZ / dz;


            // Avoids an infinite loop.
            if (dx == 0 && dy == 0 && dz == 0)
                throw new Exception("Raycast in zero direction!");



            //  Vector3 voxelCoords = new Vector3(blockX, blockY, blockZ);
            //   DirtBlock TEMP = new DirtBlock(BlockShape.Cube, Direction.North);
            //  TEMP.Position = voxelCoords;
            //  result.Add(TEMP);

            while (
                   (stepX > 0 ? x < gridUpperX : x >= gridLowerX) &&
                   (stepY > 0 ? y < gridUpperY : y >= gridLowerY) &&
                   (stepZ > 0 ? z < gridUpperZ : z >= gridLowerZ))
            {
                float blockX = x + 0.5f;
                float blockY = y + 0.5f;
                float blockZ = z + 0.5f;

                Vector3 voxelCoords = new Vector3(blockX, blockY, blockZ);
                DirtBlock TEMP = new DirtBlock(BlockShape.Cube, Direction.North);
                TEMP.Position = voxelCoords;
                result.Add(TEMP);

                if (!(x < gridLowerX || y < gridLowerY || z < gridLowerZ || x >= gridUpperX || y >= gridUpperY || z >= gridUpperZ))
                {
                    //Blocks are stored by centre, but x,y and z are bottom corners of blocks

                    Vector2 chunkKey = ChunkManager.GetInstance().GetChunkCoordinates(voxelCoords);

                    //Get nearest object in block
                    if (ChunkManager.GetInstance().ContainsChunk(chunkKey))
                    {
                        DirtBlock TEMPblock = new DirtBlock(BlockShape.Cube, Direction.North);
                        TEMPblock.Position = voxelCoords;

                        AbstractWorldObject obj = ChunkManager.GetInstance().GetChunk(chunkKey).GetNearestWorldObjectAt(voxelCoords,ray);

                        if (obj != null)
                        {
                            //result = obj;
                            result.Add(obj);
                            break;
                        }

                        //TEMP
                        ChunkManager.GetInstance().GetChunk(chunkKey).wireFrameColour = Color.LimeGreen;
                        ChunkManager.GetInstance().GetChunk(chunkKey).GetCubeWireframe();
                        // break;
                    }

                }
                // tMaxX stores the t-value at which we cross a cube boundary along the
                // X axis, and similarly for Y and Z. Therefore, choosing the least tMax
                // chooses the closest cube boundary. Only the first case of the four
                // has been commented in detail.
                if (tMaxX < tMaxY)
                {
                    if (tMaxX < tMaxZ)
                    {
                        // Update which cube we are now in.
                        x += stepX;
                        //if (x > gridUpperX)
                        //     break;//Outside grid
                        // Adjust tMaxX to the next X-oriented boundary crossing.
                        tMaxX += tDeltaX;
                        // Record the normal vector of the cube face we entered.
                    }
                    else
                    {
                        z += stepZ;
                        //  if (z > gridUpperZ)
                        //      break;//Outside grid
                        tMaxZ += tDeltaZ;

                    }
                }
                else
                {
                    if (tMaxY < tMaxZ)
                    {
                        y += stepY;
                        //  if (y > gridUpperY)
                        //    break;//Outside grid
                        tMaxY += tDeltaY;
                    }
                    else
                    {
                        // Identical to the second case, repeated for simplicity in
                        // the conditionals.
                        z += stepZ;
                        tMaxZ += tDeltaZ;
                    }
                }
            }

            return result;
        }

        public static AbstractWorldObject GetNearestIntersectingWorldObject(Ray ray)
        {
            AbstractWorldObject result = null;

            Vector3? gridLowerBound = ChunkManager.GetInstance().GetMinimumBounds();
            Vector3? gridUpperBound = ChunkManager.GetInstance().GetMaximumBounds();

            if (gridLowerBound == null || gridUpperBound == null)//Will be null if chunk manager has no chunks
            {
                return null;
            }

            float gridLowerX = ((Vector3)gridLowerBound).X, gridUpperX = ((Vector3)gridUpperBound).X;
            float gridLowerY = ((Vector3)gridLowerBound).Y, gridUpperY = ((Vector3)gridUpperBound).Y;
            float gridLowerZ = ((Vector3)gridLowerBound).Z, gridUpperZ = ((Vector3)gridUpperBound).Z;

            float gridWidth = gridUpperX - gridLowerX;
            float gridHeight = gridUpperY - gridLowerY;
            float gridBreadth = gridUpperZ - gridLowerZ;

            Vector3? gridIntersectionPoint = GetNearestGridIntersectionPoint(ray, (Vector3)gridLowerBound, gridWidth, gridHeight, gridBreadth);

            Vector3 rayPosition = ray.Position;
            //Direction vector.
            float dx = ray.Direction.X;
            float dy = ray.Direction.Y;
            float dz = ray.Direction.Z;

        /*    if (gridIntersectionPoint != null)
            {
                rayPosition = ((Vector3)gridIntersectionPoint)+new Vector3(-dx,-dy,-dz);
            }*/
            
            //Lower position of block containing origin point.
            float x = GetVoxelCoordinates(rayPosition.X);// GetVoxelLowerCorner(rayPosition.X);// GetVoxelCoordinates(rayPosition.X, 1);
            float y = GetVoxelCoordinates(rayPosition.Y);// GetVoxelLowerCorner(rayPosition.Y);//(float)Math.Floor(rayPosition.Y) - 0.5f;// GetVoxelCoordinates(rayPosition.Y,1);
            float z = GetVoxelCoordinates(rayPosition.Z); //GetVoxelLowerCorner(rayPosition.Z);//(float)Math.Floor(rayPosition.Z) - 0.5f;// GetVoxelCoordinates(rayPosition.Z,1);


         

            float stepX = 1 * signNum(dx);
            float stepY = 1 * signNum(dy);
            float stepZ = 1 * signNum(dz);


            float tMaxX = initMaxT(ray.Position.X, dx);//stepX > 0 ? (x+stepX)-rayPosition.X :x-rayPosition.X;//
            float tMaxY = initMaxT(ray.Position.Y, dy);
            float tMaxZ = initMaxT(ray.Position.Z, dz);

            // The change in t when taking a step (always positive).
            float tDeltaX = stepX / dx;
            float tDeltaY = stepY / dy;
            float tDeltaZ = stepZ / dz;


            // Avoids an infinite loop.
            if (dx == 0 && dy == 0 && dz == 0)
                throw new Exception("Raycast in zero direction!");

            while (
                   (stepX > 0 ? x < gridUpperX : x >= gridLowerX) &&
                   (stepY > 0 ? y < gridUpperY : y >= gridLowerY) &&
                   (stepZ > 0 ? z < gridUpperZ : z >= gridLowerZ))
            {
                float blockX = x + 0.5f;
                float blockY = y + 0.5f;
                float blockZ = z + 0.5f;

                Vector3 voxelCoords = new Vector3(blockX, blockY, blockZ);

                if (!(x < gridLowerX || y < gridLowerY || z < gridLowerZ || x >= gridUpperX || y >= gridUpperY || z >= gridUpperZ))
                {
                    //Blocks are stored by centre, but x,y and z are bottom corners of blocks

                    Vector2 chunkKey = ChunkManager.GetInstance().GetChunkCoordinates(voxelCoords);

                    //Get nearest object in block
                    if (ChunkManager.GetInstance().ContainsChunk(chunkKey))
                    {
                        DirtBlock TEMPblock = new DirtBlock(BlockShape.Cube, Direction.North);
                        TEMPblock.Position = voxelCoords;

                        AbstractWorldObject obj = ChunkManager.GetInstance().GetChunk(chunkKey).GetNearestWorldObjectAt(voxelCoords,ray);

                        if (obj != null)
                        {
                            result = obj;

                            break;
                        }

                        //TEMP
                        ChunkManager.GetInstance().GetChunk(chunkKey).wireFrameColour = Color.LimeGreen;
                        ChunkManager.GetInstance().GetChunk(chunkKey).GetCubeWireframe();
                        // break;
                    }

                }
                // tMaxX stores the t-value at which we cross a cube boundary along the
                // X axis, and similarly for Y and Z. Therefore, choosing the least tMax
                // chooses the closest cube boundary. Only the first case of the four
                // has been commented in detail.
                if (tMaxX < tMaxY)
                {
                    if (tMaxX < tMaxZ)
                    {
                        // Update which cube we are now in.
                        x += stepX;
                        //if (x > gridUpperX)
                        //     break;//Outside grid
                        // Adjust tMaxX to the next X-oriented boundary crossing.
                        tMaxX += tDeltaX;
                        // Record the normal vector of the cube face we entered.
                    }
                    else
                    {
                        z += stepZ;
                        //  if (z > gridUpperZ)
                        //      break;//Outside grid
                        tMaxZ += tDeltaZ;

                    }
                }
                else
                {
                    if (tMaxY < tMaxZ)
                    {
                        y += stepY;
                        //  if (y > gridUpperY)
                        //    break;//Outside grid
                        tMaxY += tDeltaY;
                    }
                    else
                    {
                        // Identical to the second case, repeated for simplicity in
                        // the conditionals.
                        z += stepZ;
                        tMaxZ += tDeltaZ;
                    }
                }
            }

            return result;
        }

        private static Vector3? GetNearestGridIntersectionPoint(Ray ray, Vector3 gridMinCoord, float gridWidth, float gridHeight, float gridBreadth)
        {
            List<CollisionFace> faces = new List<CollisionFace>();
            //Bottom
            faces.Add(new CollisionFace(gridMinCoord, gridMinCoord + new Vector3(gridWidth, 0, 0), gridMinCoord + new Vector3(gridWidth, 0, gridBreadth), gridMinCoord + new Vector3(0, 0, gridBreadth), new Vector3(0, 1, 0)));

            //Top
            faces.Add(new CollisionFace(gridMinCoord + new Vector3(0, gridHeight, 0), gridMinCoord + new Vector3(gridWidth, gridHeight, 0), gridMinCoord + new Vector3(gridWidth, gridHeight, gridBreadth), gridMinCoord + new Vector3(0, gridHeight, gridBreadth), new Vector3(0, 1, 0)));


            faces.Add(new CollisionFace(gridMinCoord, gridMinCoord + new Vector3(gridWidth, 0, 0), gridMinCoord + new Vector3(gridWidth, gridHeight, 0), gridMinCoord + new Vector3(0, gridHeight, 0), new Vector3(0, 0, 0)));

            faces.Add(new CollisionFace(gridMinCoord, gridMinCoord + new Vector3(0, 0, gridBreadth), gridMinCoord + new Vector3(0, gridHeight, gridBreadth), gridMinCoord + new Vector3(0, gridHeight, 0), new Vector3(0, 0, 0)));



            faces.Add(new CollisionFace(gridMinCoord + new Vector3(gridWidth, 0, 0), gridMinCoord + new Vector3(gridWidth, 0, gridBreadth), gridMinCoord + new Vector3(gridWidth, gridHeight, gridBreadth), gridMinCoord + new Vector3(gridWidth, gridHeight, 0), new Vector3(0, 1, 0)));

            faces.Add(new CollisionFace(gridMinCoord + new Vector3(0, 0, gridBreadth), gridMinCoord + new Vector3(gridWidth, 0, gridBreadth), gridMinCoord + new Vector3(gridWidth, gridHeight, gridBreadth), gridMinCoord + new Vector3(0, gridHeight, gridBreadth), new Vector3(0, 1, 0)));

            CollisionFace nearestFace =GetNearestCollisionFace(faces, ray);

            Vector3? nearestPoint = null;
            if (nearestFace != null)
            {
                nearestPoint = nearestFace.GetRayFaceIntersectionPoint(ray);
            }

            return nearestPoint;
        }

        /// <summary>
        ///Get the initial maxT values - the distance along a specific axis between s and the nearest corresponding
        ///tile border 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="ds"></param>
        /// <param name="tileWidth"></param>
        /// <returns></returns>
        public static float initMaxT(float s, float ds)
        {
           
            float gridBound = GetVoxelCoordinates(s);


            if (ds > 0)
            {
                gridBound += 1;
            }

            //Get the difference between the grid bound and the point
            float sDist = Math.Abs(s - gridBound);



            //if (ds < 0)
            //{
            //    //get the distance between sDeci and lowerPoint
            //    sDist = Math.Abs(sDeci - lowerPoint);
            //}
            //else
            //{
            //    sDist = Math.Abs(upperPoint - sDeci);
            //}

            return Math.Abs(sDist / ds);

            //  Find the smallest positive t such that s+t*ds is an integer.
            //if (ds < 0)
            //{
            //    return initMaxT(-s, -ds/*, Math.Abs(tileWidth)*/);
            //}
            //else
            //{
            //    float sDeci = mod(s, Math.Abs(1/*tileWidth*/));


            //    // problem is now s+t*ds = 1
            //    return ((Math.Abs(1/*tileWidth*/) - sDeci) / ds);
            //}
        }

        /// <summary>
        /// gets the nearest voxel coordinate to a point 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="voxelWidth"></param>
        /// <returns></returns>
        private static float GetVoxelCoordinates(float x)
        {
            float result;
            float xDeci = (float)Math.Round(Math.Abs(x - (int)x), 3);

            if (xDeci >= 0.5f)
            {
                if (x > 0)
                {
                    result = (float)Math.Ceiling(x);
                }
                else
                {
                    result = (float)Math.Floor(x);
                }
            }
            else
            {
                if (x > 0)
                {
                    result = (float)Math.Floor(x);

                }
                else
                {
                    result = (float)Math.Ceiling(x);
                }
            }
            return (result - 0.5f);
        }

        public static float mod(float value, float modulus)
        {
            return ((value % modulus) + modulus) % modulus;
        }


        /// <summary>
        /// Direction to increment x,y,z when stepping.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>

        private static int signNum(float x)
        {
            if (x > 0)
            {
                return 1;
            }
            else if (x < 0)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }


        public static CollisionFace GetNearestCollisionFace(List<CollisionFace> faces, Ray ray)
        {

            float nearestBlockFaceDist = float.MaxValue;
            CollisionFace nearestFace = null;
            foreach (CollisionFace face in faces)
            {
                if (ray != null)
                {
                    float? dist = face.Intersects((Ray)ray); //((Ray)mouseRay).Intersects(face.Value.Plane);

                    if (dist != null && ((float)dist) < nearestBlockFaceDist)
                    {
                        nearestBlockFaceDist = (float)dist;
                        nearestFace = face;
                    }
                }

            }
            return nearestFace;
        }

        public static Vector3? GetNearestWallAnchor(CollisionFace face, Ray ray)
        {
            Vector3? result = null;
            Vector3? pointOnFace = face.GetRayFaceIntersectionPoint((Ray)ray);
            if (pointOnFace != null)
            {
                Vector3 point = (Vector3)pointOnFace;
                //Get nearest wall anchor (i.e val.5)

                float xDecimal;
                float yDecimal;
                float zDecimal;
             
                if (point.X >= 0)
                    xDecimal = (int)point.X + 0.5f;
                else
                    xDecimal = (int)point.X - 0.5f;

                if (point.Y >= 0)
                    yDecimal = (int)point.Y + 0.5f;
                else
                    yDecimal = (int)point.Y - 0.5f;

                if (point.Z >= 0)
                    zDecimal = (int)point.Z + 0.5f;
                else
                    zDecimal = (int)point.Z - 0.5f;

                result = new Vector3(xDecimal, yDecimal, zDecimal);
            }
            return result;
        }
    }
}
