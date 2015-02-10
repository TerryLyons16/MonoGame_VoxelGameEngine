using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using VoxelRPGGame.MenuSystem.MenuElements;

namespace VoxelRPGGame.MenuSystem.Screens
{
    public class MenuScreen: AbstractScreen
    {
        protected SpriteFont font;
        protected string menuTitle;
        private float elementStartingPosition = 220;
        protected float screenAlpha = 1.0f;


        bool keyboardHasFocus = false;
        bool isMousePersistant = false;//Determines whether the mouse will go to sleep or not
        //int keyboardScrollDirection = 0;//-1 is Up, 1 is Down

        int mouseSleepTimer = 0, mouseDeactivateTime = 40;//SleepTimer counts how many updates the mouse has been asleep for, DeactivateTime specifies how long it must be asleep before it deactivates


        protected List<MenuElement> elements = new List<MenuElement>();
        private int selectedElement = -1;

        public MenuScreen(string title)
        {
            menuTitle = title;
        }

        public override void Update(GameTime theTime)
        {
            foreach (MenuElement element in elements)
            {
                if (element.IsActive)
                {
                    element.Update(theTime);
                }
            }


        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input.CurrentMouseState == input.PreviousMouseState)
            {
                mouseSleepTimer++;
            }

            else
            {
                mouseSleepTimer = 0;
                ScreenManager.GetInstance().Game.IsMouseVisible = true;
            }

            if (input.CurrentKeyboardState.IsKeyDown(Keys.Down)&&input.PreviousKeyboardState.IsKeyUp(Keys.Down))
            {
                selectedElement++;
                if (selectedElement >= elements.Count)
                {
                    selectedElement = 0;
                }

                //keyboardScrollDirection = 1;
                keyboardHasFocus = true;

            }

            else if (input.CurrentKeyboardState.IsKeyDown(Keys.Up) && input.PreviousKeyboardState.IsKeyUp(Keys.Up))
            {
                selectedElement--;
                if (selectedElement < 0)
                {
                    selectedElement = elements.Count - 1;
                }

                //keyboardScrollDirection= - 1;
                keyboardHasFocus = true;

            }

           /* else
            {
                keyboardHasFocus = false;
            }*/

        /*    if ((new Point(input.CurrentMouseState.X, input.CurrentMouseState.Y) != (new Point(input.PreviousMouseState.X, input.PreviousMouseState.Y))))
            {
                keyboardHasFocus = true;
            }
            */
            

            foreach (MenuElement element in elements)
            {
                if (element.HasFocus)
                {
                    element.HandleInput(gameTime, input);
                }

                if (element.MouseOver)
                {
                    keyboardHasFocus = false;
                    selectedElement = elements.IndexOf(element);
                }
            }

            if (!isMousePersistant)
            {
                if (mouseSleepTimer == mouseDeactivateTime)
                {
                    ScreenManager.GetInstance().Game.IsMouseVisible = false;
                    keyboardHasFocus = true;
                    selectedElement = -1;
                }
            }
            if (keyboardHasFocus)
            {
                setSelectedElement();
            }
        }

        //  public virtual void HandleInput(GameTime gameTime, InputState input) { }

        //Handles user input. This is separate to Update, as a screen can still be updated even if it cannot handle input  
      /*  public virtual void HandleInput(GameTime gameTime, KeyboardState previous, KeyboardState current, MouseState previousMouse, MouseState currentMouse)
        {
            foreach (MenuElement element in elements)
            {
                if(element.IsActive)
                {
                    element.HandleInput(gameTime, previous, current, previousMouse,currentMouse);
                }
            }
        }*/

        public override void Draw(SpriteBatch Batch)
        {

        }


        protected void setSelectedElement()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (i != selectedElement)
                {
                    elements[i].IsHighlighted = false;
                }

                else
                {
                    elements[i].setHighlighted();
                }
            }

            
        }

        protected void setElementPositions_Centre(int spacing)
        {
            float midpoint;
            float startingPosition;
            if (elements.Count % 2 != 0)
            {
                //Odd number of elements => middle element will be drawn across centre of screen
                midpoint = (ScreenManager.GetInstance().GraphicsDeviceManager.GraphicsDevice.Viewport.Height / 2) + (elements[0].BoundingBox.Height / 2);
                startingPosition=midpoint-(elements[0].BoundingBox.Height+10)*(elements.Count-1);

            }

            else
            {
                midpoint = ((ScreenManager.GetInstance().GraphicsDeviceManager.GraphicsDevice.Viewport.Height / 2) + 5) + (elements[0].BoundingBox.Height);
                startingPosition = midpoint - (elements[0].BoundingBox.Height + 10) * (elements.Count-1);
            }

           
            float currentPosition = startingPosition;
            foreach (MenuElement element in elements)
            {
                element.Position = new Vector2(ScreenManager.GetInstance().GraphicsDeviceManager.GraphicsDevice.Viewport.Width / 2, currentPosition);

                currentPosition += element.BoundingBox.Height + spacing;
            }
        }

        protected void setElementPositions_Left(int paddingLeft,int spacing)
        {
            float distFromEdge = paddingLeft;
            float midpoint;
            float startingPosition;
            if (elements.Count % 2 != 0)
            {
                //Odd number of elements => middle element will be drawn across centre of screen
                midpoint = (ScreenManager.GetInstance().GraphicsDeviceManager.GraphicsDevice.Viewport.Height / 2) + (elements[0].BoundingBox.Height / 2);
                startingPosition = midpoint - (elements[0].BoundingBox.Height + 10) * (elements.Count - 1);

            }

            else
            {
                midpoint = ((ScreenManager.GetInstance().GraphicsDeviceManager.GraphicsDevice.Viewport.Height / 2) + 5) + (elements[0].BoundingBox.Height);
                startingPosition = midpoint - (elements[0].BoundingBox.Height + 10) * (elements.Count - 1);
            }


            float currentPosition = startingPosition;
            foreach (MenuElement element in elements)
            {
                element.Position = new Vector2(distFromEdge + element.BoundingBox.Width/2, currentPosition);

                currentPosition += element.BoundingBox.Height + spacing;
            }
        }


        public void OnTransitionIn()
        {
            foreach (MenuElement e in elements)
            {
                if (e is TextElement)
                {
                    (e as TextElement).Alpha = 0;//Ensure that elements are always fading in from invisible
                    (e as TextElement).FadeIn();
                }
            }
        }

        public void OnTransitionOut()
        {
            foreach (MenuElement e in elements)
            {
                if (e is TextElement)
                {
                    (e as TextElement).FadeOut();
                }
            }
        }

#region Event Handlers
        public void isHighlighted(object sender, EventArgs e)
        {
            if(sender is MenuElement)
            {
                if (elements.Contains((MenuElement)sender))
                {
                    if (keyboardHasFocus)
                    {
                        selectedElement = elements.IndexOf((MenuElement)sender);
                    }
                }
            }
        }

        public void isUnHighlighted(object sender, EventArgs e)
        {
            if (sender is MenuElement)
            {
                if (elements.Contains((MenuElement)sender))
                {
                    if (keyboardHasFocus)
                    {
                        // selectedElement = elements.IndexOf((MenuElement)sender);

                        //selectedElement = -1;
                        ((MenuElement)sender).IsHighlighted = false;
                    }
                }
            }
        }

#endregion


    }
}
