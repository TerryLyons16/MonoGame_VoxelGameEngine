using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxelRPGGame.MenuSystem;
using VoxelRPGGame.MenuSystem.Screens;
using VoxelRPGGame.GameEngine.World.Building;
using VoxelRPGGame.GameEngine.World.Textures;
using VoxelRPGGame.GameEngine.Physics;
using VoxelRPGGame.GameEngine.Rendering;


namespace VoxelRPGGame.GameEngine.World.Voxels
{

    /// <summary>
    /// 3D Grid of Blocks
    /// </summary>
    public class Chunk
    {
        public delegate void OnBlockAdded(Vector2 chunkKey, int blockX, int blockY, int blockZ, AbstractBlock block);
        public event OnBlockAdded BlockAdded;

        public delegate void OnBlockRemoved(Vector2 chunkKey, int blockX, int blockY, int blockZ);
        public event OnBlockRemoved BlockRemoved;

        public delegate void OnWallAdded(Vector2 chunkKey, Vector3 firstPosition, Vector3 SecondPosition);
        public event OnWallAdded WallAdded;

        public delegate void OnWallRemoved(Vector2 chunkKey, Vector3 firstPosition, Vector3 SecondPosition);
        public event OnWallRemoved WallRemoved;

        protected VertexPositionNormalTexture[] vertices;
        protected int[] indices;

        protected VertexBuffer vertexBuffer = null;
        protected IndexBuffer indexBuffer = null;

        public TextureName TEMPTexture { get; set; }


#region Blocks
        /// <summary>
        /// x,y,x y is Up axis
        /// </summary>
        protected AbstractBlock[,,] blocks;
#endregion

        #region Walls
        /// <summary>
        /// Stores all wall objects in chunk
        /// </summary>
        Dictionary<Vector3,Dictionary<Vector3,Wall>> walls;//Top-level key is start position of wall, second-level key is end position

        #endregion

        /// <summary>
        /// The global position of the Chunk's 0,0 coordinate (NOTE: make it a Vector 3 if chunks are to be stacked vertically)
        /// </summary>
        protected Vector2 position;

        protected List<VertexPositionColor> chunkWireframe;
        public Color wireFrameColour = Color.Red;

        public Chunk(Vector2 pos,int width, int height, int breadth)
        {
            position = new Vector2(pos.X, pos.Y);

            int x = width;
            int y = height;
            int z = breadth;
            blocks = new AbstractBlock[x,y,z];

            walls = new Dictionary<Vector3,Dictionary<Vector3,Wall>>();


            GetCubeWireframe();
        }

#region Blocks
        /// <summary>
        /// Attempts to add a block at the specified position
        /// </summary>
        /// <param name="position">NOTE: Will need to determine that global position is within chunk</param>
        /// <returns></returns>
        public bool AddBlock(Vector3 globalPosition,AbstractBlock block)
        {
            if (TEMPTexture == null)
            {
                TEMPTexture = block.TEMPTexture;
            }

            //--------------TEMP------------------------------
            block.OnBuildBuffersRequest += BuildBuffers;
            //-------------------------------------------------
            bool result = false;

            //NOTE: Will need to set the block's position here to ensure valid placement
            Vector3 localPosition =new Vector3(Math.Abs(Math.Abs(globalPosition.X)-Math.Abs(position.X)),globalPosition.Y,Math.Abs(Math.Abs(globalPosition.Z)-Math.Abs(position.Y))); 

           // if(localPosition.X<blocks.GetLength(0)
            //NOTE: Validate position
            if ((int)localPosition.X >= 0 && (int)localPosition.Y >= 0 && (int)localPosition.Z >= 0
                && (int)localPosition.X < blocks.GetLength(0) && (int)localPosition.Y < blocks.GetLength(1) && (int)localPosition.Z < blocks.GetLength(2))
            {
                if (blocks[(int)localPosition.X, (int)localPosition.Y, (int)localPosition.Z] == null)
                {
                    blocks[(int)localPosition.X, (int)localPosition.Y, (int)localPosition.Z] = block;
                    block.Position = new Vector3(localPosition.X + position.X, localPosition.Y, localPosition.Z + position.Y);
                    //NOTE: May need to move this to OnBlockAdded in chunkManager
                    HideBlockNeighbourFaces((int)localPosition.X, (int)localPosition.Y, (int)localPosition.Z, blocks[(int)localPosition.X, (int)localPosition.Y, (int)localPosition.Z]);

                    if (BlockAdded != null)
                    {
                        BlockAdded(position, (int)localPosition.X, (int)localPosition.Y, (int)localPosition.Z, blocks[(int)localPosition.X, (int)localPosition.Y, (int)localPosition.Z]);
                    }
                    result = true;
                }
            }


            return result;
        }

        /// <summary>
        /// Attempts to remove a block at the specified position
        /// </summary>
        /// <param name="position">NOTE: Will need to determine that global position is within chunk</param>
        /// <returns></returns>
        public bool RemoveBlock(Vector3 globalPosition)
        {
             bool result = false;

            //NOTE: Will need to set the block's position here to ensure valid placement
            Vector3 localPosition =new Vector3(Math.Abs(Math.Abs(globalPosition.X)-Math.Abs(position.X)),globalPosition.Y,Math.Abs(Math.Abs(globalPosition.Z)-Math.Abs(position.Y))); 

           // if(localPosition.X<blocks.GetLength(0)
            //NOTE: Validate position

            if (blocks[(int)localPosition.X, (int)localPosition.Y, (int)localPosition.Z] != null)
            {
                blocks[(int)localPosition.X, (int)localPosition.Y, (int)localPosition.Z] = null;
                //NOTE: May need to move this to OnBlockRemoved in chunkManager
                ShowBlockNeighbourFaces((int)localPosition.X, (int)localPosition.Y, (int)localPosition.Z);
                if (BlockRemoved != null)
                {
                    BlockRemoved(position, (int)localPosition.X, (int)localPosition.Y, (int)localPosition.Z);
                }
            }


            return result;
        }

        public AbstractBlock GetBlockAt(Vector3 globalPosition)
        {
            AbstractBlock result = null;
            Vector3 localPosition = new Vector3(Math.Abs(Math.Abs(globalPosition.X) - Math.Abs(position.X)), globalPosition.Y, Math.Abs(Math.Abs(globalPosition.Z) - Math.Abs(position.Y)));
            if ((int)localPosition.X < blocks.GetLength(0) && (int)localPosition.Y < blocks.GetLength(1) && (int)localPosition.Z < blocks.GetLength(2)
                &&(int)localPosition.X >=0 && (int)localPosition.Y >=0 && (int)localPosition.Z >=0)
            {
                if (blocks[(int)localPosition.X, (int)localPosition.Y, (int)localPosition.Z] != null)
                {
                    result = blocks[(int)localPosition.X, (int)localPosition.Y, (int)localPosition.Z];
                }
            }

            return result;
        }


        /// <summary>
        /// Sets the visibility ob a blocks neighbours' faces 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public bool HideBlockNeighbourFaces(int x, int y, int z, AbstractBlock block)
        {
            bool result = false;

            foreach(KeyValuePair<Direction,AbstractBlock> pair in GetNeighbours(x,y,z))
            {
                if (pair.Key == Direction.West)
                {
                    pair.Value.OnNeighbourAdded(Direction.East, block);
                    block.OnNeighbourAdded(Direction.West, pair.Value);
                }

                if (pair.Key == Direction.East)
                {
                    pair.Value.OnNeighbourAdded(Direction.West, block);
                    block.OnNeighbourAdded(Direction.East, pair.Value);
                }

                if (pair.Key == Direction.North)
                {
                    pair.Value.OnNeighbourAdded(Direction.South, block);
                    block.OnNeighbourAdded(Direction.North, pair.Value);
                }

                if (pair.Key == Direction.South)
                {
                    pair.Value.OnNeighbourAdded(Direction.North, block);
                    block.OnNeighbourAdded(Direction.South, pair.Value);
                }

                if (pair.Key == Direction.Up)
                {
                    pair.Value.OnNeighbourAdded(Direction.Down, block);
                    block.OnNeighbourAdded(Direction.Up, pair.Value);
                }

                if (pair.Key == Direction.Down)
                {
                    pair.Value.OnNeighbourAdded(Direction.Up, block);
                    block.OnNeighbourAdded(Direction.Down, pair.Value);
                }
            }


            return result;
        }

        public bool ShowBlockNeighbourFaces(int x, int y, int z)
        {
            bool result = false;

            foreach (KeyValuePair<Direction, AbstractBlock> pair in GetNeighbours(x, y, z))
            {
                if (pair.Key == Direction.West)
                {
                    pair.Value.OnNeighbourRemoved(Direction.East);
                  
                }

                if (pair.Key == Direction.East)
                {
                    pair.Value.OnNeighbourRemoved(Direction.West);
                
                }

                if (pair.Key == Direction.North)
                {
                    pair.Value.OnNeighbourRemoved(Direction.South);
                   
                }

                if (pair.Key == Direction.South)
                {
                    pair.Value.OnNeighbourRemoved(Direction.North);
                  
                }

                if (pair.Key == Direction.Up)
                {
                    pair.Value.OnNeighbourRemoved(Direction.Down);
                    
                }

                if (pair.Key == Direction.Down)
                {
                    pair.Value.OnNeighbourRemoved(Direction.Up);
                   
                }
            }


            return result;
        }

        protected Dictionary<Direction,AbstractBlock> GetNeighbours(int x, int y, int z)
        {
            Dictionary<Direction,AbstractBlock> result = new Dictionary<Direction,AbstractBlock>();

            if ((x < blocks.GetLength(0) && x >= 0) && (y < blocks.GetLength(1) && y >= 0) && (z < blocks.GetLength(2) & z >= 0))
            {
                //Block below
                if (y - 1 >= 0)
                {
                    if (blocks[x, y - 1, z] != null)
                    {
                        result.Add(Direction.Down,blocks[x, y - 1, z]);
                    }
                }

                if (y + 1 < blocks.GetLength(1))
                {
                    if (blocks[x, y + 1, z] != null)
                    {
                        result.Add(Direction.Up,blocks[x, y + 1, z]);
                    }
                }

                if (x - 1 >= 0)
                {
                    if (blocks[x-1, y, z] != null)
                    {
                        result.Add(Direction.South,blocks[x - 1, y, z]);
                    }
                }

                if (x + 1 < blocks.GetLength(0))
                {
                    if (blocks[x + 1, y, z] != null)
                    {
                        result.Add(Direction.North,blocks[x + 1, y, z]);
                    }
                }

                if (z - 1 >= 0)
                {
                    if (blocks[x, y, z-1] != null)
                    {
                        result.Add(Direction.West,blocks[x, y, z-1]);
                    }
                }

                if (z + 1 < blocks.GetLength(2))
                {
                    if (blocks[x, y, z+1] != null)
                    {
                        result.Add(Direction.East,blocks[x, y, z + 1]);
                    }
                }

            }
                //External chunk location
            else if (x >= blocks.GetLength(0) || x < 0)
            {
                if (x - 1 >= 0)
                {
                    if (blocks[x - 1, y, z] != null)
                    {
                        result.Add(Direction.South, blocks[x - 1, y, z]);
                    }
                }

                else if (x + 1 < blocks.GetLength(0))
                {
                    if (blocks[x + 1, y, z] != null)
                    {
                        result.Add(Direction.North, blocks[x + 1, y, z]);
                    }
                }
              
            }

            else if(z >= blocks.GetLength(2) || z < 0)
            {
                 if (z - 1 >= 0)
                {
                    if (blocks[x, y, z - 1] != null)
                    {
                        result.Add(Direction.West, blocks[x, y, z - 1]);
                    }
                }

                else if (z + 1 < blocks.GetLength(2))
                {
                    if (blocks[x, y, z + 1] != null)
                    {
                        result.Add(Direction.East, blocks[x, y, z + 1]);
                    }
                }
            }


            return result;
        }

        public List<AbstractBlock> GetAllBlocks()
        {
            List<AbstractBlock> result = new List<AbstractBlock>();

            for (int i = 0; i < blocks.GetLength(0); i++)
            {
                for (int  j= 0; j < blocks.GetLength(1); j++)
                {
                    for (int k = 0; k < blocks.GetLength(2); k++)
                    {
                        if(blocks[i,j,k]!=null)
                            result.Add(blocks[i, j, k]);
                    }
                }
            }

            return result;
        }
#endregion
#region Walls

        public bool AddWall(Vector3 firstPos, Vector3 secondPos, Wall.OnRequestRemove onRequestRemove)
        {
            bool result = false;

            //NOTE: Need control to ensure that positions are in chunk
            //NOTE: Need to ensure that y-positions are equal i.e. no sloping walls
            Vector3 startPosition, endPosition;


            //Decide which is start and end point.;
            if (walls.ContainsKey(secondPos))//If secondPos is already top-level key, use it as startPosition
            {
                startPosition = secondPos;
                endPosition = firstPos;
            }
            else
            {
                startPosition = firstPos;
                endPosition = secondPos;
            }

           
           //NOTE: Need to check that wall not intersect existing walls (Check at wall placement)
            Wall wall = new Wall(startPosition,endPosition);
            wall.RequestRemove += onRequestRemove;

            if (walls.ContainsKey(startPosition))//A wall exists that starts at the same position
            {
                if (!walls[startPosition].ContainsKey(endPosition))//Wall doesn't already exist
                {
                    walls[startPosition].Add(endPosition, wall);
                    result = true;
                }
            }
            else//No wall exists that starts at the same position
            {
                Dictionary<Vector3, Wall> wallDict = new Dictionary<Vector3, Wall>();
                wallDict.Add(endPosition, wall);
                walls.Add(startPosition, wallDict);
                result = true;
                WallAdded(position, startPosition, endPosition);
            }

            //NB: Fire event to chunk manager

            return result;
        }


        public bool RemoveWall(Vector3 firstPos, Vector3 secondPos)
        {
            bool result = false;

            //NOTE: Need control to ensure that positions are in chunk
            //NOTE: Need to ensure that y-positions are equal i.e. no sloping walls
            Vector3 startPosition, endPosition;


            //Decide which is start and end point.;
            if (walls.ContainsKey(secondPos))//If secondPos is already top-level key, use it as startPosition
            {
                startPosition = secondPos;
                endPosition = firstPos;
            }
            else
            {
                startPosition = firstPos;
                endPosition = secondPos;
            }


            if (walls.ContainsKey(startPosition))
            {
                if(walls[startPosition].ContainsKey(endPosition))
                {
                    walls[startPosition].Remove(endPosition);
                    result = true;
                    WallRemoved(position, startPosition, endPosition);
                }

                if (walls[startPosition].Count == 0)//Remove top-level key if all of the walls it stores have been deleted
                {
                    walls.Remove(startPosition);
                }
            }

            //NB: Fire event to chunk manager

            return result;
        }


        public Wall GetWallAt(Vector3 firstPosition, Vector3 secondPosition)
        {
            Wall result = null;

            if (walls.ContainsKey(firstPosition))
            {
                if (walls[firstPosition].ContainsKey(secondPosition))
                {
                    result = walls[firstPosition][secondPosition];
                }
            }
            else if (walls.ContainsKey(secondPosition))
            {
                if (walls[secondPosition].ContainsKey(firstPosition))
                {
                    result = walls[secondPosition][firstPosition];
                }
            }

            return result;
        }

        #endregion


        /// <summary>
        /// Gets the nearest world object bounded by the voxel box centred at the input coordinates
        /// </summary>
        /// <param name="globalCoordinates"></param>
        /// <returns></returns>
        public AbstractWorldObject GetNearestWorldObjectAt(Vector3 globalCoordinates,Ray ray)
        {
            AbstractWorldObject result = null;

            AbstractBlock block = GetBlockAt(globalCoordinates);

            float? nearestDist=float.MaxValue;
            if (block != null)
            {
                CollisionFace face = VoxelRaycastUtility.GetNearestCollisionFace(block.GetCollisionFaces(), ray);
                if (face != null)
                {
                    nearestDist = face.Intersects(ray);
                    result = block;
                }
            }

            List<Wall> wallsInBlock = new List<Wall>();

            Vector3 blockLowerLeft = globalCoordinates + new Vector3(-0.5f, -0.5f, -0.5f);
            Vector3 blockUpperLeft = globalCoordinates + new Vector3(0.5f, -0.5f, -0.5f);
            Vector3 blockLowerRight = globalCoordinates + new Vector3(-0.5f, -0.5f, 0.5f);
            Vector3 blockUpperRight = globalCoordinates + new Vector3(0.5f, -0.5f, 0.5f);

            //Get walls 
            wallsInBlock.Add(GetWallAt(blockLowerLeft, blockUpperLeft));
            wallsInBlock.Add(GetWallAt(blockLowerLeft, blockLowerRight));
            wallsInBlock.Add(GetWallAt(blockLowerLeft, blockUpperRight));
            wallsInBlock.Add(GetWallAt(blockUpperLeft, blockLowerRight));

            foreach (Wall w in wallsInBlock)
            {
                if (w != null)
                {
                    //Check if ray crosses wall and get nearest point
                   CollisionFace face = VoxelRaycastUtility.GetNearestCollisionFace(w.GetCollisionFaces(), ray);
                   if (face != null)
                   {
                       float?dist = face.Intersects(ray);
                       if (dist != null && nearestDist != null && ((float)dist) < ((float)nearestDist))
                       {
                           nearestDist = dist;
                           result = w;
                       }
                   }
                }
            }
            return result;
        }

        public void DrawChunk()
        {
            ScreenManager.GetInstance().GraphicsDevice.SetVertexBuffer(vertexBuffer);
            ScreenManager.GetInstance().GraphicsDevice.Indices = indexBuffer;

            ShaderManager.GetInstance().DefaultEffect.Texture = TextureAtlas.GetInstance().Atlas;

            ShaderManager.GetInstance().DefaultEffect.TextureEnabled = true;
            ShaderManager.GetInstance().DefaultEffect.VertexColorEnabled = false;


            /*  SamplerState ss = new SamplerState();
              ss.AddressU = TextureAddressMode.Clamp;
              ss.AddressV = TextureAddressMode.Clamp;
          
             ShaderManager.GetInstance().DefaultEffect.GraphicsDevice.SamplerStates[0] = ss;
             *             ScreenManager.GetInstance().GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
              */
            ShaderManager.GetInstance().DefaultEffect.CurrentTechnique.Passes[0].Apply();


            //      DebugScreen.GetInstance().PolysDrawn = vertices.Length / 3;
            //DebugScreen.GetInstance().PolysDrawn += indices.Length / 3;
            //DebugScreen.GetInstance().VertsDrawn += vertices.Length;

            //ScreenManager.GetInstance().GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertices.Length / 3);
            if (vertices.Length > 0)
            {
                ScreenManager.GetInstance().GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Length / 3);
            }
        }


        public bool BuildBuffers()
        {
            List<VertexPositionNormalTexture> unindexedVertices = new List<VertexPositionNormalTexture>();
            List<int> indices = new List<int>();
            int offset = 0;
            foreach (AbstractBlock b in GetAllBlocks())
            {
                unindexedVertices.AddRange(b.Vertices);
                for (int i = 0; i < b.Indices.Length; i++)
                {
                    indices.Add(b.Indices[i] + offset);
                }
                offset = unindexedVertices.Count;
            }


            foreach (var wall in walls)
            {
                foreach (var w in wall.Value)
                {
                    unindexedVertices.AddRange(w.Value.Vertices);
                    for (int i = 0; i < w.Value.Indices.Length; i++)
                    {
                        indices.Add(w.Value.Indices[i] + offset);
                    }
                    offset = unindexedVertices.Count;
                }
            }


            this.vertices = unindexedVertices.ToArray();
            this.indices = indices.ToArray();
            SetBuffers(this.vertices, this.indices);
            return true;//BuildBuffersFromBlocks(unindexedVertices.ToArray<VertexPositionNormalTexture>(), indices.ToArray<int>());
        }
     /*   protected bool BuildBuffersFromBlocks(VertexPositionNormalTexture[] blockVertices, int[] blockindices)
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

            if (indices != null && indices.Length > 0 && vertices != null && vertices.Length > 0)
            {
                result = true;
            }

            return result;
        }*/

   /*     public void SetBuffers()
        {
            SetBuffers(vertices,indices);
        }*/

        protected void SetBuffers(VertexPositionNormalTexture[] vertices, int[] indices)
        {
            if (vertices.Length > 0)
            {
                vertexBuffer = new VertexBuffer(ScreenManager.GetInstance().GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration,
                 vertices.Length, BufferUsage.WriteOnly);

                vertexBuffer.SetData(vertices);

                indexBuffer = new IndexBuffer(ScreenManager.GetInstance().GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
                indexBuffer.SetData(indices);
            }

        }



        public void GetCubeWireframe()
        {
            chunkWireframe = new List<VertexPositionColor>();

            Vector3[] bottomFace = new Vector3[4];
            Vector3[] topFace = new Vector3[4];

            bottomFace[0] =new Vector3(position.X - 0.5f, -0.5f, position.Y - 0.5f);
            bottomFace[1] = new Vector3(position.X+blocks.GetLength(0)-1+0.5f, -0.5f,position.Y -0.5f);
            bottomFace[2] = new Vector3(position.X + blocks.GetLength(0)-1 + 0.5f, -0.5f, position.Y + blocks.GetLength(2)-1 + 0.5f);
            bottomFace[3] = new Vector3(position.X - 0.5f, -0.5f, position.Y + blocks.GetLength(2)-1 + 0.5f);

            topFace[0] = new Vector3(position.X - 0.5f, blocks.GetLength(1)-1 + 0.5f, position.Y - 0.5f);
            topFace[1] = new Vector3(position.X + blocks.GetLength(0)-1 + 0.5f, blocks.GetLength(1)-1 + 0.5f, position.Y - 0.5f);
            topFace[2] = new Vector3(position.X + blocks.GetLength(0)-1 + 0.5f, blocks.GetLength(1)-1 + 0.5f, position.Y + blocks.GetLength(2)-1 + 0.5f);
            topFace[3] = new Vector3(position.X - 0.5f, blocks.GetLength(1)-1 + 0.5f, position.Y + blocks.GetLength(2)-1 + 0.5f);

            for (int i = 0; i < bottomFace.Length; i++)
            {
                chunkWireframe.Add(new VertexPositionColor(bottomFace[i], wireFrameColour));
            }
            //Loop connect vertex back to original
            chunkWireframe.Add(new VertexPositionColor(bottomFace[0], wireFrameColour));
            //create top Face
            for (int i = 0; i < topFace.Length; i++)
            {
                chunkWireframe.Add(new VertexPositionColor(topFace[i], wireFrameColour));
            }
            chunkWireframe.Add(new VertexPositionColor(topFace[0], wireFrameColour));


            chunkWireframe.Add(new VertexPositionColor(topFace[1], wireFrameColour));
            chunkWireframe.Add(new VertexPositionColor(bottomFace[1], wireFrameColour));


            chunkWireframe.Add(new VertexPositionColor(bottomFace[2], wireFrameColour));
            chunkWireframe.Add(new VertexPositionColor(topFace[2], wireFrameColour));
            chunkWireframe.Add(new VertexPositionColor(topFace[3], wireFrameColour));
            chunkWireframe.Add(new VertexPositionColor(bottomFace[3], wireFrameColour));


        }

        public List<VertexPositionColor> Wireframe
        {
            get
            {
                return chunkWireframe;
            }
        }

        public VertexPositionNormalTexture[] Vertices
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
        }

        public VertexBuffer VertexBuffer
        {
            get
            {
                return vertexBuffer;
            }
        }

        public IndexBuffer IndexBuffer
        {
            get
            {
                return indexBuffer;
            }
        }
    }
}
