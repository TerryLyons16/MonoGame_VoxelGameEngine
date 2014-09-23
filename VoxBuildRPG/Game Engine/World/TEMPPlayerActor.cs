using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxelRPGGame.MenuSystem;
using VoxelRPGGame.GameEngine.World.Voxels;
using VoxelRPGGame.GameEngine.Physics;
using VoxelRPGGame.GameEngine.World.Textures;
using VoxelRPGGame.GameEngine.World.Building;
using VoxelRPGGame.GameEngine.Rendering;

namespace VoxelRPGGame.GameEngine.World
{
    public class TEMPPlayerActor:AbstractActor
    {
        public enum TEMP_Tools
        {
            NULL,
            AddBlock,
            RemoveBlock,
            AddWall,
            RemoveWall,
            PaintFace,
            PaintHalfFace
        }
      
        TEMP_Tools tools = TEMP_Tools.NULL;

        protected Camera camera = null;

        private float moveSpeed = 0.15f;
        private float rotationSpeedY = 0.15f;
        private float TEMP_mouseDelta = 0.0f;
        private float TEMP_velJump = 0;
     
  //      private Vector2 TEMP_mouseClickPosition = Vector2.Zero;
        bool OnGround = false;

        List<VertexPositionColor> facingLine;
        List<List<VertexPositionColor>>movementLines;
        List<VertexPositionColor> xAxisLine; 
        List<VertexPositionColor> zAxisLine;

        List<VertexPositionColor> mouseRayLine;

        public BoundingBox BoundingBox { get; set; }
        protected CollisionBox _collisionBox;

        Vector2? TEMP_MouseClick = null;
        Ray TEMP_MouseRay;

        AbstractWorldObject TEMP_selectedObject = null;
        List<AbstractWorldObject> TEMP_selectedObjects = new List<AbstractWorldObject>();

        public TEMPPlayerActor(string modelLocation)
            : base(modelLocation)
        {
            position = new Vector3(0f, 6,0);//new Vector3(0,0f, -1.2f);
            scale = new Vector3(1, 1, 1);

            float collisionBoxLength = 0.4f;
            float height = 2;
          
            _collisionBox = new CollisionBox(position,collisionBoxLength,collisionBoxLength, height);

            BoundingBox = new BoundingBox(position + new Vector3(-(float)Math.Sqrt(collisionBoxLength / 2), 0f, -(float)Math.Sqrt(collisionBoxLength / 2)), position + new Vector3((float)Math.Sqrt(collisionBoxLength / 2), height+0.2f, (float)Math.Sqrt(collisionBoxLength / 2)));
        }

        public void RotateY(float movement)
        {
            float mouseMovement = movement;

          
            float rotationDegreesY = mouseMovement * rotationSpeedY;
            Rotate(0,rotationDegreesY,0);

        }
        public override void Update()
        {
        }

        public void Update(List<AbstractBlock> collidingTerrain)
        {
            OnGround = false;
            velocity.Y += TEMP_velJump;
         //   velocity.Y -= 0.1f;
            //velocity.X = -0.01f;
          //  if (!OnGround)
            {
                TEMP_velJump -= (9.8f / 200);
            }

            if (position.Y + velocity.Y <= 0)
            {
                TEMP_velJump = 0;
                velocity.Y = 0;
             //   OnGround = true;
            }

            Matrix rotationMatrix = Matrix.CreateRotationY(rotation.Y);

            Vector3 vec = new Vector3(-1, 0, 0);

            Vector3 transformedReferenceDirection = Vector3.Transform(vec, rotationMatrix);
            Vector3 direction = position + transformedReferenceDirection;
            facingLine = new List<VertexPositionColor>();

            facingLine.Add(new VertexPositionColor(position, Color.Orange));
            facingLine.Add(new VertexPositionColor(direction, Color.Orange));


         //   BoundingBox = new BoundingBox(position + new Vector3(-0.5f, 0, -0.5f), position + new Vector3(0.5f, 2, 0.5f));

            Vector3 desiredVelocity = Vector3.Transform(velocity, rotationMatrix);
            Vector3 desiredPosition =position+ desiredVelocity;
            Vector3 TEMP_DrawVel = new Vector3(desiredVelocity.X, desiredVelocity.Y, desiredVelocity.Z);

            BoundingBox lookAheadBox = new BoundingBox(desiredPosition + new Vector3(-(float)Math.Sqrt(_collisionBox.LengthX / 2), 0, -(float)Math.Sqrt(_collisionBox.LengthZ / 2)), desiredPosition + new Vector3((float)Math.Sqrt(_collisionBox.LengthX / 2), _collisionBox.Height, (float)Math.Sqrt(_collisionBox.LengthZ / 2)));
            
            //BoundingBox lookAheadBox = new BoundingBox(desiredPosition + new Vector3(-0.5f, 0, -0.5f), desiredPosition + new Vector3(0.5f, 2, 0.5f));

           // bool stopX = false, stopY = false, stopZ = false;
            Ray? mouseRay = null;
            if (TEMP_MouseClick != null && camera != null)
            {
                mouseRay = GetPointRay((Vector2)TEMP_MouseClick, camera);
            }
            float nearestBlockDist = float.MaxValue;
            AbstractBlock nearestBlock=null;
          
            foreach (AbstractBlock block in collidingTerrain)
            {
                
                BoundingBox voxelBox = new BoundingBox(block.TEMPMinPoint, block.TEMPMaxPoint);

                if (mouseRay != null)
                {
                    float? dist = ((Ray)mouseRay).Intersects(voxelBox);
                    if (dist != null&&((float)dist)<nearestBlockDist)
                    {
                        nearestBlock = block;
                        nearestBlockDist = (float)dist;
                    //    block.OnHit();
                   //     mouseRay = null;
                    }
                }

                Vector3 nor = new Vector3(desiredVelocity.X, desiredVelocity.Y, desiredVelocity.Z);
                nor.Normalize();

                Vector3 p1 = new Vector3(BoundingBox.Min.X, BoundingBox.Min.Y, BoundingBox.Min.Z);
                Vector3 p2 = new Vector3(BoundingBox.Min.X, BoundingBox.Min.Y, BoundingBox.Max.Z);
                Vector3 p3 = new Vector3(BoundingBox.Max.X, BoundingBox.Min.Y, BoundingBox.Min.Z);
                Vector3 p4 = new Vector3(BoundingBox.Max.X, BoundingBox.Min.Y, BoundingBox.Min.Z);

                List<Ray> rays = new List<Ray>();
                rays.Add(new Ray(p1, nor));
                rays.Add(new Ray(p2, nor));
                rays.Add(new Ray(p3, nor));
                rays.Add(new Ray(p4, nor));
                rays.Add(new Ray(position, nor));

                bool rayIntersects = false;
                foreach (Ray r in rays)
                {
                     if(r.Intersects(voxelBox) != null)
                     {
                         rayIntersects=true;
                         break;
                     }
                }
                if (lookAheadBox.Intersects(voxelBox) || rayIntersects)
                {
                    //1. Get all collision faces for Voxel
                    Dictionary<Direction, BlockCollisionFace> collisionFaces = block.GetCollisionFaceDirections();
                    List<BlockCollisionFace> collisionFaceList = new List<BlockCollisionFace>();
                    Direction firstFace = GetFirstFace(BoundingBox, position, voxelBox);

                    if (firstFace != Direction.NULL && collisionFaces.ContainsKey(firstFace))
                    {
                        collisionFaceList.Add(collisionFaces[firstFace]);
                    }

                    List<BlockCollisionFace> unorderedCollisionFaces = new List<BlockCollisionFace>();

                    foreach (var pair in collisionFaces)
                    {
                        if (pair.Key != firstFace)
                        {
                            unorderedCollisionFaces.Add(pair.Value);
                        }
                    }
                    //2. Order them by distance from position
                    unorderedCollisionFaces = unorderedCollisionFaces.OrderBy(x => x.GetDistanceFrom(position)).ToList<BlockCollisionFace>();
                    collisionFaceList.AddRange(unorderedCollisionFaces);


                    //3. Test for collision. If collision resolve and check further faces against resolved velocity
                    foreach (BlockCollisionFace face in collisionFaceList)//CollisionFace face = collisionFaces[0];
                    {
                        desiredPosition = position + desiredVelocity;
                        lookAheadBox = new BoundingBox(desiredPosition + new Vector3(-(float)Math.Sqrt(_collisionBox.LengthX / 2), 0, -(float)Math.Sqrt(_collisionBox.LengthZ / 2)), desiredPosition + new Vector3((float)Math.Sqrt(_collisionBox.LengthX / 2), _collisionBox.Height, (float)Math.Sqrt(_collisionBox.LengthZ / 2)));

                         nor = new Vector3(desiredVelocity.X, desiredVelocity.Y, desiredVelocity.Z);
                         if (nor != Vector3.Zero)
                         {
                             nor.Normalize();
                         }

                        rays = new List<Ray>();
                        rays.Add(new Ray(p1, nor));
                        rays.Add(new Ray(p2, nor));
                        rays.Add(new Ray(p3, nor));
                        rays.Add(new Ray(p4, nor));
                        rays.Add(new Ray(position, nor));

                         rayIntersects = false;
                        foreach (Ray r in rays)
                        {
                            if (r.Intersects(voxelBox) != null)
                            {
                                rayIntersects = true;
                                break;
                            }
                        }


                        if (lookAheadBox.Intersects(voxelBox) || rayIntersects)
                        {
                            //NOTE: Collisions disabled
                        //    desiredVelocity = face.ResolveCollisionAABB(lookAheadBox, desiredVelocity);
                        }
                    }
                }
            }

            //if (nearestBlock != null)
            //{
            //    if (TEMP_ColourBlock)
            //    {
            //        if (nearestBlock.BlockTextures[Direction.Up] == TextureName.Stone)
            //        {
            //            nearestBlock.BlockTextures[Direction.Up] = TextureName.Dirt;
            //        }
            //        else
            //        {
            //            nearestBlock.BlockTextures[Direction.Up] = TextureName.Stone;
            //        }

            //        nearestBlock.SetFaces();
            //        nearestBlock.TEMP_RequestBuildBuffers();
            //    }

            //    else if (TEMP_removeBlock)
            //    {
            //         nearestBlock.OnHit();
            //    }
            //    else
            //    {
            //        Direction nearestFaceDirection = Direction.NULL;
            //        float nearestBlockFaceDist = float.MaxValue;
            //        BlockCollisionFace nearestFace = null;
            //        foreach (KeyValuePair<Direction,BlockCollisionFace> face in nearestBlock.GetCollisionFaces())//CollisionFace face = collisionFaces[0];
            //        {
            //            if (mouseRay != null)
            //            {
            //                float? dist = face.Value.Intersects((Ray)mouseRay); //((Ray)mouseRay).Intersects(face.Value.Plane);

            //                if (dist != null && ((float)dist) < nearestBlockFaceDist)
            //                {
            //                    nearestBlockFaceDist = (float)dist;
            //                    nearestFaceDirection = face.Value.Facing;
            //                    nearestFace = face.Value;
            //                }
            //            }

            //        }

            //        if (nearestFace != null&&mouseRay!=null)
            //        {
            //            Vector3 ? pointOnFace=nearestFace.GetRayFaceIntersectionPoint((Ray)mouseRay);
            //            if (pointOnFace != null)
            //            {
            //                Vector3 point = (Vector3)pointOnFace;
            //                //Get nearest wall anchor (i.e val.5)

            //                float xDecimal;
            //                 float yDecimal;
            //                float zDecimal;
            //                //NOTE: Current code makes angle walls difficult to draw
            //                if(point.X>=0)
            //                    xDecimal = (int)point.X + 0.5f;
            //                else
            //                    xDecimal = (int)point.X - 0.5f;

            //                  if(point.Y>=0)
            //                yDecimal= (int)point.Y + 0.5f;
            //                  else
            //                       yDecimal= (int)point.Y - 0.5f;

            //                  if(point.Z>=0)
            //                 zDecimal = (int)point.Z + 0.5f;
            //                else
            //                      zDecimal = (int)point.Z - 0.5f ;

            //                Vector3 wallAnchor = new Vector3(xDecimal, yDecimal, zDecimal);

            //                if (TEMP_secondWallPoint == null)
            //                {
            //                    if (TEMP_firstWallPoint == null)//If no existing point
            //                    {
            //                        TEMP_firstWallPoint = wallAnchor;
            //                    }
            //                    else if ((Vector3)TEMP_firstWallPoint != wallAnchor)//If current point = existing point
            //                    {
            //                        //NOTE: Check that second point is within range of first
            //                        TEMP_secondWallPoint = new Vector3(wallAnchor.X, ((Vector3)TEMP_firstWallPoint).Y, wallAnchor.Z);//Ensure that both points have same Y value
            //                    }
            //                }

            //                if (TEMP_secondWallPoint != null)
            //                {
            //                    if (Math.Abs(((Vector3)TEMP_firstWallPoint).X - ((Vector3)wallAnchor).X) <= 1 &&
            //                       Math.Abs(((Vector3)TEMP_firstWallPoint).Z - ((Vector3)wallAnchor).Z) <= 1 &&
            //                       ((Vector3)TEMP_firstWallPoint).Y == ((Vector3)wallAnchor).Y)
            //                    {
            //                        TEMP_secondWallPoint = wallAnchor;
            //                    }
            //                    else
            //                    {

            //                        //Check that second point is within range of first
            //                        if (Math.Abs(((Vector3)TEMP_firstWallPoint).X - ((Vector3)TEMP_secondWallPoint).X) <= 1 &&
            //                            Math.Abs(((Vector3)TEMP_firstWallPoint).Z - ((Vector3)TEMP_secondWallPoint).Z) <= 1 &&
            //                            ((Vector3)TEMP_firstWallPoint).Y == ((Vector3)TEMP_secondWallPoint).Y)
            //                        {
            //                            for (int i = 1; i <= TEMP_WallHeight; i++)
            //                            {
            //                                ChunkManager.GetInstance().AddWall((Vector3)TEMP_firstWallPoint + new Vector3(0, i - 1, 0), (Vector3)TEMP_secondWallPoint + new Vector3(0, i - 1, 0));
            //                            }

            //                        }
            //                        TEMP_firstWallPoint = TEMP_secondWallPoint;
            //                        TEMP_secondWallPoint = null;
            //                    }
            //                }
            //            }
            //        }

            //       /* if (nearestFaceDirection != Direction.NULL)
            //        {
            //            //NOTE: Need to determine the face that was clicked to determine where to place block
            //            ChunkManager.GetInstance().AddWall(new Vector3(nearestBlock.Position.X + 0.5f, nearestBlock.Position.Y + 0.5f, nearestBlock.Position.Z + 0.5f),
            //                new Vector3(nearestBlock.Position.X + 0.5f, nearestBlock.Position.Y + 0.5f, nearestBlock.Position.Z + 1.5f));

            //         //   nearestBlock.OnAddBlock(nearestFaceDirection, new DirtBlock(BlockShape.Cube, Direction.North));
            //        }*/
            //    }
            //}

                //----Detailed Collision - NOTE: NOT WORKING---------

               // //1. Get all collision faces for Voxel
               // List<CollisionFace> collisionFaces = b.GetCollisionFaces();

               // //2. Order them by distance from position
               //collisionFaces=collisionFaces.OrderBy(x=>x.GetDistanceFrom(position)).ToList<CollisionFace>();

               // //3. Test for collision. If collision resolve and check further faces against resolved velocity
               //foreach (CollisionFace face in collisionFaces)//CollisionFace face = collisionFaces[0];
               //{
               //    desiredVelocity = face.ResolveCollision(_collisionBox, desiredVelocity);
               //}
    
            movementLines = new List<List<VertexPositionColor>>();
            List<VertexPositionColor> movementLine = new List<VertexPositionColor>();
            movementLine.Add(new VertexPositionColor(position,Color.LightBlue));
            movementLine.Add(new VertexPositionColor((position + /*desiredVelocity*5*/TEMP_DrawVel), Color.LightBlue));

            movementLines.Add(movementLine);

            xAxisLine = new List<VertexPositionColor>();
            xAxisLine.Add(new VertexPositionColor(position, Color.Red));
            xAxisLine.Add(new VertexPositionColor(new Vector3(position.X + 1, position.Y, position.Z), Color.Red));

            zAxisLine = new List<VertexPositionColor>();
            zAxisLine.Add(new VertexPositionColor(position, Color.Blue));
            zAxisLine.Add(new VertexPositionColor(new Vector3(position.X, position.Y, position.Z + 1), Color.Blue));

            foreach (Vector3 v in _collisionBox.GetVertices())
            {
                List<VertexPositionColor> line = new List<VertexPositionColor>();
                line.Add(new VertexPositionColor(v, Color.LightBlue));
                line.Add(new VertexPositionColor((v + /*desiredVelocity*5*/TEMP_DrawVel), Color.LightBlue));
                movementLines.Add(line);
            }

            _collisionBox.Update(position, rotation.Y);
         
            BoundingBox = new BoundingBox(_collisionBox.Position + new Vector3(-(float)Math.Sqrt(_collisionBox.LengthX / 2), 0, -(float)Math.Sqrt(_collisionBox.LengthZ / 2)), _collisionBox.Position + new Vector3((float)Math.Sqrt(_collisionBox.LengthX / 2), _collisionBox.Height, (float)Math.Sqrt(_collisionBox.LengthZ / 2)));
          
            position = position + desiredVelocity;



 
        }

        public override void HandleInput(Microsoft.Xna.Framework.GameTime gameTime, InputState input)
        {
          /*  if (input.CurrentKeyboardState.IsKeyDown(Keys.D1) && input.PreviousKeyboardState.IsKeyUp(Keys.D1))
            {
                tools = TEMP_Tools.AddBlock;
            }
            if (input.CurrentKeyboardState.IsKeyDown(Keys.D2) && input.PreviousKeyboardState.IsKeyUp(Keys.D2))
            {
                tools = TEMP_Tools.RemoveBlock;
            }
            if (input.CurrentKeyboardState.IsKeyDown(Keys.D3) && input.PreviousKeyboardState.IsKeyUp(Keys.D3))
            {
                tools = TEMP_Tools.AddWall;
            }
            if (input.CurrentKeyboardState.IsKeyDown(Keys.D4) && input.PreviousKeyboardState.IsKeyUp(Keys.D4))
            {
                tools = TEMP_Tools.RemoveWall;
            }
            if (input.CurrentKeyboardState.IsKeyDown(Keys.D5) && input.PreviousKeyboardState.IsKeyUp(Keys.D5))
            {
                tools = TEMP_Tools.PaintFace;
            }
            if (input.CurrentKeyboardState.IsKeyDown(Keys.D6) && input.PreviousKeyboardState.IsKeyUp(Keys.D6))
            {
                tools = TEMP_Tools.PaintHalfFace;
            }*/

            TEMP_MouseClick = null;

            TEMP_MouseClick = new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y);

            if (camera != null&&tools!=TEMP_Tools.NULL)
            {
                TEMP_MouseRay = GetPointRay((Vector2)TEMP_MouseClick, camera);

                TEMP_selectedObject = VoxelRaycastUtility.GetNearestIntersectingWorldObject(TEMP_MouseRay);
            }


            switch (tools)
            {
                case TEMP_Tools.AddBlock:
                    {
                        if (input.CurrentMouseState.LeftButton == ButtonState.Pressed && input.PreviousMouseState.LeftButton == ButtonState.Released)
                        {
                            if (TEMP_selectedObject != null && TEMP_selectedObject is AbstractBlock)
                            {
                                BuildTools.AddBlock(TEMP_MouseRay, TEMP_selectedObject as AbstractBlock);
                            }
                        }

                        break;
                    }
                case TEMP_Tools.RemoveBlock:
                    {
                        if (input.CurrentMouseState.LeftButton == ButtonState.Pressed && input.PreviousMouseState.LeftButton == ButtonState.Released)
                        {
                            if (TEMP_selectedObject != null && TEMP_selectedObject is AbstractBlock)
                            {
                                BuildTools.RemoveBlock(TEMP_selectedObject as AbstractBlock);
                            }
                        }

                        break;
                    }
                case TEMP_Tools.AddWall:
                    {
                        if (input.CurrentMouseState.LeftButton == ButtonState.Pressed)
                        {
                            BuildTools.AddWall((AbstractTerrainObject)TEMP_selectedObject, TEMP_MouseRay);
                        }

                        if (input.CurrentMouseState.LeftButton == ButtonState.Released)
                        {
                            //Clear wall, build wall
                            BuildTools.BuildWalls_Released();
                        }

                        break;
                    }
                case TEMP_Tools.RemoveWall:
                    {
                        if (input.CurrentMouseState.LeftButton == ButtonState.Pressed && input.PreviousMouseState.LeftButton == ButtonState.Released)
                        {
                            if (TEMP_selectedObject != null && TEMP_selectedObject is Wall)
                            {
                                BuildTools.RemoveWall(TEMP_selectedObject as Wall);
                            }
                        }
                        break;
                    }
                case TEMP_Tools.PaintFace:
                    {
                        if (input.CurrentMouseState.LeftButton == ButtonState.Pressed && input.PreviousMouseState.LeftButton == ButtonState.Released)
                        {
                            BuildTools.PaintFullFace((AbstractTerrainObject)TEMP_selectedObject, TEMP_MouseRay);
                        }
                        break;
                    }
                case TEMP_Tools.PaintHalfFace:
                        {
                            break;
                        }

            }

             if (input.CurrentMouseState.LeftButton == ButtonState.Pressed && input.PreviousMouseState.LeftButton == ButtonState.Released)
             {
                 TEMP_selectedObjects = null;

             
             /*    TEMP_selectedObjects = VoxelRaycastUtility.VoxelIntersectionGrid(TEMP_MouseRay);

                 mouseRayLine = new List<VertexPositionColor>();
                 mouseRayLine.Add(new VertexPositionColor(TEMP_MouseRay.Position, Color.White));
                 mouseRayLine.Add(new VertexPositionColor(TEMP_MouseRay.Position + TEMP_MouseRay.Direction * 1000, Color.White));
                 
                 */

                 if (TEMP_selectedObject != null && TEMP_selectedObject is AbstractBlock)
                 {
                  //   ((AbstractBlock)TEMP_selectedObject).OnHit();
                 }
             }


            if (input.CurrentKeyboardState.IsKeyDown(Keys.Tab) && input.PreviousKeyboardState.IsKeyUp(Keys.Tab))
            {
                BuildTools.ToggleWallHeight(); 
            }

            if (input.CurrentMouseState.LeftButton == ButtonState.Released)
            {

            }

            if (input.CurrentMouseState.LeftButton == ButtonState.Pressed /*&& input.PreviousMouseState.LeftButton == ButtonState.Released*/)
            {
              /*  if(input.CurrentKeyboardState.IsKeyDown(Keys.LeftControl))
                {
                    TEMP_removeBlock=false;
                }
                else
                {
                    TEMP_removeBlock = true;
                }

                if (input.CurrentKeyboardState.IsKeyDown(Keys.LeftShift))
                {
                    TEMP_ColourBlock = true;
                }
                else
                {
                    TEMP_ColourBlock = false;
                }*/


              
                if(camera!=null)
                {
                    Ray mouseRay = GetPointRay((Vector2)TEMP_MouseClick, camera);
                    Vector3 point = GetWorldSpacePoint(new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y), camera);
                 


                    Vector3 rotVec = position-point;
                    rotVec = new Vector3((float)Math.Round(rotVec.X, 2), (float)Math.Round(rotVec.Y, 2),(float) Math.Round(rotVec.Z, 2));
                    if (rotVec != Vector3.Zero)
                    {
                        rotVec.Normalize();
                    }

                    //Default rotation is along x axis
                    Vector3 defRotVec = new Vector3(1, 0, 0);


                    float dot = Vector3.Dot(rotVec, defRotVec);

                    int sign = 1;

                    //If angle is greater than 180 degrees (i.e. on the negative side of the default rotation vector), give it
                    //a negative angle
                    if (point.Z<position.Z)
                    {
                        sign = -1;
                    }

                    double angle = sign*Math.Acos(dot) * (180 / Math.PI);
                    rotation.Y = MathHelper.ToRadians((float)angle);
                }
            }
         /*   if (input.CurrentMouseState.MiddleButton == ButtonState.Pressed)
            {
                ScreenManager.GetInstance().Game.IsMouseVisible = false;
                //Only record position on click
                if (input.PreviousMouseState.MiddleButton == ButtonState.Released)
                {
                    TEMP_mouseClickPosition = new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                }

                TEMP_mouseDelta = input.CurrentMouseState.X - TEMP_mouseClickPosition.X;
                RotateY(TEMP_mouseDelta);
                Mouse.SetPosition((int)TEMP_mouseClickPosition.X, (int)TEMP_mouseClickPosition.Y);
            }
            else
            {
                ScreenManager.GetInstance().Game.IsMouseVisible = true;
            }*/

        //    Vector3 positiveCameraDirection = cameraTarget - CameraPosition;
             velocity = Vector3.Zero;

             float slow = 1;
             if (input.CurrentKeyboardState.IsKeyDown(Keys.LeftShift))
             {
                 slow = 5;
             }
             else
             {
                 slow = 1;
             }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.W))
            {
                velocity.X -= moveSpeed/slow;
           
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.D))
            {
                velocity.Z -= moveSpeed / slow;
               
            }
            if (input.CurrentKeyboardState.IsKeyDown(Keys.S))
            {
                velocity.X += moveSpeed / slow;
               

            }
            if (input.CurrentKeyboardState.IsKeyDown(Keys.A))
            {
                velocity.Z += moveSpeed / slow;
              
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.Space)/* && input.PreviousKeyboardState.IsKeyUp(Keys.Space)*/)
            {
              //  if (OnGround)
                {
                   // position = new Vector3(0, 2, 0);
                    TEMP_velJump = 0.4f;
                   // velocity.Y = -4;
                }
            }


        }
      /*  public override void Update()
        {
        }*/
        public override void Draw(Camera camera)
        {
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch Batch, Camera camera)
        {
            this.camera = camera;

            if (isDrawable)
            {
                BoundingFrustum Frustum = new BoundingFrustum(camera.CameraViewMatrix * camera.CameraProjectionMatrix);

                DrawWireframe(ShaderManager.GetInstance().DefaultEffect, ScreenManager.GetInstance().GraphicsDevice);

                if (TEMP_selectedObject != null)
                {
                    TEMP_selectedObject.DrawRotatedBoundingBox(ShaderManager.GetInstance().DefaultEffect, ScreenManager.GetInstance().GraphicsDevice);
                }

                // if (type == Frustum.Contains(worldObject.Position))
             /*   if (model != null)
                {
                    foreach (ModelMesh mesh in model.Meshes)
                    {
                        BoundingSphere sourceSphere = new BoundingSphere(Position, mesh.BoundingSphere.Radius);

                        //Get the radius and position of the Object's bounding sphere
                        // BoundingSphere sourceSphere = new BoundingSphere(new Vector3(0, 0, 0), mesh.BoundingSphere.Radius);

                        if (Frustum.Intersects(sourceSphere))
                        {
                            foreach (BasicEffect effect in mesh.Effects)
                            {
                                // Make sure lighting and texturing is enabled so we can see the result
                                //  effect.EnableDefaultLighting();
                                //   effect.PreferPerPixelLighting=true;
                                effect.TextureEnabled = true;

                                // Begin to render with the specified texture
                                if (texture != null)
                                {
                                    effect.Texture = texture;
                                }

                                effect.World = Matrix.CreateFromYawPitchRoll(-rotation.Y, rotation.X, rotation.Z)
                                      * Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);

                                effect.Projection = camera.CameraProjectionMatrix;
                                effect.View = camera.CameraViewMatrix;
                            }


                            mesh.Draw();
                        }
                      
                    }
                }*/
            }
        }

        public void DrawWireframe(BasicEffect effect, GraphicsDevice graphicsDevice)
        {
            List<VertexPositionColor> wireframe = GetAABBWireframe();
           ShaderManager.GetInstance().DefaultEffect.TextureEnabled = false;
           ShaderManager.GetInstance().DefaultEffect.VertexColorEnabled = true;
           ShaderManager.GetInstance().DefaultEffect.CurrentTechnique.Passes[0].Apply();

            graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, wireframe.ToArray(), 0, wireframe.Count - 1);


            List<VertexPositionColor> collisionWireframe = _collisionBox.GetCollisionBoxWireframe(Color.White);

            graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, facingLine.ToArray(), 0, facingLine.Count - 1);

#region Axes
          

            graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, xAxisLine.ToArray(), 0, xAxisLine.Count - 1);

            graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, zAxisLine.ToArray(), 0, zAxisLine.Count - 1);
#endregion
            if (mouseRayLine != null && mouseRayLine.Count > 1)
            {
                graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, mouseRayLine.ToArray(), 0, mouseRayLine.Count - 1);
            }

            if (TEMP_selectedObjects!=null&&TEMP_selectedObjects.Count > 0)
            {
                foreach (AbstractWorldObject obj in TEMP_selectedObjects)
                {
                    obj.DrawRotatedBoundingBox(effect, graphicsDevice);
                }
            }

            foreach (List<VertexPositionColor> movementLine in movementLines)
            {
                graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, movementLine.ToArray(), 0, movementLine.Count - 1);
            }
            graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, collisionWireframe.ToArray(), 0, collisionWireframe.Count - 1);
        }


        protected List<VertexPositionColor> GetAABBWireframe()
        {
            List<VertexPositionColor> result = new List<VertexPositionColor>();
            Color colour = Color.Orange;
            Vector3[] bottomFace = new Vector3[4];
            Vector3[] topFace = new Vector3[4];

            bottomFace[0] = BoundingBox.Min;
            bottomFace[1] = new Vector3(BoundingBox.Max.X, BoundingBox.Min.Y, BoundingBox.Min.Z);
            bottomFace[2] =new Vector3(BoundingBox.Max.X, BoundingBox.Min.Y, BoundingBox.Max.Z);
            bottomFace[3] = new Vector3(BoundingBox.Min.X, BoundingBox.Min.Y, BoundingBox.Max.Z);

            topFace[0] =new Vector3(BoundingBox.Min.X, BoundingBox.Max.Y, BoundingBox.Min.Z);
            topFace[1] = new Vector3(BoundingBox.Max.X, BoundingBox.Max.Y, BoundingBox.Min.Z);
            topFace[2] = BoundingBox.Max;
            topFace[3] =new Vector3(BoundingBox.Min.X, BoundingBox.Max.Y, BoundingBox.Max.Z);


            //Create 6 faces of cube
            //create bottom Face
            result = new List<VertexPositionColor>();
            
            for (int i = 0; i < bottomFace.Length; i++)
            {
                result.Add(new VertexPositionColor(bottomFace[i], colour));
            }
            //Loop connect vertex back to original
            result.Add(new VertexPositionColor(bottomFace[0], colour));
            //create top Face
            for (int i = 0; i < topFace.Length; i++)
            {
                result.Add(new VertexPositionColor(topFace[i], colour));
            }
            result.Add(new VertexPositionColor(topFace[0], colour));


            result.Add(new VertexPositionColor(topFace[1], colour));
            result.Add(new VertexPositionColor(bottomFace[1], colour));


            result.Add(new VertexPositionColor(bottomFace[2], colour));
            result.Add(new VertexPositionColor(topFace[2], colour));
            result.Add(new VertexPositionColor(topFace[3], colour));
            result.Add(new VertexPositionColor(bottomFace[3], colour));

            return result;
        }

        /// <summary>
        /// Gets the first face of a box the collision detection should query
        /// </summary>
        /// <param name="box"></param>
        /// <param name="voxelCentre"></param>
        /// <returns></returns>
        private Direction GetFirstFace(BoundingBox box, Vector3 position, BoundingBox voxelBox)
        {
            Direction result = Direction.NULL;
            if (position.Y >= voxelBox.Max.Y)//Above voxel
            {
                result = Direction.Up;
            }
            else if (position.Y + box.Max.Y <= voxelBox.Min.Y)//below voxel
            {
                result = Direction.Down;
            }

            else if (position.Z <= voxelBox.Min.Z)
            {
                result = Direction.West;
            }

            else if (position.Z >= voxelBox.Max.Z)
            {
                result = Direction.East;
            }

            else if (position.X <= voxelBox.Min.X)
            {
                result = Direction.South;
            }

            else if (position.X >= voxelBox.Max.X)
            {
                result = Direction.North;
            }

            //NOTE: Need to include corner cases

            //In corner case, take previous velocity into account

            return result;
        }


        /// <summary>
        /// Takes a viewport point and converts to a world space point on the same horizontal plane as the player's position
        /// </summary>
        /// <param name="point"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        protected Vector3 GetWorldSpacePoint(Vector2 point, Camera camera)
        {
            Vector2 result = Vector2.Zero;

            Ray ray = GetPointRay(point, camera);
            // Calculate the ray-plane intersection point.
            Vector3 n = new Vector3(0f, 1f, 0f);
            Plane p = new Plane(position, position + new Vector3(1, 0, 0), position + new Vector3(0, 0, 1));

            // Calculate distance of intersection point from r.origin.
            float denominator = Vector3.Dot(p.Normal, ray.Direction);
            float numerator = Vector3.Dot(p.Normal, ray.Position) + p.D;
            float t = -(numerator / denominator);

            // Calculate the picked position on the horizontal plane.
            Vector3 pickedPosition = ray.Position + ray.Direction * t;



            //  result = new Vector2(GetValidCoordinate(pickedPosition.X), GetValidCoordinate(pickedPosition.Z));

            return pickedPosition;
        }

        protected Ray GetPointRay(Vector2 point, Camera camera)
        {
            //GraphicsDevice graphicsDevice = game.GraphicsDevice;

            Vector3 nearSource = new Vector3((float)point.X, (float)point.Y, 0f);
            Vector3 farSource = new Vector3((float)point.X, (float)point.Y, 1f);
            Vector3 nearPoint = ScreenManager.GetInstance().GraphicsDevice.Viewport.Unproject(nearSource, camera.CameraProjectionMatrix, camera.CameraViewMatrix, Matrix.Identity);
            Vector3 farPoint = ScreenManager.GetInstance().GraphicsDevice.Viewport.Unproject(farSource, camera.CameraProjectionMatrix, camera.CameraViewMatrix, Matrix.Identity);

            // Create a ray from the near clip plane to the far clip plane.
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            // Create a ray.
            Ray ray = new Ray(nearPoint, direction);



            return ray;
        }
    }
}
