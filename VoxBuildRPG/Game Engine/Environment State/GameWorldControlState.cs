using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame;
using Microsoft.Xna.Framework.Input;

namespace VoxelRPGGame.GameEngine.EnvironmentState
{
    /// <summary>
    /// Singleton class that maps player input to actions and fires the action
    /// </summary>
    public class GameWorldControlState
    {

        public enum Actions
        {
            NULL,
            SelectAbilityOne,
            SelectAbilityTwo,
            SelectAbilityThree,
            SelectAbilityFour,
            SelectAbilityFive,
            SelectAbilitySix,
            SelectAbilitySeven,
            SelectAbilityEight,
            SelectAbilityNine,
            SelectAbilityTen,
            PrimaryHandClickAbility,
            SecondaryHandClickAbility
        }

        private static GameWorldControlState controlState = null;

        protected InputState _inputState;

        public InputState InputState
        {
            get
            {
                return _inputState;
            }
        }

        private GameWorldControlState()
        {
           
        }

        public static GameWorldControlState GetInstance()
        {
            if (controlState == null)
            {
                controlState = new GameWorldControlState();
            }
            return controlState;
        }

        public void HandleInput(InputState inputState)
        {
            _inputState = inputState;
            List<Actions> selectedActions = new List<Actions>();
            bool abilitySelected = false;

            //NOTE: THis will be moved to a mapping dictionary
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.D1) && inputState.PreviousKeyboardState.IsKeyUp(Keys.D1))
            {
                selectedActions.Add(Actions.SelectAbilityOne);
            }
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.D2) && inputState.PreviousKeyboardState.IsKeyUp(Keys.D2))
            {
                selectedActions.Add(Actions.SelectAbilityTwo);
            }
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.D3) && inputState.PreviousKeyboardState.IsKeyUp(Keys.D3))
            {
                selectedActions.Add(Actions.SelectAbilityThree);
            }
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.D4) && inputState.PreviousKeyboardState.IsKeyUp(Keys.D4))
            {
                selectedActions.Add(Actions.SelectAbilityFour);
            }
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.D5) && inputState.PreviousKeyboardState.IsKeyUp(Keys.D5))
            {
                selectedActions.Add(Actions.SelectAbilityFive);
            }
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.D6) && inputState.PreviousKeyboardState.IsKeyUp(Keys.D6))
            {
                selectedActions.Add(Actions.SelectAbilitySix);
            }
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.D7) && inputState.PreviousKeyboardState.IsKeyUp(Keys.D7))
            {
                selectedActions.Add(Actions.SelectAbilitySeven);
            }
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.D8) && inputState.PreviousKeyboardState.IsKeyUp(Keys.D8))
            {
                selectedActions.Add(Actions.SelectAbilityEight);
            }
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.D9) && inputState.PreviousKeyboardState.IsKeyUp(Keys.D9))
            {
                selectedActions.Add(Actions.SelectAbilityNine);
            }
            if (inputState.CurrentKeyboardState.IsKeyDown(Keys.D0) && inputState.PreviousKeyboardState.IsKeyUp(Keys.D0))
            {
                selectedActions.Add(Actions.SelectAbilityTen);
            }

            if (inputState.CurrentMouseState.LeftButton == ButtonState.Released && inputState.PreviousMouseState.LeftButton == ButtonState.Pressed)
            {
                //NOTE: Will need to determine if mouse is not over UI before firing event
                selectedActions.Add(Actions.PrimaryHandClickAbility);
            }

            if (inputState.CurrentMouseState.RightButton == ButtonState.Released && inputState.PreviousMouseState.RightButton == ButtonState.Pressed)
            {
                //NOTE: Will need to determine if mouse is not over UI before firing event i.e. if over UI, fire a UILeftClickEvent instead etc...
                selectedActions.Add(Actions.SecondaryHandClickAbility);
            }


            foreach (Actions action in selectedActions)
            {
                //Ability selection actions are mutually exclusive - can only select 1 per frame
                if (!abilitySelected)
                {
                    if (action == Actions.SelectAbilityOne)
                    {
                        OnAbilitySelected(1);
                        abilitySelected = true;
                    }

                    else if (action == Actions.SelectAbilityTwo)
                    {
                        OnAbilitySelected(2);
                        abilitySelected = true;
                    }
                    else if (action == Actions.SelectAbilityThree)
                    {
                        OnAbilitySelected(3);
                        abilitySelected = true;
                    }
                    else if (action == Actions.SelectAbilityFour)
                    {
                        OnAbilitySelected(4);
                        abilitySelected = true;
                    }
                    else if (action == Actions.SelectAbilityFive)
                    {
                        OnAbilitySelected(5);
                        abilitySelected = true;
                    }
                    else if (action == Actions.SelectAbilitySix)
                    {
                        OnAbilitySelected(6);
                        abilitySelected = true;
                    }
                    else if (action == Actions.SelectAbilitySeven)
                    {
                        OnAbilitySelected(7);
                        abilitySelected = true;
                    }
                    else if (action == Actions.SelectAbilityEight)
                    {
                        OnAbilitySelected(8);
                        abilitySelected = true;
                    }
                    else if (action == Actions.SelectAbilityNine)
                    {
                        OnAbilitySelected(9);
                        abilitySelected = true;
                    }
                    else if (action == Actions.SelectAbilityTen)
                    {
                        OnAbilitySelected(10);
                        abilitySelected = true;
                    }

                    //...

                    if (action == Actions.PrimaryHandClickAbility)
                    {
                        OnPrimaryHandAbilityClick(inputState);
                    }

                    if (action == Actions.SecondaryHandClickAbility)
                    {
                        OnSecondaryHandAbilityClick(inputState);
                    }
                }
            }

        }





#region Events
          public delegate void SelectAbility(int abilityPosition);
          public event SelectAbility SelectAbilityEvent;
 
        public void OnAbilitySelected(int abilityNo)
          {
            if(SelectAbilityEvent!=null)
            {
                SelectAbilityEvent(abilityNo);
            }
          }

        public delegate void PrimaryHandAbilityOnClick(InputState input);
        public event PrimaryHandAbilityOnClick PrimaryHandAbilityOnClickEvent;

        public void OnPrimaryHandAbilityClick(InputState input)
        {
            if (PrimaryHandAbilityOnClickEvent != null)
            {
                PrimaryHandAbilityOnClickEvent(input);
            }
        }

        public delegate void SecondaryHandAbilityOnClick(InputState input);
        public event SecondaryHandAbilityOnClick SecondaryHandAbilityOnClickEvent;

        public void OnSecondaryHandAbilityClick(InputState input)
        {
            if (SecondaryHandAbilityOnClickEvent != null)
            {
                SecondaryHandAbilityOnClickEvent(input);
            }
        }

#endregion
    }
}
