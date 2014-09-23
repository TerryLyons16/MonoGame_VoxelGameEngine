using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using VoxelRPGGame.GameEngine.UI;
using VoxelRPGGame.MenuSystem;
using VoxelRPGGame.GameEngine.World;

namespace VoxelRPGGame.GameEngine.Rendering
{
    public class Camera
    {
      //  private Rectangle viewport;
        private Vector3 cameraPosition;
        private float rotationSpeed = 0.15f;
        private Vector3 cameraOffset = Vector3.Zero;
        private Vector3 cameraRotation = Vector3.Zero;
        private float rotationRadians = 0.0f;
        private Vector3 cameraTarget;
        private AbstractWorldObject targetObject = null;
        private Matrix cameraProjectionMatrix;
        private Matrix cameraViewMatrix;

        private float TEMP_mouseDelta = 0.0f;
        private Vector2 TEMP_mouseClickPosition = Vector2.Zero;
        private float TEMP_velJump=0;

        private bool isCameraControllable = false;//i.e does player have direct control over the camera
        private float cameraMoveSpeed = 0.25f; //speed of camera when not connected to player

        private bool isSoftLocked = false;//If a target object is softLocked, the camera can disconnect from it if the user moves the camera

        private int defaultZoom = 20, minimumZoom = 1, maximumZoom = 50/*100*/, currentZoom;

        public Camera(GraphicsDevice graphics)
        {

            currentZoom = defaultZoom;
            cameraTarget = new Vector3(0, 0, 0);

            cameraOffset = new Vector3(5 * currentZoom, 5 * currentZoom, 0);
            Matrix rotationMatrix = Matrix.CreateRotationY(rotationRadians);
            Vector3 transformedReference = Vector3.Transform(cameraOffset, rotationMatrix);
            cameraPosition = transformedReference + cameraTarget;
            //Camera setup for top-down view
            //cameraPosition = new Vector3(0, 50, 0);
            //cameraViewMatrix = Matrix.CreateLookAt(cameraPosition, cameraTarget, new Vector3(1, 0, 0));


            //cameraPosition = new Vector3((cameraTarget.X - (5 * currentZoom)), (cameraTarget.Y + (5 + (5 * currentZoom))), (cameraTarget.Z - (5 * currentZoom)));
            cameraViewMatrix = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
            cameraProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), graphics.Viewport.AspectRatio, 1.0f, 10000.0f);
        }

        public void Update(Vector3 target)
        {
            if (!isCameraControllable || isSoftLocked)
            {
                if (targetObject == null)
                {
                    cameraTarget = target;
                }
                else
                {
                    cameraTarget = targetObject.Position;
                }
            }
            //Camera update for top-down view
            //cameraPosition = new Vector3((cameraTarget.X ), (cameraTarget.Y + (5 + (5 * currentZoom))), (cameraTarget.Z));
            //cameraViewMatrix = Matrix.CreateLookAt(cameraPosition, cameraTarget, new Vector3(1, 0, 0));

         //   cameraPosition =cameraRotation+ new Vector3((cameraTarget.X - (5 * currentZoom)), (cameraTarget.Y + (5 + (5 * currentZoom))), (cameraTarget.Z - (5 * currentZoom)));
         //   cameraPosition = cameraRotation + cameraPosition;
            Matrix rotationMatrix = Matrix.CreateRotationY(-rotationRadians);
            cameraOffset = new Vector3(5 * currentZoom, 5 * currentZoom, 0);
            Vector3 transformedReference = Vector3.Transform(cameraOffset, rotationMatrix);
            cameraPosition = transformedReference + cameraTarget;
            cameraViewMatrix = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
         //   cameraPosition = new Vector3((cameraTarget.X - (5 * currentZoom)), (cameraTarget.Y + (5 + (5 * currentZoom))), (cameraTarget.Z - (5 * currentZoom)));
            //cameraProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), graphics.Viewport.AspectRatio, 1.0f, 10000.0f);
        }
        public void RotateCamera(float movement)
        {
            float mouseMovement = movement;

            float rotationDegrees = MathHelper.ToDegrees(rotationRadians);
            rotationDegrees += mouseMovement * rotationSpeed ;

         //   if ((int)rotationDegrees/360>0)
            {
                rotationDegrees = rotationDegrees % 360;
            }


            rotationRadians = MathHelper.ToRadians(rotationDegrees);
        }

        public void HandleInput(InputState input)
        {
            ButtonState currentMouseState=new ButtonState();
            ButtonState previousMouseState = new ButtonState(); ;

            if (input.CurrentMouseState.MiddleButton == ButtonState.Pressed)
            {
                currentMouseState = input.CurrentMouseState.MiddleButton;
                previousMouseState = input.PreviousMouseState.MiddleButton;
            }
            else if (input.CurrentMouseState.RightButton == ButtonState.Pressed)
            {
                currentMouseState = input.CurrentMouseState.RightButton;
                previousMouseState = input.PreviousMouseState.RightButton;
            }

            if (currentMouseState == ButtonState.Pressed)
            {
                ScreenManager.GetInstance().Game.IsMouseVisible = false;
                //Only record position on click
                if (previousMouseState == ButtonState.Released)
                {
                    TEMP_mouseClickPosition = new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                }

                TEMP_mouseDelta = input.CurrentMouseState.X - TEMP_mouseClickPosition.X;

                RotateCamera(TEMP_mouseDelta);

                if (input.CurrentMouseState.RightButton == ButtonState.Pressed)
                {
                    if (targetObject != null && targetObject is TEMPPlayerActor)
                    {
                        (targetObject as TEMPPlayerActor).Rotation = new Vector3((targetObject as TEMPPlayerActor).Rotation.X,-rotationRadians, (targetObject as TEMPPlayerActor).Rotation.Z);
                    }
                }

                Mouse.SetPosition((int)TEMP_mouseClickPosition.X, (int)TEMP_mouseClickPosition.Y);
            }
            else
            {
                ScreenManager.GetInstance().Game.IsMouseVisible = true;
            }
         


            bool trackingObject = false;

            if (targetObject != null)
            {
             /*   if (input.CurrentKeyboardState.IsKeyDown(Keys.LeftControl) || isSoftLocked)
                {
                    trackingObject = true;
                }*/
            }
            else
            {
                trackingObject = true;
            }

            if (trackingObject)
            {

                Vector3 positiveCameraDirection = cameraTarget - CameraPosition;
                Vector3 cameraMove = Vector3.Zero;

                isCameraControllable = true;




                //if (input.CurrentKeyboardState.IsKeyDown(Keys.W))
                //{
                //    cameraMove.X -= cameraMoveSpeed;
                //    if (isSoftLocked)
                //    {
                //        targetObject = null;
                //        isSoftLocked = false;
                //    }
                //}

                //if (input.CurrentKeyboardState.IsKeyDown(Keys.D))
                //{
                //    cameraMove.Z -= cameraMoveSpeed;
                //    if (isSoftLocked)
                //    {
                //        targetObject = null;
                //        isSoftLocked = false;
                //    }
                //}
                //if (input.CurrentKeyboardState.IsKeyDown(Keys.S))
                //{
                //    cameraMove.X += cameraMoveSpeed;
                //    if (isSoftLocked)
                //    {
                //        targetObject = null;
                //        isSoftLocked = false;
                //    }

                //}
                //if (input.CurrentKeyboardState.IsKeyDown(Keys.A))
                //{
                //    cameraMove.Z += cameraMoveSpeed;
                //    if (isSoftLocked)
                //    {
                //        targetObject = null;
                //        isSoftLocked = false;
                //    }
                //}

                //if (input.CurrentKeyboardState.IsKeyDown(Keys.Space)/* && input.PreviousKeyboardState.IsKeyUp(Keys.Space)*/)
                //{
                //    if (TEMP_velJump == 0)
                //    {
                //        TEMP_velJump = 0.5f;
                //    }
                //}

                //cameraTarget.Y += TEMP_velJump;
                //TEMP_velJump -= (9.8f / 200);

                //if (cameraTarget.Y < 0)
                //{
                //    cameraTarget.Y = 0;
                //    TEMP_velJump = 0;
                //}
                

                //Matrix rotationMatrix = Matrix.CreateRotationY(-rotationRadians);
                //Vector3 transformedReference = Vector3.Transform(cameraMove, rotationMatrix);
                //cameraTarget += transformedReference;

            }

            else
            {
                isCameraControllable = false;
            }


          
            float scrollWheelChange = 0;

            if (input != null)
            {
                scrollWheelChange = input.PreviousMouseState.ScrollWheelValue - input.CurrentMouseState.ScrollWheelValue;
            }


            if (scrollWheelChange < 0 || (input.CurrentKeyboardState.IsKeyDown(Keys.Add) && input.PreviousKeyboardState.IsKeyUp(Keys.Add)))
            {
                ZoomCameraIn();
            }

            else if (scrollWheelChange > 0 || (input.CurrentKeyboardState.IsKeyDown(Keys.Subtract) && input.PreviousKeyboardState.IsKeyUp(Keys.Subtract)))
            {
                ZoomCameraOut();
            }


        }


        public void ZoomCameraIn()
        {
            if (currentZoom > minimumZoom)
            {
                currentZoom--;
            }
        }

        public void ZoomCameraOut()
        {
            if (currentZoom < maximumZoom)
            {
                currentZoom++;
            }
        }

        public void Draw()
        {
            DrawCircle(cameraTarget, Color.Red);
        }

        protected void DrawCircle(Vector3 pos, Color c)
        {
            VertexPositionColor[] selectionCircle = new VertexPositionColor[100];
            for (int i = 0; i < 99; i++)
            {
                float angle = (float)(i / 100.0 * Math.PI * 2);
                selectionCircle[i].Position = new Vector3((pos.X) + (float)Math.Cos(angle) * 0.5f, pos.Y, (pos.Z) + (float)Math.Sin(angle) * 0.5f);
                selectionCircle[i].Color = c;
            }
            selectionCircle[99] = selectionCircle[0];

            ShaderManager.GetInstance().DefaultEffect.View = CameraViewMatrix;
            ShaderManager.GetInstance().DefaultEffect.Projection = CameraProjectionMatrix;
           ShaderManager.GetInstance().DefaultEffect.TextureEnabled = false;
           ShaderManager.GetInstance().DefaultEffect.VertexColorEnabled = true;
           ShaderManager.GetInstance().DefaultEffect.CurrentTechnique.Passes[0].Apply();
            ScreenManager.GetInstance().GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, selectionCircle, 0, selectionCircle.Length - 1);
        }

        public void SoftLockToObject(AbstractWorldObject target)
        {
            TargetObject = target;
            isSoftLocked = true;
        }


        public void DisconnectSoftLock(AbstractWorldObject target)
        {
            if (TargetObject == target)
            {
                cameraTarget = TargetObject.Position;
                isSoftLocked = false;
                TargetObject = null;
            }
        }

        #region Properties

        public Vector3 CameraTarget
        {
            get
            {
                return cameraTarget;
            }
        }

        public AbstractWorldObject TargetObject
        {
            get
            {
                return targetObject;
            }

            set
            {
                targetObject = value;
            }
        }

        public Matrix CameraProjectionMatrix
        {
            get
            {
                return cameraProjectionMatrix;
            }
        }

        public Matrix CameraViewMatrix
        {
            get
            {
                return cameraViewMatrix;
            }
        }


        public Vector3 CameraPosition
        {
            get
            {
                return cameraPosition;
            }
        }

        public int CameraZoom
        {
            get
            {
                return currentZoom;
            }
        }


        #endregion

    }
}
