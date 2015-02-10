using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxBuildRPG.Game_Engine.Rendering.Lighting;
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
        private Model sphereModel, sphereModel2;

        private Color voidColour = Color.LightBlue;

        private float TEMP_Ambient = 0.4f;

        RenderTarget2D _depthTarget;
        RenderTarget2D _normalTarget;
        RenderTarget2D _lightTarget;
        RenderTarget2D _outputTarget;

        List<RenderTarget2D> _renderTargets = new List<RenderTarget2D>();
        int _selectedRenderTarget = 0;

        GraphicsDevice graphicsDevice;
        int viewWidth = 0, viewHeight = 0;
        List<PointLight> TEMPlights = new List<PointLight>();
        int TEMPRenderCount = 0;
        int TEMPincrement = 1;

        public Renderer(GraphicsDevice GraphicsDevice)
        {
             TEMPlights = new List<PointLight>();
            TEMPlights.Add(new PointLight
            {
                DiffuseColour = new Vector3(1, 1, 1),
                Position = new Vector3(50, 10, 30),
                SpecularColour = new Vector3(0, 0, 1),
                Attenuation = 50,
                Falloff = 2
            }
                );

            TEMPlights.Add(new PointLight
            {
                DiffuseColour = new Vector3(1, 0, 0),
                Position = new Vector3(-10, 10, -10),
                SpecularColour = new Vector3(1, 0, 0),
                Attenuation = 20,
                Falloff = 20
            }
              );

            TEMPlights.Add(new PointLight
            {
                DiffuseColour = new Vector3(1, 1, 1),
                Position = new Vector3(0, 10, 30),
                SpecularColour = new Vector3(0, 1, 0),
                Attenuation = 50,
                Falloff = 2
            }
              );



            sphereModel = ScreenManager.GetInstance().ContentManager.Load<Model>("Models\\sphere");
            GameWorldControlState.GetInstance().ToggleShadersEvent += ToggleActiveRenderTarget;

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

            */
            this.graphicsDevice = GraphicsDevice;
        }



        public void Render(SpriteBatch Batch, Camera activeCamera, List<IRenderable> renderItems)
        {
            TEMPRenderCount += TEMPincrement;
            if (TEMPRenderCount >= 100)
            {
                TEMPincrement = -1;
            }
            if (TEMPRenderCount <= 0)
            {
                TEMPincrement = 1;
            }


            ScreenManager.GetInstance().GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ScreenManager.GetInstance().GraphicsDevice.Clear(voidColour);

            //Build depth and normal buffers
            RenderDepthNormal(activeCamera, renderItems);
            _renderTargets.Add(_normalTarget);
            _renderTargets.Add(_depthTarget);

            //Build lightmap

            TEMPlights[1].SpecularColour = new Vector3(TEMPRenderCount / 10f, 0, 0);
            TEMPlights[0].Attenuation = TEMPRenderCount;

            RenderPointLightmap(activeCamera, TEMPlights);
            _renderTargets.Add(_lightTarget);



            //Render final pass
            RenderFinalPass(activeCamera, renderItems);
            _renderTargets.Add(_outputTarget);

            if (_renderTargets.Count > 0 && _selectedRenderTarget >= 0 && _selectedRenderTarget < _renderTargets.Count)
            {
                Batch.Draw(_renderTargets[_selectedRenderTarget], new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), Color.White);
            }

        }

#region Render Passes

      
        public void RenderDepthNormal(Camera activeCamera, List<IRenderable> renderItems)
        {
            ShaderManager.GetInstance().DepthNormal.Parameters["World"].SetValue(Matrix.CreateTranslation(0, 0, 0));
            ShaderManager.GetInstance().DepthNormal.Parameters["View"].SetValue(activeCamera.CameraViewMatrix);
            ShaderManager.GetInstance().DepthNormal.Parameters["Projection"].SetValue(activeCamera.CameraProjectionMatrix);
          //  ShaderManager.GetInstance().DepthNormal.CurrentTechnique = ShaderManager.GetInstance().DepthNormal.Techniques["Technique1"];

            //Set the texture that the effect will be rendered to
            graphicsDevice.SetRenderTargets(_normalTarget, _depthTarget);
            graphicsDevice.Clear(Color.White);

            foreach (EffectPass pass in ShaderManager.GetInstance().DepthNormal.CurrentTechnique.Passes)
            {
                pass.Apply();

            }

            foreach (IRenderable renderItem in renderItems)
            {
                renderItem.Render(ScreenManager.GetInstance().GraphicsDevice);
            }

            // Un-set the render targets
            graphicsDevice.SetRenderTargets(null);
        }

        public void RenderPointLightmap(Camera activeCamera, List<PointLight> pointLights)
        {
            // Set the depth and normal map info to the effect
            ShaderManager.GetInstance().Lightmap.Parameters["DepthTexture"].SetValue(_depthTarget);
            ShaderManager.GetInstance().Lightmap.Parameters["NormalTexture"].SetValue(_normalTarget);
            // Set effect parameters to light mapping effect
            ShaderManager.GetInstance().Lightmap.Parameters["viewportWidth"].SetValue((float)viewWidth);
            ShaderManager.GetInstance().Lightmap.Parameters["viewportHeight"].SetValue((float)viewHeight);

            // Calculate the view * projection matrix
            Matrix viewProjection = activeCamera.CameraViewMatrix * activeCamera.CameraProjectionMatrix;

            // Set the inverse of the view * projection matrix to the effect
            Matrix invViewProjection = Matrix.Invert(viewProjection);
            ShaderManager.GetInstance().Lightmap.Parameters["InvViewProjection"].SetValue(invViewProjection);

            // Set the render target to the graphics device
            graphicsDevice.SetRenderTarget(_lightTarget);

            // Clear the render target to black (no light)
            graphicsDevice.Clear(Color.Black);

            // Set render states to additive (lights will add their influences)
            graphicsDevice.BlendState = BlendState.Additive;
            graphicsDevice.DepthStencilState = DepthStencilState.None;

            //Loop through all lights in the scene and render them
            foreach (PointLight pointLight in pointLights)
            {
                pointLight.SetEffectParameters(ShaderManager.GetInstance().Lightmap);

                // Calculate the world * view * projection matrix and set it to
                // the effect
                Matrix wvp = (Matrix.CreateScale(pointLight.Attenuation)
                   * Matrix.CreateTranslation(pointLight.Position)) * viewProjection;

                ShaderManager.GetInstance().Lightmap.Parameters["WorldViewProjection"].SetValue(wvp);

                // Determine the distance between the light and camera
                float dist = Vector3.Distance(activeCamera.CameraPosition,
                  pointLight.Position);
                // If the camera is inside the light-sphere, invert the cull mode
                // to draw the inside of the sphere instead of the outside
                if (dist < pointLight.Attenuation)
                    graphicsDevice.RasterizerState = RasterizerState.CullClockwise;

                // Draw the point-light-sphere
            //    sphereModel2.Meshes[0].Draw();//NOTE: Model does not render properly - problem with world matrix
                foreach (ModelMesh mesh in sphereModel.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = ShaderManager.GetInstance().Lightmap;
                    }
                    mesh.Draw();
                }

                // Revert the cull mode
                graphicsDevice.RasterizerState = RasterizerState.
              CullCounterClockwise;

            }

            // Revert the blending and depth render states
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Un-set the render target 
            graphicsDevice.SetRenderTarget(null);
        }

        public void RenderFinalPass(Camera activeCamera, List<IRenderable> renderItems)
        {
            ShaderManager.GetInstance().Ambient.Parameters["World"].SetValue(Matrix.CreateTranslation(0, 0, 0));
            ShaderManager.GetInstance().Ambient.Parameters["View"].SetValue(activeCamera.CameraViewMatrix);
            ShaderManager.GetInstance().Ambient.Parameters["Projection"].SetValue(activeCamera.CameraProjectionMatrix);
            ShaderManager.GetInstance().Ambient.Parameters["AmbientIntensity"].SetValue(TEMP_Ambient);
          //  ShaderManager.GetInstance().Ambient.CurrentTechnique = ShaderManager.GetInstance().Ambient.Techniques["Technique1"];

            //Set the texture that the effect will be rendered to
            graphicsDevice.SetRenderTargets(_outputTarget);
            graphicsDevice.Clear(Color.LightBlue);

            foreach (EffectPass pass in ShaderManager.GetInstance().Ambient.CurrentTechnique.Passes)
            {
                pass.Apply();

            }

            activeCamera.Render(ScreenManager.GetInstance().GraphicsDevice);

            foreach (IRenderable renderItem in renderItems)
            {
                renderItem.Render(ScreenManager.GetInstance().GraphicsDevice);
            }

            // Un-set the render targets
            graphicsDevice.SetRenderTargets(null);

          
        }

#endregion

        private VertexPositionColor[] SetUpVertices()
        {
            VertexPositionColor[] vertices = new VertexPositionColor[3];

            vertices[0].Position = new Vector3(-0.5f, -0.5f, 0f);
            vertices[0].Color = Color.Red;
            vertices[1].Position = new Vector3(0, 0.5f, 0f);
            vertices[1].Color = Color.Green;
            vertices[2].Position = new Vector3(0.5f, -0.5f, 0f);
            vertices[2].Color = Color.Yellow;

            return vertices;
        }

        #region Event Handlers

        protected void ToggleActiveRenderTarget()
        {
            _selectedRenderTarget++;
            if(_selectedRenderTarget>=_renderTargets.Count)
            {
                _selectedRenderTarget = 0;
            }
        }

        #endregion

    }
}
