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
    public class InventoryGridView : InventoryView
    {
        protected InventorySlot[,] _inventoryGrid;
        


        protected Rectangle _boundingBox;
        protected Vector2 _positionRelative;//position relative to whatever inventory it is in
        protected Vector2 _positionAbsolute;//Position on screen it is drawn at
        protected int _numCols;


        public override Vector2 Position
        {
            get
            {
                return _positionAbsolute;
            }
        }

        public override float Width
        {
            get
            {
                float result = 0;

                if (_inventoryGrid[_inventoryGrid.GetLength(0) - 1, 0] != null)
                {
                    //Width is the x position of the right-most slot-position.X 
                    //Width is dimension [0] of array
                    result = _inventoryGrid[_inventoryGrid.GetLength(0) - 1, 0].Width + _inventoryGrid[_inventoryGrid.GetLength(0) - 1, 0].Position.X - Position.X; 
                }
                return result;
            }
        }

        public override float Height
        {
            get
            {
                float result = 0;

                if (_inventoryGrid[0,_inventoryGrid.GetLength(1) - 1] != null)
                {
                    //Width is the x position of the right-most slot-position.X 
                    //Width is dimension [0] of array
                    result = _inventoryGrid[0, _inventoryGrid.GetLength(1) - 1].Height + _inventoryGrid[0, _inventoryGrid.GetLength(1) - 1].Position.Y - Position.Y;
                }
                return result;
                return result;
            }
        }


        public InventoryGridView(InventorySystem.IInventory inventoryModel,int numCols,Vector2 positionAbsolute):base(inventoryModel)
        {
            isVisible = true;
            _positionAbsolute = new Vector2(positionAbsolute.X, positionAbsolute.Y);

            hasFocus = true;

            _numCols = numCols;

            int numRows=0;

       
            if (_inventoryModel.HasMaxCapacity)
            {
                numRows = _inventoryModel.MaxCapacity / numCols;
                
                if(_inventoryModel.MaxCapacity % numCols>0)
                {
                    numRows += 1;
                }
            }

            _inventoryGrid = InitializeGrid(numCols,numRows);

            //Update View on creation
            OnInventoryModelUpdate();
        }


        public override void HandleInput(GameTime gameTime, InputState input, GameState state)
        {
            foreach (InventorySlot i in _inventoryGrid)
            {
                if (i != null)
                {
                    i.HandleInput(gameTime, input, state);
                }
            }

            //NOTE: TEMP - Input will need to be handled centrally so that if this is not visible it and its children will not
            //handle input
       /*     if (input.CurrentKeyboardState.IsKeyDown(Keys.I) && input.PreviousKeyboardState.IsKeyUp(Keys.I))
            {
                isVisible = !isVisible;
            }*/
          /*  if (input.CurrentKeyboardState.IsKeyDown(Keys.Z) && input.PreviousKeyboardState.IsKeyUp(Keys.Z))
            {
                InventorySystem.InventoryItem tmp = new InventorySystem.InventoryItem("Textures\\UI\\TestIcon");
                desiredPositions.Add(tmp,new int[]{3,3});
                _inventoryModel.AddItem(tmp);
            }*/


        }

        public override void Update(GameTime theTime, GameState state)
        {
            foreach (InventorySlot i in _inventoryGrid)
            {
                if (i != null)
                {
                    i.Update(theTime, state);
                }
            }
        }

        public override void Draw(SpriteBatch Batch, GameState state)
        {
            foreach (InventorySlot i in _inventoryGrid)
            {
                if (i != null)
                {
                    i.Draw(Batch, state);
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

            foreach(InventoryItem item in _inventoryModel.Items)
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
                            updatedGrid[posX, posY] = new InventorySlot(_inventoryGrid[posX, posY].PositionAbsolute, item, this);
                        }
                        //1.2 If updated grid does have item in desired position
                        else
                        {
                            int[] pos = GetFirstAvailablePosition(updatedGrid);

                            posX = pos[0];
                            posY = pos[1];
                            updatedGrid[posX, posY] = new InventorySlot(_inventoryGrid[posX, posY].PositionAbsolute, item, this);
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
                            updatedGrid[posX, posY] = new InventorySlot(_inventoryGrid[posX, posY].PositionAbsolute, item, this);
                        }
                        //2.2 If updated grid does have item in desired position
                        else
                        {
                            int[] pos = GetFirstAvailablePosition(updatedGrid);

                            posX = pos[0];
                            posY = pos[1];
                            updatedGrid[posX, posY] = new InventorySlot(_inventoryGrid[posX, posY].PositionAbsolute, item, this);
                        }
                        //Remove from update list as it has been added
                        currentItems.Remove(item);
                    }
                    //3.Item not already in view, and does not have desired position - add at first available
                    else
                    {
                        int[] pos = GetFirstAvailablePosition(updatedGrid);

                        int posX = pos[0];
                        int posY = pos[1];
                        updatedGrid[posX, posY] = new InventorySlot(_inventoryGrid[posX, posY].PositionAbsolute, item, this);
                    }
                }
            }
            //Clear all desired positions once inventory updated
            desiredPositions.Clear();
            //Update inventory
            _inventoryGrid = updatedGrid;


        }

        public Dictionary<InventoryItem,int[]> CurrentItemsInView()
        {
            Dictionary<InventoryItem, int[]> result = new Dictionary<InventoryItem, int[]>();

            for(int i =0;i<_inventoryGrid.GetLength(0);i++)
            {
                for(int j =0;j<_inventoryGrid.GetLength(1);j++)
                {
                    if (_inventoryGrid[i, j]!=null&&_inventoryGrid[i, j].InventoryItem != null)
                    {
                        result.Add(_inventoryGrid[i, j].InventoryItem,new int[]{i,j});
                    }
                }
            }

            return result;

        }

        public int[] GetFirstAvailablePosition(InventorySlot[,] grid)
        {
            int[] result = new int[2];

            bool resultFound = false;
            for (int i = 0; i < grid.GetLength(1); i++)
            {
                for (int j = 0; j < grid.GetLength(0); j++)
                {
                    if (grid[j,i]!=null&&!grid[j, i].ContainsItem())
                    {
                          //break from inner loop
                      result=new int[]{j,i};
                      resultFound = true;
                    break;
                    }

                }
                //Break from outer loop
                if (resultFound)
                {
                    break;
                }
            }

            return result;
        }

   /*     public override bool AddAtFirstAvailable(InventoryItemView item)
        {
            bool itemAdded = false;
            for (int i = 0; i < _inventoryGrid.GetLength(0); i++)
            {
                for (int j = 0; j < _inventoryGrid.GetLength(1); j++)
                {
                    if (_inventoryGrid[i, j]!=null&&!_inventoryGrid[i, j].ContainsItem())
                    {
                       itemAdded= _inventoryGrid[i, j].AddInventoryItem(item);
                    }
                    //break from inner loop
                    if (itemAdded)
                    {
                        break;
                    }
                }
                //Break from outer loop
                if (itemAdded)
                {
                    break;
                }
            }

            return itemAdded;
        }*/

        protected virtual InventorySlot[,] InitializeGrid(int cols, int rows )
        {
            int slotsAdded = 0;
            bool allSlotsAdded = false;
            InventorySlot[,] result = new InventorySlot[cols, rows];
            for (int j = 0; j < result.GetLength(1); j++)
            {
                for (int i = 0; i < result.GetLength(0); i++)
                {
                    InventorySlot InventorySlot = new InventorySlot(new Vector2(_positionAbsolute.X + (45 * i), _positionAbsolute.Y + (45 * j)),this);
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
                }
                if(allSlotsAdded)
                {
                    break;
                }
            }

            return result;
        }


        public override int[] GetSlotPosition(InventorySlot slot)
        {
            int[] result = null;
            bool resultFound = false;

            for (int i = 0; i < _inventoryGrid.GetLength(1); i++)
            {
                for (int j = 0; j < _inventoryGrid.GetLength(0); j++)
                {
                    if (_inventoryGrid[j, i] != null && _inventoryGrid[j, i] == slot)
                    {
                        //break from inner loop
                        result = new int[2] { j, i };
                        resultFound = true;
                        break;
                    }
                }
                //Break from outer loop
                if (resultFound)
                {
                    break;
                }
            }
            return result;
        }

        public override bool ContainsSlot(InventorySlot slot)
        {
            bool result = false;

            for (int i = 0; i < _inventoryGrid.GetLength(1); i++)
            {
                for (int j = 0; j < _inventoryGrid.GetLength(0); j++)
                {
                    if (_inventoryGrid[j, i] != null && _inventoryGrid[j, i]==slot)
                    {
                        //break from inner loop
                        result = true;
                        break;
                    }
                }
                //Break from outer loop
                if (result)
                {
                    break;
                }
            }
            return result;

        }
    }
}
