using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VoxelRPGGame.MenuSystem;

namespace VoxelRPGGame.GameEngine.Rendering
{
    public class ShaderManager
    {
        private BasicEffect _defaultEffect;
        private Effect _ambient;
        private Effect _depthNormal;
        private Effect _lightmap;

        private static ShaderManager _shaderManager = null;
           

        private ShaderManager(GraphicsDevice device)
        {
            //Always initialise the default effect
            _defaultEffect = new BasicEffect(device);
        }

        public static ShaderManager CreateShaderManager(GraphicsDevice currentGraphicsDevice)
        {
            if(_shaderManager==null)
            {
                _shaderManager = new ShaderManager(currentGraphicsDevice);
            }
            return _shaderManager;
        }

        public static ShaderManager GetInstance()
        {
            ShaderManager result = null;
            if (_shaderManager != null)
            {
                result = _shaderManager;
            }

            return result;
        }

        //Load shaders into memory
        public void LoadShaders()
        {
            _ambient = ScreenManager.GetInstance().ContentManager.Load<Effect>(@"Effects/AmbientLight");
            _depthNormal = ScreenManager.GetInstance().ContentManager.Load<Effect>(@"Effects/DepthNormal");
            _lightmap = ScreenManager.GetInstance().ContentManager.Load<Effect>(@"Effects/Lightmap");
        }


        public BasicEffect DefaultEffect
        {
            get
            {
                return _defaultEffect;
            }
        }

        public Effect Ambient
        {
            get
            {
                return _ambient;
            }
        }

        public Effect DepthNormal
        {
            get
            {
                return _depthNormal;
            }

        }

        public Effect Lightmap
        {
            get
            {
                return _lightmap;
            }

        }

    }
}
