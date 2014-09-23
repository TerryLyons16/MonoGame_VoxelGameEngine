//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Microsoft.Xna.Framework;

//namespace VoxelRPGGame.GameEngine.Subsystems.PhysicsSystem
//{
//    /// <summary>
//    /// Any Object that is part of the physics system
//    /// </summary>
//    public abstract class AbstractPhysicsObject
//    {
//        public delegate CollisionSquare GetWorldBounds();
//        public event GetWorldBounds OnWorldBoundsRequest;


//        public delegate AbstractEntity GetEntity();
//        public event GetEntity OnEntityRequest;

//        protected Vector3 position;
//        protected Vector3 rotation;

//        protected bool blocksMovement;

//        protected List<AbstractPhysicsObject> perceivedObjects;


//        protected CollisionSquare collisionBounds;
//        /// <summary>
//        /// The range within which an object can perceive other objects
//        /// </summary>
//        protected CollisionSquare perceptionBounds;

//        public AbstractPhysicsObject(Vector3 position, double width, double height, bool doesBlockMovement,double perceptionRadius)
//        {
//            perceivedObjects = new List<AbstractPhysicsObject>();
//            this.position = position;
//            blocksMovement = doesBlockMovement;
//            collisionBounds = new CollisionSquare(position, width, height);

//            perceptionBounds = new CollisionSquare(position, width + perceptionRadius * 2, height+perceptionRadius * 2);
            
//        }



//        public virtual void Update()
//        {
//            collisionBounds.MoveSquare(position);
//            perceptionBounds.MoveSquare(position);
//         //   perceivedObjects.Clear();

//        }

//        public virtual bool IsColliding(AbstractPhysicsObject physicsObject)
//        {
//            return collisionBounds.IsColliding(physicsObject.CollisionBounds);
//        }

//        public virtual bool IsColliding(CollisionSquare physicsSquare)
//        {
//            bool result = true;


//            //Covers all cases where two rectangles are not touching
//            if (physicsSquare.TopX <= collisionBounds.BottomX || physicsSquare.TopY <= collisionBounds.BottomY
//                || physicsSquare.BottomX >= collisionBounds.TopX || physicsSquare.BottomY >= collisionBounds.TopY)
//            {
//                result = false;
//            }

//            if(result)
//            {
//            }

//            return result;
//        }

//        public CollisionSquare RequestWorldBounds()
//        {
//            CollisionSquare result = null;

//            if (OnWorldBoundsRequest != null)
//            {
//                result = OnWorldBoundsRequest();
//            }

//            return result;
//        }

//        public void AddPerceivedObject(AbstractPhysicsObject physicsObject)
//        {
//            if (!perceivedObjects.Contains(physicsObject))
//            {
//                perceivedObjects.Add(physicsObject);
//            }
//        }


//        /// <summary>
//        /// 1: Collided from West
//        /// 2: Collided from South
//        /// 3: Collided from North
//        /// 4: Collided from East
//        /// 
//        /// </summary>
//        /// <param name="physicsSquare"></param>
//        /// <returns></returns>
//      /*  public virtual Vector3 GetCollisionDirection(CollisionSquare physicsSquare)
//        {
         

//            Vector3 collisionVector = Vector3.Zero;

//            if (physicsSquare.TopY <= collisionBounds.BottomY)
//            {
//                // 1: Collided from West
//                collisionVector = new Vector3(0, 0, 1);
//            }

//            else if(physicsSquare.TopX <= collisionBounds.BottomX)
//            {
//                // 2: Collided from South

//                collisionVector = new Vector3(1, 0, 0);
//            }
//            else if (physicsSquare.BottomX >= collisionBounds.TopX)
//            {
//                // 3: Collided from North
//                collisionVector = new Vector3(-1, 0, 0);
//            }

//            else if (physicsSquare.BottomY >= collisionBounds.TopY)
//            {
//                // 4: Collided from East
//                collisionVector = new Vector3(0, 0, -1);
//            }






//            return collisionVector;
//        }
//        */


//        /// <summary>
//        /// Provides simple perception of physical objects.
//        /// Will return true if object's collisionBounds is within current object's perceptionBounds
//        /// </summary>
//        public virtual bool CanPerceive(AbstractPhysicsObject physicalObject)
//        {
//            bool result = true;

//            //Covers all cases where two rectangles are not touching
//            if (physicalObject.PerceptionBounds.TopX < perceptionBounds.BottomX || physicalObject.PerceptionBounds.TopY < perceptionBounds.BottomY
//                || physicalObject.PerceptionBounds.BottomX > perceptionBounds.TopX || physicalObject.PerceptionBounds.BottomY > perceptionBounds.TopY)
//            {
//                result = false;
//            }

//            return result;
//        }


//        public CollisionSquare CollisionBounds
//        {
//            get
//            {
//                return collisionBounds;
//            }
//        }

//        public CollisionSquare PerceptionBounds
//        {
//            get
//            {
//                return perceptionBounds;
//            }
//        }

//        public bool BlocksMovement
//        {
//            get
//            {
//                return blocksMovement;
//            }

//            set
//            {
//                blocksMovement = value;
//            }
//        }

//        public virtual Vector3 Position
//        {
//            get
//            {
//                return position;
//            }

//            set
//            {
//                position = value;
//                collisionBounds.MoveSquare(position);
                
//            }
//        }

//        public Vector3 Rotation
//        {
//            get
//            {
//                return rotation;
//            }

//            set
//            {
//                rotation = value;
//            }
//        }


//        public List<AbstractEntity> GetPerceivedEntities()
//        {
//            List<AbstractEntity> result = new List<AbstractEntity>();
//            foreach(AbstractPhysicsObject physicsObject in perceivedObjects)
//            {
//                result.Add(physicsObject.RequestOwner());
//            }

//            return result;
//        }

//        public List<AbstractPhysicsObject> GetPerceivedPhysicsObjects()
//        {
//            return perceivedObjects;
//        }

//        public void ClearPerceivedEntities()
//        {
//            perceivedObjects.Clear();
//        }


//        public AbstractEntity RequestOwner()
//        {
//            AbstractEntity result = null;

//            if(OnEntityRequest!=null)
//            {
//                result = OnEntityRequest();
//            }

//            return result;
//        }
//    }
//}
