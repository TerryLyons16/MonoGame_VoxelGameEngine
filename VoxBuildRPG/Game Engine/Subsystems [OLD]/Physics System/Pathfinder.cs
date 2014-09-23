//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Microsoft.Xna.Framework;

//namespace VoxelRPGGame.GameEngine.Subsystems.PhysicsSystem
//{
//    public class Pathfinder
//    {
//        public delegate List<StaticPhysicsObject> GetStaticPhysicsObjects();
//        public event GetStaticPhysicsObjects RequestStaticPhysicsObjects;

//        public delegate Dictionary<CollisionSquare, List<CollisionSquare>> GetStaticPhysicsObjectSectors();
//        public event GetStaticPhysicsObjectSectors RequestStaticPhysicsObjectSectors;

//        public delegate CollisionSquare GetWorldBounds();
//        public event GetWorldBounds RequestWorldBounds;

//        private static Pathfinder pathfinder=null;

//        //Note: These would have to be updated whenever the world layout changes..
//        private Dictionary<Vector3,PathfindWaypoint> waypoints;
//        private List<PathfindNode> knownPaths;


//        private List<PathfindNode> openList;
//        private List<PathfindNode> closedList;

//        private Pathfinder()
//        {
//            waypoints = new Dictionary<Vector3, PathfindWaypoint>();

//            openList = new List<PathfindNode>();
//            closedList = new List<PathfindNode>();
            
//        }

//        public static Pathfinder GetInstance()
//        {
//            if (pathfinder == null)
//            {
//                pathfinder = new Pathfinder();
//            }

//            return pathfinder;
//        }

//        public static void DeleteInstance()
//        {
//            pathfinder = null;
//        }


//#region Pathfinding
//        public Stack<Vector3> Pathfind(Vector3 position, Vector3 destination)
//        {
//            openList = new List<PathfindNode>();
//            closedList = new List<PathfindNode>();

//            Stack<Vector3> result = new Stack<Vector3>();

//            if (waypoints.Count > 0 && (!IsPointReachable(position, destination)))
//            {
//                PathfindWaypoint nearestWaypointToCurrent = GetNearestAvailableWaypoint(position);

//                //If no nearest accessible waypoint to destination, get nearest waypoint that is not directly accessible
//                 PathfindWaypoint nearestWaypointToDestination = GetNearestAvailableWaypoint(destination);

//                if (nearestWaypointToCurrent != null)
//                {
//                    PathfindNode start = new PathfindNode(nearestWaypointToCurrent, null, 0, destination);

//                    openList.Add(start);

//                    bool pathFound = false;
//                    PathfindNode current = null;

//                    while (!pathFound)
//                    {
//                        current = PathfindStep(destination);
//                        if ((current != null && current.Position == nearestWaypointToDestination.Position) || current == null || closedList.Count < 0)
//                        {
//                            pathFound = true;
//                            break;
//                        }
//                    }


//                    Queue<PathfindNode> backPath = new Queue<PathfindNode>();


//                    if (current != null)
//                    {
//                        backPath.Enqueue(current);
//                        while (current.Parent != null)
//                        {
//                            backPath.Enqueue(current.Parent);
//                            current = current.Parent;
//                        }
//                    }

//                    Stack<Vector3> routePoints = new Stack<Vector3>();

//                    routePoints.Push(destination);

//                    while (backPath.Count > 0)
//                    {
//                        routePoints.Push(backPath.Dequeue().Position);
//                    }


//                    result = routePoints;
//                }
//            }

//            else
//            {
//                result = new Stack<Vector3>();
//                result.Push(destination);
//            }


//            return result;

//        }


//        public PathfindNode PathfindStep(Vector3 destination)
//        {
            
//            PathfindNode current = GetFittestNode(openList);

//            openList.Remove(current);

//            closedList.Add(current);
//            if (current!=null&&current.ConnectedWaypoints!=null&&current.ConnectedWaypoints.Count > 0)
//            {
//                foreach (KeyValuePair<PathfindWaypoint, double> waypoint in current.ConnectedWaypoints)
//                {
//                    PathfindNode p = new PathfindNode(waypoint.Key, current, waypoint.Value, destination);

//                    if (!closedList.Contains(p))
//                    {
//                        if (!openList.Contains(p))
//                        {
//                            openList.Add(p);
//                        }

//                        else
//                        {
//                            //check to see if path is better
//                            openList[openList.IndexOf(p)].IsPathBetter(current);
//                        }
//                    }
//                }
//            }

//            return current;

//        }

//        protected PathfindNode GetFittestNode(List<PathfindNode> nodes)
//        {
//            PathfindNode result = null;

//            double lowestFitness = Double.MaxValue;

//            PathfindNode currentNearest = null;

//            foreach (PathfindNode node in nodes)
//            {
//                if (node.F < lowestFitness)
//                {
//                    lowestFitness = node.F;
//                    currentNearest = node;
//                }
//            }
//            result = currentNearest;

//            return result;
//        }

//        protected PathfindWaypoint GetNearestWaypoint(Vector3 position)
//        {
//            PathfindWaypoint result = null;

//            List<PathfindWaypoint> waypointsToCheck = new List<PathfindWaypoint>();

//            foreach (PathfindWaypoint waypoint in waypoints.Values)
//            {
//                waypointsToCheck.Add(waypoint);
//            }

//            double smallestDistance = Double.MaxValue;

//            PathfindWaypoint currentNearest = null;

//            foreach (PathfindWaypoint waypoint in waypointsToCheck)
//            {
//                if (waypoint.GetDistanceTo(position) <= smallestDistance)
//                {
//                    smallestDistance = waypoint.GetDistanceTo(position);
//                    currentNearest = waypoint;
//                }
//            }


//            result = currentNearest;




//            return result;
//        }

//        /// <summary>
//        /// Attempts to get the nearest accessible waypoint.
//        /// If it can't, it will return the nearest waypoint that is
//        /// not directly accessible (i.e. can't draw a straight line to the position)
//        /// </summary>
//        /// <param name="position"></param>
//        /// <returns></returns>
//        public PathfindWaypoint GetNearestAvailableWaypoint(Vector3 position)
//        {
//            PathfindWaypoint result = null;

//            List<PathfindWaypoint> waypointsToCheck = new List<PathfindWaypoint>();

//            foreach(PathfindWaypoint waypoint in waypoints.Values)
//            {
//                waypointsToCheck.Add(waypoint);
//            }

//            bool waypointFound = false;

//            while (!waypointFound)
//            {
//                double smallestDistance = Double.MaxValue;

//                PathfindWaypoint currentNearest = null;

//                foreach (PathfindWaypoint waypoint in waypointsToCheck)
//                {
//                    if (waypoint.GetDistanceTo(position) <= smallestDistance)
//                    {
//                        smallestDistance = waypoint.GetDistanceTo(position);
//                        currentNearest = waypoint;
//                    }
//                }

//                waypointsToCheck.Remove(currentNearest);

//                if (currentNearest != null && IsPointReachable(position, currentNearest.Position))
//                {
//                    waypointFound = true;
//                    result = currentNearest;
//                    break;
//                }
//                else if (waypointsToCheck.Count <= 0)
//                {
//                    result = GetNearestWaypoint(position);
//                    waypointFound = true;
//                    break;
//                }
//            }

         

//            return result;
//        }

//        public bool IsPointReachable(Vector3 currentPosition,Vector3 target)
//        {

//            bool result = true;
//            foreach (StaticPhysicsObject staticObject in RequestStaticPhysicsObjects())
//            {
              
//                if (!staticObject.CollisionBounds.IsInside(new Vector2(currentPosition.X, currentPosition.Z)) && staticObject.BlocksMovement && staticObject.CollisionBounds.LineIntersects(new Vector2(target.X, target.Z), new Vector2(currentPosition.X, currentPosition.Z)))
//                {
//                    result = false;
//                    break;
//                }
//            }


          

//            return result;
//        }

//        public bool IsPointReachableSectorBased(Vector3 currentPosition, Vector3 point)
//        {
        
//            bool result = true;
//            Dictionary<CollisionSquare, List<CollisionSquare>> sectors = RequestStaticPhysicsObjectSectors();

//            foreach (CollisionSquare sector in sectors.Keys)
//            {
              
//                //If the line intersects the sector, or the line is completely inside the sector, check the objects in the sector
//                if (sector.LineIntersects(new Vector2(currentPosition.X, currentPosition.Z), new Vector2(point.X, point.Z))
//                    || (sector.IsInside(new Vector2(currentPosition.X, currentPosition.Z)) && sector.IsInside(new Vector2(point.X, point.Z))))
//                {
//                    foreach (CollisionSquare building in sectors[sector])
//                    {
                       
//                        if (building.LineIntersects(new Vector2(currentPosition.X, currentPosition.Z), new Vector2(point.X, point.Z)))
//                        {
//                            result = false;
//                            break;
//                        }
//                    }
//                }

//                if (result == false)
//                {
//                    break;
//                }

//            }

//            return result;
//        }

//#endregion

//#region Waypoint Creation

//        /// <summary>
//        /// Adds waypoints to graph, but does not connect them.
//        /// To be used during loading
//        /// </summary>
//        /// <param name="positions"></param>
//        public void PlaceWaypoints(List<Vector3> positions)
//        {
//            foreach (Vector3 position in positions)
//            {
//                PlaceWaypoint(position);
//            }
//        }

//        public void PlaceWaypoint(Vector3 position)
//        {
//            if (!waypoints.ContainsKey(position)&&RequestWorldBounds().IsInside(new Vector2(position.X, position.Z)))
//            {
                
//                    waypoints.Add(position, new PathfindWaypoint(position));
                
                

              
//            }
//        }

//        /// <summary>
//        /// [Out of Date]
//        /// To be used if waypoints are being added during simulation
//        /// Adds the waypoint to the graph and connects it, if it does not collide or intersect static objects
//        /// </summary>
//        /// <param name="positions"></param>
//        /// <param name="staticObjects"></param>
//        public void AddWaypointsAndConnect(List<Vector3> positions, List<StaticPhysicsObject> staticObjects)
//        {
//            foreach (Vector3 position in positions)
//            {
//                AddWaypointAndConnect(position, staticObjects);
//            }
//        }

//        /// <summary>
//        /// [Out of Date]
//        /// To be used if a waypoint is being added during simulation
//        /// Adds the waypoint to the graph and connects it, if it does not collide or intersect static objects
//        /// </summary>
//        /// <param name="position"></param>
//        /// <param name="staticObjects"></param>
//        public void AddWaypointAndConnect(Vector3 position,List<StaticPhysicsObject> staticObjects)
//        {
//            PathfindWaypoint waypoint = new PathfindWaypoint(position);
//            if (!waypoints.ContainsKey(position))
//            {
//                if (RequestWorldBounds().IsInside(new Vector2(position.X, position.Z)))
//                {

//                    bool canAdd = true;
//                    foreach (StaticPhysicsObject staticObject in staticObjects)
//                    {
//                        if (staticObject.CollisionBounds.IsInside(new Vector2(waypoint.Position.X, waypoint.Position.Z)) && staticObject.BlocksMovement)
//                        {
//                            canAdd = false;
//                            break;
//                        }
//                    }

//                    if (canAdd)
//                    {
//                        foreach (PathfindWaypoint way in waypoints.Values)
//                        {

//                            bool canConnect = true;

//                            foreach (StaticPhysicsObject staticObject in staticObjects)
//                            {

//                                if (staticObject.BlocksMovement && staticObject.CollisionBounds.LineIntersects(new Vector2(waypoint.Position.X, waypoint.Position.Z), new Vector2(way.Position.X, way.Position.Z)))
//                                {
//                                    canConnect = false;
//                                    break;
//                                }
//                            }


//                            if (canConnect)
//                            {
//                                waypoint.ConnectWaypoints(way);
//                            }
//                        }
//                        waypoints.Add(waypoint.Position, waypoint);

//                    }
//                }
//            }

            
//        }

//        public void ConnectAllWaypoints()
//        {
//            SweepAndPruneNodes();
//            ConnectEdges();
//        }

//        public void ConnectWaypoints(Vector3 way1, Vector3 way2)
//        {
//            if (waypoints.ContainsKey(way1) && waypoints.ContainsKey(way2))
//            {
//                waypoints[way1].ConnectWaypoints(waypoints[way2]);
//            }
//        }

//        /// <summary>
//        /// Removes all waypoints that collide with static objects
//        /// </summary>
//        public void SweepAndPruneNodes()
//        {
//            List<CollisionSquare> xAxisList = new List<CollisionSquare>();
//            List<CollisionSquare> activeList = new List<CollisionSquare>();
//            //Add static physics objects to list 
//            foreach (StaticPhysicsObject o in RequestStaticPhysicsObjects())
//            {
//                xAxisList.Add(o.CollisionBounds);
//            }

//            //Add waypoint as a collision square with no width or height
//            foreach (PathfindWaypoint w in waypoints.Values)
//            {
//                xAxisList.Add(new CollisionSquare(w.Position, 0, 0));
//            }

//            //Sort along 2D x-axis
//            xAxisList = xAxisList.OrderBy(o => o.BottomX).ToList<CollisionSquare>();

//            Queue<CollisionSquare> xAxisQueue = new Queue<CollisionSquare>();

//            foreach (CollisionSquare e in xAxisList)
//            {
//                xAxisQueue.Enqueue(e);
//            }



//            while (xAxisQueue.Count > 0)
//            {

//                List<CollisionSquare> toRemove = new List<CollisionSquare>();

//                CollisionSquare current = xAxisQueue.Dequeue();
              
//                for (int i = 0; i < activeList.Count; i++)
//                {

//                    //Check if current overlaps with activeList elements on XAxis
//                    if (current.BottomX <= activeList[i].TopX)
//                    {
//                        //if overlap, check for full collision between perception squares
//                        if (activeList[i].IsColliding(current))
//                        {
//                            //If either is a waypoint, it will be removed from the graph
//                            RemoveWaypoint(current.GlobalPosition);
//                            RemoveWaypoint(activeList[i].GlobalPosition);
//                        }

//                    }
//                    //Else, remove activeList element
//                    else
//                    {

//                        toRemove.Add(activeList[i]);
//                    }

//                }

//                foreach (CollisionSquare e in toRemove)
//                {
//                    activeList.Remove(e);
//                }

//                //Add current to activeList
//                activeList.Add(current);

//            }

//        }


//        public void ConnectEdges()
//        {

//            Dictionary<CollisionSquare, List<CollisionSquare>> sectors = RequestStaticPhysicsObjectSectors();
            
//            //Dictionary of edges. String values are empty

//            List<PathfindWaypoint> ways = waypoints.Values.ToList<PathfindWaypoint>();

//            List<PathfindWaypoint[]> edges = new List<PathfindWaypoint[]>();
//            for (int i = 0; i < ways.Count - 1; i++)
//            {
//                for (int j = i + 1; j < ways.Count; j++)
//                {

//                    PathfindWaypoint[] edge = new PathfindWaypoint[2];

//                    edge[0] = ways[i];
//                    edge[1] = ways[j];
//                    //Attempt to add unique edge

//                    edges.Add(edge);
//                }
//                }
            

//            foreach (PathfindWaypoint[] edge in edges)
//            {

//                bool canConnect = true;
//                foreach (CollisionSquare sector in sectors.Keys)
//                {


//                    //If the line intersects the sector, or the line is completely inside the sector, check the objects in the sector
//                    if (sector.LineIntersects(new Vector2(edge[0].Position.X, edge[0].Position.Z), new Vector2(edge[1].Position.X, edge[1].Position.Z))
//                        || (sector.IsInside(new Vector2(edge[0].Position.X, edge[0].Position.Z)) && sector.IsInside(new Vector2(edge[1].Position.X, edge[1].Position.Z))))
//                    {
//                        foreach (CollisionSquare building in sectors[sector])
//                        {
//                            if (building.LineIntersects(new Vector2(edge[0].Position.X, edge[0].Position.Z), new Vector2(edge[1].Position.X, edge[1].Position.Z)))
//                            {
//                                canConnect = false;
//                                break;
//                            }
//                        }
//                    }

//                    if (!canConnect)
//                    {
//                        break;
//                    }
//                }


//                if (canConnect)
//                {
//                    edge[0].ConnectWaypoints(edge[1]);
//                }
//            }
           
//        }


//#endregion

//        /// <summary>
//        /// Used when a new staticPhysicsObject is added
//        /// NOTE: Should change to using sector-based object lookup
//        /// </summary>
//        public void RecalculateWaypoints()
//        {

//            SweepAndPruneNodes();

//            //NOTE: Will have to clear the existing edges before performing sweep and prune edges

//            Queue<PathfindWaypoint> waypointsToRemove = new Queue<PathfindWaypoint>();

//            foreach (PathfindWaypoint way in waypoints.Values)
//            {
//                bool removeWaypoint = false;


//                foreach (StaticPhysicsObject staticObject in RequestStaticPhysicsObjects())
//                {
//                    if (staticObject.CollisionBounds.IsInside(new Vector2(way.Position.X, way.Position.Z)) && staticObject.BlocksMovement)
//                    {
//                        removeWaypoint = true;
//                        break;
//                    }
//                }

//                if (!removeWaypoint)
//                {
//                    List<PathfindWaypoint> tempConnectedWaypoints = new List<PathfindWaypoint>();

//                    foreach (PathfindWaypoint w in way.ConnectedWaypoints)
//                    {
//                        tempConnectedWaypoints.Add(w);
//                    }

//                    foreach (PathfindWaypoint waypoint in tempConnectedWaypoints)
//                    {

//                        foreach (StaticPhysicsObject staticObject in RequestStaticPhysicsObjects())
//                        {

//                            if (staticObject.BlocksMovement && staticObject.CollisionBounds.LineIntersects(new Vector2(waypoint.Position.X, waypoint.Position.Z), new Vector2(way.Position.X, way.Position.Z)))
//                            {
//                                way.RemoveConnectedWaypoint(waypoint);
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    waypointsToRemove.Enqueue(way);
//                }

//            }

//            while (waypointsToRemove.Count > 0)
//            {
//                RemoveWaypoint(waypointsToRemove.Dequeue());
//            }
            
//        }


//#region Graph Cleanup
//        public void CleanupWaypointGraph()
//        {
//            DateTime start = DateTime.Now;
//            RemoveRedundantNodes();
//            DateTime end = DateTime.Now;

//            TimeSpan taken = end - start;
//            start = DateTime.Now;
//            CleanClosedTriads();
//             end = DateTime.Now;

//             taken = end - start;
//        }

//        /// <summary>
//        /// Remove all redundant waypoints from graph
//        /// NOTE: Add version of this for single waypoint
//        /// </summary>
//        public void RemoveRedundantNodes()
//        {
//            //1. Get waypoint.
//            //2. Get its connected waypoints.
//            //3. Check connected waypoints' neighbourhood.
//            //4. If it contains no unique waypoints, remove it
          
//            List<PathfindWaypoint> points = waypoints.Values.ToList<PathfindWaypoint>();

//            for (int i = 0; i < points.Count; i++)
//            {
//                List<PathfindWaypoint> connectedWaypoints = points[i].ConnectedWaypoints;
//                for (int j = 0; j < connectedWaypoints.Count; j++)
//                {
//                    if (connectedWaypoints[j].ConnectedWaypoints.Count > 1)
//                    {
//                        if (i >= waypoints.Count)
//                        {
//                            break;
//                        }
//                        if (!ContainsUniqueWaypoints(points[i], connectedWaypoints, connectedWaypoints[j].ConnectedWaypoints))
//                        {
//                            RemoveWaypoint(connectedWaypoints[j]);
//                            points.Remove(connectedWaypoints[j]);
//                            connectedWaypoints.Remove(connectedWaypoints[j]);
                            
//                        }
//                    }
//                    if (j >= connectedWaypoints.Count)
//                    {
//                        break;
//                    }
//                }
//                if (i >= waypoints.Count)
//                {
//                    break;
//                }

//            }
           
//        }

//        public void CleanClosedTriads()
//        {
//             List<PathfindWaypoint> points = waypoints.Values.ToList<PathfindWaypoint>();

//             for (int i = 0; i < points.Count; i++)
//             {
//                   List<PathfindWaypoint> connectedWaypoints = points[i].ConnectedWaypoints;
//                   for (int j = 0; j < connectedWaypoints.Count; j++)
//                   {
//                       RemoveClosedTriadEdge(points[i], connectedWaypoints[j]);
//                   }
//             }

//        }

//        protected int CountEdges()
//        {
//            int result = 0;
//            foreach (PathfindWaypoint point in waypoints.Values)
//            {
//                result += point.ConnectedWaypoints.Count;
//            }

//            result /= 2;

//            return result;
//        }

//        /// <summary>
//        /// Checks if the neighbour's list contains unique waypoints to the current's list (excluding current)
//        /// </summary>
//        /// <param name="ignoreCurrent"></param>
//        /// <param name="currentNeighbours"></param>
//        /// <param name="neighbourNeighbours"></param>
//        /// <returns></returns>
//        protected bool ContainsUniqueWaypoints(PathfindWaypoint ignoreCurrent, List<PathfindWaypoint> currentNeighbours, List<PathfindWaypoint> neighbourNeighbours)
//        {
//            bool result = false;
//         /*   if (neighbourNeighbours.Count == 1)
//            {
//                result = true;
//            }
//            else*/
//            {

//                neighbourNeighbours.Remove(ignoreCurrent);

//                foreach (PathfindWaypoint point in neighbourNeighbours)
//                {
//                    if (!currentNeighbours.Contains(point))
//                    {
//                        result = true;
//                        break;
//                    }
//                }
//            }

//            return result;
//        }

//        protected bool HasIdenticalNeighbours(PathfindWaypoint ignoreCurrent, List<PathfindWaypoint> currentNeighbours, List<PathfindWaypoint> neighbourNeighbours)
//        {
//            bool result = true;
            
//            if (neighbourNeighbours.Count == currentNeighbours.Count)
//            {
//                neighbourNeighbours.Remove(ignoreCurrent);
//                foreach (PathfindWaypoint point in neighbourNeighbours)
//                {
//                    if (!currentNeighbours.Contains(point))
//                    {
//                        result = false;
//                        break;
//                    }
//                }
//            }
//            else
//            {
//                result = false;
//            }

//            return result;
//        }

//        //Removes the longest edge from a closed triad
//        protected void RemoveClosedTriadEdge(PathfindWaypoint current, PathfindWaypoint neighbour)
//        {
//            double distanceA = current.GetDistanceTo(neighbour.Position);

//            foreach (PathfindWaypoint point in neighbour.ConnectedWaypoints)
//            {
//                if (current.ConnectedWaypoints.Contains(point))
//                {
//                    double distanceB = current.GetDistanceTo(point.Position);
//                    double distanceC = neighbour.GetDistanceTo(point.Position);
//                  //If they share a waypoint, get the distances, and remove the longest
//                    double longest = Math.Max(Math.Max(distanceA, distanceB), distanceC);

//                    if (longest == distanceA)
//                    {
//                        current.RemoveConnectedWaypoint(neighbour);
//                    }

//                    else if (longest == distanceB)
//                    {
//                        current.RemoveConnectedWaypoint(point);
//                    }

//                    else
//                    {
//                        neighbour.RemoveConnectedWaypoint(point);
//                    }
              

//                }
//            }
           
//        }



//#endregion

//        protected void RemoveWaypoint(PathfindWaypoint waypoint)
//        {
//            foreach (PathfindWaypoint point in waypoint.ConnectedWaypoints)
//            {
//                point.RemoveConnectedWaypoint(waypoint);
//            }
//            waypoints.Remove(waypoint.Position);
//        }

//        protected void RemoveWaypoint(Vector3 pointName)
//        {
//            if (waypoints.ContainsKey(pointName))
//            {
//                RemoveWaypoint(waypoints[pointName]);
//            }

//        }


//        public List<PathfindWaypoint> Waypoints
//        {
//            get
//            {
//                return waypoints.Values.ToList<PathfindWaypoint>();
//            }
//        }

//    }
//}
