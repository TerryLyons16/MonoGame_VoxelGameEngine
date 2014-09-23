using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VoxelRPGGame.GameEngine.Subsystems.Resources;



namespace VoxelRPGGame.GameEngine.Subsystems.ResourceFactories
{
    /// <summary>
    /// Resource rules take resources to produce or consume resources
    /// </summary>
    public abstract class AbstractResourceRule
    {
        /// <summary>
        /// Performs the calculation to create/consume Resource, using other resources and agents
        /// </summary>
        public abstract void Execute();
    }
}
