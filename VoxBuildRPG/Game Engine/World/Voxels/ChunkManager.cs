using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxelRPGGame.GameEngine.World.Textures;
using VoxBuildRPG.GameEngine.World.TerrainGeneration;

namespace VoxelRPGGame.GameEngine.World.Voxels
{
    public class ChunkManager
    {
     //   public delegate void OnBuffersChanged(VertexPositionNormalTexture[] vertices, int[] indices);
      //  public event OnBuffersChanged BuffersChanged;

        public readonly int ChunkWidth = 16;
        public readonly int ChunkHeight = 64;//Change this to allow for different world height
        public readonly int ChunkBreadth = 16;
        private static ChunkManager chunkManager;

       Dictionary<Vector2,Chunk> chunks;

       public TextureName TEMPTexture { get; set; }
     //  VertexPositionNormalTexture[] vertices;
     //  int[] indices;

       bool isLoading = true;

        public static ChunkManager GetInstance()
        {
            if (chunkManager == null)
            {
                chunkManager = new ChunkManager();
            }
            return chunkManager;
        }

        //NOTE: Will normally need to save before dispose
        public static void Dispose()
        {
            chunkManager = null;
        }

        private ChunkManager()
        {
            chunks = new Dictionary<Vector2, Chunk>();

            Random r = new Random();
            AddBlock(new Vector3(0,0, 0), new DirtBlock(BlockShape.Cube, Direction.North));

            int width = 32;
            int seedx = 1;
            for (int i = -width; i < width; i++)
            {
                for (int k = -width; k < width; k++)
                {
                    int height = 1;
                    
                    //Sample noise and add to get smoothed noise
                        float octave1 = PerlinNoise.noise((i + seedx) * 0.0001f, (k + seedx) * 0.0001f) * 3f;
                        float octave2 = PerlinNoise.noise((i + seedx) * 0.0005f, (k + seedx) * 0.0005f) * 1f;
                        float octave3 = PerlinNoise.noise((i + seedx) * 0.005f, (k + seedx) * 0.005f) * 1f;
                        float octave4 = PerlinNoise.noise((i + seedx) * 0.01f, (k + seedx) * 0.01f) * 2f;
                        float octave5 = PerlinNoise.noise((i + seedx) * 0.03f, (k + seedx) * 0.03f) * 8f;
                        float lowerGroundHeight = octave1 + octave2 + octave3 + octave4 + octave5;


                        height = (int)lowerGroundHeight;// (int)(10 * PerlinNoise.noise(i, k, 10)); //r.Next(0, 10);
                        int j = height-1;
               //     for (int j = 0; j < height; j++)
                    {

                        AddBlock(new Vector3(i, j, k), new DirtBlock(BlockShape.Cube, Direction.North));

                    }
                }
            }

            //TODO: write AddWall method in chunkManager that decides what chunk to add the wall to
         /*   for (int x = -5; x < 5; x++)
            {
                Chunks[0].AddWall(new Vector3(-0.5f, 0.5f, x+0.5f), new Vector3(-0.5f, 0.5f, x-0.5f));
            }

            Chunks[0].AddWall(new Vector3(-0.5f, 0.5f, 0 - 0.5f), new Vector3(0.5f, 0.5f, 0 - 0.5f));

            Chunks[0].AddWall(new Vector3(0.5f, 0.5f, 0 - 0.5f), new Vector3(1.5f, 0.5f, 0.5f));

            Chunks[0].AddWall(new Vector3(1.5f, 0.5f, 0.5f), new Vector3(2.5f, 0.5f, 1.5f));

            Chunks[0].AddWall(new Vector3(2.5f, 0.5f, 1.5f),new Vector3(3.5f, 0.5f, 1.5f));
            */
            foreach (KeyValuePair<Vector2, Chunk> c in chunks)
            {
                c.Value.BuildBuffers();
            }


            //BuildBuffers();
            //BuildBuffers(verts);
            isLoading = false;
        }

        public  void Initialise()
        {
          
        }

        public  void Update()
        {
          
        }

        //NOTE: Does not work for negative numbers (? check if this is still true)
        public bool AddChunk(Vector2 position)
        {
            bool result = false;
            if (position.X % 16 == 0 && position.Y % 16 == 0)
            {
                if (!chunks.ContainsKey(position))
                {
                    Chunk c =  new Chunk(position,ChunkWidth,ChunkHeight,ChunkBreadth);
                    c.BlockAdded+=OnBlockAdded;
                    c.BlockRemoved += OnBlockRemoved;
                    c.WallAdded += OnWallAdded;
                    c.WallRemoved += OnWallRemoved;
                    chunks.Add(position,c);
                    result = true;
                }
            }
            return result;
        }

#region Blocks
        public bool AddBlock(Vector3 globalPosition,AbstractBlock block)
        {
            bool result = false;

            block.RequestRemove += RemoveBlock;
            block.RequestAddNeighbour += AddBlock;

         /*   if (TEMPTexture == null)
            {
                TEMPTexture = block.TEMPTexture;
            }*/

            int chunkX = 0, chunkY = 0;

            //Get the coordinates of the chunk the block is to be placed in
         /*   if (globalPosition.X >= 0)
            {
                chunkX = (((int)globalPosition.X) / 16) * 16;
            }
            else
            {
                chunkX = (((((int)globalPosition.X)+1) / 16) * 16)-16;
            }

            if (globalPosition.Z >= 0)
            {
                chunkY = (((int)globalPosition.Z) / 16) * 16;
            }
            else
            {
                chunkY = (((((int)globalPosition.Z) + 1) / 16) * 16) - 16;
            }
            */
            Vector2 chunkPos = GetChunkCoordinates(globalPosition); //new Vector2(chunkX, chunkY);
            if (chunks.ContainsKey(chunkPos))
            {
                result=chunks[chunkPos].AddBlock(globalPosition, block);
            }
                //Create the chunk if it doesn't exist
            else
            {
                if (AddChunk(chunkPos))
                {
                    result=chunks[chunkPos].AddBlock(globalPosition, block);
                }
            }

            return result;
        }

        public bool RemoveBlock(Vector3 globalPosition)
        {
            bool result = false;
            int chunkX = 0, chunkY = 0;

            //Get the coordinates of the chunk the block is to be placed in
        /*    if (globalPosition.X >= 0)
            {
                chunkX = (((int)globalPosition.X) / 16) * 16;
            }
            else
            {
                chunkX = (((((int)globalPosition.X) + 1) / 16) * 16) - 16;
            }

            if (globalPosition.Z >= 0)
            {
                chunkY = (((int)globalPosition.Z) / 16) * 16;
            }
            else
            {
                chunkY = (((((int)globalPosition.Z) + 1) / 16) * 16) - 16;
            }*/

            Vector2 chunkPos = GetChunkCoordinates(globalPosition); //new Vector2(chunkX, chunkY);
            if (chunks.ContainsKey(chunkPos))
            {
                chunks[chunkPos].RemoveBlock(globalPosition);
            }
            return result;
        }

#endregion
        #region Walls

        public bool AddWall(Vector3 globalFirstPos, Vector3 globalSecondPos)
        {
            bool result = false;

            //Find chunk the wall should go in


            //Get the nearest block (whole number) position of each wall coordinate
            Vector3 firstPosBlockCoord = new Vector3(globalFirstPos.X - 0.5f, globalFirstPos.Y, globalFirstPos.Z - 0.5f);
            Vector3 secondPosBlockCoord = new Vector3(globalSecondPos.X - 0.5f, globalSecondPos.Y, globalSecondPos.Z - 0.5f);

            if (firstPosBlockCoord.Y == secondPosBlockCoord.Y)//Only continue if both have same Up coordinate
            {

                //Get the midpoint between the coords
                Vector3 midpoint = new Vector3((firstPosBlockCoord.X + secondPosBlockCoord.X) / 2, firstPosBlockCoord.Y, (firstPosBlockCoord.Z + secondPosBlockCoord.Z) / 2);
                //NB: Case where both are in different chunks - develop strategy to choose which chunk wall should go in
                //Get chunk space coordinate (i.e. whole number values)
                Vector3 chunkSpaceMidpoint = new Vector3((int)midpoint.X, (int)midpoint.Y, (int)midpoint.Z);

                //Map block coordinates to correct chunk
                Vector2 chunkPos = GetChunkCoordinates(chunkSpaceMidpoint);

                if (chunks.ContainsKey(chunkPos))
                {
                    chunks[chunkPos].AddWall(globalFirstPos, globalSecondPos,RemoveWall);
                }
                //Create the chunk if it doesn't exist
                else
                {
                    if (AddChunk(chunkPos))
                    {
                        chunks[chunkPos].AddWall(globalFirstPos, globalSecondPos, RemoveWall);
                    }
                }
                //NB Event handlers below
            }
            return result;
        }

        public bool RemoveWall(Vector3 globalFirstPos, Vector3 globalSecondPos)
        {
            bool result = false;

            //Find chunk the wall should go in


            //Get the nearest block (whole number) position of each wall coordinate
            Vector3 firstPosBlockCoord = new Vector3(globalFirstPos.X - 0.5f, globalFirstPos.Y, globalFirstPos.Z - 0.5f);
            Vector3 secondPosBlockCoord = new Vector3(globalSecondPos.X - 0.5f, globalSecondPos.Y, globalSecondPos.Z - 0.5f);

            if (firstPosBlockCoord.Y == secondPosBlockCoord.Y)//Only continue if both have same Up coordinate
            {

                //Get the midpoint between the coords
                Vector3 midpoint = new Vector3((firstPosBlockCoord.X + secondPosBlockCoord.X) / 2, firstPosBlockCoord.Y, (firstPosBlockCoord.Z + secondPosBlockCoord.Z) / 2);
                //NB: Case where both are in different chunks - develop strategy to choose which chunk wall should go in
                //Get chunk space coordinate (i.e. whole number values)
                Vector3 chunkSpaceMidpoint = new Vector3((int)midpoint.X, (int)midpoint.Y, (int)midpoint.Z);

                //Map block coordinates to correct chunk
                Vector2 chunkPos = GetChunkCoordinates(chunkSpaceMidpoint);

                if (chunks.ContainsKey(chunkPos))
                {
                    chunks[chunkPos].RemoveWall(globalFirstPos, globalSecondPos);
                }
            }

            return result;
        }


        
        #endregion

        /// <summary>
        /// Gets the coordinates of the chunk in which the globalCoordinates lies
        /// </summary>
        /// <param name="globalCoordinates"></param>
        /// <returns></returns>
        public Vector2 GetChunkCoordinates(Vector3 globalCoordinates)
        {

            int chunkX = 0, chunkY = 0;
            //Get the coordinates of the chunk the block is to be placed in
            if (globalCoordinates.X >= 0)
            {
                chunkX = (((int)globalCoordinates.X) / 16) * 16;
            }
            else
            {
                chunkX = (((((int)globalCoordinates.X) + 1) / 16) * 16) - 16;
            }

            if (globalCoordinates.Z >= 0)
            {
                chunkY = (((int)globalCoordinates.Z) / 16) * 16;
            }
            else
            {
                chunkY = (((((int)globalCoordinates.Z) + 1) / 16) * 16) - 16;
            }


            return new Vector2(chunkX, chunkY);
        }



        public Chunk GetChunk(Vector2 chunkKey)
        {
            Chunk result = null;

            if (chunks.ContainsKey(chunkKey))
            {
                result = chunks[chunkKey];
            }

            return result;
        }

        public Dictionary<Direction,Chunk> GetNeighbouringChunks(List<Direction> neighbourDirection, Vector2 chunkKey)
        {
            Dictionary<Direction, Chunk> result = new Dictionary<Direction, Chunk>();

            foreach (Direction dir in neighbourDirection)
            {
                if (dir == Direction.North)
                {
                    result.Add(dir,GetChunk(new Vector2(chunkKey.X + 16, chunkKey.Y)));
                }

                else if (dir == Direction.South)
                {
                    result.Add(dir, GetChunk(new Vector2(chunkKey.X - 16, chunkKey.Y)));
                }
                else if (dir == Direction.East)
                {
                    result.Add(dir, GetChunk(new Vector2(chunkKey.X, chunkKey.Y + 16)));
                }

                else if (dir == Direction.West)
                {
                    result.Add(dir, GetChunk(new Vector2(chunkKey.X, chunkKey.Y - 16)));
                }
            }

            return result;
        }


   /*     public bool BuildBuffers(List<VertexPositionNormalTexture> unindexedVertices)
        {
            //NOTE: Will need to be rewritten to accept a vertex list and index list once chunks are internally
            //managing indexing
            bool result = false;

            List<int> indexList = new List<int>();
            List<VertexPositionNormalTexture> vertexList = new List<VertexPositionNormalTexture>();

            Dictionary<Vector3, int> usedVertices = new Dictionary<Vector3, int>();

            for (int i = 0; i < unindexedVertices.Count; i++)
            {
                if (usedVertices.ContainsKey(unindexedVertices[i].Position))
                {
                    indexList.Add(usedVertices[unindexedVertices[i].Position]);
                }
                else
                {
                    vertexList.Add(unindexedVertices[i]);
                    indexList.Add(vertexList.Count - 1);
                    usedVertices.Add(unindexedVertices[i].Position, vertexList.Count - 1);
                }
                
            }

            indices = indexList.ToArray<int>();
            vertices = vertexList.ToArray<VertexPositionNormalTexture>();

            return result;
        }*/

        //public bool BuildBuffers()
        //{
        //    List<VertexPositionNormalTexture> chunkVerts = new List<VertexPositionNormalTexture>();
        //    List<int> chunkIndices = new List<int>();
        //    int offset = 0;
        //    foreach (KeyValuePair<Vector2, Chunk> c in chunks)
        //    {
        //       // c.Value.BuildBuffers();
        //        chunkVerts.AddRange(c.Value.Vertices);
        //        for (int i = 0; i < c.Value.Indices.Length; i++)
        //        {
        //            chunkIndices.Add(c.Value.Indices[i] + offset);
        //        }
        //        offset = chunkVerts.Count;
        //        //  chunkIndices.AddRange(c.Value.Indices);
        //        /* foreach(AbstractBlock b in c.Value.GetAllBlocks())
        //         {
        //             verts.AddRange(b.Faces);
        //         }*/
        //    }
        //    this.vertices = chunkVerts.ToArray();
        //    this.indices = chunkIndices.ToArray();
        //    return true;//BuildBuffersFromChunks(chunkVerts.ToArray<VertexPositionNormalTexture>(), chunkIndices.ToArray<int>());
        //}


      /*  public bool BuildBuffersFromChunks(VertexPositionNormalTexture[] blockVertices, int[] blockindices)
        {
            bool result = false;

            List<int> indexList = new List<int>();
            List<VertexPositionNormalTexture> vertexList = new List<VertexPositionNormalTexture>();

            Dictionary<Vector3, int> usedVertices = new Dictionary<Vector3, int>();

            for (int i = 0; i < blockindices.Length; i++)
            {
                if (usedVertices.ContainsKey(blockVertices[blockindices[i]].Position))
                {
                    indexList.Add(usedVertices[blockVertices[blockindices[i]].Position]);
                }
                else
                {
                    vertexList.Add(blockVertices[blockindices[i]]);
                    indexList.Add(vertexList.Count - 1);
                    usedVertices.Add(blockVertices[blockindices[i]].Position, vertexList.Count - 1);
                }

            }

            indices = indexList.ToArray<int>();
            vertices = vertexList.ToArray<VertexPositionNormalTexture>();

            return result;
        }*/


#region Event Handlers
        public void OnBlockAdded(Vector2 chunkKey, int blockX, int blockY, int blockZ, AbstractBlock block)
        {
            List<Direction> neighbourDirections = new List<Direction>();
            int x = 0, y = 0, z = 0;

            y = blockY;
            if (blockX == 15)
            {
                neighbourDirections.Add(Direction.North);
               // x = -1;
            }

            if (blockX == 0)
            {
                neighbourDirections.Add(Direction.South);
             //   x = 16;
            }

            if (blockZ == 15)
            {
                neighbourDirections.Add(Direction.East);
             //   z = -1;
            }

            if (blockZ == 0)
            {
                neighbourDirections.Add(Direction.West);
            //    z = 16;
            }


            foreach (KeyValuePair<Direction,Chunk> c in GetNeighbouringChunks(neighbourDirections, chunkKey))
            {
                if (c.Value != null)
                {
                    if (c.Key==Direction.North)
                    {
                         x = -1;
                         z = blockZ;
                    }

                    if (c.Key == Direction.South)
                    {
                         x = 16;
                         z = blockZ;
                    }

                    if (c.Key == Direction.East)
                    {
                        z = -1;
                        x = blockX;
                    }

                    if (c.Key == Direction.West)
                    {
                          z = 16;
                          x = blockX;
                    }

                    c.Value.HideBlockNeighbourFaces(x, y, z, block);
                    if (!isLoading)
                    {
                        c.Value.BuildBuffers();
                    }

                }
            }

            if (!isLoading)
            {
                if (chunks.ContainsKey(chunkKey))
                {
                    chunks[chunkKey].BuildBuffers();
                }
                //BuildBuffers();
               /* if (BuffersChanged != null)
                {
                    BuffersChanged(Vertices, Indices);
                }*/
            }

        }

        public void OnBlockRemoved(Vector2 chunkKey, int blockX, int blockY, int blockZ)
        {
            List<Direction> neighbourDirections = new List<Direction>();
            int x = 0, y = 0, z = 0;

            y = blockY;
            if (blockX == 15)
            {
                neighbourDirections.Add(Direction.North);
                // x = -1;
            }

            if (blockX == 0)
            {
                neighbourDirections.Add(Direction.South);
                //   x = 16;
            }

            if (blockZ == 15)
            {
                neighbourDirections.Add(Direction.East);
                //   z = -1;
            }

            if (blockZ == 0)
            {
                neighbourDirections.Add(Direction.West);
                //    z = 16;
            }


            foreach (KeyValuePair<Direction, Chunk> c in GetNeighbouringChunks(neighbourDirections, chunkKey))
            {
                if (c.Value != null)
                {
                    if (c.Key == Direction.North)
                    {
                        x = -1;
                        z = blockZ;
                    }

                    if (c.Key == Direction.South)
                    {
                        x = 16;
                        z = blockZ;
                    }

                    if (c.Key == Direction.East)
                    {
                        z = -1;
                        x = blockX;
                    }

                    if (c.Key == Direction.West)
                    {
                        z = 16;
                        x = blockX;
                    }

                    c.Value.ShowBlockNeighbourFaces(x, y, z);
                    if (!isLoading)
                    {
                        c.Value.BuildBuffers();
                    }
                }
            }

            if (!isLoading)
            {
                if (chunks.ContainsKey(chunkKey))
                {
                    chunks[chunkKey].BuildBuffers();
                }
                //BuildBuffers();
                /*if (BuffersChanged != null)
                {
                    BuffersChanged(Vertices, Indices);
                }*/
            }

        }


        public void OnWallAdded(Vector2 chunkKey,Vector3 firstPosition, Vector3 SecondPosition)
        {
            //NOTE:Inform neighbouring walls that wall has been added 

            if (!isLoading)
            {
                if (chunks.ContainsKey(chunkKey))
                {
                    chunks[chunkKey].BuildBuffers();
                }
            }
        }
        public void OnWallRemoved(Vector2 chunkKey, Vector3 firstPosition, Vector3 SecondPosition)
        {
            //NOTE:Inform neighbouring walls that wall has been removed 

            if (!isLoading)
            {
                if (chunks.ContainsKey(chunkKey))
                {
                    chunks[chunkKey].BuildBuffers();
                }
            }
        }

#endregion

        public List<AbstractBlock> GetNearbyBlocks(Vector3 position, int radX,int radY,int radZ)
        {

            List<AbstractBlock> result = new List<AbstractBlock>();
            float x,y,z;

                x = (float)Math.Round(position.X, MidpointRounding.AwayFromZero);

                y =(float)Math.Round(position.Y, MidpointRounding.AwayFromZero);

                z = (float)Math.Round(position.Z, MidpointRounding.AwayFromZero);

            Vector3 chunkPosition = new Vector3(x, y, z);

            //Get Blocks Below position
            for (int j = (int)y - radY; j < y; j++)
            {
                for (int i = (int)(x - radX); i <= x + radX; i++)
                {
                    for (int k = (int)(z - radZ); k <= z + radZ; k++)
                    {
                        AbstractBlock b = GetBlockAt(new Vector3(i, j, k));
                        if (b != null)
                        {
                            result.Add(b);
                        }
                    }
                }
            }

            //Get Blocks Around and Above position
                for (int i = (int)(x - radX); i <= x + radX; i++)
                {
                    for (int k = (int)(z - radZ); k <= z + radZ; k++)
                    {
                        for (int j = (int)y; j <= y + radY; j++)
                        {
                            AbstractBlock b = GetBlockAt(new Vector3(i, j, k));
                            if (b != null)
                            {
                                result.Add(b);
                            }
                        }
                    }
            }

            
          //  AbstractBlock b = GetBlockAt(new Vector3(0,10,0));
          //  result.Add(b);

            return result;
        }


        public AbstractBlock GetBlockAt(Vector3 globalPosition)
        {
            AbstractBlock result = null;
            int chunkX = 0, chunkY = 0;
             //Get the coordinates of the chunk the block is to be placed in
            if (globalPosition.X >= 0)
            {
                chunkX = (((int)globalPosition.X) / 16) * 16;
            }
            else
            {
                chunkX = (((((int)globalPosition.X) + 1) / 16) * 16) - 16;
            }

            if (globalPosition.Z >= 0)
            {
                chunkY = (((int)globalPosition.Z) / 16) * 16;
            }
            else
            {
                chunkY = (((((int)globalPosition.Z) + 1) / 16) * 16) - 16;
            }

            Vector2 chunkPos = new Vector2(chunkX, chunkY);
            if (chunks.ContainsKey(chunkPos))
            {
                result=chunks[chunkPos].GetBlockAt(globalPosition);
            }

            return result;
        }


        public Vector3 ? GetMinimumBounds()
        {
            Vector3 ? result=null;

            //Get the lowest coordinate of chunks in manager 
            if (chunks.Keys.Count > 0)
            {
                List<Vector2> chunkKeys = chunks.Keys.OrderBy(x => x.X).ThenBy(x => x.Y).ToList<Vector2>();

                result = new Vector3(chunkKeys[0].X-0.5f, 0-0.5f, chunkKeys[0].Y-0.5f);//lower bounds of blocks are 0.5 less
            }

            return result;
        }

        public Vector3 ? GetMaximumBounds()
        {
            Vector3? result = null;
            //Get the highest coordinate of chunks in manager and add width and breadth 
            if (chunks.Keys.Count > 0)
            {
                List<Vector2> chunkKeys = chunks.Keys.OrderByDescending(x => x.X).ThenByDescending(x => x.Y).ToList<Vector2>();
                result = new Vector3(chunkKeys[0].X+ChunkWidth - 0.5f, ChunkHeight - 0.5f, chunkKeys[0].Y +ChunkBreadth- 0.5f);//Highest key + chunk width-0.5 gets highest point
            }
            return result;
        }


        #region Properties


        public bool ContainsChunk(Vector2 chunkKey)
        {
          bool result = false;
          if (chunks.ContainsKey(chunkKey))
          {
              result = true;
          }
          return result;

        }

        public List<Chunk> Chunks
        {
            get
            {
                return chunks.Values.ToList<Chunk>();
            }
        }

       /* public VertexPositionNormalTexture[] Vertices
        {
            get
            {
                return vertices;
            }
        }

        public int[] Indices
        {
            get
            {
                return indices;
            }

        }*/
#endregion
    }
}
