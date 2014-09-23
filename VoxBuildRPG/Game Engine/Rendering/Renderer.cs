using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine;
using VoxelRPGGame.GameEngine.EnvironmentState;
using VoxelRPGGame.GameEngine.Rendering;
using VoxelRPGGame.GameEngine.World.Textures;
using VoxelRPGGame.GameEngine.World.Voxels;
using VoxelRPGGame.MenuSystem;
using VoxelRPGGame.MenuSystem.Screens;

namespace VoxBuildRPG.GameEngine.Rendering
{
    public class Renderer
    {
        private Color voidColour = Color.LightBlue;

        RenderTarget2D _depthTarget;
        RenderTarget2D _normalTarget;
        RenderTarget2D _lightTarget;
        RenderTarget2D _outputTarget;

        GraphicsDevice graphicsDevice;
        int viewWidth = 0, viewHeight = 0;

        public Renderer(GraphicsDevice GraphicsDevice)
        {
            viewWidth = GraphicsDevice.Viewport.Width;
            viewHeight = GraphicsDevice.Viewport.Height;

            // Create the three render targets
            _depthTarget = new RenderTarget2D(GraphicsDevice, viewWidth,
             viewHeight, false, SurfaceFormat.Single, DepthFormat.Depth24);

            _normalTarget = new RenderTarget2D(GraphicsDevice, viewWidth,
             viewHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);

            _lightTarget = new RenderTarget2D(GraphicsDevice, viewWidth,
             viewHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);

            _outputTarget = new RenderTarget2D(GraphicsDevice, viewWidth,
             viewHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);

            // Load effects
           /* depthNormalEffect = Content.Load<Effect>("PPDepthNormal");
            lightingEffect = Content.Load<Effect>("PPLight");

            // Set effect parameters to light mapping effect
            lightingEffect.Parameters["viewportWidth"].SetValue(viewWidth);
            lightingEffect.Parameters["viewportHeight"].SetValue(viewHeight);
            */
            this.graphicsDevice = GraphicsDevice;
        }



        public void Draw(SpriteBatch Batch, GameState state)
        {

            ShaderManager.GetInstance().DefaultEffect.View = state.ActiveCamera.CameraViewMatrix;
            ShaderManager.GetInstance().DefaultEffect.Projection = state.ActiveCamera.CameraProjectionMatrix;

            ScreenManager.GetInstance().GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ScreenManager.GetInstance().GraphicsDevice.Clear(voidColour);
            state.ActiveCamera.Draw();

            //NOTE: Will need to split drawing into DrawRenderTargets and DrawObjects...
        /*    foreach (AbstractWorldObject gameObject in state.GetRenderState())
            {
                if (gameObject.IsDrawable)
                {
                    gameObject.Draw(Batch, state.ActiveCamera);
                }
            }*/

            ShaderManager.GetInstance().DefaultEffect.Texture = TextureAtlas.GetInstance().Atlas;

            ShaderManager.GetInstance().DefaultEffect.TextureEnabled = true;
            ShaderManager.GetInstance().DefaultEffect.VertexColorEnabled = false;
            ShaderManager.GetInstance().DefaultEffect.CurrentTechnique.Passes[0].Apply();

                
            foreach (Chunk c in state.GetChunks())
            {
                ScreenManager.GetInstance().GraphicsDevice.SetVertexBuffer(c.VertexBuffer);
                ScreenManager.GetInstance().GraphicsDevice.Indices = c.IndexBuffer;

                DebugScreen.GetInstance().PolysDrawn += c.Indices.Length / 3;
                DebugScreen.GetInstance().VertsDrawn += c.Vertices.Length;
                if (c.Vertices.Length > 0)
                {
                    ScreenManager.GetInstance().GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, c.Vertices.Length, 0, c.Indices.Length / 3);
                }
               // c.DrawChunk();
            }

        }

    }
}
