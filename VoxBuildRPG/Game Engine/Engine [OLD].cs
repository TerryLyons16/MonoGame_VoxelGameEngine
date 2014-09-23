//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

////using VoxelRPGGame.GameEngine.SubsystemManagers.Managers;
//using VoxelRPGGame.GameEngine.Subsystems;
//using VoxelRPGGame.GameEngine.EnvironmentState;



//using VoxelRPGGame.GameEngine.UI;
//using VoxelRPGGame.MenuSystem;
//using Microsoft.Xna.Framework;
//using VoxelRPGGame.GameEngine.World.Voxels;

//namespace VoxelRPGGame.GameEngine
//{
//    /// <summary>
//    /// Class that controls the execution of all game engine components
//    /// </summary>
//    public class Engine
//    {
//        public delegate void ExitEngine();
//        public event ExitEngine OnExit;


//        private static int globalTick = 1;
//        /// <summary>
//        /// Counts every 60 ticks as a single cycle
//        /// </summary>
//        private static int cycleNo = 0;
//        private static int previousCycleNo = -1;

//        /// <summary>
//        /// Holds all the subsystem managers in the game engine, including any component managers.
//        /// Each one is called when the engine updates 
//        /// </summary>
//      //  private List<AbstractSubsystemManager> gameSubsystemManagers = new List<AbstractSubsystemManager>();

//        private GameState gameState;

//        //Also need to store a game state which contains a reference to every component in every manager
//        //Agent entity will percieve environment by querying the game state
//        //Game state will also be sent to the renderer for visualisation

//        public Engine()
//        {
//            globalTick = 1;
//            cycleNo = 0;
//            previousCycleNo = -1;



//        }

//        public override void Initialise()
//        {
//            gameState = new GameState(null, null, null);
//            gameSubsystemManagers.Add(ChunkManager.GetInstance());
//        }

//        //public void Initialise(AbstractModelMap map)
//        //{
//        //    gameState = map.LoadGameState();

//        //    //Take in a "Map", which defines all the components to be instantiated and instantiate them in the correct manager

//        //    foreach (AbstractSubsystemManager manager in map.LoadSubsystems(this))
//        //    {
//        //        AddSubsystem(manager);
//        //    }


//        //    map.LoadComponents(this);
//        //}

//        ///// <summary>
//        ///// Method to initialise the engine to be used as a map editor - components are not loaded
//        ///// </summary>
//        ///// <param name="map"></param>
//        //public void InitialiseMapEditor(AbstractModelMap map)
//        //{
//        //    gameState = map.LoadGameState();

//        //    //Take in a "Map", which defines all the components to be instantiated and instantiate them in the correct manager

//        //    foreach (AbstractSubsystemManager manager in map.LoadSubsystems(this))
//        //    {
//        //        AddSubsystem(manager);
//        //    }

//        //}
//        /// <summary>
//        /// Updates all active subsystem managers
//        /// </summary>
//        public override void Update()
//        {
            
//            if (cycleNo >= 25000)
//            {
//                IsActive = false;
//                    OnExit();
//            }

//            if (IsActive)
//            {
//                if (previousCycleNo != cycleNo)
//                {
//                    //Save cycle data

                 
//                    previousCycleNo = cycleNo;

//                }


//             //   gameState.Update();

//                //1. Update Component Managers
//                foreach (AbstractSubsystemManager manager in gameSubsystemManagers)
//                {
//                    if (manager.IsActive)
//                    {
//                        manager.Update();
//                    }
//                }


                
//                if (globalTick % 60 == 0)
//                {
//                    globalTick = 1;
//                    cycleNo++;
//                }
//                else
//                {
//                    globalTick++;
                  
//                }

//            }
//        }


        


//#region Subsystem Access
//        /// <summary>
//        /// Adds a subsystem manager to the engine.
//        /// If the manager is a component manager, also sets the component manager's event handlers
//        /// </summary>
//        /// <param name="subsystem"></param>
//     //   public void AddSubsystem(AbstractSubsystemManager subsystem)
//     //   {
//     //       gameSubsystemManagers.Add(subsystem);

//     //    /*   if (IsSubclassOfRawGeneric(typeof(AbstractComponentManager<>),subsystem.GetType()))
//     //       {
//     //           (subsystem as AbstractComponentManager<>).OnCreateComponent += OnGameComponentCreated;
//     //           (subsystem as AbstractComponentManager<>).OnDeleteComponent += OnGameComponentDeleted;
//     //       }
//     //       */
            
//     //   }
//     //   public List<AbstractSubsystemManager> SubsystemManagers
//     //   {
//     //       get
//     //       {
//     //           return gameSubsystemManagers;
//     //       }
//     //   }

//     ///*   static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
//     //   {
//     //       bool result = false;
//     //       while (toCheck != null && toCheck != typeof(object))
//     //       {
//     //           var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
//     //           if (generic == cur)
//     //           {
//     //               result= true;
//     //               break;
//     //           }
//     //           toCheck = toCheck.BaseType;
//     //       }
//     //       return result;
//     //   }*/

//#endregion

//#region Event Handlers
//        /// <summary>
//        /// NOTE: May need to add components synchronously at end of step
//        /// </summary>
//        /// <param name="component"></param>
//     /*   public bool OnGameComponentCreated(AbstractGameComponent component)
//        {
//            bool result = false;
//            if (gameState != null)
//            {
//                result=gameState.AddComponent(component);
//            }

//            return result;
//        }*/

//   /*     public bool OnGameComponentDeleted(AbstractGameComponent component)
//        {
//            bool result = false;
//            if (gameState != null)
//            {
//                result=gameState.RemoveComponent(component);
//            }
//            return result;
//        }*/

//        public void LoadComplete()
//        {
//            gameState.LoadComplete();
//            OnPlay();
//        }

//        public void OnPause()
//        {
//            IsActive = false;
//            foreach (AbstractSubsystemManager m in SubsystemManagers)
//            {
//                m.IsActive = false;
//            }
//        }


//        public void OnPlay()
//        {
//            IsActive = true;
//            foreach (AbstractSubsystemManager m in SubsystemManagers)
//            {
//                m.IsActive = true;
//            }
//        }
//#endregion

//#region Accessors

//        public GameState GetGameState()
//        {
//            return gameState;
//        }

//#endregion

//#region Properties

//        public static int SimulationTick
//        {
//            get
//            {
//                return globalTick;
//            }
//        }

//        public static int CycleNo
//        {
//            get
//            {
//                return cycleNo;
//            }
//        }

//#endregion




//    }
//}
