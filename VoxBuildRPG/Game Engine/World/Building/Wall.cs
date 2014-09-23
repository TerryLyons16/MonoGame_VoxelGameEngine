using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxelRPGGame.MenuSystem;

using VoxelRPGGame.GameEngine.World.Geometry;
using VoxelRPGGame.GameEngine.World.Textures;
using VoxelRPGGame.GameEngine.Physics;
using VoxelRPGGame.GameEngine.Rendering;


namespace VoxelRPGGame.GameEngine.World.Building
{
    public class Wall : AbstractTerrainObject
    {
        public delegate bool OnRequestRemove(Vector3 firstPosition, Vector3 secondPosition);
        public event OnRequestRemove RequestRemove;

        private Vector3 firstPoint;
        private Vector3 secondPoint;

        Vector3[] bottomVertices = new Vector3[4];
        Vector3[] topVertices = new Vector3[4];
        public Color wireFrameColour = Color.Red;

        protected VertexPositionNormalTexture[] vertices;
        protected int[] indices;

        protected List<TextureVertex> faces;

        //4 possible textures - 2 long sides and 2 short sides
        protected Dictionary<int, Texture2D> wallTextures;

        public Wall(Vector3 point1, Vector3 point2)
        {
            //TEMP
            wallTextures = new Dictionary<int, Texture2D>();
            Texture2D tex = ScreenManager.GetInstance().ContentManager.Load<Texture2D>("dirt");//dirt2
            wallTextures.Add(0, tex);

            firstPoint = new Vector3(point1.X, point1.Y, point1.Z);
            secondPoint = new Vector3(point2.X, point2.Y, point2.Z);

            faces = new List<TextureVertex>();

          

            SetFaces();
        }


        protected void SetVertices()
        {
            float xDiff=0, zDiff=0;

            Vector3 point1 = firstPoint;
               Vector3  point2 = secondPoint;

            //Walls can be created in any order, but bottomVertices[0] must be south-west point etc
            if (secondPoint.Z > firstPoint.Z)
            {
                point2 = secondPoint;
                point1 = firstPoint;
            }
            else if (secondPoint.Z < firstPoint.Z) 
            {
                point2 = firstPoint;
                point1 = secondPoint;
            }

            if (secondPoint.X > firstPoint.X)
            {
                point2 = firstPoint;
                point1 = secondPoint;
                
            }
            else if (secondPoint.X < firstPoint.X)
            {
                point2 = secondPoint;
                point1 = firstPoint;
            }
            //NOTE: Need cases for angle walls

            zDiff = 0;
            xDiff = 0;
             if (point2.X != point1.X/* && point2.Z == point1.Z*/)//East - West
            {
               
                zDiff = 0.05f;
            }
            else if (/*point2.X == point1.X && */point2.Z != point1.Z)//North - South
            {
                
                xDiff = 0.05f;
            }


          /*  else//NOTE: This will need to be changed for angle walls to correctly connect to other walls
            {
                xDiff = -0.05f;
                zDiff = 0.05f;
            }*/

            //NOTE: Need 45 degree walls

            bottomVertices[0] = point1 + new Vector3(-xDiff, 0, -zDiff);
            bottomVertices[1] = point1 + new Vector3(xDiff, 0, zDiff);

            bottomVertices[2] = point2 + new Vector3(xDiff, 0, zDiff);
            bottomVertices[3] = point2 + new Vector3(-xDiff, 0, -zDiff);


            topVertices[0] = point1 + new Vector3(-xDiff, 1, -zDiff);
            topVertices[1] = point1 + new Vector3(xDiff, 1, zDiff);

            topVertices[2] = point2 + new Vector3(xDiff, 1, zDiff);
            topVertices[3] = point2 + new Vector3(-xDiff, 1, -zDiff);
          
        }


        public void SetFaces()
        {
            faces.Clear();
            SetVertices();

            Texture2D tex = wallTextures[0];//NOTE: Will need to change per wall face
            Vector2 textureTopLeft = new Vector2(0, 0);
            Vector2 textureTopRight = new Vector2(1, 0);
            Vector2 textureBottomLeft = new Vector2(0, 1);
            Vector2 textureBottomRight = new Vector2(1, 1);


            //NOTE: May be facing incorrect directions
            Vector3 normalNorth = new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 normalSouth = new Vector3(-1.0f, 0.0f, 0.0f);
            Vector3 normalTop = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 normalBottom = new Vector3(0.0f, -1.0f, 0.0f);
            Vector3 normalEast = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 normalWest = new Vector3(0.0f, 0.0f, -1.0f);

            //Create Top Face
             Vector3[] quad1 = { topVertices[1], topVertices[2], topVertices[3], topVertices[0]};
            faces.AddRange(GeometryServices.BuildQuad(quad1,true,normalTop,TextureName.Stone,TextureName.Stone));

          /*  faces.Add(new VertexPositionNormalTexture(topVertices[0], normalTop, textureBottomLeft));
            faces.Add(new VertexPositionNormalTexture(topVertices[1], normalTop, textureTopLeft));
            faces.Add(new VertexPositionNormalTexture(topVertices[2], normalTop, textureTopRight));
            faces.Add(new VertexPositionNormalTexture(topVertices[2], normalTop, textureTopRight));
            faces.Add(new VertexPositionNormalTexture(topVertices[3], normalTop, textureBottomRight));
            faces.Add(new VertexPositionNormalTexture(topVertices[1], normalTop, textureBottomLeft));*/

            //Bottom face
             Vector3[] quad2 = { bottomVertices[1], bottomVertices[0], bottomVertices[3], bottomVertices[2] };
             faces.AddRange(GeometryServices.BuildQuad(quad2, true, normalBottom, TextureName.Stone, TextureName.Stone));
         /*   faces.Add(new VertexPositionNormalTexture(bottomVertices[2], normalBottom, textureTopRight));
            faces.Add(new VertexPositionNormalTexture(bottomVertices[0], normalBottom, textureTopLeft));
            faces.Add(new VertexPositionNormalTexture(bottomVertices[1], normalBottom, textureBottomLeft));
            faces.Add(new VertexPositionNormalTexture(bottomVertices[1], normalBottom, textureBottomLeft));
            faces.Add(new VertexPositionNormalTexture(bottomVertices[3], normalBottom, textureBottomRight));
            faces.Add(new VertexPositionNormalTexture(bottomVertices[2], normalBottom, textureTopRight));*/

             Vector3[] quad3 = { topVertices[1], topVertices[0], bottomVertices[0], bottomVertices[1] };
             faces.AddRange(GeometryServices.BuildQuad(quad3, true, normalWest, TextureName.Stone, TextureName.Stone));
           /* faces.Add(new VertexPositionNormalTexture(bottomVertices[0], normalWest, textureBottomRight));
            faces.Add(new VertexPositionNormalTexture(bottomVertices[1], normalWest, textureBottomLeft));
            faces.Add(new VertexPositionNormalTexture(topVertices[1], normalWest, textureTopLeft));
            faces.Add(new VertexPositionNormalTexture(topVertices[1], normalWest, textureTopLeft));
            faces.Add(new VertexPositionNormalTexture(topVertices[0], normalWest, textureTopRight));
            faces.Add(new VertexPositionNormalTexture(bottomVertices[0], normalWest, textureBottomRight));*/


             Vector3[] quad4 = { topVertices[0], topVertices[3], bottomVertices[3], bottomVertices[0] };
             faces.AddRange(GeometryServices.BuildQuad(quad4, true, normalSouth, TextureName.Stone, TextureName.Stone));
           /* faces.Add(new VertexPositionNormalTexture(bottomVertices[0], normalSouth, textureBottomLeft));
            faces.Add(new VertexPositionNormalTexture(bottomVertices[2], normalSouth, textureTopLeft));
            faces.Add(new VertexPositionNormalTexture(topVertices[2], normalSouth, textureTopRight));
            faces.Add(new VertexPositionNormalTexture(topVertices[2], normalSouth, textureTopRight));
            faces.Add(new VertexPositionNormalTexture(topVertices[0], normalSouth, textureBottomRight));
            faces.Add(new VertexPositionNormalTexture(bottomVertices[0], normalSouth, textureBottomLeft));*/

             Vector3[] quad5 = { topVertices[2], topVertices[1], bottomVertices[1], bottomVertices[2] };
             faces.AddRange(GeometryServices.BuildQuad(quad5, true, normalNorth, TextureName.Stone, TextureName.Stone));
           /* faces.Add(new VertexPositionNormalTexture(bottomVertices[1], normalNorth, textureBottomRight));
            faces.Add(new VertexPositionNormalTexture(topVertices[1], normalNorth, textureBottomLeft));
            faces.Add(new VertexPositionNormalTexture(topVertices[3], normalNorth, textureTopLeft));
            faces.Add(new VertexPositionNormalTexture(topVertices[3], normalNorth, textureTopLeft));
            faces.Add(new VertexPositionNormalTexture(bottomVertices[3], normalNorth, textureTopRight));
            faces.Add(new VertexPositionNormalTexture(bottomVertices[1], normalNorth, textureBottomRight));*/

             Vector3[] quad6 = { topVertices[3], topVertices[2], bottomVertices[2], bottomVertices[3] };
             faces.AddRange(GeometryServices.BuildQuad(quad6, true, normalEast, TextureName.Stone, TextureName.Stone));
           /* faces.Add(new VertexPositionNormalTexture(bottomVertices[2], normalEast, textureBottomRight));
            faces.Add(new VertexPositionNormalTexture(bottomVertices[3], normalEast, textureBottomLeft));
            faces.Add(new VertexPositionNormalTexture(topVertices[3], normalEast, textureTopLeft));
            faces.Add(new VertexPositionNormalTexture(topVertices[3], normalEast, textureTopLeft));
            faces.Add(new VertexPositionNormalTexture(topVertices[2], normalEast, textureTopRight));
            faces.Add(new VertexPositionNormalTexture(bottomVertices[2], normalEast, textureBottomRight));*/


            BuildBuffers(faces);
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
                    usedVertices = new Dictionary<TextureVector, int>();
                }
            }

            indices = indexList.ToArray<int>();
            vertices = vertexList.ToArray<VertexPositionNormalTexture>();

            return result;
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
         
        }

        public override void Update()
        {
        }

        public override void Draw(SpriteBatch Batch, Camera camera)
        {
         
        }

        public override void Draw(Camera c)
        {

        }

        public List<VertexPositionColor> GetWallRotatedBoundingBox()
        {

            List<VertexPositionColor> result = new  List<VertexPositionColor>();
            for (int i = 0; i < bottomVertices.Length; i++)
            {
                result.Add(new VertexPositionColor(bottomVertices[i], wireFrameColour));
            }
            //Loop connect vertex back to original
            result.Add(new VertexPositionColor(bottomVertices[0], wireFrameColour));
            //create top Face
            for (int i = 0; i < topVertices.Length; i++)
            {
                result.Add(new VertexPositionColor(topVertices[i], wireFrameColour));
            }
            result.Add(new VertexPositionColor(topVertices[0], wireFrameColour));


            result.Add(new VertexPositionColor(topVertices[1], wireFrameColour));
            result.Add(new VertexPositionColor(bottomVertices[1], wireFrameColour));


            result.Add(new VertexPositionColor(bottomVertices[2], wireFrameColour));
            result.Add(new VertexPositionColor(topVertices[2], wireFrameColour));
            result.Add(new VertexPositionColor(topVertices[3], wireFrameColour));
            result.Add(new VertexPositionColor(bottomVertices[3], wireFrameColour));


            return result;
        }

        public override void DrawRotatedBoundingBox(BasicEffect effect, GraphicsDevice graphicsDevice)
        {
            List<VertexPositionColor> wall_wireframe = GetWallRotatedBoundingBox();
           ShaderManager.GetInstance().DefaultEffect.TextureEnabled = false;
           ShaderManager.GetInstance().DefaultEffect.VertexColorEnabled = true;
           ShaderManager.GetInstance().DefaultEffect.CurrentTechnique.Passes[0].Apply();

            if (wall_wireframe.Count > 0)
            {
                graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, wall_wireframe.ToArray(), 0, wall_wireframe.Count - 1);
            }
        
        }
           

    


        public override List<CollisionFace> GetCollisionFaces()
        {
            List<CollisionFace> result = null;
            SetVertices();

            result = new List<CollisionFace>();

                // Normal vectors for block faces (will be different for different shapes)
                Vector3 normalNorth = new Vector3(1.0f, 0.0f, 0.0f);
                Vector3 normalSouth = new Vector3(-1.0f, 0.0f, 0.0f);
                Vector3 normalTop = new Vector3(0.0f, 1.0f, 0.0f);
                Vector3 normalBottom = new Vector3(0.0f, -1.0f, 0.0f);
                Vector3 normalEast = new Vector3(0.0f, 0.0f, 1.0f);
                Vector3 normalWest = new Vector3(0.0f, 0.0f, -1.0f);

                result.Add(new CollisionFace(this, topVertices[0], topVertices[1], topVertices[2], topVertices[3], normalTop));

                result.Add(new CollisionFace( this, bottomVertices[0], bottomVertices[1], bottomVertices[2], bottomVertices[3], normalBottom));

                result.Add(new CollisionFace(this, bottomVertices[0], bottomVertices[1], topVertices[1], topVertices[0], normalWest));

                result.Add(new CollisionFace(this, bottomVertices[0], bottomVertices[3], topVertices[3], topVertices[0], normalSouth));

                result.Add(new CollisionFace( this, bottomVertices[1], bottomVertices[2], topVertices[2], topVertices[1], normalNorth));

                result.Add(new CollisionFace(this, bottomVertices[2], bottomVertices[3], topVertices[3], topVertices[2], normalEast));
              
          

            return result;
        }


        public override void OnHit()
        {
            if (RequestRemove != null)
            {
                bool removed = RequestRemove(firstPoint, secondPoint);
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
    }
}
