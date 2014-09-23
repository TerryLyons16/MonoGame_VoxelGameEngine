//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Microsoft.Xna.Framework;
//using VoxelRPGGame.GameEngine.Visualisation;
//using VoxelRPGGame.GameEngine.Subsystems;
//using VoxelRPGGame.GameEngine.Subsystems.AIFramework.Agents;
//using VoxelRPGGame.GameEngine.Subsystems.PhysicsSystem;

//namespace VoxelRPGGame.GameEngine.Subsystems.ResourceFactories
//{
//    public abstract class AbstractResourceFactoryEntity:AbstractEntity
//    {


//        protected List<AbstractAgentEntity> occupants;

//        public AbstractResourceFactoryEntity(double perceptionLimitRadius,Vector3 position,double boundsWidth, double boundsLength) 
//        {
//            physicsObject = new BuildingPhysicsObject(position,0,2.5,2.5, boundsWidth, boundsLength, true, perceptionLimitRadius);
//            occupants = new List<AbstractAgentEntity>();

//            SetPhysicsObjectEntityRequest();
//        }

//        public override void UpdateLogic()
//        {

//            List<AbstractEntity> nearbyObjects = physicsObject.GetPerceivedEntities();
//          //  occupants.Clear();
//            if (nearbyObjects != null)
//            {
//                foreach (AbstractEntity a in nearbyObjects)
//                {
//                    if (a is AbstractAgentEntity)
//                    {
//                        if (IsInside(a as AbstractAgentEntity))
//                        {
//                            //Ensure that agentEntity is stored in list

//                            //NOTE: Removed this, as it could cause agent to incorrectly become a participant
//                            //if it had entered the factory through a collision detection error
//                           // JoinOccupants(a as AbstractAgentEntity);
//                            (a as AbstractAgentEntity).IsInFactory(this); 
//                        }
//                        else if (EnteredEntranceArea(a as AbstractAgentEntity))
//                        {
//                            //tell agent it has entered the factory
//                            (a as AbstractAgentEntity).EnteredFactoryEntrance(this);

//                            JoinOccupants(a as AbstractAgentEntity);
//                        }
//                        else if (HasExited(a as AbstractAgentEntity))
//                        {
//                            //tell agent it has exited the factory
//                            //  (a as AbstractAgentEntity).OnFactoryExited();

//                            LeaveOccupants(a as AbstractAgentEntity);
//                            //Remove agentEntity from list
//                        }
//                        else if (IsAtEntrance(a as AbstractAgentEntity))
//                        {
//                            //Tell the agent it is outside, but at the entrance of the building
//                            (a as AbstractAgentEntity).IsAtFactoryEntrance(this);
//                            LeaveOccupants(a as AbstractAgentEntity);
//                        }
//                        else if (HasExitedEntrance(a as AbstractAgentEntity))
//                        {
//                            //Tell the agent it no longer is no longer at a factory
//                            (a as AbstractAgentEntity).ExitedFactoryEntrance();
//                            LeaveOccupants(a as AbstractAgentEntity);
//                        }
//                    }
//                }
//            }
               
//        }

//        /// <summary>
//        /// Send Agent-based rules to all participants
//        /// </summary>
//        /// <param name="rules"></param>
//        public abstract void SendRules(List<AbstractResourceRule> rules);


//      /*  private void SetBounds(double width, double length)
//        {
//            double x = position.X - width / 2;
//            double y = position.Z - length / 2;

//            bounds = new Rectangle((int)x, (int)y, (int)width, (int)length);
//        }
//        */

//        public void JoinOccupants(AbstractAgentEntity agent)
//        {
//            if (!occupants.Contains(agent))
//            {
//                occupants.Add(agent);
//            }
//        }

//        public void LeaveOccupants(AbstractAgentEntity agent)
//        {
//            if(occupants.Contains(agent))
//            {
//                occupants.Remove(agent);
//            }
//        }

//        /// <summary>
//        /// Place the agent inside the bounds of the factory
//        /// </summary>
//        /// <param name="agent"></param>
//        public void EnterFactory(AbstractAgentEntity agent)
//        {
//            agent.Position = physicsObject.Position;
//            //Disable its collisions while inside
          
//        }
//        /// <summary>
//        /// Place the agent outside the bounds of the factory
//        /// </summary>
//        /// <param name="agent"></param>
//        public void LeaveFactory(AbstractAgentEntity agent)
//        {
//            agent.Position = ((BuildingPhysicsObject)physicsObject).EntranceLocation;
//            agent.OnFactoryExited();
//        }


//#region Interaction Listeners
//        protected bool EnteredEntranceArea(AbstractAgentEntity agentObject)
//        {

//            return ((BuildingPhysicsObject)physicsObject).EnteredEntranceArea((MovingPhysicsObject)agentObject.PhysicsObject);
//        }
        
//        /// <summary>
//        /// The agent is standing at the entrance (i.e. has exited but not moved)
//        /// </summary>
//        /// <param name="agentObject"></param>
//        /// <returns></returns>
//        protected bool IsAtEntrance(AbstractAgentEntity agentObject)
//        {

//            return ((BuildingPhysicsObject)physicsObject).IsAtEntrance((MovingPhysicsObject)agentObject.PhysicsObject);
//        }


//        protected bool HasExited(AbstractAgentEntity agentObject)
//        {

//            return ((BuildingPhysicsObject)physicsObject).HasExited((MovingPhysicsObject)agentObject.PhysicsObject);
//        }

//        protected bool HasExitedEntrance(AbstractAgentEntity agentObject)
//        {
//            return ((BuildingPhysicsObject)physicsObject).HasExitedEntrance((MovingPhysicsObject)agentObject.PhysicsObject);
//        }

//        protected bool IsInside(AbstractAgentEntity agentObject)
//        {
//            return ((StaticPhysicsObject)physicsObject).IsInside(agentObject.PhysicsObject);
//        }
//#endregion

//        /// <summary>
//        /// Gets the location of the factory's entrance
//        /// </summary>
//        public Vector3 Location
//        {
//            get
//            {
//                return ((BuildingPhysicsObject)physicsObject).EntranceLocation;
//            }
//        }

//        public List<AbstractAgentEntity> CurrentOccupants
//        {
//            get
//            {
//                return occupants;
//            }
//        }
//    }
//}
