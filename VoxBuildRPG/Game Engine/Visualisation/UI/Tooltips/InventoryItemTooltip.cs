using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.EnvironmentState;
using VoxelRPGGame.GameEngine.InventorySystem;
using VoxelRPGGame.GameEngine.UI.Inventory;
using VoxelRPGGame.MenuSystem;

namespace VoxelRPGGame.GameEngine.UI.Tooltips
{
    public class InventoryItemTooltip:Tooltip
    {
        protected InventoryItem _inventoryItem;
        protected Rectangle _boundingBox;

        protected Texture2D _background;
        protected Vector2 _padding = new Vector2(5, 5);
        protected Vector2 _namePositionOffset;

        protected Vector2 _mousePosition;

        public override Vector2 Position
        {
            get
            {
                return new Vector2(_boundingBox.X,_boundingBox.Y);
            }

        }

        public override float Width
        {
            get
            {
                return _boundingBox.Width;
            }

        }
        public override float Height
        {
            get
            {
                return _boundingBox.Height;
            }

        }

        public InventoryItemTooltip(InventoryItem owner, Vector2 position)
        {
            _boundingBox = new Rectangle((int)position.X, (int)position.Y, 100, 300);
            _inventoryItem = owner;
            isVisible = true;
        }

        public override void Update(GameTime theTime, GameState state)
        {
        }

        //  public virtual void HandleInput(GameTime gameTime, InputState input) { }

        //Handles user input. This is separate to Update, as a screen can still be updated even if it cannot handle input  
        public override void HandleInput(GameTime gameTime, InputState input, GameState state)
        {
          
        }


        public override void Draw(SpriteBatch Batch, GameState state)
        {
            // Batch.Draw(_icon, _boundingBox, Color.White);
            BuildTooltip();

            Batch.Draw(_background, Position, Color.White);

            Color nameColor = Color.White;

            switch(_inventoryItem.Rarity)
            {
                case Rarity.Epic:
                    {
                        nameColor = Color.Orange;
                        break;
                    }
            }

            Batch.DrawString(ScreenManager.GetInstance().DefaultMenuFont, "" + _inventoryItem.Name, Position + _namePositionOffset, nameColor);

        }


        protected void BuildTooltip()
        {
            _mousePosition = new Vector2(GameWorldControlState.GetInstance().InputState.CurrentMouseState.X, GameWorldControlState.GetInstance().InputState.CurrentMouseState.Y);

            float height = 0;
            float width = 0;//Use the width of the longest string
            _namePositionOffset = new Vector2(_padding.X, _padding.Y);
            Vector2 nameMeasurement = ScreenManager.GetInstance().DefaultMenuFont.MeasureString("" + _inventoryItem.Name);
            width = nameMeasurement.X;
            height += nameMeasurement.Y;



            _boundingBox = new Rectangle(0, 0, (int)(width + _padding.X * 2), (int)(height+_padding.Y * 2));


            _background = new Texture2D(ScreenManager.GetInstance().GraphicsDevice, _boundingBox.Width, _boundingBox.Height);
            Color[] data = new Color[_boundingBox.Width * _boundingBox.Height];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.Black;
            _background.SetData(data);

           
            //Set desired position and see if it will run offscreen

            switch(_leftRightAlignment)
            {
                case LeftRightAlignment.Left:
                    {
                        _boundingBox.X = (int)((_mousePosition.X - 5) - _boundingBox.Width);
                        

                        if(_boundingBox.X<ScreenManager.GetInstance().GraphicsDevice.Viewport.X)//If tool tip starts off left of screen
                        {
                            _boundingBox.X = (int)(_mousePosition.X + 5);
                        }

                        break;
                    }
                case LeftRightAlignment.Right:
                    {
                        _boundingBox.X = (int)(_mousePosition.X + 5);

                        if (_boundingBox.X+_boundingBox.Width > ScreenManager.GetInstance().GraphicsDevice.Viewport.Width)//If tool tip ends off right of screen
                        {
                            _boundingBox.X = (int)((_mousePosition.X - 5) - _boundingBox.Width);
                        }

                        break;
                    }
            }

            switch (_upDownAlignment)
            {
                case UpDownAlignment.Up:
                    {
                        _boundingBox.Y = (int)((_mousePosition.Y - 5) - _boundingBox.Height);


                        if (_boundingBox.Y < ScreenManager.GetInstance().GraphicsDevice.Viewport.Y)//If tool tip starts off top of screen
                        {
                            _boundingBox.Y = (int)(_mousePosition.Y + 5);
                        }
                        break;
  
                    }
                case UpDownAlignment.Down:
                    {
                        _boundingBox.Y = (int)(_mousePosition.Y + 5);

                        if (_boundingBox.Y + _boundingBox.Height > ScreenManager.GetInstance().GraphicsDevice.Viewport.Height)//If tool tip ends off bottom of screen
                        {
                            _boundingBox.Y = (int)((_mousePosition.Y - 5) - _boundingBox.Height);
                        }

                        break;
                    }
            }

        }
    }
}
