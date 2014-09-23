using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace VoxelRPGGame.GameEngine.Subsystems.PhysicsSystem
{
    public class CollisionSquare
    {
        protected double centreX;
        protected double centreY;

        protected double width;
        protected double height;

        protected double x1;
        protected double y1;

        protected double x2;
        protected double y2;

        public CollisionSquare(Vector3 position, double width, double height)
        {


            centreX = Math.Round(position.Z,4);
            centreY = Math.Round(position.X,4);
            this.width = width;
            this.height = height;

            x1 = 0;
            y1 = 0;

            x2 = 0;
            y2 = 0;

            SetPoints();
        }

        public CollisionSquare()
        {
        }

        public CollisionSquare(Vector3 bottom, Vector3 top)
        {
            x1 = bottom.Z;
            y1 = bottom.X;

            x2 = top.Z;
            y2 = top.X;
        }

        public CollisionSquare(double bottomX, double bottomY, double topX, double topY, double ? centreX, double ? centreY)
        {
            x1 = bottomX;
            y1 = bottomY;

            x2 = topX;
            y2 = topY;
        }

        public CollisionSquare(double centreX, double centreY, double width, double height)
        {


            this.centreX = Math.Round(centreX, 4);
            this.centreY = Math.Round(centreY, 4);
            this.width = width;
            this.height = height;

            x1 = 0;
            y1 = 0;

            x2 = 0;
            y2 = 0;

            SetPoints();
        }

        

        /// <summary>
        /// Updates the collision square's position
        /// </summary>
        /// <param name="newCentre"></param>
        public void MoveSquare(Vector3 newCentre)
        {
            centreX = Math.Round(newCentre.Z,4);
            centreY = Math.Round(newCentre.X,4);

            SetPoints();
        }

        public void MoveSquare(double newCentreX, double newCentreY)
        {
            centreX = Math.Round(newCentreX,4);
            centreY = Math.Round(newCentreY,4);

            SetPoints();
        }

        public void SetPoints()
        {
            x1 = centreX - (width / 2);
            x2 = x1 + width;

            y1 = centreY - (height / 2);
            y2 = y1 + height;
        }




        public virtual bool IsColliding(CollisionSquare physicsSquare)
        {
            bool result = true;


            //Covers all cases where two rectangles are not touching
            if (physicsSquare.TopX <= BottomX || physicsSquare.TopY <= BottomY
                || physicsSquare.BottomX >= TopX || physicsSquare.BottomY >= TopY)
            {
                result = false;
            }


            return result;
        }

        /// <summary>
        /// Checks if a point is inside the collision object
        /// </summary>
        /// <param name="position">X: Global X value, Y: Global Z value</param>
        /// <returns></returns>
        public bool IsInside(Vector2 position)
        {
            //Flip the points: i.e. position.X is global X axis, which is 2D y-axis
            Vector2 point = new Vector2(position.Y, position.X);

            bool result = false;

            if (point.X >= BottomX && point.Y >= BottomY
                && point.X <= TopX && point.Y <= TopY)
            {
                result = true;
            }

            return result;
        }

        public bool IsInside(CollisionSquare square)
        {
            bool result = false;

            if (square.BottomX >= BottomX && square.BottomY >= BottomY
                && square.TopX <= TopX && square.TopY <= TopY)
            {
                result = true;
            }

            return result;
        }

#region Line Intersection

        /// <summary>
        /// Detect if a line intersects with the collision square
        /// </summary>
        /// <param name="p1">X: Global X value, Y: Global Z value</param>
        /// <param name="p2">X: Global X value, Y: Global Z value</param>
        /// <returns></returns>
        public bool LineIntersects(Vector2 p1, Vector2 p2)
        {

            return LineIntersectsLine(p1, p2, new Vector2((float)y1, (float)x1), new Vector2((float)y2, (float)x1)) ||
                   LineIntersectsLine(p1, p2, new Vector2((float)y2, (float)x1), new Vector2((float)y2, (float)x2)) ||
                   LineIntersectsLine(p1, p2, new Vector2((float)y2, (float)x2), new Vector2((float)y1, (float)x2)) ||
                   LineIntersectsLine(p1, p2, new Vector2((float)y1, (float)x2), new Vector2((float)y1, (float)x1));
        }

        private bool LineIntersectsLine(Vector2 l1p1, Vector2 l1p2, Vector2 l2p1, Vector2 l2p2)
          {
              float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
              float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

              if (d == 0)
              {
                  return false;
              }

              float r = q / d;

              q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
              float s = q / d;

              if (r < 0 || r > 1 || s < 0 || s > 1)
              {
                  return false;
              }

              return true;
          }

        /// <summary>
        /// Returns the nearest point to p1 at which the line intersects the square 
        /// </summary>
        /// <param name="p1">current position,X: Global X value, Y: Global Z value</param>
        /// <param name="p2">target point,X: Global X value, Y: Global Z value</param>
        /// <returns>if not null, result[0] is global X, result[1] is global Z </returns>
        public double[] LineIntersectsPoint(Vector2 p1, Vector2 p2)
        {
            double[] result = null;

            List<double[]> points = new List<double[]>();

            points.Add(LineIntersectsLinePoint(p1, p2, new Vector2((float)y1, (float)x1), new Vector2((float)y2, (float)x1)));
            points.Add(LineIntersectsLinePoint(p1, p2, new Vector2((float)y2, (float)x1), new Vector2((float)y2, (float)x2)));
            points.Add(LineIntersectsLinePoint(p1, p2, new Vector2((float)y2, (float)x2), new Vector2((float)y1, (float)x2)));
            points.Add(LineIntersectsLinePoint(p1, p2, new Vector2((float)y1, (float)x2), new Vector2((float)y1, (float)x1)));

            if (points[0] != null)
            {
                result = points[0];
            }

            for (int i = 1; i < points.Count; i++)
            {
                if (points[i] != null)
                {
                    if (result != null)
                    {
                        //If current result is further away than points[i], change result
                        if (GetDistanceTo(new Vector2(p1.Y, p1.X), new Vector2((float)result[0], (float)result[1])) > GetDistanceTo(new Vector2(p1.Y, p1.X), new Vector2((float)points[i][0], (float)points[i][1])))
                        {
                            result = points[i];
                        }
                    }

                    else
                    {
                        result = points[i];
                    }

                }
            }

            return result;
        }

        protected double[] LineIntersectsLinePoint(Vector2 l1p1, Vector2 l1p2, Vector2 l2p1, Vector2 l2p2)
        {
            double[] result = null;

            double a1 = l1p2.Y - l1p1.Y;
            double b1 = l1p1.X - l1p2.X;
            double c1 = a1 * l1p1.X + b1 * l1p1.Y;

            double a2 = l2p2.Y - l2p1.Y;
            double b2 = l2p1.X - l2p2.X;
            double c2 = (a2 * l2p1.X) + (b2 * l2p1.Y);


            double det = (a1 * b2) - (a2 * b1);

            //If det =0 it means lines are parallel
            if (det != 0)
            {
                 double x = (b2*c1 - b1*c2)/det;
                 double y = (a1 * c2 - a2 * c1) / det;

                //Check that points are on both line segments
                 if ((Math.Min(l1p1.X, l1p2.X) <= x && x <= Math.Max(l1p1.X, l1p2.X)) && (Math.Min(l1p1.Y, l1p2.Y) <= y && y <= Math.Max(l1p1.Y, l1p2.Y))
                     && (Math.Min(l2p1.X, l2p2.X) <= x && x <= Math.Max(l2p1.X, l2p2.X)) && (Math.Min(l2p1.Y, l2p2.Y) <= y && y <= Math.Max(l2p1.Y, l2p2.Y)))
                 {


                     result = new double[2];


                     result[0] = x;
                     result[1] = y;
                 }
            }




            return result;
        }

        protected double GetDistanceTo(Vector2 startPoint,Vector2 endPoint)
        {

            double result = 0;

            result = Math.Sqrt(Math.Pow((endPoint.X - startPoint.X), 2) + Math.Pow((endPoint.Y - startPoint.Y), 2));


            return result;
        }

#endregion

    public override bool Equals(object obj)
        {
            bool result = false;

            if (obj is CollisionSquare)
            {
                if(BottomX ==(obj as CollisionSquare).BottomX&&BottomY==(obj as CollisionSquare).BottomY&&
                    TopX==(obj as CollisionSquare).TopX&&TopY==(obj as CollisionSquare).TopY)
                {
                    result = true;
                }
            }

            return result;
        }
        /// <summary>
        /// X is the global z axis
        /// </summary>
        public double BottomX
        {
            get
            {
                return x1;
            }
        }
        /// <summary>
        /// Y is the global X axis
        /// </summary>
        public double BottomY
        {
            get
            {
                return y1;
            }
        }
        /// <summary>
        /// X is the global z axis
        /// </summary>
        public double TopX
        {
            get
            {
                return x2;
            }
        }
        /// <summary>
        /// Y is the global X axis
        /// </summary>
        public double TopY
        {
            get
            {
                return y2;
            }
        }
        /// <summary>
        /// X is the global z axis
        /// </summary>
        public double CentreX
        {
            get
            {
                return centreX;
            }
        }
        /// <summary>
        /// Y is the global X axis
        /// </summary>
        public double CentreY
        {
            get
            {
                return centreY;
            }
        }

        public double Height
        {
            get
            {
                return height;
            }
        }

        public double Width
        {
            get
            {
                return width;
            }
        }


        public Vector3 GlobalPosition
        {
            get
            {
                return new Vector3((float)centreY, 0, (float)centreX);
            }
        }

        public Vector3 GlobalBottom
        {
            get
            {
                return new Vector3((float)y1, 0, (float)x1);
            }
        }

        public Vector3 GlobalTop
        {
            get
            {
                return new Vector3((float)y2, 0, (float)x2);
            }
        }
    }
}
