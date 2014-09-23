using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.World;
using Microsoft.Xna.Framework;

namespace VoxelRPGGame.GameEngine.Physics
{
    public class BlockCollisionFace:CollisionFace
    {
        private Direction _facing;

        public Direction Facing
        {
            get
            {
                return _facing;
            }
        }

        public BlockCollisionFace(Direction facing, AbstractWorldObject owningObject, Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, Vector3 faceNormal)
            :base(owningObject,point1,point2,point3,point4,faceNormal)
        {
            _facing = facing;
        }
    }
}
