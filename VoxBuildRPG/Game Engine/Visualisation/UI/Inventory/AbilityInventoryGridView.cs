using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxelRPGGame.GameEngine.EnvironmentState;
using Microsoft.Xna.Framework.Input;
using VoxelRPGGame.GameEngine.InventorySystem;
namespace VoxelRPGGame.GameEngine.UI.Inventory
{
    public class AbilityInventoryGridView: InventoryGridView
    {
        /// <summary>
        /// Used to limit the amount of items from the inventory to be shown onscreen i.e. for when the player has
        /// two tools equipped, the first tool will have 10 abilities, but only want to give access to the first 5
        /// </summary>
        protected int _restrictViewTo=-1;
        protected bool _isSecondaryHand = false;

          public AbilityInventoryGridView(InventorySystem.IInventory inventoryModel, int numCols, Vector2 positionRelative,Vector2 parentPosition,bool isSecondaryHand)
            : base(inventoryModel, numCols, positionRelative, parentPosition)
        {
            isActive = true;
            _isSecondaryHand = isSecondaryHand;

           
            //Update View on creation
            OnInventoryModelUpdate();
        }

          public AbilityInventoryGridView(InventorySystem.IInventory inventoryModel, int numCols, Vector2 positionRelative,Vector2 parentPosition, int restrictViewTo, bool isSecondaryHand)
              : base(inventoryModel, numCols, positionRelative,parentPosition)
        {
            _restrictViewTo = restrictViewTo;
            _isSecondaryHand = isSecondaryHand;

            
            //Update View on creation
            OnInventoryModelUpdate();
        }

        public override float Width
        {
            get
            {
                float result = 0;

                if (_restrictViewTo >= 0 && _inventoryGrid[_restrictViewTo - 1, 0] != null)
                {
                    //Width is the x position of the right-most slot-position.X 
                    //Width is dimension [0] of array
                    result = _inventoryGrid[_restrictViewTo - 1, 0].Width + _inventoryGrid[_restrictViewTo - 1, 0].Position.X - Position.X;
                }

                else
                {
                    result = base.Width;
                }
                return result;
            }
        }

        public override float Height
        {
            get
            {
                float result = 0;

                if (_restrictViewTo >= 0 && _inventoryGrid[0, _restrictViewTo - 1] != null)
                {
                    //Width is the x position of the right-most slot-position.X 
                    //Width is dimension [0] of array
                    result = _inventoryGrid[0, _restrictViewTo - 1].Height + _inventoryGrid[0, _restrictViewTo - 1].Position.Y - Position.Y;
                }
                else
                {
                    result = base.Height;
                }
                return result;
            }
        }

        /// <summary>
        /// Restricts the number of slots that are displayed to the amount specified
        /// </summary>
        /// <param name="amount"></param>
        public void RestrictDisplayTo(int amount)
        {
            _restrictViewTo = amount;
        }

        public override void HandleInput(GameTime gameTime, InputState input, GameState state)
        {
            int numAccessed = 0;
            bool allAccessed = false;
            for (int i = 0; i < _inventoryGrid.GetLength(0); i++)
            {
                for (int j = 0; j < _inventoryGrid.GetLength(1); j++)
                {
                    if (_restrictViewTo >= 0 && numAccessed >= _restrictViewTo)
                    {
                        allAccessed = true;
                        break;
                    }
                    if (_inventoryGrid[i, j] != null)
                    {
                        _inventoryGrid[i, j].HandleInput(gameTime, input, state);
                        numAccessed++;
                    }

                  
                }
                if(allAccessed)
                {
                    break;
                }
            }
            

        }

        public override void Update(GameTime theTime, GameState state)
        {
            int numAccessed = 0;
            bool allAccessed = false;
            for (int i = 0; i < _inventoryGrid.GetLength(0); i++)
            {
                for (int j = 0; j < _inventoryGrid.GetLength(1); j++)
                {
                    if (_restrictViewTo >= 0 && numAccessed >= _restrictViewTo)
                    {
                        allAccessed = true;
                        break;
                    }
                    if (_inventoryGrid[i, j] != null)
                    {
                        _inventoryGrid[i, j].Update(theTime, state);
                        numAccessed++;
                    }

                  
                }
                if (allAccessed)
                {
                    break;
                }
            }

        }

        public override void Draw(SpriteBatch Batch, GameState state)
        {
            int activeAbility = -1;
            if(_inventoryModel is ToolAbilityInventory)
            {
                if (((ToolAbilityInventory)_inventoryModel).ActiveAbility != null)
                {
                    activeAbility = (int)((ToolAbilityInventory)_inventoryModel).ActiveAbility;
                }
            }

            int numAccessed = 0;
            bool allAccessed = false;
            for (int i = 0; i < _inventoryGrid.GetLength(0); i++)
            {
                for (int j = 0; j < _inventoryGrid.GetLength(1); j++)
                {
                    if (_restrictViewTo >= 0 && numAccessed >= _restrictViewTo)
                    {
                        allAccessed = true;
                        break;
                    }
                    if (_inventoryGrid[i, j] != null)
                    {
                        if (activeAbility>=0&&i==activeAbility)
                        {
                             ((AbilityInventorySlot)_inventoryGrid[i, j]).IsActiveAbility=true;
                        }
                        _inventoryGrid[i, j].Draw(Batch, state);
                        numAccessed++;
                    }

                  
                }
                if (allAccessed)
                {
                    break;
                }
            }
        }

        protected override void OnInventoryModelUpdate()
        {
            Dictionary<InventoryItem, int[]> currentItems = CurrentItemsInView();

            int numRows = 0;

            //If inventory size changes, need to reinitialize the grid with a different number of slots
           
            if (_inventoryModel.HasMaxCapacity)
            {
                numRows = _inventoryModel.MaxCapacity / _numCols;

                if (_inventoryModel.MaxCapacity % _numCols > 0)
                {
                    numRows += 1;
                }
            }

            InventorySlot[,] updatedGrid = InitializeGrid(_numCols, numRows); //InitializeGrid(_inventoryGrid.GetLength(0),_inventoryGrid.GetLength(1));

            int currentPosition = 0;

            int slotNumber = 1;
            if(_isSecondaryHand)//Primary hand is 1-5/1-10, second hand is always 6-10
            {
                slotNumber = 6;
            }
            foreach (InventoryItem item in _inventoryModel.Items)
            {
                if (item != null)
                {
                    //1.If item has desired position (i.e. has been moved) add it to that position
                    if (desiredPositions.ContainsKey(item))
                    {
                        int posX = desiredPositions[item][0];
                        int posY = desiredPositions[item][1];

                        //1.1 If updated grid does not have item in desired position
                        if (!updatedGrid[posX, posY].ContainsItem())
                        {
                            updatedGrid[posX, posY] = new AbilityInventorySlot(_inventoryGrid[posX, posY].PositionAbsolute-_positionAbsolute,_positionAbsolute, item, this, slotNumber);
                        }
                        //1.2 If updated grid does have item in desired position
                        else
                        {
                            int[] pos = GetFirstAvailablePosition(updatedGrid);

                            posX = pos[0];
                            posY = pos[1];
                            updatedGrid[posX, posY] = new AbilityInventorySlot(_inventoryGrid[posX, posY].PositionAbsolute - _positionAbsolute, _positionAbsolute, item, this, slotNumber);
                        }

                        //If item was already in the view, remove it, as it has been moved
                        if (currentItems.ContainsKey(item))
                        {
                            currentItems.Remove(item);
                        }
                        //Remove from update list as it has been added
                        desiredPositions.Remove(item);

                    }
                    //2.Item is already in view and hasn't changed view
                    else if (currentItems.ContainsKey(item))
                    {
                        int posX = currentItems[item][0];
                        int posY = currentItems[item][1];
                        //2.1 If updated grid does not have item in desired position
                        if (!updatedGrid[posX, posY].ContainsItem())
                        {
                            updatedGrid[posX, posY] = new AbilityInventorySlot(_inventoryGrid[posX, posY].PositionAbsolute - _positionAbsolute, _positionAbsolute, item, this, slotNumber);
                        }
                        //2.2 If updated grid does have item in desired position
                        else
                        {
                            int[] pos = GetFirstAvailablePosition(updatedGrid);

                            posX = pos[0];
                            posY = pos[1];
                            updatedGrid[posX, posY] = new AbilityInventorySlot(_inventoryGrid[posX, posY].PositionAbsolute - _positionAbsolute, _positionAbsolute, item, this, slotNumber);
                        }
                        //Remove from update list as it has been added
                        currentItems.Remove(item);
                    }
                    //3.Item not already in view, and does not have desired position - add at first available
                    else
                    {
                      //  int[] pos = GetFirstAvailablePosition(updatedGrid);

                        int posX = currentPosition;
                        int posY = 0;
                        updatedGrid[posX, posY] = new AbilityInventorySlot(_inventoryGrid[posX, posY].PositionAbsolute - _positionAbsolute, _positionAbsolute, item, this, slotNumber);
                    }
                }
                currentPosition++;
                slotNumber++;
            }
            //Clear all desired positions once inventory updated
            desiredPositions.Clear();
            //Update inventory
            _inventoryGrid = updatedGrid;


        }

        protected virtual InventorySlot[,] InitializeGrid(int cols, int rows)
        {
            int slotsAdded = 0;
            bool allSlotsAdded = false;
            InventorySlot[,] result = new InventorySlot[cols, rows];
            for (int j = 0; j < result.GetLength(1); j++)
            {
                int slotNumber = 1;
                if (_isSecondaryHand)//Primary hand is 1-5/1-10, second hand is always 6-10
                {
                    slotNumber = 6;
                }
                for (int i = 0; i < result.GetLength(0); i++)
                {
                    InventorySlot InventorySlot = new AbilityInventorySlot(new Vector2(_positionAbsolute.X + (45 * i), _positionAbsolute.Y + (45 * j))-_positionAbsolute,_positionAbsolute, this, slotNumber);
                    InventorySlot.IsVisible = true;
                    InventorySlot.IsActive = true;
                    InventorySlot.HasFocus = true;
                    result[i, j] = InventorySlot;
                    slotsAdded++;
                    if (slotsAdded >= _inventoryModel.MaxCapacity)
                    {
                        allSlotsAdded = true;
                        break;
                    }
                    slotNumber++;
                }
                if (allSlotsAdded)
                {
                    break;
                }
            }

            return result;
        }
    }
}
