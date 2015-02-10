using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxBuildRPG.Game_Engine.Rendering.Lighting
{
    public class PointLight: AbsractLight
    {

        public override void SetEffectParameters(Effect effect)
        {
         /*   if (effect.Parameters["AmbientLightColor"] != null)
                effect.Parameters["AmbientLightColor"].SetValue(
                 DiffuseColour);*/

            if (effect.Parameters["LightPosition"] != null)
                effect.Parameters["LightPosition"].SetValue(Position);

            if (effect.Parameters["LightColor"] != null)
                effect.Parameters["LightColor"].SetValue(SpecularColour);

            if (effect.Parameters["LightAttenuation"] != null)
                effect.Parameters["LightAttenuation"].SetValue(
                 Attenuation);

            if (effect.Parameters["LightFalloff"] != null)
                effect.Parameters["LightFalloff"].SetValue(Falloff);
        }

        public void Render(GraphicsDevice graphicsDevice)
        {
        }
    }
}
