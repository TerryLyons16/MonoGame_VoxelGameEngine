//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using VoxelRPGGame.GameEngine.Subsystems;
//using VoxelRPGGame.GameEngine.Subsystems.Resources;
//using VoxelRPGGame.GameEngine.Subsystems.AIFramework.Agents;

//using VoxelRPGGame.GameEngine.Visualisation;


//namespace VoxelRPGGame.GameEngine.Subsystems.ResourceFactories
//{
//    /// <summary>
//    /// A resource factory is any object that either consumes or generates resources (or both). 
//    /// </summary>
//    /// <typeparam name="T">Type of Resources stored</typeparam>
//    /// <typeparam name="N">Type of Rules stored</typeparam>
//    /// /// <typeparam name="A">Type of Agent stored</typeparam>
//    public abstract class AbstractResourceFactory<T/*,R,A*/>: AbstractGameComponent
//    {
//        protected List<T> localResources;
//      //  protected List<A> participatingAgents;
//        protected List<AbstractResourceRule> rules;

//        public override AbstractDrawableGameObject DrawableObject
//        {
//            get
//            {
//                AbstractDrawableGameObject result = null;

//                if (componentEntity != null)
//                {
//                    result = componentEntity.DrawableObject;
//                }

//                return result;
//            }
//        }

//    /*    protected virtual void JoinFactory(A participant)
//        {
//            if(!participatingAgents.Contains(participant))
//            {
//                participatingAgents.Add(participant);
//            }
//        }

//        protected virtual void LeaveFactory(A participant)
//        {
//            if (participatingAgents.Contains(participant))
//            {
//                participatingAgents.Remove(participant);
//            }
//        }*/
//    }
//}
