using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelRPGGame.GameEngine.Physics
{
    /// <summary>
    /// Collision box that is free to rotate on the y (up) axis
    /// </summary>
    public class CollisionBox
    {
       protected Vector3[] bottomVerts;
       protected Vector3[] topVerts;
       protected Vector3 position;

        protected float lengthX;
        protected float lengthZ;
        protected float height;

       protected float rotationY;

        /// <summary>
        /// Points are bottom of box. Height is used to get top vertices from bottom
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="point4"></param>
        /// <param name="height"></param>
        public CollisionBox(Vector3 origin,float lengthX,float lengthZ, float height)
        {
            this.lengthX=lengthX;
            this.lengthZ = lengthZ;
            this.height = height;
            rotationY = 0;

            position = new Vector3(origin.X, origin.Y, origin.Z);
            bottomVerts = new Vector3[4];
            topVerts = new Vector3[4];

            bottomVerts[0] = position+new Vector3(-this.lengthX / 2, 0, -this.lengthZ/2);
            bottomVerts[1] = position + new Vector3(this.lengthX / 2, 0, -this.lengthZ / 2);
            bottomVerts[2] = position + new Vector3(this.lengthX / 2, 0, this.lengthZ / 2);
            bottomVerts[3] = position + new Vector3(-this.lengthX / 2, 0, this.lengthZ / 2);

            topVerts[0] =bottomVerts[0] +new Vector3(0,height,0);
            topVerts[1] = bottomVerts[1] + new Vector3(0, height, 0);
            topVerts[2] = bottomVerts[2] + new Vector3(0, height, 0);
            topVerts[3] = bottomVerts[3] + new Vector3(0, height, 0);
        }

        public void Update(Vector3 newPosition, float newRotation)
        {
            position = new Vector3(newPosition.X, newPosition.Y, newPosition.Z);
            rotationY = newRotation;
            float rotationRadians = -rotationY;

            RotatePointY(bottomVerts[0], position, rotationRadians);
            bottomVerts[0] = RotatePointY(position + new Vector3(-this.lengthX / 2, 0, -this.lengthZ / 2), position, rotationRadians);
            bottomVerts[1] = RotatePointY(position + new Vector3(this.lengthX / 2, 0, -this.lengthZ / 2), position, rotationRadians);
            bottomVerts[2] = RotatePointY(position + new Vector3(this.lengthX / 2, 0, this.lengthZ / 2), position, rotationRadians);
            bottomVerts[3] = RotatePointY(position + new Vector3(-this.lengthX / 2, 0, this.lengthZ / 2), position, rotationRadians);


            topVerts[0] = bottomVerts[0] + new Vector3(0, height, 0);
            topVerts[1] = bottomVerts[1] + new Vector3(0, height, 0);
            topVerts[2] = bottomVerts[2] + new Vector3(0, height, 0);
            topVerts[3] = bottomVerts[3] + new Vector3(0, height, 0);
        }
      
        public List<VertexPositionColor> GetCollisionBoxWireframe(Color colour)
        {
            List<VertexPositionColor> result = new List<VertexPositionColor>();
          
            //Create 6 faces of cube
            //create bottom Face
            result = new List<VertexPositionColor>();

            for (int i = 0; i < bottomVerts.Length; i++)
            {
                result.Add(new VertexPositionColor(bottomVerts[i], colour));
            }
            //Loop connect vertex back to original
            result.Add(new VertexPositionColor(bottomVerts[0], colour));
            //create top Face
            for (int i = 0; i < topVerts.Length; i++)
            {
                result.Add(new VertexPositionColor(topVerts[i], colour));
            }
            result.Add(new VertexPositionColor(topVerts[0], colour));


            result.Add(new VertexPositionColor(topVerts[1], colour));
            result.Add(new VertexPositionColor(bottomVerts[1], colour));


            result.Add(new VertexPositionColor(bottomVerts[2], colour));
            result.Add(new VertexPositionColor(topVerts[2], colour));
            result.Add(new VertexPositionColor(topVerts[3], colour));
            result.Add(new VertexPositionColor(bottomVerts[3], colour));

            return result;
        }

       

        public float RotationY
        {
            get
            {
                return rotationY;
            }
         
        }

        protected Vector3 RotatePointY(Vector3 point, Vector3 pivot, float angleRadians)
        {
            Vector3 result = new Vector3(point.X, point.Y, point.Z);

            float cos = (float)Math.Cos(angleRadians);
            float sin = (float)Math.Sin(angleRadians);

            Vector3 p = new Vector3(pivot.X, pivot.Y, pivot.Z);

  //          p'x = cos(theta) * (px-ox) - sin(theta) * (py-oy) + ox
            result.X = cos * (point.X - p.X) - sin * (point.Z - p.Z) + p.X;
            //     p'y = sin(theta) * (px-ox) + cos(theta) * (py-oy) + oy
            result.Z = sin * (point.X - p.X) + cos * (point.Z - p.Z) + p.Z;

            return result;
        }

        /// <summary>
        /// Applies velocity to each of the box's corners and represents the movement of each
        /// of the box's edges as a plane. 
        /// NOTE: Currently not getting horizontal planes
        /// </summary>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public List<Plane> GetVelocityPlanes(Vector3 velocity)
        {
            List<Plane> result = new List<Plane>();

            Vector3[] lookAheadBottomVerts = new Vector3[4];
            Vector3[] lookAheadTopVerts = new Vector3[4];

            for (int i = 0; i < bottomVerts.Length; i++)
            {
                lookAheadBottomVerts[i] = bottomVerts[i] + velocity;
                lookAheadTopVerts[i] = topVerts[i] + velocity;

                //Build all vertical velocity planes
                result.Add(new Plane(bottomVerts[i],topVerts[i],lookAheadBottomVerts[i]));
            }

            // NOTE: Currently not getting horizontal planes

            return result;
        }

        /// <summary>
        /// Gets movement rays for box vertices
        /// </summary>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public List<Ray> GetVertexMovementRays(Vector3 velocity)
        {
            List<Ray> result = new List<Ray>();

            Vector3 normalVelocity = new Vector3(velocity.X, velocity.Y, velocity.Z);
            if (normalVelocity != Vector3.Zero)
            {
                normalVelocity.Normalize();
            }

            for (int i = 0; i < bottomVerts.Length; i++)
            {
                result.Add(new Ray(bottomVerts[i], normalVelocity));
                result.Add(new Ray(topVerts[i], normalVelocity));
            }
            result.Add(new Ray(position, normalVelocity));

            return result;
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }
        }

        public float LengthX
        {
            get
            {
                return lengthX;
            }
        }

        public float LengthZ
        {
            get
            {
                return lengthZ;
            }
        }
        public float Height
        {
            get
            {
                return height;
            }
        }

        public List<Vector3> GetVertices()
        {
            List<Vector3> result = new List<Vector3>();

            foreach (Vector3 v in bottomVerts)
            {
                result.Add(v);
            }

            return result;
        }
    }
}
