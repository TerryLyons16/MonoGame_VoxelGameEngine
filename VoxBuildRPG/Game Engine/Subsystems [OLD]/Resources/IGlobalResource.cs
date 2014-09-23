using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VoxelRPGGame.GameEngine.UI;

namespace VoxelRPGGame.GameEngine.Subsystems.Resources
{
    public interface IGlobalResource
    {

        List<AbstractResource> LocalResources
        {
            get;
        }

        AbstractWorldObject PhysicalEntity
        {
            get;
        }
    }
}
