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
        private Effect DepthNormal;

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
        //    DepthNormal = ScreenManager.GetInstance().ContentManager.Load<Effect>("Effets/DepthNormal");
         /*   BinaryReader Reader = new BinaryReader(File.Open(@"Content\\Effects\\output.mgfx", FileMode.Open));
            DepthNormal = new Effect(ScreenManager.GetInstance().GraphicsDevice, Reader.ReadBytes((int)Reader.BaseStream.Length)); */
        }


        public BasicEffect DefaultEffect
        {
            get
            {
                return _defaultEffect;
            }
        }
    }
}
