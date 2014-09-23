using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace VoxelRPGGame.GameEngine.Subsystems.PhysicsSystem
{
    /// <summary>
    /// Used to construct the path 
    /// </summary>
    public class PathfindNode:IEquatable<PathfindNode>
    {
        private Vector3 position;
        private double defaultG=0,g = 0, h = 0, f = 0;
        private PathfindNode parent = null;
        private PathfindWaypoint owner;

        public PathfindNode(PathfindWaypoint owningWaypoint, PathfindNode parentNode, double distanceFromParent, Vector3 destination)
        {
            position = owningWaypoint.Position;
            parent = parentNode;
            owner = owningWaypoint;

            defaultG = distanceFromParent;
          
            h = GetDistanceTo(destination);
            CalculateFitness();
        }

        protected void CalculateFitness()
        {
            if (parent != null)
            {
                g = defaultG + Parent.G;
            }

            else
            {
                g = defaultG;
            }
            f = g + h;
        }

        /// <summary>
        /// Gets the distance between two positions
        /// </summary>
        /// <param name="startPoint"></param>
        /// <returns></returns>
        public double GetDistanceTo(Vector3 startPoint)
        {

            double result = 0;

            result = Math.Sqrt(Math.Pow((position.X - startPoint.X), 2) + Math.Pow((position.Z - startPoint.Z), 2));


            return result;
        }

        public bool IsPathBetter(PathfindNode n)
        {
            //Tests to see if fitness is better if going from n to current, rather than from parent to current
            bool result = false;

            if (n.GetDistanceTo(position) + n.G <= g)
            {
                //If path is better, change parent



                result = true;
                parent = n;

                //Set the new g,f values
                defaultG = n.GetDistanceTo(position);
                defaultG = n.GetDistanceTo(position);
                CalculateFitness();
            }


            return result;
        }

        public bool Equals(PathfindNode other)
        {
            bool result = false;

            if (other.Position == this.Position)
            {
                result = true;
            }

            return result;
        }


#region Properties


        public Dictionary<PathfindWaypoint, double> ConnectedWaypoints
        {
            get
            {
                return owner.ConnectedWaypointDictionary;
            }
        }

        public PathfindNode Parent
        {
            get
            {
                return parent;
            }
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }
        }

        public double G
        {
            get
            {
                return g;
            }
        }

        public double F
        {
            get
            {
                return f;
            }
        }


        public double H
        {
            get
            {
                return h;
            }
        }

#endregion
    }
}
