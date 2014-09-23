using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using VoxelRPGGame.GameEngine.Subsystems;
using VoxelRPGGame.GameEngine.UI;
using VoxelRPGGame.GameEngine.Subsystems.PhysicsSystem;

using Microsoft.Xna.Framework;
using VoxelRPGGame.GameEngine.World.Voxels;
using Microsoft.Xna.Framework.Graphics;
using VoxelRPGGame.GameEngine.World;
using VoxelRPGGame.GameEngine.World.Textures;
using VoxelRPGGame.MenuSystem;
using Microsoft.Xna.Framework.Content;
using VoxelRPGGame.GameEngine.Rendering;

namespace VoxelRPGGame.GameEngine.EnvironmentState
{
    /// <summary>
    /// Stores all components in the simulation environment.
    /// Can be queried by components to return their perception of the environment.
    /// 
    /// NOTE: May make this a singleton
    /// NOTE: May also store garphics information, such as menus, to be passed to the renderer
    /// </summary>
    public class GameState
    {
        Camera _camera;

        List<AbstractBlock> TEMPwireframes = new List<AbstractBlock>();

        /// <summary>
        /// Global Random number generator for use for world generation
        /// </summary>
        protected static Random worldGeneratorRNG;

        /// <summary>
        /// Global Random number generator for use for engine components
        /// </summary>
        protected static Random engineRNG;

        protected List<AbstractWorldObject> worldObjects = new List<AbstractWorldObject>();

       
      //  protected List<AbstractEntity> xAxisList;
     //   protected List<AbstractEntity> activeList;
  
        protected CollisionSquare worldBounds;

        protected bool isLoading;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="worldSeed">Static seed for the worlds's random number generator</param>
        public GameState(int ? worldSeed,int ? engineRNGSeed, int ? graphRNGSeed)
        {
            _camera = new Camera(ScreenManager.GetInstance().GraphicsDevice);
         
            if (worldSeed != null)
            {
                worldGeneratorRNG = new Random((int)worldSeed);
            }

            else
            {
                worldGeneratorRNG = new Random();
            }


            if (engineRNGSeed != null)
            {
                engineRNG = new Random((int)engineRNGSeed);
            }

            else
            {
                engineRNG = new Random();
            }

         
            initialise(/*200, 200*/);
        }

        public void initialise(/*double worldWidth, double worldHeight*/)
        {
            TEMPPlayerActor player = new TEMPPlayerActor("Models\\OrangeBox");
           //player.Position = Vector3.Zero;
            AddWorldObject(player);
            _camera.TargetObject = player;
       //     worldBounds = new CollisionSquare(Vector3.Zero, worldWidth, worldHeight);
            isLoading = true;

            foreach (AbstractWorldObject gameObject in worldObjects)
            {
                Load3DModel(gameObject);
            }
        }



#region WorldObject Addition and Deletion

        public void AddWorldObject(AbstractWorldObject obj)
        {
            worldObjects.Add(obj);

        }

        public void RemoveWorldObject(AbstractWorldObject obj)
        {
            worldObjects.Remove(obj);
        }

#endregion

        public void LoadComplete()
        {
            isLoading = false;
        }



        /// <summary>
        /// Assembles the gameState and performs collision detection
        /// </summary>
        public void Update()
        {
            foreach (AbstractWorldObject gameObject in worldObjects)
            {
                gameObject.Update();

                if (gameObject is TEMPPlayerActor)
                {
                    TEMPwireframes = new List<AbstractBlock>();
                    foreach (AbstractBlock b in ChunkManager.GetInstance().GetNearbyBlocks(gameObject.Position, 4, 3, 4))
                    {
                        BoundingBox box = new BoundingBox(b.TEMPMinPoint, b.TEMPMaxPoint);

                        /*  if ((gameObject as TEMPPlayerActor).BoundingBox.Intersects(box))
                          {
                              b.wireFrameColour = Color.Red;
                          }
                          else
                          {
                              b.wireFrameColour = new Color(40,40,40);
                          }*/

                        TEMPwireframes.Add(b);


                    }
                    (gameObject as TEMPPlayerActor).Update(TEMPwireframes);

                }
            }

            _camera.Update(Vector3.Zero);


               // SweepAndPrune();
         
        }

        public void HandleInput(GameTime gameTime,InputState input)
        {
            _camera.HandleInput(input);
            input.WorldCamera = _camera;
         


            foreach (AbstractWorldObject gameObject in worldObjects)
            {
                if (gameObject.HasFocus)
                {
                    gameObject.HandleInput(gameTime, input);
                }
            }
          
        }

        //Collision detection for perception squares
        //public void SweepAndPrune()
        //{
        //   List<AbstractEntity> xAxisList = new List<AbstractEntity>();
        //   List<AbstractEntity> activeList = new List<AbstractEntity>();

        //    //1. Sort entitys' list based on X coordinate
       
        //    //Order objects in increasing order along the (2D, not global) X-axis
        //    //NOTE: OrderBy may be a O(N log N) quicksort
        //    xAxisList = gameComponentEntities.OrderBy(o => o.PhysicsObject.PerceptionBounds.BottomX).ToList<AbstractEntity>();

        //    Queue<AbstractEntity> xAxisQueue = new Queue<AbstractEntity>();

        //    foreach (AbstractEntity e in xAxisList)
        //    {
        //        xAxisQueue.Enqueue(e);
        //    }
        //    //3. Add first object to active list
        //   // activeList.Add(xAxisQueue.Dequeue());


        //    while (xAxisQueue.Count > 0)
        //    {
        //        List<AbstractEntity> toRemove = new List<AbstractEntity>();

        //        AbstractEntity current = xAxisQueue.Dequeue();
        //        current.PhysicsObject.ClearPerceivedEntities();//Clear the perception of the object before updating

        //        for (int i = 0; i < activeList.Count; i++)
        //        {
                

        //            //Check if current overlaps with activeList elements on XAxis
        //            if (current.PhysicsObject.PerceptionBounds.BottomX <= activeList[i].PhysicsObject.PerceptionBounds.TopX)
        //            {
        //                //if overlap, check for full collision between perception squares
        //                if (activeList[i].PhysicsObject.PerceptionBounds.IsColliding(current.PhysicsObject.PerceptionBounds))
        //                {
        //                    //collision

        //                    //Check if each perceives the other, if so report to object
        //                    if(current.PhysicsObject.PerceptionBounds.IsColliding(activeList[i].PhysicsObject.CollisionBounds))
        //                    {
        //                        current.PhysicsObject.AddPerceivedObject(activeList[i].PhysicsObject);
        //                    }

        //                    if (activeList[i].PhysicsObject.PerceptionBounds.IsColliding(current.PhysicsObject.CollisionBounds))
        //                    {
        //                        activeList[i].PhysicsObject.AddPerceivedObject(current.PhysicsObject);

        //                    }

                         
        //                }

        //            }
        //            //Else, remove activeList element
        //            else
        //            {
                        
        //                toRemove.Add(activeList[i]);
        //            }
                
        //        }

        //        foreach (AbstractEntity e in toRemove)
        //        {
        //            activeList.Remove(e);
        //        }

        //        //Add current to activeList
        //        activeList.Add(current);

        //    }

        //}

        //public void SweepAndPruneStaticObjects(AbstractEntity entity)
        //{
        //    List<AbstractEntity> xAxisList = new List<AbstractEntity>();
        //    List<AbstractEntity> activeList = new List<AbstractEntity>();

        //    foreach (AbstractEntity e in gameComponentEntities)
        //    {
        //        xAxisList.Add(e);

        //    }

        //    xAxisList.Add(entity);

        //    //1. Sort entitys' list based on X coordinate

        //    //Order objects in increasing order along the (2D, not global) X-axis
        //    //NOTE: OrderBy may be a O(N log N) quicksort
        //    xAxisList = xAxisList.OrderBy(o => o.PhysicsObject.PerceptionBounds.BottomX).ToList<AbstractEntity>();

        //    Queue<AbstractEntity> xAxisQueue = new Queue<AbstractEntity>();

        //    foreach (AbstractEntity e in xAxisList)
        //    {
        //        xAxisQueue.Enqueue(e);
        //    }
        //    //3. Add first object to active list
        //    // activeList.Add(xAxisQueue.Dequeue());


        //    while (xAxisQueue.Count > 0)
        //    {

        //        List<AbstractEntity> toRemove = new List<AbstractEntity>();

        //        AbstractEntity current = xAxisQueue.Dequeue();
        //        current.PhysicsObject.ClearPerceivedEntities();//Clear the perception of the object before updating



        //        for (int i = 0; i < activeList.Count; i++)
        //        {

        //            //Check if current overlaps with activeList elements on XAxis
        //            if (current.PhysicsObject.PerceptionBounds.BottomX <= activeList[i].PhysicsObject.PerceptionBounds.TopX)
        //            {
        //                //if overlap, check for full collision between perception squares
        //                if (activeList[i].PhysicsObject.PerceptionBounds.IsColliding(current.PhysicsObject.PerceptionBounds))
        //                {
        //                    //collision

        //                        current.PhysicsObject.AddPerceivedObject(activeList[i].PhysicsObject);
        //                        activeList[i].PhysicsObject.AddPerceivedObject(current.PhysicsObject);




        //                }

        //            }
        //            //Else, remove activeList element
        //            else
        //            {

        //                toRemove.Add(activeList[i]);
        //            }

        //        }

        //        foreach (AbstractEntity e in toRemove)
        //        {
        //            activeList.Remove(e);
        //        }

        //        //Add current to activeList
        //        activeList.Add(current);

        //    }

        //}
   

        #region Loading

        public bool Load3DModel(AbstractWorldObject gameObject)
        {
            Boolean result = false;
            if (gameObject.ModelFileLocation != null)
            {
                try
                {
                    gameObject.Model = ScreenManager.GetInstance().ContentManager.Load<Model>(gameObject.ModelFileLocation);
                    result = true;
                }

                catch (ContentLoadException e)
                {
                    result = false;
                }
            }

            if (gameObject.TextureLocation != null)
            {
                try
                {
                    gameObject.Texture = ScreenManager.GetInstance().ContentManager.Load<Texture2D>(gameObject.TextureLocation);
                    result = true;
                }
                catch (ContentLoadException e)
                {
                    result = false;
                }
            }
            return result;
        }



        #endregion
#region Accessors

        public Camera ActiveCamera
        {
            get
            {
                return _camera;
            }

        }

     /*   public CollisionSquare GetWorldBounds()
        {
            return worldBounds;
        }*/
        /// <summary>
        /// Gets all worldObjects (but not chunks) for the renderer
        /// </summary>
        /// <returns></returns>
        public List<AbstractWorldObject> GetRenderState()
        {
            return worldObjects;
        }
        
        //NOTE: Very inefficient for rendering, do not use per frame
        public List<AbstractBlock> GetBlocks()
        {
            List<AbstractBlock> result = new List<AbstractBlock>();

            foreach(Chunk c in ChunkManager.GetInstance().Chunks)
            {
                result.AddRange(c.GetAllBlocks());
            }

            return result;
        }

        public TextureName GetTEMPTexture()
        {
            return ChunkManager.GetInstance().TEMPTexture;
        }

        public List<Chunk> GetChunks()
        {
            return ChunkManager.GetInstance().Chunks;
        }
        /// <summary>
        /// Used for World Generation
        /// </summary>
        public static Random WorldRandomNumberGenerator
        {
            get
            {
                return worldGeneratorRNG;
            }
        }

        /// <summary>
        /// Used for random functions in engine
        /// </summary>
        public static Random EngineRandomNumberGenerator
        {
            get
            {
                return engineRNG;
            }
        }
#endregion
        public void SnapToCamera(AbstractWorldObject cameraFocusObject)
        {
            _camera.SoftLockToObject(cameraFocusObject);
        }
    }
}
