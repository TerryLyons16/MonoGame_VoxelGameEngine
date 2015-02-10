using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


using VoxelRPGGame.MenuSystem;
using VoxelRPGGame.GameEngine.Rendering;



namespace VoxelRPGGame
{

    //Class that holds the current and previous states of user input
    public class InputState
    {
        private KeyboardState currentKeyboardState, previousKeyboardState;
        private MouseState currentMouseState, previousMouseState;
        private bool isMouseVisible;
        private Camera _gameWorldCamera = null;
        //GamePad State...

        public InputState()
        {
            currentKeyboardState = new KeyboardState();
            previousKeyboardState = new KeyboardState();

            currentMouseState = new MouseState();
            previousMouseState = new MouseState();

            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();
        }


        public void Update(bool isMouseVisible)
        {
            //Only handles input if the application is currently the active application
            if (ScreenManager.GetInstance().Game.IsActive)
            {
                previousKeyboardState = currentKeyboardState;
                previousMouseState = currentMouseState;

                currentKeyboardState = Keyboard.GetState();
                currentMouseState = Mouse.GetState();

                currentMouseState = Mouse.GetState();

                this.isMouseVisible = isMouseVisible;
            }
        }

        public Ray? GetMouseRay()
        {
            Ray? ray = null;
            if (_gameWorldCamera != null)
            {
                Vector2 mousePoint = new Vector2(CurrentMouseState.X, CurrentMouseState.Y);
                //GraphicsDevice graphicsDevice = game.GraphicsDevice;

                Vector3 nearSource = new Vector3((float)mousePoint.X, (float)mousePoint.Y, 0f);
                Vector3 farSource = new Vector3((float)mousePoint.X, (float)mousePoint.Y, 1f);
                Vector3 nearPoint = ScreenManager.GetInstance().GraphicsDevice.Viewport.Unproject(nearSource, _gameWorldCamera.CameraProjectionMatrix, _gameWorldCamera.CameraViewMatrix, Matrix.Identity);
                Vector3 farPoint = ScreenManager.GetInstance().GraphicsDevice.Viewport.Unproject(farSource, _gameWorldCamera.CameraProjectionMatrix, _gameWorldCamera.CameraViewMatrix, Matrix.Identity);

                // Create a ray from the near clip plane to the far clip plane.
                Vector3 direction = farPoint - nearPoint;
                direction.Normalize();

                // Create a ray.
                 ray = new Ray(nearPoint, direction);

            }

            return ray;
        }


#region Properties

        public Camera WorldCamera
        {
            set
            {
                _gameWorldCamera = value;
            }
        }

        public KeyboardState CurrentKeyboardState
        {
            get
            {
                return currentKeyboardState;
            }
        }

        public KeyboardState PreviousKeyboardState
        {
            get
            {
                return previousKeyboardState;
            }
        }

        public MouseState CurrentMouseState
        {
            get
            {
                return currentMouseState;
            }
        }

        public MouseState PreviousMouseState
        {
            get
            {
                return previousMouseState;
            }
        }

        public bool IsMouseVisible
        {
            get
            {
                return isMouseVisible;
            }
        }

#endregion

    }
}
