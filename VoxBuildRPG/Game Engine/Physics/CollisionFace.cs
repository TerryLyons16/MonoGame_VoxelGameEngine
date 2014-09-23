using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace VoxelRPGGame.GameEngine.Physics
{
    public class CollisionFace
    {
        AbstractWorldObject owner;
        Vector3 _centre;
        Vector3 _normal;

        List<Vector3> vertices;
        public CollisionFace(AbstractWorldObject owningObject,Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, Vector3 faceNormal):
            this(point1,point2,point3,point4,faceNormal)
        {
            owner = owningObject;
        }

        public CollisionFace(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, Vector3 faceNormal)
        {
            _normal = faceNormal;

            vertices = new List<Vector3>();
            vertices.Add(point1);
            vertices.Add(point2);
            vertices.Add(point3);
            vertices.Add(point4);

            //L1 = P1 + a V1
            //L2 = P2 + b V2
            Vector3 v1 = new Vector3(point3.X - point1.X, point3.Y - point1.Y, point3.Z - point1.Z);
            Vector3 v2 = new Vector3(point4.X - point2.X, point4.Y - point2.Y, point4.Z - point2.Z);

            

            // a (V1 X V2) = (P2 - P1) X V2
            Vector3 alpa = Vector3.Cross(v1, v2);
            Vector3 beta = point2 - point1;
            beta = Vector3.Cross(beta, v2);

            Vector3 result = beta/alpa;
            //P1 + aV1==Pintersection
            List<float> aVals = new List<float>();
            if (!float.IsNaN(result.X) && !float.IsInfinity(result.X))
            {
                aVals.Add(result.X);
            }

            if (!float.IsNaN(result.Y) && !float.IsInfinity(result.Y))
            {
              aVals.Add(result.Y);
            }

            if (!float.IsNaN(result.Z) && !float.IsInfinity(result.Z))
            {
                aVals.Add(result.Z);
            }

            bool allowA = true;

            if (aVals.Count > 1)
            {
                for (int i = 1; i < aVals.Count; i++)
                {
                    if (aVals[i - 1] != aVals[i])
                    {
                        allowA = false;
                        break;
                    }
                }

            }

            if (allowA && aVals.Count > 0)
            {
                float a;
                a = aVals[0];
                _centre = point1 + a * v1;
            }

       
        }

        /// <summary>
        /// Axis aligned bounding box collision resolution
        /// </summary>
        public Vector3 ResolveCollisionAABB(BoundingBox lookAheadBoundingBox, Vector3 desiredVelocity)
        {
            Vector3 result = Vector3.Zero;

            try
            {
                //Check angle between desired velocity and _normal. If >90 degrees, don't perform collision
                //detection, as boundingbox is on back side of face
                if (_normal != Vector3.Zero)
                {
                    _normal.Normalize();
                }

                Vector3 desiredVelVector = new Vector3(desiredVelocity.X, desiredVelocity.Y, desiredVelocity.Z);

                if (desiredVelVector != Vector3.Zero)
                {
                    desiredVelVector.Normalize();
                }

                Vector3 p1 = new Vector3(lookAheadBoundingBox.Min.X, lookAheadBoundingBox.Min.Y, lookAheadBoundingBox.Min.Z);
                Vector3 p2 = new Vector3(lookAheadBoundingBox.Min.X, lookAheadBoundingBox.Min.Y, lookAheadBoundingBox.Max.Z);
                Vector3 p3 = new Vector3(lookAheadBoundingBox.Max.X, lookAheadBoundingBox.Min.Y, lookAheadBoundingBox.Min.Z);
                Vector3 p4 = new Vector3(lookAheadBoundingBox.Max.X, lookAheadBoundingBox.Min.Y, lookAheadBoundingBox.Min.Z);
                Vector3 p5 = new Vector3(lookAheadBoundingBox.Min.X + (lookAheadBoundingBox.Max.X - lookAheadBoundingBox.Min.X) / 2, lookAheadBoundingBox.Min.Y, lookAheadBoundingBox.Min.Z + (lookAheadBoundingBox.Max.Z - lookAheadBoundingBox.Min.Z) / 2);

                List<Ray> rays = new List<Ray>();
                rays.Add(new Ray(p1, desiredVelVector));
                rays.Add(new Ray(p2, desiredVelVector));
                rays.Add(new Ray(p3, desiredVelVector));
                rays.Add(new Ray(p4, desiredVelVector));
                rays.Add(new Ray(p5, desiredVelVector));

                float dot = Vector3.Dot(_normal, desiredVelVector);

                double angle = Math.Acos(dot) * (180 / Math.PI);

                //if dot<0, the angle between the two vectors is more than 90 degrees. i.e. object is on the front of the face

                if (dot < 0)
                {
                    result = new Vector3(desiredVelocity.X, desiredVelocity.Y, desiredVelocity.Z);

                    Plane facePlane = new Plane(vertices[0], vertices[1], vertices[2]);


                    bool rayIntersects = false;
                    List<Vector3> rayIntersectionPoints = new List<Vector3>();

                       foreach (Ray r in rays)
                       {
                            float? distance = r.Intersects(facePlane);

                            if (distance.HasValue)
                            {
                                //NOTE: May return incorrect values
                                Vector3 res = (r.Position + (r.Direction * distance.Value));
                              

                                //NOTE: Need to check if point is on line segment before reporting that it intersects
                                if (IsPointOnLineSegment(res, r.Position, r.Position + desiredVelocity))
                                {
                                    rayIntersectionPoints.Add(res);
                                    rayIntersects = true;
                                }
                            }
                       }

                       if (lookAheadBoundingBox.Intersects(facePlane) == PlaneIntersectionType.Intersecting || rayIntersects)
                           {
                               if (_normal.X != 0)
                               {
                                   result.X = 0;
                               }
                               if (_normal.Y != 0)
                               {
                                   if (rayIntersectionPoints.Count > 0)//NOTE: Temp
                                   {
                                       result.Y = rayIntersectionPoints[0].Y - lookAheadBoundingBox.Min.Y;
                                   }
                                   else
                                   {
                                       //NOTE: Should calculate velocity that brings box to this point, not just stop velocity
                                       result.Y = 0;//((Vector3)intersection).Y - collisionBox.Position.Y;
                                   }
                               }
                               if (_normal.Z != 0)
                               {
                                   result.Z = 0;
                               }
                           }
                       
                }

            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// Tests for collision along a collision box's desired path and resolves it.
        /// </summary>
        /// <param name="collisionBox"></param>
        /// <param name="desiredVelocity"></param>
        /// <returns></returns>
        public Vector3 ResolveCollision(CollisionBox collisionBox, Vector3 desiredVelocity)
        {
            Vector3 result = Vector3.Zero;
            try
            {
                //Get velocity planes
                List <Plane> velocityPlanes= collisionBox.GetVelocityPlanes(desiredVelocity);

                Plane facePlane = new Plane(vertices[0], vertices[1], vertices[2]);
               // facePlane.Normal = _normal;

                //NOTE: Move to plane-plane intersection
              /*  foreach (Plane p in velocityPlanes)
                {
                    if (p.Intersects(facePlane))
                    {
                    }

                }*/

                //Pre-collision velocity. i.e. return this if no collision occurs

                result = new Vector3(desiredVelocity.X, desiredVelocity.Y, desiredVelocity.Z);
                //NOTE: Temp ray-plane intersection method
                List<Ray> rays = collisionBox.GetVertexMovementRays(desiredVelocity);
              /*  if (_normal.Y != 0)
                {
                }*/
                foreach (Ray r in rays)
                {
                    Vector3? intersection = GetRayPlaneIntersectionPoint(r, facePlane);
                    if (intersection!=null)
                    {
                        if (IsPointOnLineSegment((Vector3)intersection, r.Position, r.Position + desiredVelocity)&&(IsPointOnFaceSegment((Vector3)intersection)))
                        {
                            //Collision has occured
                            result = new Vector3(desiredVelocity.X, desiredVelocity.Y, desiredVelocity.Z);

                            //NOTE: Must determine if collision was on plane segment and velocity line segment

                           /*   if (_normal.X != 0)
                              {
                                  result.X = 0;
                              }*/
                            if (_normal.Y != 0)
                            {
                                //NOTE: Should calculate velocity that brings box to this point, not just stop velocity
                                result.Y = 0;//((Vector3)intersection).Y - collisionBox.Position.Y;
                                break;
                            }
                            /*  if (_normal.Z != 0)
                              {
                                  result.Z = 0;
                              }*/
                          
                            //NOTE: for slopes and steps, all rays would need to be checked
                           
                        }
                    }
                }

            }
            catch
            {
                //NOTE: Should stop velocity if error occurs...
                throw;
            }


            return result;
        }

        /// <summary>
        /// NOTE: Not stable - still cases where intersection not correctly reported
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public float? Intersects(Ray ray)
        {
            float? result = null;
            Plane facePlane = new Plane(vertices[0], vertices[1], vertices[2]);
             Vector3? intersectionPoint = GetRayPlaneIntersectionPoint(ray, facePlane);

             if (intersectionPoint != null)
             {
                 Vector3 point = (Vector3)intersectionPoint;
                
                 //If intersection point is not null, check if point is on face segment
                 if (IsPointOnFaceSegment(point))
                 {
                     result = ray.Intersects(facePlane);
                 }
             }
             else
             {
                 result = null;
             }

            return result;
        }

        /// <summary>
        /// Determine if a point is on the face
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsPointOnFaceSegment(Vector3 point)
        {
            bool result = false;
            //NOTE: Error when point is outside segment but along a primary axis
            Vector3 ? line1=LineIntersectionPoint(vertices[0], point, vertices[1], vertices[2]);
            Vector3? line2=LineIntersectionPoint(vertices[0], point, vertices[2], vertices[3]);
            Vector3? line3=LineIntersectionPoint(vertices[1], point, vertices[2], vertices[3]);
            Vector3? line4=LineIntersectionPoint(vertices[1], point, vertices[3], vertices[0]);

            Vector3? line5 = LineIntersectionPoint(vertices[2], point, vertices[3], vertices[0]);
            Vector3? line6 = LineIntersectionPoint(vertices[2], point, vertices[0], vertices[1]);

            bool line1Intersects = false;
            bool line2Intersects = false;
            bool line3Intersects = false;
            bool line4Intersects = false;
            bool line5Intersects = false;
            bool line6Intersects = false;

            if(line1!=null)
            {
                line1Intersects = IsPointOnLineSegment((Vector3)line1, vertices[1], vertices[2]);
            }
            if (line2 != null)
            {
                line2Intersects = IsPointOnLineSegment((Vector3)line2, vertices[2], vertices[3]);
            }
            if (line3 != null)
            {
                line3Intersects = IsPointOnLineSegment((Vector3)line3, vertices[2], vertices[3]);
            }
            if (line4 != null)
            {
                line4Intersects = IsPointOnLineSegment((Vector3)line4, vertices[3], vertices[0]);
            }

            if (line5 != null)
            {
                line5Intersects = IsPointOnLineSegment((Vector3)line5, vertices[3], vertices[0]);
            }

            if (line6 != null)
            {
                line6Intersects = IsPointOnLineSegment((Vector3)line6, vertices[0], vertices[1]);
            }
  
            if ((line1Intersects || line2Intersects) && (line3Intersects || line4Intersects) && (line5Intersects || line6Intersects))
            {
                result = true;
            }



            return result;
        }


        /// <summary>
        /// Get the point of intersection between 2 lines
        /// </summary>
        /// <param name="line1P1"></param>
        /// <param name="line1P2"></param>
        /// <param name="line2P1"></param>
        /// <param name="line2P2"></param>
        /// <returns></returns>
        protected Vector3? LineIntersectionPoint(Vector3 line1P1, Vector3 line1P2,Vector3 line2P1, Vector3 line2P2)
        {
            Vector3? result = null;
            //L1 = P1 + a V1
            //L2 = P2 + b V2
            Vector3 v1 = new Vector3(line1P1.X - line1P2.X, line1P1.Y - line1P2.Y, line1P1.Z - line1P2.Z);
            Vector3 v2 = new Vector3(line2P1.X - line2P2.X, line2P1.Y - line2P2.Y, line2P1.Z - line2P2.Z);



            // a (V1 X V2) = (P2 - P1) X V2
            Vector3 alpa = Vector3.Cross(v1, v2);
            Vector3 beta = line2P2 - line1P2;
            beta = Vector3.Cross(beta, v2);

            Vector3 ans = beta / alpa;
            //P1 + aV1==Pintersection
            List<float> aVals = new List<float>();
            if (!float.IsNaN(ans.X) && !float.IsInfinity(ans.X))
            {
                aVals.Add(ans.X);
            }

            if (!float.IsNaN(ans.Y) && !float.IsInfinity(ans.Y))
            {
                aVals.Add(ans.Y);
            }

            if (!float.IsNaN(ans.Z) && !float.IsInfinity(ans.Z))
            {
                aVals.Add(ans.Z);
            }

            bool allowA = true;

            if (aVals.Count > 1)
            {
                for (int i = 1; i < aVals.Count; i++)
                {
                    if (aVals[i - 1] != aVals[i])
                    {
                        allowA = false;
                        break;
                    }
                }

            }

            if (allowA && aVals.Count > 0)
            {
                float a;
                a = aVals[0];
                result = line1P2 + a * v1;
            }

            return result;
        }

      /*  public bool Intersects(CollisionBox collisionBox)
        {
            bool result = false;

            return result;
        }*/

        public Vector3? GetRayFaceIntersectionPoint(Ray ray)
        {
            Plane facePlane = new Plane(vertices[0], vertices[1], vertices[2]);
            Vector3? intersectionPoint = GetRayPlaneIntersectionPoint(ray, facePlane);

            return intersectionPoint;
        }

        /// <summary>
        /// NOTE: Needs to account for floating point error
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public Vector3? GetRayPlaneIntersectionPoint(Ray ray, Plane plane)
        {
            Vector3 ? result = null;

            float? distance = ray.Intersects(plane);

            if (distance.HasValue)
            {
                Vector3 res = (ray.Position +(ray.Direction * distance.Value));
             /*   if (plane.Normal.X != 0)
                {
                    res.X *= plane.Normal.X;
                }
                if (plane.Normal.Y != 0)
                {
                    res.Y *= plane.Normal.Y;
                }
                if (plane.Normal.Z != 0)
                {
                    res.Z *= plane.Normal.Z;
                }*/

                //Round to 3 decimal palces to account for floating-point error
                res.X = (float)Math.Round(res.X, 3);
                res.Y = (float)Math.Round(res.Y, 3);
                res.Z = (float)Math.Round(res.Z, 3);
                result = res;
            }
            return result;
        }

        public bool IsPointOnLineSegment(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
        {
            bool result = false;

            //Check that point is on line

            //(x - x1) / (x2 - x1) = (y - y1) / (y2 - y1) = (z - z1) / (z2 - z1)

            //(x-x1)(z2-z1)(y2-y1)=(y-y1)(x2-x1)(z2-z1)=(z-z1)(x2-x1)(y2-y1)

            //NOTE: Errors if line is along any primary axis (i.e linePoint2.X - linePoint1.X=0 etc.)
          /*  float xEquat = (float)Math.Round((point.X - linePoint1.X)*(linePoint2.Y - linePoint1.Y)*(linePoint2.Z - linePoint1.Z) ,3);
            float yEquat = (float)Math.Round((point.Y - linePoint1.Y)*(linePoint2.X - linePoint1.X)*(linePoint2.Z - linePoint1.Z)  , 3);
            float zEquat = (float)Math.Round((point.Z - linePoint1.Z)*(linePoint2.Y - linePoint1.Y)*(linePoint2.X - linePoint1.X) , 3);

            
            if (xEquat == zEquat && yEquat == zEquat)*/
            {
                //if on line, check that it is on line segment
                //i.e. dist linePoint1=>linePoint2>dist LinePoint1=>point ...
             
                float dist1 = (float)Math.Sqrt(Math.Pow(linePoint1.X - linePoint2.X, 2) + Math.Pow(linePoint1.Y - linePoint2.Y, 2) + Math.Pow(linePoint1.Z - linePoint2.Z, 2));
                float dist2 = (float)Math.Sqrt(Math.Pow(linePoint1.X - point.X, 2) + Math.Pow(linePoint1.Y - point.Y, 2) + Math.Pow(linePoint1.Z - point.Z, 2));
                float dist3 = (float)Math.Sqrt(Math.Pow(point.X - linePoint2.X, 2) + Math.Pow(point.Y - linePoint2.Y, 2) + Math.Pow(point.Z - linePoint2.Z, 2));

                if (Math.Round(dist2 + dist3,3) == Math.Round(dist1,3))
                {
                    result = true;
                }

             /*   if (dist1 <= dist2)
                {
                    result = true;
                }*/
 
            }

            return result;
        }
        
        /// <summary>
        /// Gets the distance from a point to the face's centre
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double GetDistanceFrom(Vector3 point)
        {
            double result = 0;
            try
            {
                if (_centre != null)
                {
                    //SQqrt((x2-x1)^2+(y2-y1)^2+(z2-z1)^2)
                    result = Math.Sqrt(Math.Pow(point.X - _centre.X, 2) + Math.Pow(point.Y - _centre.Y, 2) + Math.Pow(point.Z - _centre.Z, 2));
                }
                else
                {
                    throw new Exception("CollisionFace centre not set");
                }
            }
            catch
            {
                throw;
            }
            return result;
        }

        public Plane Plane
        {
            get
            {
                Plane p = new Plane(vertices[0], vertices[1], vertices[2]);
               // p.Normal = _normal;
                return p;
            }
        }

      
    }
}
