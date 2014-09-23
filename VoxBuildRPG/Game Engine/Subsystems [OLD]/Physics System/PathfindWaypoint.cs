using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace VoxelRPGGame.GameEngine.Subsystems.PhysicsSystem
{
    /// <summary>
    /// Map waypoints used for pathfinding
    /// </summary>
    public class PathfindWaypoint
    {
        private Vector3 position;
        private Dictionary<PathfindWaypoint,double> connectedWaypoints;

        public PathfindWaypoint parent = null;

    //    private int g = 0, h = 0, f = 0;


        public PathfindWaypoint(Vector3 pos)
        {
            position = pos;
            connectedWaypoints = new Dictionary<PathfindWaypoint, double>();
        }


        public void ConnectWaypoints(PathfindWaypoint otherWaypoint)
        {
            if (!connectedWaypoints.ContainsKey(otherWaypoint))
            {
                connectedWaypoints.Add(otherWaypoint, GetDistanceTo(otherWaypoint.Position));
                otherWaypoint.ConnectWaypoints(this);
            }
        }


        public PathfindWaypoint Copy()
        {
            PathfindWaypoint result = new PathfindWaypoint(position);

            foreach (KeyValuePair<PathfindWaypoint,double> way in connectedWaypoints)
            {
                result.connectedWaypoints.Add(way.Key, way.Value);
            }

            return result;
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

        public Vector3 Position
        {
            get
            {
                return position;
            }
        }

        public List<PathfindWaypoint> ConnectedWaypoints
        {
            get
            {
                List<PathfindWaypoint> result = new List<PathfindWaypoint>();

                foreach (PathfindWaypoint waypoint in connectedWaypoints.Keys)
                {
                    result.Add(waypoint);
                }

                return result;
            }
        }
        public Dictionary<PathfindWaypoint, double> ConnectedWaypointDictionary
        {
            get
            {
                return connectedWaypoints;
            }
        }

        public void RemoveConnectedWaypoint(PathfindWaypoint p)
        {
            if (connectedWaypoints.ContainsKey(p))
            {
                connectedWaypoints.Remove(p);
                p.RemoveConnectedWaypoint(this);
            }
        }
    }
}
