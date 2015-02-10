//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;

//using VoxelRPGGame.MenuSystem;
//using VoxelRPGGame.MenuSystem.Screens;

//using VoxelRPGGame.GameEngine.EnvironmentState;
//using VoxelRPGGame.GameEngine.World.Voxels;
//using VoxelRPGGame.GameEngine.World;
//using VoxelRPGGame.GameEngine.World.Textures;
//

//namespace VoxelRPGGame.GameEngine.UI.World
//{
//    /// <summary>
//    /// Class that handles the rendering of all 3D elements of the game state
//    /// </summary>
//    public class GameWorldScreen : AbstractRendererScreen
//    {
//        protected Camera camera;


//        protected VertexBuffer vertexBuffer = null;
//        protected IndexBuffer indexBuffer = null;

//        private Color voidColour = Color.LightBlue;

//        List<AbstractBlock> TEMPwireframes = new List<AbstractBlock>();

//        int x = 0, y = 16, z = 0;
//        Random r = new Random();

//        public GameWorldScreen()
//        {
//            camera = new Camera(ScreenManager.GetInstance().GraphicsDevice);

//            //  ChunkManager.GetInstance().BuffersChanged += SetBuffers;

//        }

//        public void Initialise(GameState gameState)
//        {
//            foreach (AbstractWorldObject gameObject in gameState.GetRenderState())
//            {
//                Load3DObject(gameObject);
//                if (gameObject is TEMPPlayerActor)
//                {
//                    camera.TargetObject = gameObject;
//                }
//            }


//        }

//        public void Load3DObject(AbstractWorldObject gameObject)
//        {
//            //  gameObject.SetRendererEventHandlers(this);
//            Load3DModel(gameObject);
//        }

//        public override void Update(GameTime theTime, GameState gameState)
//        {

//            //foreach (AbstractWorldObject gameObject in gameState.GetRenderState())
//            //{
//            //    gameObject.Update();

//            //    if (gameObject is TEMPPlayerActor)
//            //    {
//            //        TEMPwireframes = new List<AbstractBlock>();
//            //        foreach (AbstractBlock b in ChunkManager.GetInstance().GetNearbyBlocks(gameObject.Position, 4, 3, 4))
//            //        {
//            //            BoundingBox box = new BoundingBox(b.TEMPMinPoint, b.TEMPMaxPoint);

//            //          /*  if ((gameObject as TEMPPlayerActor).BoundingBox.Intersects(box))
//            //            {
//            //                b.wireFrameColour = Color.Red;
//            //            }
//            //            else
//            //            {
//            //                b.wireFrameColour = new Color(40,40,40);
//            //            }*/

//            //            TEMPwireframes.Add(b);


//            //        }
//            //        (gameObject as TEMPPlayerActor).Update(TEMPwireframes);

//            //    }
//            //}

//            //camera.Update(Vector3.Zero);
//        }

//        public override void HandleInput(GameTime gameTime, InputState input, GameState gameState)
//        {
//            //NOTE:Make GameWorldScreen in charge of gameState and collapse Engine functionality into this class
//            //gameState.HandleInput(input);
//            //  //NOTE: May need to find a better way of doing this
//            //  //Possibly make the game stat a singleton and store the world camera there
//            //  input.WorldCamera = camera;

//            ////Add input handling for 3D objects i.e. user selection etc.

//            //  camera.HandleInput(input);


//            //  if(input.CurrentKeyboardState.IsKeyDown(Keys.Enter)&&input.PreviousKeyboardState.IsKeyUp(Keys.Enter))
//            //  {
//            //      if (input.CurrentKeyboardState.IsKeyDown(Keys.LeftShift))
//            //      {
//            //          ChunkManager.GetInstance().RemoveBlock(new Vector3(x, y-1, z));
//            //      }
//            //      else
//            //      {
//            //          ChunkManager.GetInstance().AddBlock(new Vector3(x, y, z), new DirtBlock(BlockShape.Cube, Direction.North));
//            //      }
//            ////      ChunkManager.GetInstance().AddBlock(new Vector3(x+1, y, z), new DirtBlock(BlockShape.Cube, Direction.North));
//            ////      ChunkManager.GetInstance().AddBlock(new Vector3(x, y, z+1), new DirtBlock(BlockShape.Cube, Direction.North));
//            ////      ChunkManager.GetInstance().AddBlock(new Vector3(x+1, y, z + 1), new DirtBlock(BlockShape.Cube, Direction.North));
//            //      if (r.NextDouble() < 0.5)
//            //      {
//            //          x++;
//            //      }
//            //      else
//            //      {
//            //          z++;
//            //      }
//            //  }

//            //Check for mouse selection
//            /*    if (input.CurrentMouseState.LeftButton == ButtonState.Pressed && input.CurrentMouseState.LeftButton == ButtonState.Released)
//                {

//                }*/


//            //foreach (AbstractWorldObject gameObject in gameState.GetRenderState())
//            //{
//            //    if (gameObject.HasFocus)
//            //    {
//            //        gameObject.HandleInput(gameTime, input);
//            //    }
//            //}
//        }

//        public override void Draw(SpriteBatch Batch, GameState state)
//        {
//            //  //NOTE: Will need to split drawing into DrawRenderTargets and DrawObjects...

//            //  ScreenManager.GetInstance().GraphicsDevice.DepthStencilState = DepthStencilState.Default;
//            //  ScreenManager.GetInstance().GraphicsDevice.Clear(voidColour);
//            //  //NOTE: Will need to split drawing into DrawRenderTargets and DrawObjects...
//            //  foreach (AbstractWorldObject gameObject in state.GetRenderState())
//            //  {
//            //      if (gameObject.IsDrawable)
//            //      {
//            //          gameObject.Draw(Batch, camera);
//            //      }
//            //  }
//            // // DrawBlocks(state.GetBlocks());
//            ////  DrawVertices(state.GetVertices(), state.GetIndices(), state.GetTEMPTexture());

//            //  foreach (Chunk c in state.GetChunks())
//            //  {
//            //      c.DrawChunk();
//            //  }
//            //  camera.Draw();

//            //  if (DebugScreen.GetInstance().IsVisible)
//            //  {
//            //      foreach (AbstractBlock b in TEMPwireframes)
//            //      {
//            //          b.DrawWireframe(ShaderManager.GetInstance().DefaultEffect, ScreenManager.GetInstance().GraphicsDevice);
//            //      }

//            //      ShaderManager.GetInstance().DefaultEffect.TextureEnabled = false;
//            //      ShaderManager.GetInstance().DefaultEffect.VertexColorEnabled = true;
//            //      ShaderManager.GetInstance().DefaultEffect.CurrentTechnique.Passes[0].Apply();

//            //      foreach (Chunk c in state.GetChunks())
//            //      {
//            //          ScreenManager.GetInstance().GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, c.Wireframe.ToArray(), 0, c.Wireframe.Count - 1);
//            //      }
//            //  }

//        }


//        //protected void DrawBlocks(List<AbstractBlock> blocks)
//        //{
//        //    TextureName texture;

//        //    if (blocks.Count > 0)
//        //    {
//        //        texture = blocks[0].TEMPTexture;


//        //        //NOTE: Need to centralise the setting of effects, textures and drawing of voxels
//        //        List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
//        //    /*    if (DebugScreen.GetInstance().IsVisible)//Only call this if drawing wireframes - calls to apply passes are expensive
//        //        {
//        //           ShaderManager.GetInstance().DefaultEffect.TextureEnabled = false;
//        //           ShaderManager.GetInstance().DefaultEffect.VertexColorEnabled = true;
//        //           ShaderManager.GetInstance().DefaultEffect.CurrentTechnique.Passes[0].Apply();
//        //        }*/

//        //        AbstractBlock[] arrayBlocks = blocks.ToArray<AbstractBlock>();

//        //        for(int i =0;i<arrayBlocks.Length;i++)
//        //        {
//        //            //Draw wireframes when debug showing
//        //        /*    if (DebugScreen.GetInstance().IsVisible)
//        //            {
//        //                ScreenManager.GetInstance().GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, arrayBlocks[i].Wireframe.ToArray(), 0, arrayBlocks[i].Wireframe.Count - 1);
//        //            }*/


//        //        //    vertices.AddRange(arrayBlocks[i].Faces);
//        //         //   arrayBlocks[i].Draw(camera);
//        //        }

//        //        //NOTE: Don't create the vertex buffer on each draw, only change it if the data to go in the buffer changes
//        //        if (vertexBuffer == null)
//        //        {
//        //            vertexBuffer = new VertexBuffer(ScreenManager.GetInstance().GraphicsDevice,VertexPositionNormalTexture.VertexDeclaration,
//        //          vertices.Count,BufferUsage.WriteOnly);

//        //            vertexBuffer.SetData(vertices.ToArray());

//        //        }
//        //        if(indexBuffer ==null)
//        //        ScreenManager.GetInstance().GraphicsDevice.SetVertexBuffer(vertexBuffer);
//        //        ShaderManager.GetInstance().DefaultEffect.Texture = TextureAtlas.GetInstance().Atlas;

//        //        ShaderManager.GetInstance().DefaultEffect.TextureEnabled = true;
//        //        ShaderManager.GetInstance().DefaultEffect.VertexColorEnabled = false;
//        //        ShaderManager.GetInstance().DefaultEffect.CurrentTechnique.Passes[0].Apply();


//        //        DebugScreen.GetInstance().PolysDrawn = vertices.Count / 3;
//        //        DebugScreen.GetInstance().VertsDrawn = vertices.Count;
//        //        ScreenManager.GetInstance().GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertices.Count / 3);

//        //    }



//        //}


//        //protected void DrawVertices(/*List<AbstractBlock> blocks*/VertexPositionNormalTexture[] vertices, int[] indices, Texture2D texture)
//        //{
//        //    // Texture2D texture=null;

//        //    /*   if (blocks.Count > 0)
//        //       {
//        //           texture = blocks[0].TEMPTexture;
//        //       */

//        //    //NOTE: Need to centralise the setting of effects, textures and drawing of voxels
//        //    //     List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
//        //    /*    if (DebugScreen.GetInstance().IsVisible)//Only call this if drawing wireframes - calls to apply passes are expensive
//        //        {
//        //           ShaderManager.GetInstance().DefaultEffect.TextureEnabled = false;
//        //           ShaderManager.GetInstance().DefaultEffect.VertexColorEnabled = true;
//        //           ShaderManager.GetInstance().DefaultEffect.CurrentTechnique.Passes[0].Apply();
//        //        }*/

//        //    //   AbstractBlock[] arrayBlocks = blocks.ToArray<AbstractBlock>();

//        //    //   for(int i =0;i<arrayBlocks.Length;i++)
//        //    //   {
//        //    //Draw wireframes when debug showing
//        //    /*    if (DebugScreen.GetInstance().IsVisible)
//        //        {
//        //            ScreenManager.GetInstance().GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, arrayBlocks[i].Wireframe.ToArray(), 0, arrayBlocks[i].Wireframe.Count - 1);
//        //        }*/


//        //    //    vertices.AddRange(arrayBlocks[i].Faces);
//        //    //   arrayBlocks[i].Draw(camera);
//        //    //    }

//        //    //NOTE: Don't create the vertex buffer on each draw, only change it if the data to go in the buffer changes
//        //    if (vertexBuffer == null)
//        //    {
//        //        vertexBuffer = new VertexBuffer(ScreenManager.GetInstance().GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration,
//        //      vertices.Length, BufferUsage.WriteOnly);

//        //        vertexBuffer.SetData(vertices);

//        //    }
//        //    if (indexBuffer == null)
//        //    {
//        //        indexBuffer = new IndexBuffer(ScreenManager.GetInstance().GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
//        //        indexBuffer.SetData(indices);
//        //    }

//        //    ScreenManager.GetInstance().GraphicsDevice.SetVertexBuffer(vertexBuffer);
//        //    ScreenManager.GetInstance().GraphicsDevice.Indices = indexBuffer;

//        //   ShaderManager.GetInstance().DefaultEffect.Texture = texture;

//        //   ShaderManager.GetInstance().DefaultEffect.TextureEnabled = true;
//        //   ShaderManager.GetInstance().DefaultEffect.VertexColorEnabled = false;



//        //   ShaderManager.GetInstance().DefaultEffect.CurrentTechnique.Passes[0].Apply();


//        //    //      DebugScreen.GetInstance().PolysDrawn = vertices.Length / 3;
//        //    DebugScreen.GetInstance().PolysDrawn = indices.Length / 3;
//        //    DebugScreen.GetInstance().VertsDrawn = vertices.Length;

//        //    //ScreenManager.GetInstance().GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertices.Length / 3);
//        //    ScreenManager.GetInstance().GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Length / 3);
//        //    //  }



//        //}

//        //public void SetBuffers(VertexPositionNormalTexture[] vertices, int[] indices)
//        //{
//        //    vertexBuffer = new VertexBuffer(ScreenManager.GetInstance().GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration,
//        //     vertices.Length, BufferUsage.WriteOnly);

//        //    vertexBuffer.SetData(vertices);

//        //    indexBuffer = new IndexBuffer(ScreenManager.GetInstance().GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
//        //    indexBuffer.SetData(indices);


//        //}

//        #region Loading

//        public bool Load3DModel(AbstractWorldObject gameObject)
//        {
//            Boolean result = false;
//            if (gameObject.ModelFileLocation != null)
//            {
//                try
//                {
//                    gameObject.Model = ScreenManager.GetInstance().ContentManager.Load<Model>(gameObject.ModelFileLocation);
//                    result = true;
//                }

//                catch (ContentLoadException e)
//                {
//                    result = false;
//                }
//            }

//            if (gameObject.TextureLocation != null)
//            {
//                try
//                {
//                    gameObject.Texture = ScreenManager.GetInstance().ContentManager.Load<Texture2D>(gameObject.TextureLocation);
//                    result = true;
//                }
//                catch (ContentLoadException e)
//                {
//                    result = false;
//                }
//            }
//            return result;
//        }



//        #endregion

//        #region Event Handlers



//        public void SnapToCamera(AbstractWorldObject cameraFocusObject)
//        {
//            camera.SoftLockToObject(cameraFocusObject);
//        }


//        #endregion


//    }
//}
