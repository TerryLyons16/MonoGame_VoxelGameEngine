//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Microsoft.Xna.Framework;

//namespace VoxelRPGGame.GameEngine.Subsystems.PhysicsSystem
//{
//    public class BuildingPhysicsObject: StaticPhysicsObject
//    {
//        protected CollisionSquare entranceBounds;

//        protected int entranceSide = -1;

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="position"></param>
//        /// <param name="entranceSide">0: North, 1:East,2:South, 3:West</param>
//        /// <param name="entranceWidth"></param>
//        /// <param name="entranceHeight"></param>
//        /// <param name="width"></param>
//        /// <param name="height"></param>
//        /// <param name="doesBlockMovement"></param>
//        /// <param name="perceptionRange"></param>
//        public BuildingPhysicsObject(Vector3 position, int entranceSide, double entranceWidth, double entranceHeight, double width, double height, bool doesBlockMovement, double perceptionRange) :
//            base(position, width, height, doesBlockMovement, perceptionRange)
//        {
//            entranceBounds = new CollisionSquare(position, entranceWidth, entranceHeight);
//            SetEntranceLoaction(entranceSide);
//        }

//        public void SetEntranceLoaction(int entranceSide)
//        {
//            this.entranceSide = entranceSide;
//            int xSign = 0, zSign = 0;

//            if (entranceSide == 0)
//            {
//                xSign = 1;
//            }

//            else if (entranceSide == 1)
//            {
//                zSign = 1;
//            }

//            else if (entranceSide == 2)
//            {
//                xSign = -1;
//            }

//            else if (entranceSide == 3)
//            {
//                zSign = -1;
//            }


//            Vector3 location = new Vector3((float)(collisionBounds.CentreY + (xSign * ((collisionBounds.Height / 2) + (Entrance.Height / 2)))), 0, (float)(collisionBounds.CentreX + (zSign * ((collisionBounds.Width / 2) + (Entrance.Width / 2)))));
//            Entrance.MoveSquare(location);
           
//        }

//        public void RotateEntrance()
//        {
//            entranceSide = (entranceSide+1)%4;
//            SetEntranceLoaction(entranceSide);
//        }

//        /// <summary>
//        /// Gets the unit vector from the centre of the building to the centre of the entrance
//        /// </summary>
//        /// <returns></returns>
//        public Vector3 GetEntranceVector()
//        {
//            Vector3 result = new Vector3((float)(entranceBounds.CentreY - collisionBounds.CentreY), 0,(float)( entranceBounds.CentreX - collisionBounds.CentreX));
//            result.Normalize();

//            return result;
//        }


//        /// <summary>
//        /// Checks if the input square is colliding with the entrance
//        /// </summary>
//        /// <param name="physicsSquare"></param>
//        /// <returns></returns>
//        public virtual bool IsEntranceColliding(CollisionSquare physicsSquare)
//        {
//            return IsColliding(entranceBounds, physicsSquare);
//        }


//        public virtual bool IsColliding(CollisionSquare currentSquare, CollisionSquare physicsSquare)
//        {
//            bool result = true;


//            //Covers all cases where two rectangles are not touching
//            if (physicsSquare.TopX <= currentSquare.BottomX || physicsSquare.TopY <= currentSquare.BottomY
//                || physicsSquare.BottomX >= currentSquare.TopX || physicsSquare.BottomY >= currentSquare.TopY)
//            {
//                result = false;
//            }

    

//            return result;
//        }

//        /// <summary>
//        /// Checks if the movingPhysicsObject has entered the area of the building's entrance
//        /// </summary>
//        /// <param name="physicsObject"></param>
//        /// <returns></returns>
//        public bool EnteredEntranceArea(MovingPhysicsObject physicsObject)
//        {
//            bool result = false;

//            if (!IsColliding(entranceBounds, physicsObject.PreviousCollisionBounds) && IsColliding(entranceBounds, physicsObject.CollisionBounds))
//            {
//                result = true;
//            }
//            return result;
//        }

//        /// <summary>
//        /// Checks if the movingPhysicsObject is in the building's entrance
//        /// </summary>
//        /// <param name="physicsObject"></param>
//        /// <returns></returns>
//        public bool IsAtEntrance(MovingPhysicsObject physicsObject)
//        {
//            bool result = false;

//            if (IsColliding(entranceBounds, physicsObject.PreviousCollisionBounds) && IsColliding(entranceBounds, physicsObject.CollisionBounds))
//            {
//                result = true;
//            }
//            return result;
//        }


//        /// <summary>
//        /// Checks if the movingPhysicsObject has exited the building
//        /// </summary>
//        /// <param name="physicsObject"></param>
//        /// <returns></returns>
//        public bool AtExit(MovingPhysicsObject physicsObject)
//        {
//            bool result = false;

//            if (IsInside(physicsObject.PreviousCollisionBounds) && IsColliding(entranceBounds, physicsObject.CollisionBounds))
//            {
//                result = true;
//            }
//            return result;
//        }
//        /// <summary>
//        /// Checks if the movingPhysicsObject has left the entrance area
//        /// </summary>
//        /// <param name="physicsObject"></param>
//        /// <returns></returns>
//        public bool HasExitedEntrance(MovingPhysicsObject physicsObject)
//        {
//            bool result = false;

//            if (IsColliding(entranceBounds,physicsObject.PreviousCollisionBounds) && !IsColliding(entranceBounds, physicsObject.CollisionBounds))
//            {
//                result = true;
//            }
//            return result;
//        }


//        public CollisionSquare Entrance
//        {
//            get
//            {
//                return entranceBounds;
//            }
//        }

//        public Vector3 EntranceLocation
//        {
//            get
//            {
//                return new Vector3((float)entranceBounds.CentreY, 0, (float)entranceBounds.CentreX);
//            }
//        }
//    }
//}
