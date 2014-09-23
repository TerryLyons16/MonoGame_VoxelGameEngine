using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelRPGGame.GameEngine.World
{
    public enum Direction
    {
        /// <summary>
        /// Default null value
        /// </summary>
        NULL,
        /// <summary>
        /// +x
        /// </summary>
        North,
        /// <summary>
        /// -x
        /// </summary>
        South,
        /// <summary>
        /// +z
        /// </summary>
        East,
        /// <summary>
        /// -z
        /// </summary>
        West,
        /// <summary>
        /// +y
        /// </summary>
        Up,
        /// <summary>
        /// -y
        /// </summary>
        Down

    }
}
