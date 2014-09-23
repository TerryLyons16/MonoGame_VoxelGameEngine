//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Microsoft.Xna.Framework;

//namespace VoxelRPGGame.GameEngine.Subsystems.PhysicsSystem
//{
//    /// <summary>
//    /// Any physics object that does not move i.e. a building
//    /// </summary>
//    public class StaticPhysicsObject:AbstractPhysicsObject
//    {

//        public StaticPhysicsObject(Vector3 position, double width, double height, bool doesBlockMovement,double perceptionRange)
//            : base(position, width, height, doesBlockMovement, perceptionRange)
//        {
//        }

//        /// <summary>
//        /// Checks if the input physics object is fully within the current physics object
//        /// (for checking occupants of buildings etc.
//        /// </summary>
//        /// <param name="physicsObject"></param>
//        /// <returns></returns>
//        public bool IsInside(AbstractPhysicsObject physicsObject)
//        {
//            return IsInside(physicsObject.CollisionBounds);
//        }

//        protected bool IsInside(CollisionSquare square)
//        {
//            return collisionBounds.IsInside(square);
//        }

        

//        /// <summary>
//        /// Checks if the movingPhysicsObject has fully entered the static physics object
//        /// </summary>
//        /// <param name="physicsObject"></param>
//        /// <returns></returns>
//        public bool HasEntered(MovingPhysicsObject physicsObject)
//        {
//            bool result = false;

//            if (!IsInside(physicsObject.PreviousCollisionBounds) && IsInside(physicsObject.CollisionBounds))
//            {
//                result = true;
//            }
//                return result;
//        }

//        /// <summary>
//        /// Checks if the movingPhysicsObject has exited the static physics object
//        /// </summary>
//        /// <param name="physicsObject"></param>
//        /// <returns></returns>
//        public bool HasExited(MovingPhysicsObject physicsObject)
//        {
//            bool result = false;

//            if (IsInside(physicsObject.PreviousCollisionBounds) && !IsInside(physicsObject.CollisionBounds))
//            {
//                result = true;
//            }
//            return result;
//        }
//    }
//}
