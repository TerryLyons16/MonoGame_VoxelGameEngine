using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VoxelRPGGame.GameEngine.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoxelRPGGame.MenuSystem;
using VoxelRPGGame.GameEngine.Physics;
using VoxelRPGGame.GameEngine.World.Textures;
using VoxelRPGGame.GameEngine.World.Geometry;
using VoxelRPGGame.GameEngine.Rendering;
 


namespace VoxelRPGGame.GameEngine.World.Voxels
{


    public abstract class AbstractBlock:AbstractTerrainObject
    {

        public delegate bool OnRequestRemove(Vector3 globalPosition);
        public event OnRequestRemove RequestRemove;

        public delegate bool OnRequestBuildBuffers();
        public event OnRequestBuildBuffers OnBuildBuffersRequest;

        public delegate bool OnRequestAddNeighbour(Vector3 globalPosition,AbstractBlock block);
        public event OnRequestAddNeighbour RequestAddNeighbour;


      //NOTE: Need to build the bounding box
   //   protected BoundingBox boundingBox;


        protected BlockShape blockShape;
        protected MaterialType blockType;
        protected Direction blockFacing;//Determines which way the block is facing (Important if block is not cube)

        protected Dictionary<Direction, bool> ActiveFaces;

        protected List<VertexPositionColor> faces_wireframe;

        public Vector3 TEMPMinPoint { get; set; }
        public Vector3 TEMPMaxPoint { get; set; }

        public Color wireFrameColour = Color.Red;

        protected List<TextureVertex> faces;

        protected VertexPositionNormalTexture[] vertices;
        protected int[] indices;

        Vector3[] bottomVertices = new Vector3[4];
        Vector3[] topVertices = new Vector3[4];

        //Note: use a list of textures for animated blcoks. Default all to Top texture if no other textures provided
        public Dictionary<Direction, TextureName> BlockTextures;

        public AbstractBlock(/*Vector3 centre,*/ BlockShape shape,MaterialType type, Direction facing)
        {
            faces = new List<TextureVertex>();
            faces_wireframe = new List<VertexPositionColor>();
            BlockTextures = new Dictionary<Direction, TextureName>();

         //   position = new Vector3(centre.X,centre.Y,centre.Z);
            blockType = type;
            blockFacing = facing;

            ActiveFaces = new Dictionary<Direction, bool>();
            foreach (Direction face in Enum.GetValues(typeof(Direction)))
            {
                ActiveFaces.Add(face, true);
            }
            //NOTE: Bottom faces set to invisible by default
          //  ActiveFaces[Direction.Down] = false;
             //Must be called last, as it sets block faces
            blockShape = shape;
        }

        protected void SetVertices()
        {
            bottomVertices[0] = position + new Vector3(-0.5f, -0.5f, -0.5f);
            bottomVertices[1] = position + new Vector3(0.5f, -0.5f, -0.5f);
            bottomVertices[2] = position + new Vector3(0.5f, -0.5f, 0.5f);
            bottomVertices[3] = position + new Vector3(-0.5f, -0.5f, 0.5f);

            topVertices[0] = position + new Vector3(-0.5f, 0.5f, -0.5f);
            topVertices[1] = position + new Vector3(0.5f, 0.5f, -0.5f);
            topVertices[2] = position + new Vector3(0.5f, 0.5f, 0.5f);
            topVertices[3] = position + new Vector3(-0.5f, 0.5f, 0.5f);
        }
        /// <summary>
        /// Sets the blocks faces. Called whenever the BlockShape changes
        /// </summary>
        public void SetFaces()
        {

            TEMPMinPoint = position + new Vector3(-0.5f, -0.5f, -0.5f);
            TEMPMaxPoint = position + new Vector3(0.5f, 0.5f, 0.5f);

            faces.Clear();
            //Y-up world
            SetVertices();
         



           // Texture2D tex = BlockTextures[Direction.Up];
            Vector2 textureTopLeft = new Vector2(0, 0);
            Vector2 textureTopRight = new Vector2(1, 0);
            Vector2 textureBottomLeft = new Vector2(0, 1);
            Vector2 textureBottomRight = new Vector2(1, 1);


            // Normal vectors for block faces (will be different for different shapes)
            Vector3 normalNorth = new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 normalSouth = new Vector3(-1.0f, 0.0f, 0.0f);
            Vector3 normalTop = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 normalBottom = new Vector3(0.0f, -1.0f, 0.0f);
            Vector3 normalEast = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 normalWest = new Vector3(0.0f, 0.0f, -1.0f);

          

            if (blockShape == BlockShape.Cube)
            {

            // UV texture coordinates. TextureID is the index of the texture, from 0 to 15 (4x4 square texture atlas)
        /*    Texture2D tex = BlockTextures[BlockFace.Top];
            Vector2 textureTopLeft = new Vector2(0, 0);
            Vector2 textureTopRight = new Vector2(tex.Bounds.Width, 0);
            Vector2 textureBottomLeft = new Vector2(0, tex.Bounds.Height);
            Vector2 textureBottomRight = new Vector2(tex.Bounds.Width, tex.Bounds.Height);*/
            
                //Create 6 faces of cube
                //create bottom Face

            //Create Top Face
                if (ActiveFaces[Direction.Up])
                {
                    Vector3[] quad = { topVertices[1], topVertices[2], topVertices[3], topVertices[0] };
                   faces.AddRange(GeometryServices.BuildQuad(quad, true, normalTop,BlockTextures[Direction.Up], BlockTextures[Direction.Up]));
                }

                if (ActiveFaces[Direction.Down])
                {
                    Vector3[] quad = { bottomVertices[1], bottomVertices[0], bottomVertices[3], bottomVertices[2] };
                    faces.AddRange(GeometryServices.BuildQuad(quad, true, normalBottom, BlockTextures[Direction.Down], BlockTextures[Direction.Down]));
                }


             //Create West Face
                if (ActiveFaces[Direction.West])
                {
                    Vector3[] quad = { topVertices[1], topVertices[0], bottomVertices[0], bottomVertices[1] };
                    faces.AddRange(GeometryServices.BuildQuad(quad, true, normalWest, BlockTextures[Direction.West], BlockTextures[Direction.West]));
                }

             //Create South Face
                if (ActiveFaces[Direction.South])
                {
                    Vector3[] quad = { topVertices[0], topVertices[3], bottomVertices[3], bottomVertices[0] };
                    faces.AddRange(GeometryServices.BuildQuad(quad, true, normalSouth, BlockTextures[Direction.South], BlockTextures[Direction.South]));
                }

            //Create North Face
                if (ActiveFaces[Direction.North])
                {
                    Vector3[] quad = { topVertices[2], topVertices[1], bottomVertices[1], bottomVertices[2] };
                    faces.AddRange(GeometryServices.BuildQuad(quad, true, normalNorth, BlockTextures[Direction.North], BlockTextures[Direction.North]));
                }

            //Create East Face
                if (ActiveFaces[Direction.East])
                {
                    Vector3[] quad = { topVertices[3], topVertices[2], bottomVertices[2], bottomVertices[3] };
                    faces.AddRange(GeometryServices.BuildQuad(quad, true, normalEast, BlockTextures[Direction.East], BlockTextures[Direction.East]));
                }


               
            }
            else if (blockShape == BlockShape.Slope)
            {
                //NOTE: Slope normal will need to be changed
                int w=0, x=0,y=0, z=0;
                //NOTE: Must also flip texture coords

                //NOTE: Facing is the direction of the downwards slope
                if (blockFacing == Direction.West)
                {
                    w = 0;
                    x = 1;
                    y = 2;
                    z = 3;
                }
                else if (blockFacing == Direction.South)
                {
                    w = 3;
                    x = 0;
                    y = 1;
                    z = 2;
                }
                else if (blockFacing == Direction.East)
                {
                    w = 2;
                    x = 3;
                    y = 0;
                    z = 1;
                }

                else if (blockFacing == Direction.North)
                {
                    w = 1;
                    x = 2;
                    y = 3;
                    z = 0;
                }

                Vector2[] textureCoordinates = new Vector2[4];
                textureCoordinates[0] = textureBottomLeft;
                textureCoordinates[1] = textureTopLeft;
                textureCoordinates[2] = textureTopRight;
                textureCoordinates[3] = textureBottomRight;

              //  normalTop = new Vector3(1.0f, 1.0f, 0.0f);
                //create Top Face
                if (ActiveFaces[Direction.Up])
                {
                  /*  faces.Add(new VertexPositionNormalTexture(bottomVertices[w], normalTop, textureCoordinates[w]));
                    faces.Add(new VertexPositionNormalTexture(bottomVertices[x], normalTop, textureCoordinates[x]));
                    faces.Add(new VertexPositionNormalTexture(topVertices[y], normalTop, textureCoordinates[y]));
                    faces.Add(new VertexPositionNormalTexture(topVertices[y], normalTop, textureCoordinates[y]));
                    faces.Add(new VertexPositionNormalTexture(topVertices[z], normalTop, textureCoordinates[z]));
                    faces.Add(new VertexPositionNormalTexture(bottomVertices[w], normalTop, textureCoordinates[w]));*/
                }



             /*   faces.Add(new VertexPositionNormalTexture(topVertices[z], normalSouth, textureTopRight));
                faces.Add(new VertexPositionNormalTexture(bottomVertices[z], normalSouth, textureBottomRight));
                faces.Add(new VertexPositionNormalTexture(bottomVertices[w], normalSouth, textureBottomLeft));

                faces.Add(new VertexPositionNormalTexture(topVertices[y], normalNorth, textureTopRight));
                faces.Add(new VertexPositionNormalTexture(bottomVertices[x], normalNorth, textureBottomLeft));
                faces.Add(new VertexPositionNormalTexture(bottomVertices[y],normalNorth, textureBottomRight));*/
             
            }
            BuildBuffers(faces);
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch Batch, Camera camera)
        {
            throw new NotImplementedException();
        }

        public override void Draw(Camera c)
        {


       //    ShaderManager.GetInstance().DefaultEffect.Texture = BlockTextures[BlockFace.Top];
       //    ShaderManager.GetInstance().DefaultEffect.CurrentTechnique.Passes[0].Apply();


        /*    using (VertexBuffer buffer = new VertexBuffer(
               ScreenManager.GetInstance().GraphicsDevice,
               VertexPositionNormalTexture.VertexDeclaration,
              faces.Count,
               BufferUsage.WriteOnly))
            {
                buffer.SetData(faces.ToArray());
                 ScreenManager.GetInstance().GraphicsDevice.SetVertexBuffer(buffer);
            }
           ShaderManager.GetInstance().DefaultEffect.Texture = BlockTextures[BlockFace.Top];

                       ShaderManager.GetInstance().DefaultEffect.TextureEnabled = true;
           ShaderManager.GetInstance().DefaultEffect.VertexColorEnabled = false;
           ShaderManager.GetInstance().DefaultEffect.CurrentTechnique.Passes[0].Apply();

            ScreenManager.GetInstance().GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, faces.Count / 3);
            DrawWireframe(faces_wireframe,ShaderManager.GetInstance().DefaultEffect, ScreenManager.GetInstance().GraphicsDevice);
           */
        }


        public override void DrawRotatedBoundingBox(BasicEffect effect, GraphicsDevice graphicsDevice)
        {
            GetCubeWireframe();

           ShaderManager.GetInstance().DefaultEffect.TextureEnabled = false;
           ShaderManager.GetInstance().DefaultEffect.VertexColorEnabled = true;
           ShaderManager.GetInstance().DefaultEffect.CurrentTechnique.Passes[0].Apply();

            graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, faces_wireframe.ToArray(), 0, faces_wireframe.Count - 1);
        }


        public void DrawWireframe(BasicEffect effect, GraphicsDevice graphicsDevice)
        {
            GetCubeWireframe();

           ShaderManager.GetInstance().DefaultEffect.TextureEnabled = false;
           ShaderManager.GetInstance().DefaultEffect.VertexColorEnabled = true;
           ShaderManager.GetInstance().DefaultEffect.CurrentTechnique.Passes[0].Apply();
           
            graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, faces_wireframe.ToArray(), 0, faces_wireframe.Count - 1);
        }

   /*     protected void DrawCircle(Vector3 pos, Color c, GraphicsDevice graphicsDevice)
        {
            VertexPositionColor[] selectionCircle = new VertexPositionColor[100];
            for (int i = 0; i < 99; i++)
            {
                float angle = (float)(i / 100.0 * Math.PI * 2);
                selectionCircle[i].Position = new Vector3((pos.X) + (float)Math.Cos(angle) * 1, 0.2f, (pos.Z) + (float)Math.Sin(angle) * 1);
                selectionCircle[i].Color = c;
            }
            selectionCircle[99] = selectionCircle[0];
           ShaderManager.GetInstance().DefaultEffect.CurrentTechnique.Passes[0].Apply();
           graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, selectionCircle, 0, selectionCircle.Length - 1);
        }
        */
        protected void GetCubeWireframe()
        {
            Vector3[] bottomFace = new Vector3[4];
            Vector3[] topFace = new Vector3[4];

            bottomFace[0] = position + new Vector3(-0.5f, -0.5f, -0.5f);
            bottomFace[1] = position + new Vector3(0.5f, -0.5f, -0.5f);
            bottomFace[2] = position + new Vector3(0.5f, -0.5f, 0.5f);
            bottomFace[3] = position + new Vector3(-0.5f, -0.5f, 0.5f);

            topFace[0] = position + new Vector3(-0.5f, 0.5f, -0.5f);
            topFace[1] = position + new Vector3(0.5f, 0.5f, -0.5f);
            topFace[2] = position + new Vector3(0.5f, 0.5f, 0.5f);
            topFace[3] = position + new Vector3(-0.5f, 0.5f, 0.5f);


            //Create 6 faces of cube
            //create bottom Face
            faces_wireframe = new List<VertexPositionColor>();
            //NOTE: Will be different fot textured polygons, as poly=3 verts, 6 per 4-sided face
            for (int i = 0; i < bottomFace.Length; i++)
            {
                faces_wireframe.Add(new VertexPositionColor(bottomFace[i], wireFrameColour));
            }
            //Loop connect vertex back to original
            faces_wireframe.Add(new VertexPositionColor(bottomFace[0], wireFrameColour));
            //create top Face
            for (int i = 0; i < topFace.Length; i++)
            {
                faces_wireframe.Add(new VertexPositionColor(topFace[i], wireFrameColour));
            }
            faces_wireframe.Add(new VertexPositionColor(topFace[0], wireFrameColour));


            faces_wireframe.Add(new VertexPositionColor(topFace[1], wireFrameColour));
            faces_wireframe.Add(new VertexPositionColor(bottomFace[1], wireFrameColour));


            faces_wireframe.Add(new VertexPositionColor(bottomFace[2], wireFrameColour));
            faces_wireframe.Add(new VertexPositionColor(topFace[2], wireFrameColour));
            faces_wireframe.Add(new VertexPositionColor(topFace[3], wireFrameColour));
            faces_wireframe.Add(new VertexPositionColor(bottomFace[3], wireFrameColour));


        }

        public bool BuildBuffers(List<TextureVertex> unindexedVertices)
        {
            //NOTE:Need to take textures into account when indexing
            bool result = false;

            List<int> indexList = new List<int>();
            List<VertexPositionNormalTexture> vertexList = new List<VertexPositionNormalTexture>();

            Dictionary<TextureVector, int> usedVertices = new Dictionary<TextureVector, int>();

           
            for (int i = 0; i < unindexedVertices.Count; i++)
            {
                TextureVector texVec = new TextureVector(unindexedVertices[i].vertex.Position, unindexedVertices[i].texture);

                if (usedVertices.ContainsKey(texVec))
                {
                    indexList.Add(usedVertices[texVec]);
                }
                else
                {
                    vertexList.Add(unindexedVertices[i].vertex);
                    indexList.Add(vertexList.Count - 1);
                    usedVertices.Add(texVec, vertexList.Count - 1);
                }

                if ((i + 1) % 6 == 0)//Drop used vertex list whenever going to a new face
                {
                    //NOTE: giving incorrect results for top[0] position
                    usedVertices = new Dictionary<TextureVector, int>();
                }
            }

            indices = indexList.ToArray<int>();
            vertices = vertexList.ToArray<VertexPositionNormalTexture>();

            return result;
        }


        public void OnNeighbourAdded(Direction neighbourLocation, AbstractBlock block)
        {
            //NOTE: Multiple factors will determine if the face is set inactive: if it or the neighbour
            //are transparent, if either are slopes etc...

            SetFaceActive(neighbourLocation, false);
        }

        public void OnNeighbourRemoved(Direction neighbourLocation)
        {
            SetFaceActive(neighbourLocation, true);
        }

        protected virtual void SetFaceActive(Direction face, bool active)
        {
            if (ActiveFaces.ContainsKey(face))
                ActiveFaces[face] = active;
            else
                ActiveFaces.Add(face, active);

            SetFaces();
        }

        //NOTE: Change to get AABB faces and make a separate getCollisionFaces method
        public Dictionary<Direction, BlockCollisionFace> GetCollisionFaceDirections()
        {
            Dictionary<Direction, BlockCollisionFace> result = null;
            SetVertices();

            if (blockShape == BlockShape.Cube)
            {
                result = new Dictionary<Direction, BlockCollisionFace>();

                // Normal vectors for block faces (will be different for different shapes)
                Vector3 normalNorth = new Vector3(1.0f, 0.0f, 0.0f);
                Vector3 normalSouth = new Vector3(-1.0f, 0.0f, 0.0f);
                Vector3 normalTop = new Vector3(0.0f, 1.0f, 0.0f);
                Vector3 normalBottom = new Vector3(0.0f, -1.0f, 0.0f);
                Vector3 normalEast = new Vector3(0.0f, 0.0f, 1.0f);
                Vector3 normalWest = new Vector3(0.0f, 0.0f, -1.0f);

                if (ActiveFaces[Direction.Up])
                {
                    result.Add(Direction.Up, new BlockCollisionFace(Direction.Up,this, topVertices[0], topVertices[1], topVertices[2], topVertices[3], normalTop));
                }
                if (ActiveFaces[Direction.Down])
                {
                    result.Add(Direction.Down, new BlockCollisionFace(Direction.Down,this, bottomVertices[0], bottomVertices[1], bottomVertices[2], bottomVertices[3], normalBottom));
                }
                if (ActiveFaces[Direction.West])
                {
                    result.Add(Direction.West, new BlockCollisionFace(Direction.West,this, bottomVertices[0], bottomVertices[1], topVertices[1], topVertices[0], normalWest));
                }
                if (ActiveFaces[Direction.South])
                {
                    result.Add(Direction.South, new BlockCollisionFace(Direction.South,this, bottomVertices[0], bottomVertices[3], topVertices[3], topVertices[0], normalSouth));
                }
                if (ActiveFaces[Direction.North])
                {
                    result.Add(Direction.North, new BlockCollisionFace(Direction.North,this, bottomVertices[1], bottomVertices[2], topVertices[2], topVertices[1], normalNorth));
                }
                if (ActiveFaces[Direction.East])
                {
                    result.Add(Direction.East, new BlockCollisionFace(Direction.East,this, bottomVertices[2], bottomVertices[3], topVertices[3], topVertices[2], normalEast));
                }
            }
            else if (blockShape == BlockShape.Slope)     //NOTE: TEMPORARY
            {

                result = new Dictionary<Direction, BlockCollisionFace>();
                // Normal vectors for block faces (will be different for different shapes)
           
                Vector3 normalTop = new Vector3(1.0f, 1.0f, 0.0f);
                normalTop.Normalize();
                result.Add(Direction.Up, new BlockCollisionFace(Direction.Up,this,bottomVertices[0], bottomVertices[1], topVertices[2], topVertices[3], normalTop));
          
            }




            return result;
        }

        public override List<CollisionFace> GetCollisionFaces()
        {
            List<CollisionFace> result = null;
            SetVertices();

            if (blockShape == BlockShape.Cube)
            {
                result = new List<CollisionFace>();

                // Normal vectors for block faces (will be different for different shapes)
                Vector3 normalNorth = new Vector3(1.0f, 0.0f, 0.0f);
                Vector3 normalSouth = new Vector3(-1.0f, 0.0f, 0.0f);
                Vector3 normalTop = new Vector3(0.0f, 1.0f, 0.0f);
                Vector3 normalBottom = new Vector3(0.0f, -1.0f, 0.0f);
                Vector3 normalEast = new Vector3(0.0f, 0.0f, 1.0f);
                Vector3 normalWest = new Vector3(0.0f, 0.0f, -1.0f);

                if (ActiveFaces[Direction.Up])
                {
                    result.Add(new BlockCollisionFace(Direction.Up, this, topVertices[0], topVertices[1], topVertices[2], topVertices[3], normalTop));
                }
                if (ActiveFaces[Direction.Down])
                {
                    result.Add(new BlockCollisionFace(Direction.Down, this, bottomVertices[0], bottomVertices[1], bottomVertices[2], bottomVertices[3], normalBottom));
                }
                if (ActiveFaces[Direction.West])
                {
                    result.Add(new BlockCollisionFace(Direction.West, this, bottomVertices[0], bottomVertices[1], topVertices[1], topVertices[0], normalWest));
                }
                if (ActiveFaces[Direction.South])
                {
                    result.Add(new BlockCollisionFace(Direction.South, this, bottomVertices[0], bottomVertices[3], topVertices[3], topVertices[0], normalSouth));
                }
                if (ActiveFaces[Direction.North])
                {
                    result.Add(new BlockCollisionFace(Direction.North, this, bottomVertices[1], bottomVertices[2], topVertices[2], topVertices[1], normalNorth));
                }
                if (ActiveFaces[Direction.East])
                {
                    result.Add(new BlockCollisionFace(Direction.East, this, bottomVertices[2], bottomVertices[3], topVertices[3], topVertices[2], normalEast));
                }
            }
            else if (blockShape == BlockShape.Slope)     //NOTE: TEMPORARY
            {

                result = new List<CollisionFace>();
                // Normal vectors for block faces (will be different for different shapes)

                Vector3 normalTop = new Vector3(1.0f, 1.0f, 0.0f);
                normalTop.Normalize();
                result.Add(new BlockCollisionFace(Direction.Up, this, bottomVertices[0], bottomVertices[1], topVertices[2], topVertices[3], normalTop));

            }




            return result;
        }


        public override void OnHit()
        {
            if (RequestRemove != null)
            {
                bool removed = RequestRemove(position);
            }
        }

        public bool OnAddBlock(Direction direction,AbstractBlock block)
        {
            Vector3 directionPoint = Vector3.Zero; 
            if(direction==Direction.North)
            {
                directionPoint.X=1;
            }
            else if(direction==Direction.South)
            {
                 directionPoint.X=-1;
            }
            else if(direction==Direction.East)
            {
                directionPoint.Z=1;
            }
            else if(direction==Direction.West)
            {
                directionPoint.Z=-1;
            }
            else if(direction==Direction.Up)
            {
                directionPoint.Y=1;
            }
            else if(direction==Direction.Down)
            {
                directionPoint.Y=-1;
            }

            Vector3 desiredPosition = position+directionPoint;

            return RequestAddNeighbour(desiredPosition, block);
        }

#region Properties

        public Vector3 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
                SetFaces();
            }
        }
        public BlockShape BlockShape
        {
            get
            {
                return blockShape;
            }

            set
            {
                blockShape = value;
                SetFaces();
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

      /*  public List<VertexPositionNormalTexture> Faces
        {
            get
            {
                return faces;
            }
        }*/

        public List<VertexPositionColor> Wireframe
        {
            get
            {
                return faces_wireframe;
            }
        }

        public TextureName TEMPTexture
        {
            get
            {
                return BlockTextures[Direction.Up];
            }
        }

        public void TEMP_RequestBuildBuffers()
        {
            OnBuildBuffersRequest();
        }
#endregion
    }
}
