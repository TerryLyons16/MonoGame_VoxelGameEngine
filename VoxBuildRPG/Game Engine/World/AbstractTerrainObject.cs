using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine;
using Microsoft.Xna.Framework;

namespace VoxelRPGGame.GameEngine.World
{
    /// <summary>
    /// Any Object placeable in the game world
    /// </summary>
    public abstract class AbstractTerrainObject: AbstractWorldObject
    {
      //  protected Vector3 position = new Vector3();
     //   protected Vector3 rotation = new Vector3();


        public AbstractTerrainObject()
            : base(null)
        {
        }
       // public abstract Vector3 Position
      //  public abstract Vector3 Rotation
    }
}
