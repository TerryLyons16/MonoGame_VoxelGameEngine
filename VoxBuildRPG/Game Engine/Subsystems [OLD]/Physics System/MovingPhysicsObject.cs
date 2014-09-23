//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Microsoft.Xna.Framework;

//namespace VoxelRPGGame.GameEngine.Subsystems.PhysicsSystem
//{
//    /// <summary>
//    /// Handles logic for objects that move
//    /// </summary>
//    public class MovingPhysicsObject:AbstractPhysicsObject
//    {
//        public delegate void ReachedDestination();
//        public event ReachedDestination OnDestinationReached;

//        //The previous position of the object
//        protected CollisionSquare previousCollisionBounds;
      
//        protected Vector3 velocity;
//        Vector3 previousVelocity = Vector3.Zero;
//        Vector3 preCollisionVelocity = Vector3.Zero;

//        protected float movementSpeed = 0.1f;

//        protected Vector3 destination;

//        protected Stack<Vector3> path = new Stack<Vector3>();

//        protected bool routeChanged = false;
//        protected bool canStopNearDestination = false;

////---------------------------TEMP FALISAFE--------------------------------------------------------
//        protected Vector3 preRouteRecalculationTarget=Vector3.Zero;
//        protected int defaultRecalculateRouteDelay = 120;
//        protected bool increaseRecalculateRouteDelay = false;
//        protected int timesSamePathRecalculated = 0;
////------------------------------------------------------------------------------------------------


//        protected int distanceIncreasingCounter=0;
//        protected int recalculateRouteDelay = 120;
//        protected double previousDistanceToTarget=0;


       
//        public MovingPhysicsObject(Vector3 position, double width, double height, bool doesBlockMovement,double perceptionRange)
//            : base(position, width, height, doesBlockMovement, perceptionRange)
//        {
//            previousCollisionBounds = new CollisionSquare(collisionBounds.CentreX, collisionBounds.CentreY, collisionBounds.Width, collisionBounds.Height);
//            velocity = new Vector3(0, 0, 0);
//            destination = position;
           
//        }

//        public override void Update()
//        {
            
//            //NOTE: Could add clause for objects to periodically recalculate their route 
//            if (path.Count > 0)
//            {

//                double distanceToTarget = GetDistanceTo(new Vector2(position.X, position.Z), new Vector2(path.Peek().X, path.Peek().Z));


//                //1. Keep track of distance to target and recalculate route if necessary 
//                if (distanceToTarget > previousDistanceToTarget)
//                {
//                    distanceIncreasingCounter++;
//                }


//                previousDistanceToTarget = distanceToTarget;

              
              
//                //NOTE: This may cause problems when agent is going directly to
//                //destination. i.e. not using pathfinder. Only use if not at last item on path?
//                if (distanceIncreasingCounter >= recalculateRouteDelay /*&& path.Count>1*/)
//                {
//                    if (increaseRecalculateRouteDelay)
//                    {
//                        recalculateRouteDelay *= 2;
//                        timesSamePathRecalculated++;
//                    }
//                    else
//                    {
//                        recalculateRouteDelay = defaultRecalculateRouteDelay;
//                        timesSamePathRecalculated = 0;
//                    }

//                    //Store the immediate target before route recalculation
//                        preRouteRecalculationTarget = path.Peek();
//                        increaseRecalculateRouteDelay = true;

//                        RecalculateRoute();
//                        SetVelocity(path.Peek());

//                    //If new route does not contain the previous target, ignore it
//                        if (!path.Contains(preRouteRecalculationTarget))
//                        {
//                            preRouteRecalculationTarget = Vector3.Zero;
//                            increaseRecalculateRouteDelay = false;
//                            recalculateRouteDelay = defaultRecalculateRouteDelay;
//                            timesSamePathRecalculated = 0;
//                        }
                       

//                        if (path.Count == 1)
//                        {
//                            canStopNearDestination = true;
//                        }

//                        distanceIncreasingCounter = 0;
//                        previousDistanceToTarget = 0;
                    
//                }

//                //If the same path has been recalculated more than 4 times, give up
//                if (timesSamePathRecalculated >= 4)
//                {
//                    path.Clear();
//                }
//                else
//                {
//                    //2. Check if target is inaccessible

//                    //If target is not equal to path.peek, it means that the destination is not reachable
//                     Vector3 target = ResolveInaccessibleDestination(path.Peek(), perceivedObjects);

//                    if (target != path.Peek())
//                    {
//                        Vector3 t = path.Pop();
//                        if (t == preRouteRecalculationTarget)
//                        {
//                            increaseRecalculateRouteDelay = false;
//                        }

//                        path.Push(target);

//                    }

//                    if (path.Count > 0 && !IsAtPoint(path.Peek()))
//                    {

//                        UpdateVelocity(path.Peek(), perceivedObjects);

//                        if (canStopNearDestination && IsNearPoint(path.Peek(), 1))
//                        {
//                            Vector3 t = path.Pop();
//                            if (t == preRouteRecalculationTarget)
//                            {
//                                increaseRecalculateRouteDelay = false;
//                            }
//                            canStopNearDestination = false;
//                            Stop();
//                        }

//                        MoveObject();
//                    }

//                    else
//                    {
//                        if (path.Count > 0)
//                        {
//                            Vector3 t = path.Pop();
//                            if (t == preRouteRecalculationTarget)
//                            {
//                                increaseRecalculateRouteDelay = false;
//                            }

//                        }

//                        if (path.Count > 0)
//                        {
//                            //    UpdateVelocity(path.Peek(), perceivedObjects);
//                            SetVelocity(path.Peek());
//                        }
//                        else
//                        {
//                            Stop();
//                        }
//                    }
//                }

//                if (path.Count == 0)
//                {
//                    OnDestinationReached();
//                }
//            }
//            else
//            {
//                Stop();
//            }

//          //      MoveObject();

//            //1. Sets the previous square to the current
//                SetPreviousCollisionBounds();
//            //2. Moves the position of the collision square
//            base.Update();


//        }

//        /// <summary>
//        /// Called whenever the object is moved
//        /// </summary>
//        protected void SetPreviousCollisionBounds()
//        {
//            previousCollisionBounds.MoveSquare(new Vector3(((float)collisionBounds.CentreY),0,((float)collisionBounds.CentreX)));
            
//        }


//#region Movement

//        /// <summary>
//        /// Checks to see if the current velocity is acceptible (i.e. collisions or on same axis as target)
//        /// if not, resolves it
//        /// </summary>
//        /// <param name="target"></param>
//        /// <param name="nearbyObjects"></param>
//        public void UpdateVelocity(Vector3 target, List<AbstractPhysicsObject> nearbyObjects)
//        {
//          /*   float differenceX = target.X - position.X;
//             float differenceY = target.Z - position.Z;
//            //Check if position is on primary axis towards target
//             if (Math.Abs(differenceX) < movementSpeed || Math.Abs(differenceY) < movementSpeed)
//             {
//                 //Check if the current velocity lies on the primary axis between the two points
//                 Vector3 targetDirection = new Vector3(target.X - position.X, 0, target.Z - position.Z);
//                 targetDirection.Normalize();
//                 Vector3 normalisedVelocity = new Vector3(velocity.X, 0, velocity.Z);
//                 normalisedVelocity.Normalize();

//                 if (targetDirection != normalisedVelocity)
//                 {
//                     //If not, recalculate the velocity
//                     SetVelocity(target);
//                 }
//             }*/


//            //NOTE: May disable this check as it prevents agents from recalculating their
//            //velocity if they change route while moving and colliding
//            if (GetCollisionVectors(nearbyObjects).Count == 0 || velocity == Vector3.Zero/* || routeChanged*/)
//             {
//                 //NOTE: May break collisions
//                 SetVelocity(target);
//                 routeChanged = false;
//             }

            
//            //Collision Detection and resolution

//             //Check for collision with the desired precollision velocity
//             if (preCollisionVelocity != Vector3.Zero)
//             {
                 
//                 if (!TestForCollision(preCollisionVelocity,nearbyObjects))
//                 {
//                     velocity = preCollisionVelocity;
//                     preCollisionVelocity = Vector3.Zero;
//                 }
//             }
         
//                 //Check for collision with the current velocity
//             if (TestForCollision(velocity, nearbyObjects))
//             {
//                 //Resolve Collision
//                ResolveCollision(nearbyObjects);
//             }
             


//        }


//        /// <summary>
//        /// Sets the desired velocity towards the specified position 
//        /// </summary>
//        protected void SetVelocity(Vector3 target)
//        {
//            float differenceX = target.X - position.X;
//            float differenceY = target.Z - position.Z;


//            velocity = new Vector3(0, 0, 0);

//            if (differenceX != 0)
//            {
//                double moveSpeed = 0;

//                if (Math.Abs(differenceX) < movementSpeed)
//                {
//                    moveSpeed = Math.Abs(differenceX);
//                }

//                else
//                {
//                    moveSpeed = movementSpeed;
//                }

//                velocity.X += (float)moveSpeed * (differenceX / Math.Abs(differenceX));
//            }

//            if (differenceY != 0)
//            {
//                double moveSpeed = 0;

//                if (Math.Abs(differenceY) < movementSpeed)
//                {
//                    moveSpeed = Math.Abs(differenceY);
//                }

//                else
//                {
//                    moveSpeed = movementSpeed;
//                }

//                velocity.Z += (float)moveSpeed * (differenceY / Math.Abs(differenceY));
//            }


//            preCollisionVelocity = Vector3.Zero;
//        }


//        protected void MoveObject()
//        {
//            position += velocity;
//            //Prevent floating point errors
//            position.X = (float)Math.Round(position.X, 3);
//            position.Z = (float)Math.Round(position.Z, 3);
//            previousVelocity = new Vector3(velocity.X, 0, velocity.Z);
//         //   Stop();
//        }

//        protected void Stop()
//        {
//            velocity = Vector3.Zero;
//        //    previousVelocity = Vector3.Zero;
//         //   preCollisionVelocity = Vector3.Zero;
//        }

//        public void StopMoving()
//        {
//            Stop();
//            path.Clear();
//        }


//#endregion

//#region Collisions

//        /// <summary>
//        /// Tests to see if the desired velocity causes a collision
//        /// </summary>
//        /// <param name="desiredVelocity"></param>
//        /// <param name="nearbyObjects"></param>
//        /// <returns></returns>
//        public bool TestForCollision(Vector3 desiredVelocity, List<AbstractPhysicsObject> nearbyObjects)
//        {
//            bool result = false;

//            CollisionSquare lookAheadPosition = new CollisionSquare(position + desiredVelocity, collisionBounds.Width, collisionBounds.Height);
//            //Test to see if object is currently stuck inside something
//            /*   bool isStuck=false;
//               //Disables ALL collisions - can cause problems
//               foreach (AbstractPhysicsObject nearbyObject in nearbyObjects)
//               {
//                   if (nearbyObject.BlocksMovement&&nearbyObject.IsColliding(collisionBounds))
//                   {
//                       isStuck = true;
//                   }
//               }

//               if (!isStuck)
//               {*/
//            foreach (AbstractPhysicsObject nearbyObject in nearbyObjects)
//                {
//                    if (nearbyObject.BlocksMovement && nearbyObject.IsColliding(lookAheadPosition) && !nearbyObject.IsColliding(collisionBounds))
//                    {
//                        result = true;
//                        break;

//                    }

//                }
//          //  }

//            //Collides with the edge of the world
//            if (!RequestWorldBounds().IsInside(lookAheadPosition))
//            {
//                result = true;
//            }

//            if (result == false)
//            {
//            }

//            return result;
//        }

//        /// <summary>
//        /// If the desired velocity causes a collision, get the directions in which collisions will occur
//        /// </summary>
//        /// <param name="nearbyObjects"></param>
//        /// <returns></returns>
//        public List<Vector3> GetCollisionVectors(List<AbstractPhysicsObject> nearbyObjects)
//        {
//            List<Vector3> result = new List<Vector3>();

//            Queue<Vector3> possibleVelocities = new Queue<Vector3>();

//            possibleVelocities.Enqueue(new Vector3(movementSpeed, 0, 0));
//            possibleVelocities.Enqueue(new Vector3(0, 0, movementSpeed));
//            possibleVelocities.Enqueue(new Vector3(-movementSpeed, 0, 0));
//            possibleVelocities.Enqueue(new Vector3(0, 0, -movementSpeed));
//            possibleVelocities.Enqueue(new Vector3(movementSpeed, 0, movementSpeed));
//            possibleVelocities.Enqueue(new Vector3(-movementSpeed, 0, -movementSpeed));
//            possibleVelocities.Enqueue(new Vector3(-movementSpeed, 0, movementSpeed));
//            possibleVelocities.Enqueue(new Vector3(movementSpeed, 0, -movementSpeed));


//            while (possibleVelocities.Count > 0)
//            {
//                Vector3 vel = possibleVelocities.Dequeue();
//                CollisionSquare lookAheadPosition = new CollisionSquare(position + vel, collisionBounds.Width, collisionBounds.Height);

//                if (!RequestWorldBounds().IsInside(lookAheadPosition))
//                {
//                    if (!result.Contains(vel))
//                    {
//                        result.Add(vel);
//                    }
//                }

//                foreach (AbstractPhysicsObject nearbyObject in nearbyObjects)
//                {
//                    //If they are currently colliding, they will be allowed to move to resolve collision
//                    if ((nearbyObject.BlocksMovement && nearbyObject.IsColliding(lookAheadPosition) && !nearbyObject.IsColliding(collisionBounds)))
//                    {
//                        if (!result.Contains(vel))
//                        {
//                            result.Add(vel);
//                        }

//                    }

//                }


//            }

//            return result;

//        }
//        protected void ResolveCollision(List<AbstractPhysicsObject> nearbyObjects)
//        {
//          /*  if (IsDestinationAccessible(path.Peek(), nearbyObjects))
//            {*/

//                List<Vector3> collisionVectors = GetCollisionVectors(nearbyObjects);

//                if (collisionVectors.Count > 0)
//                {
//                    if (path.Count > 0)
//                    {
//                        Vector3 target = new Vector3(path.Peek().X, 0, path.Peek().Z);

//                        //Get the vector towards the current path target
//                        Vector3 direction = new Vector3(target.X - position.X, 0, target.Z - position.Z);

//                        //Get Possible solution velocities

//                        List<Vector2> resolvedVelocities = new List<Vector2>();

//                        //if the velocity on one axis =0, then the object is moving on a prime axis => rotate 90 degrees
//                        if ((velocity.X == 0 && velocity.Z != 0) || (velocity.X != 0 && velocity.Z == 0))
//                        {
//                            //    Vector2 resolvedVelocityA = new Vector2(velocity.X, velocity.Z);
//                            //    Vector2 resolvedVelocityB = new Vector2(velocity.X, velocity.Z);
//                            Vector2 resolvedVelocityA = Vector2.Transform(new Vector2(velocity.X, velocity.Z), Matrix.CreateRotationZ(MathHelper.ToRadians(90)));
//                            Vector2 resolvedVelocityB = Vector2.Transform(new Vector2(velocity.X, velocity.Z), Matrix.CreateRotationZ(MathHelper.ToRadians(-90)));

//                            resolvedVelocities.Add(resolvedVelocityA);
//                            resolvedVelocities.Add(resolvedVelocityB);

//                        }

//                        //If neither vector component =0 => is on 45 degree axis, so rotate by 45 and 135 degrees
//                        else
//                        {
//                            Vector2 resolvedVelocityA = new Vector2(movementSpeed, movementSpeed);
//                            Vector2 resolvedVelocityB = new Vector2(movementSpeed, movementSpeed);
//                            Vector2 resolvedVelocityC = new Vector2(movementSpeed, movementSpeed);
//                            Vector2 resolvedVelocityD = new Vector2(movementSpeed, movementSpeed);

//                            resolvedVelocityA = Vector2.Transform(resolvedVelocityA, Matrix.CreateRotationZ(MathHelper.ToRadians(45)));
//                            resolvedVelocityB = Vector2.Transform(resolvedVelocityB, Matrix.CreateRotationZ(MathHelper.ToRadians(135)));
//                            resolvedVelocityC = Vector2.Transform(resolvedVelocityC, Matrix.CreateRotationZ(MathHelper.ToRadians(-45)));
//                            resolvedVelocityD = Vector2.Transform(resolvedVelocityD, Matrix.CreateRotationZ(MathHelper.ToRadians(-135)));

//                            resolvedVelocities.Add(resolvedVelocityA);
//                            resolvedVelocities.Add(resolvedVelocityB);
//                            resolvedVelocities.Add(resolvedVelocityC);
//                            resolvedVelocities.Add(resolvedVelocityD);
//                        }


//                        //Round vectors to 4 decimal places
//                        for (int i = 0; i < resolvedVelocities.Count; i++)
//                        {
//                            resolvedVelocities[i] = new Vector2((float)Math.Round(resolvedVelocities[i].X, 4), (float)Math.Round(resolvedVelocities[i].Y, 4));
//                        }

//                        //Never go back in the same direction object came from
//                        Vector2 previousVelocityInverse = new Vector2(-previousVelocity.X, -previousVelocity.Z);

//                        previousVelocityInverse.Normalize();


//                        //Remove the velocity along the axis of collision
//                        List<Vector2> velocityToRemove = new List<Vector2>();

//                        foreach (Vector2 vel in resolvedVelocities)
//                        {
//                            Vector2 v = new Vector2(vel.X, vel.Y);

//                            v.Normalize();
//                            if (v.X == previousVelocityInverse.X && v.Y == previousVelocityInverse.Y)
//                            {
//                                velocityToRemove.Add(vel);
//                            }

//                            foreach (Vector3 collisionVector in collisionVectors)
//                            {
//                                collisionVector.Normalize();

//                                if (v.X == collisionVector.X && v.Y == collisionVector.Z)
//                                {
//                                    velocityToRemove.Add(vel);
//                                }
//                            }

//                        }


//                        foreach (Vector2 v in velocityToRemove)
//                        {
//                            resolvedVelocities.Remove(v);
//                        }



//                        Vector2 bestResolved = Vector2.Zero;
//                        double smallestAngle = double.MaxValue;

//                        foreach (Vector2 vel in resolvedVelocities)
//                        {
//                            Vector3 threeDvector = new Vector3(vel.X, 0, vel.Y);
//                            //Get the angle between direction and each of the resolved velocities

//                            double Vector1Magnitude = Math.Sqrt(Math.Pow(threeDvector.X, 2) + Math.Pow(threeDvector.Z, 2));
//                            double Vector2Magnitude = Math.Sqrt(Math.Pow(direction.X, 2) + Math.Pow(direction.Z, 2));
//                            double vectorsDotProduct = (threeDvector.X * direction.X) + (threeDvector.Z * direction.Z);

//                            double cosTheta = vectorsDotProduct / (Vector1Magnitude * Vector2Magnitude);
//                            double angle = MathHelper.ToDegrees((float)Math.Acos(cosTheta));



//                            if (angle < smallestAngle)
//                            {
//                                bestResolved = vel;
//                                smallestAngle = angle;
//                            }
//                        }
//                        //Take the velocity with the smallest angle
//                        preCollisionVelocity = velocity;


//                        velocity = Vector3.Zero;
//                        velocity.X = (float)Math.Round(bestResolved.X, 4);
//                        velocity.Z = (float)Math.Round(bestResolved.Y, 4);


//                    }



//                    else
//                    {
//                        velocity = Vector3.Zero;
//                    }



//                }
//         /*   }

//            else
//            {
//                //Remove target from path because it is not accessible
//                path.Pop();
//                Stop();
//            }*/
            
//        }



//#endregion


//        #region Pathfinding
//        public void SelectDestination(Vector3 location)
//        {
//            //     destination = location;
//            if (!IsAtPoint(location))
//            {
//                PlanRoute(location);
//            }
//            else
//            {
//                OnDestinationReached();
//            }
//           /* if (path.Count > 0)
//            {
 
//                SetVelocity(path.Peek());
//            }*/
//        }


//        public void PlanRoute(Vector3 location)
//        {
//            Stop();
//            path.Clear();
//            path = Pathfinder.GetInstance().Pathfind(position, location);
//            routeChanged = true;

//            destination = location;
//           // Stop();
//        }


//        public void RecalculateRoute()
//        {
//            if (path.Count > 0)
//            {
//                Vector3 target;

//                while (path.Count > 1)
//                {
//                    path.Pop();
//                }
//                //Get the last position on the path
//                target = path.Pop();

//                path = Pathfinder.GetInstance().Pathfind(position, target);
//                routeChanged = true;

//            }


//        }

//        /// <summary>
//        /// Checks if the object's position is equal to the current position
//        /// </summary>
//        /// <param name="point"></param>
//        /// <returns></returns>
//        protected bool IsAtPoint(Vector3 point)
//        {
//            bool result = false;

//            if (Math.Round(point.X, 3) == Math.Round(position.X, 3) && Math.Round(point.Z, 3) == Math.Round(position.Z, 3))
//            {
//                result = true;
//            }

//            return result;
//        }

//        protected bool IsNearPoint(Vector3 point, double threshold)
//        {
//            bool result = false;

//            CollisionSquare s = new CollisionSquare(point, threshold, threshold);

//            if (s.IsColliding(collisionBounds)||s.IsInside(new Vector2(position.X,position.Z)))
//            {
//                result = true;
//            }

         
//            return result;
//        }

//        protected bool IsTargetAccessible(Vector3 target, List<AbstractPhysicsObject> nearbyObjects)
//        {
//            bool result = true;


//                CollisionSquare world = new CollisionSquare(RequestWorldBounds().CentreX, RequestWorldBounds().CentreY, RequestWorldBounds().Width -( collisionBounds.Width*2), RequestWorldBounds().Height -( collisionBounds.Height*2));

//                if (!world.IsInside(new Vector2(target.X, target.Z)))
//                {
//                    result = false;
//                }

//                else
//                {

//                    foreach (AbstractPhysicsObject nearbyObject in nearbyObjects)
//                    {
//                        //Square that is the height and width of both objects combined.
//                        //I.e. if point is within this square, object would have to collide with other to reach it
//                        CollisionSquare b = new CollisionSquare(nearbyObject.Position, nearbyObject.CollisionBounds.Width + collisionBounds.Width, nearbyObject.CollisionBounds.Height + collisionBounds.Height);

//                        //If they are currently colliding, they will be allowed to move to resolve collision
//                        if ((nearbyObject is StaticPhysicsObject) &&nearbyObject.BlocksMovement && b.IsInside(new Vector2(target.X, target.Z)))
//                        {
//                            result = false;
//                            break;
//                        }
//                    }
//                }
//            return result;
//        }

//        /// <summary>
//        /// If the destination is not accessible,
//        /// get the nearest point that is
//        /// </summary>
//        /// <param name="target"></param>
//        /// <param name="nearbyObjects"></param>
//        /// <returns></returns>
//        protected Vector3 ResolveInaccessibleDestination(Vector3 target, List<AbstractPhysicsObject> nearbyObjects)
//        {
//            //default result is current target
//            Vector3 result = target;


//            CollisionSquare world = new CollisionSquare(RequestWorldBounds().CentreX, RequestWorldBounds().CentreY, RequestWorldBounds().Width - (collisionBounds.Width * 2), RequestWorldBounds().Height - (collisionBounds.Height * 2));

//            if (!world.IsInside(new Vector2(target.X, target.Z)))
//            {
                
//                   double[] point = world.LineIntersectsPoint(new Vector2(position.X, position.Z),new Vector2(target.X, target.Z));
//                   if (point != null)
//                   {
//                       result = new Vector3((float)point[0], 0, (float)point[1]);
//                   }
                    
//            }

//            else
//            {

//                foreach (AbstractPhysicsObject nearbyObject in nearbyObjects)
//                {
//                    //Square that is the height and width of both objects combined.
//                    //I.e. if point is within this square, object would have to collide with other to reach it
//                    CollisionSquare box = new CollisionSquare(nearbyObject.Position, nearbyObject.CollisionBounds.Width + collisionBounds.Width, nearbyObject.CollisionBounds.Height + collisionBounds.Height);

//                    //If they are currently colliding, they will be allowed to move to resolve collision
//                    if (/*(nearbyObject is StaticPhysicsObject) &&*/ nearbyObject.BlocksMovement && box.IsInside(new Vector2(target.X, target.Z)))
//                    {

//                        double[] point = box.LineIntersectsPoint(new Vector2(position.X, position.Z), new Vector2(target.X, target.Z));
//                        if (point != null)
//                        {
//                            result = new Vector3((float)point[0], 0, (float)point[1]);
//                        }
                        
//                        break;
//                    }
//                }
//            }
//            return result;
//        }

//        protected double GetDistanceTo(Vector2 startPoint, Vector2 endPoint)
//        {

//            double result = 0;

//            result = Math.Sqrt(Math.Pow((endPoint.X - startPoint.X), 2) + Math.Pow((endPoint.Y - startPoint.Y), 2));


//            return result;
//        }

//#endregion

//        #region Properties

//        public CollisionSquare PreviousCollisionBounds
//        {
//            get
//            {
//                return previousCollisionBounds;
//            }
//        }

//        public override Vector3 Position
//        {
//            get
//            {
//                return position;
//            }

//            set
//            {
//                position = value;
//                destination=position;
//                path.Clear();
//                Stop();

//                collisionBounds.MoveSquare(position);
//                //Reset previous collision bounds
//                SetPreviousCollisionBounds();

          
               
//            }
//        }

//        public Vector3 Destination
//        {
//            get
//            {
//                return destination;
//            }
//        }

//        public bool IsMovingToDestination()
//        {
//            bool result = false;

//            if(path.Count>0)
//            {
//                result = true;
//            }

//            return result;
//        }

//        public List<Vector3> Path
//        {
//            get
//            {
//                return path.ToList<Vector3>();
//            }
//        }

//#endregion
//    }
//}
