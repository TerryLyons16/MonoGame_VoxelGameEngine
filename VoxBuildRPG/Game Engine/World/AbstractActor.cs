using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelRPGGame.GameEngine.World
{
    /// <summary>
    /// An Actor is any world object that can move, have physics applied, or be interacted with 
    /// </summary>
    public abstract class AbstractActor: AbstractWorldObject
    {
       
        protected Vector3 velocity;
 
        protected Vector3 scale;

        public AbstractActor(string modelLocation)
            : base(modelLocation)
        {
        }

        public override void Update()
        {
            Matrix rotationMatrix = Matrix.CreateRotationY(-rotation.Y);
            Vector3 transformedReference = Vector3.Transform(velocity, rotationMatrix);
            position += transformedReference;
        }
        //  public override void Draw(Camera camera);


        public void Rotate(float rotDegreesX, float rotDegreesY, float rotDegreesZ)
        {
            rotDegreesX = rotDegreesX % 360;
                rotDegreesY = rotDegreesY % 360;
                rotDegreesZ = rotDegreesZ % 360;

                rotation = new Vector3((rotation.X + MathHelper.ToRadians(rotDegreesX)) % MathHelper.ToRadians(360),
               (rotation.Y + MathHelper.ToRadians(rotDegreesY)) % MathHelper.ToRadians(360),
               (rotation.Z + MathHelper.ToRadians(rotDegreesZ)) % MathHelper.ToRadians(360));
        }

#region Properties
        
        public Vector3 Velocity
        {
            get
            {
                return velocity;
            }

            set
            {
                velocity = value;
            }
        }
       
        public Vector3 Scale
        {
            get
            {
                return scale;
            }

            set
            {
                scale = value;
            }
        }
#endregion
    }
}
