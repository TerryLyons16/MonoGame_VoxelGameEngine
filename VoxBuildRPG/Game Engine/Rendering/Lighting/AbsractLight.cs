using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxBuildRPG.GameEngine.Rendering;

namespace VoxBuildRPG.Game_Engine.Rendering.Lighting
{
    public abstract class AbsractLight : IRenderable
    {
       
        public Vector3 Position { get; set; }
        public Vector3 DiffuseColour { get; set; }
        public Vector3 SpecularColour { get; set; }
        public float Attenuation { get; set; }
        public float Falloff { get; set; }


        public abstract void SetEffectParameters(Effect effect);


        public void Render(GraphicsDevice graphicsDevice)
        {

        }
    }
}
