using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework;
using VoxelRPGGame.GameEngine.Physics;
using VoxelRPGGame.GameEngine.Rendering;


namespace VoxelRPGGame.GameEngine
{
    /// <summary>
    /// Any object in the game world
    /// </summary>
    public abstract class AbstractWorldObject
    {
        protected Vector3 position;
        protected Vector3 rotation;//X,Y,Z rotation in radians

        protected bool isDrawable = true;
        protected bool hasFocus = true;

        protected Model model = null;
        protected string modelFileLocation = null;
        protected Texture2D texture = null;
        protected string textureLocation;


        public abstract void HandleInput(GameTime gameTime, InputState input);
        public abstract void Update();
        public abstract void Draw(Camera camera);
        public abstract void Draw(SpriteBatch Batch, Camera camera);


        public AbstractWorldObject(string modelLocation)
        {
            modelFileLocation = modelLocation;
        }

        public virtual void DrawRotatedBoundingBox(BasicEffect effect, GraphicsDevice graphicsDevice)
        {
           
        }

        public virtual List<CollisionFace> GetCollisionFaces()
        {
            return new List<CollisionFace>();
        }

        /// <summary>
        /// Response when hit by an object
        /// </summary>
        public virtual void OnHit()
        {
        }

        /// <summary>
        /// Action when an object interacts with this object (i.e. player presses button etc)
        /// </summary>
        public virtual void OnInteract()
        {
        }
        public Model Model
        {
            set
            {
                model = value;
            }

            get
            {
                return model;
            }
        }

        public Texture2D Texture
        {
            set
            {
                texture = value;
            }
        }

        public string TextureLocation
        {
            get
            {
                return textureLocation;
            }
        }

        public string ModelFileLocation
        {
            get
            {
                return modelFileLocation;
            }
        }

        public bool IsDrawable
        {
            get
            {
                return isDrawable;
            }

            set
            {
                isDrawable = value;
            }
        }

        public bool HasFocus
        {
            get
            {
                return hasFocus;
            }
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        public Vector3 Rotation
        {
            get
            {
                return rotation;
            }

            set
            {
                rotation = value;
            }
        }
    }
}
