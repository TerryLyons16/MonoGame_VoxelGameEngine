using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxBuildRPG.GameEngine.Rendering
{
    public interface IRenderable
    {
         void Render(GraphicsDevice graphicsDevice);
    }
}
