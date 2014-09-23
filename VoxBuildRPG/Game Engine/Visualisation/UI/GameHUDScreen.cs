using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using VoxelRPGGame.MenuSystem;
using VoxelRPGGame.MenuSystem.MenuElements;

using VoxelRPGGame.MenuSystem.Screens;

using VoxelRPGGame.GameEngine.EnvironmentState;
using VoxelRPGGame.GameEngine.UI.Inventory;
using VoxelRPGGame.GameEngine.InventorySystem.Tools;
using VoxelRPGGame.GameEngine.UI.Tooltips;
using VoxelRPGGame.GameEngine.InventorySystem;

namespace VoxelRPGGame.GameEngine.UI
{
    /// <summary>
    /// Class that handles the rendering of all 2D elements of the game state
    /// NOTE: Will need a layout class to define the HUD elements for a particular game
    /// </summary>
    public class GameHUDScreen: AbstractRendererScreen
    {
        protected LinkedList<UIElement> _UIElements;
        private static GameHUDScreen _hudScreen = null;


        private string TickText = "";
           
        private float elapsedTime = 0.0f;
        private int ups = 0;

        VoxelRPGGame.GameEngine.InventorySystem.Inventory tempInventory;
        VoxelRPGGame.GameEngine.InventorySystem.Inventory tempInventory2;

       
        public static GameHUDScreen GetInstance()
        {
            if(_hudScreen==null)
            {
                _hudScreen = new GameHUDScreen();
               
            }

            return _hudScreen;
        }

        public static void Dispose()
        {
            _hudScreen = null;
        }

        /// <summary>
        /// The object must be created before initializing any screen elements, so that when the elements
        /// call get instance to attach to the hud's event handlers, they get a valid object reference and
        /// dont'attempt to create a new object which would reult in a stack overflow
        /// </summary>
        private GameHUDScreen()
        {
            hasFocus = true;
            _UIElements = new LinkedList<UIElement>();
        }


        public void InitializeElements()
        {

            tempInventory = new InventorySystem.Inventory(22);
            tempInventory.AddItem(new InventorySystem.InventoryItem("Textures\\UI\\TestIcon"));

            tempInventory.AddItem(new InventorySystem.Abilities.Build.BlockInventoryItem(GameEngine.World.Voxels.MaterialType.Dirt,"Test",5));
            tempInventory.AddItem(new InventorySystem.Abilities.Build.RemoveBlockAbility());

            InventorySystem.Tools.ToolInventoryItem namedHammer = new InventorySystem.Tools.ToolInventoryItem(InventorySystem.Tools.ToolType.Hammer, "Textures\\UI\\TestIconTool", EquipConstraint.Secondary);
            namedHammer.Rename("Hammer of Doom");
            namedHammer.Rarity = Rarity.Epic;
            tempInventory.AddItem(namedHammer);
            tempInventory.AddItem(new InventorySystem.Tools.ToolInventoryItem(InventorySystem.Tools.ToolType.Hammer, "Textures\\UI\\TestIconTool", EquipConstraint.None));
            tempInventory.AddItem(new InventorySystem.Abilities.Build.BlockInventoryItem(GameEngine.World.Voxels.MaterialType.Dirt, "Test", 50));
            tempInventory2 = new InventorySystem.Inventory(43);
           // tempInventory2.AddItem(new InventorySystem.InventoryItem("Textures\\UI\\TestIcon"));
            
            _UIElements.AddLast(new InventoryGridView(tempInventory, 4, new Vector2(ScreenManager.GetInstance().GraphicsDevice.Viewport.Width - 200, ScreenManager.GetInstance().GraphicsDevice.Viewport.Height- 300)));
        //    _UIElements.Add(new InventoryGridView(tempInventory2, 6, new Vector2(200,300)));

            InventorySystem.CharacterInventory characterInventory = new InventorySystem.CharacterInventory();
            _UIElements.AddLast(new PlayerToolInventoryView(characterInventory.EquippedItems, new Vector2((ScreenManager.GetInstance().GraphicsDevice.Viewport.Width / 2) - 270, ScreenManager.GetInstance().GraphicsDevice.Viewport.Height - 60)));

          /*  TickText = new TextElement("");
            TickText.Alpha = 1.0f;
            TickText.Font = ScreenManager.GetInstance().DefaultMenuFont;
            TickText.Position = new Vector2(ScreenManager.GetInstance().GraphicsDevice.Viewport.Width-100, 20);*/
        }

        public override void Update(GameTime theTime, GameState state)
        {

           /* foreach (AbstractDrawable2DGameObject gameObject in state.Get2DRenderState())
            {
                gameObject.Update();
            }*/

            //Use a temp List in case list is modified during update
            List<UIElement> tempElements = new List<UIElement>();
            foreach (UIElement element in _UIElements)
            {
                tempElements.Add(element);
            }

            foreach (UIElement e in tempElements)
            {
                if (e.IsActive)
                {
                    //NOTE: May not need access to the game state
                    e.Update(theTime,state);
                }
            }

        //    TickText = "Simulation Tick: " + Engine.SimulationTick+"\nCycle No.: "+Engine.CycleNo+"\nExperiment Length: "+0;
           

        }

        public override void HandleInput(GameTime gameTime, InputState input, GameState state)
        {

               //Use a temp List in case list is modified during update
            List<UIElement> tempElements = new List<UIElement>();
            foreach (UIElement element in _UIElements)
            {
                tempElements.Add(element);
            }

            foreach (UIElement e in tempElements)
            {
            
                if (e.HasFocus)
                {
                    //NOTE: May not need access to the game state, input handling will be centralised
                    e.HandleInput(gameTime, input, state);
                }
            }



            if (input.CurrentKeyboardState.IsKeyDown(Keys.O) && input.PreviousKeyboardState.IsKeyUp(Keys.O))
            {
                tempInventory.AddItem(new InventorySystem.InventoryItem("Textures\\UI\\TestIcon"));
            }
        /*    if (input.CurrentKeyboardState.IsKeyDown(Keys.P) && input.PreviousKeyboardState.IsKeyUp(Keys.P))
            {
                _UIElements.Add(new InventoryGridView(tempInventory, 4, new Vector2(400, 200)));
            }*/
            //NOTE: Need to determine if UI has focus in order to handle input i.e. if mouse is over UI elements

            /*foreach (AbstractDrawable2DGameObject gameObject in state.Get2DRenderState())
            {
                if (gameObject.HasFocus)
                {
                    gameObject.HandleInput(gameTime, input);
                }
            }*/


        }


        public override void Draw(SpriteBatch Batch, GameState state)
        {
            Vector2 pos= new Vector2(ScreenManager.GetInstance().GraphicsDevice.Viewport.Width, 0);
            float currY = 20;
            
           // Batch.DrawString(ScreenManager.GetInstance().DefaultMenuFont, TickText, new Vector2(pos.X - 220, currY), Color.White);

            //Use a temp List in case list is modified during update
            List<UIElement> tempElements = new List<UIElement>();
            foreach (UIElement element in _UIElements)
            {
                tempElements.Add(element);
            }

            foreach (UIElement e in tempElements)
            {
                if (e.IsVisible)
                {
                    //NOTE: May not need access to the game state
                    e.Draw(Batch, state);
                }
            }

          /*  foreach (AbstractDrawable2DGameObject gameObject in state.Get2DRenderState())
            {
                if (gameObject.IsDrawable)
                {
                    gameObject.Draw(Batch);
                }
            }
          */
        }


        #region Event Handlers

        /// <summary>
        /// Move a UI element to the front of the screen
        /// </summary>
        /// <param name="element"></param>
        public void MoveToFront(UIElement element)
        {
            if(_UIElements.Contains(element))
            {
                _UIElements.Remove(element);
                _UIElements.AddLast(element);
            }
        }

        public void AddTooltip(UIElement requestor,Tooltip tooltip)
        {
            //NOTE: Use requestor to determine where on screen to draw tooltip so it doesn't clip


            if(!_UIElements.Contains(tooltip))
            {
                _UIElements.AddLast(tooltip);
            }
        }

        public void RemoveTooltip( Tooltip tooltip)
        {
          
            if (_UIElements.Contains(tooltip))
            {
                _UIElements.Remove(tooltip);
            }
        }


        #endregion
    }
}
