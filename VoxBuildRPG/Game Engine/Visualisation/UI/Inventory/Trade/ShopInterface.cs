using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.EnvironmentState;
using VoxelRPGGame.GameEngine.InventorySystem.Trade;

namespace VoxelRPGGame.GameEngine.UI.Inventory.Trade
{
    public class ShopInterface: UIElement
    {
        Dictionary<TradeType, ShopInventroyListView> _tradeViews = new Dictionary<TradeType, ShopInventroyListView>();

        protected TradeType _currentView = TradeType.Buy;//Set default view to be displayed

        protected ITradeInventory _shopInventory;
        protected ITradeInventory _playerInventory;

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
                float largestWidth = 0;
                foreach(KeyValuePair<TradeType, ShopInventroyListView> pair in _tradeViews)
                {
                    if(_tradeViews[pair.Key].Width>largestWidth)
                    {
                        largestWidth = _tradeViews[pair.Key].Width;
                    }
                }

                return largestWidth;
            }

        }
        public override float Height
        {
            get
            {
                float largestHeight = 0;
                foreach (KeyValuePair<TradeType, ShopInventroyListView> pair in _tradeViews)
                {
                    if (_tradeViews[pair.Key].Height > largestHeight)
                    {
                        largestHeight = _tradeViews[pair.Key].Height;
                    }
                }
                return largestHeight;
            }

        }

        public ShopInterface(Vector2 positionAbsolute, ITradeInventory shop , ITradeInventory player)
        {
            _shopInventory = shop;
            _playerInventory = player;
            isActive = true;
            _positionAbsolute = positionAbsolute;

            _tradeViews.Add(TradeType.Buy, new ShopInventroyListView(shop, player, _positionAbsolute, TradeType.Buy));
            _tradeViews.Add(TradeType.Sell, new ShopInventroyListView(player, shop, _positionAbsolute, TradeType.Sell));

        }

        public override void Update(GameTime theTime, GameState state)
        {
            foreach (KeyValuePair<TradeType, ShopInventroyListView> pair in _tradeViews)
            {
                _tradeViews[pair.Key].Update(theTime, state);
            }
        }

        //  public virtual void HandleInput(GameTime gameTime, InputState input) { }

        //Handles user input. This is separate to Update, as a screen can still be updated even if it cannot handle input  
        public override void HandleInput(GameTime gameTime, InputState input, GameState state)
        {

            if (input.CurrentKeyboardState.IsKeyDown(Keys.Q) && input.PreviousKeyboardState.IsKeyUp(Keys.Q))
            {
                if(_currentView==TradeType.Sell)
                {
                    _currentView = TradeType.Buy;
                }
                else if(_currentView==TradeType.Buy)
                {
                    _currentView = TradeType.Sell;
                }
            }
            

            if (_tradeViews.ContainsKey(_currentView))
            {
                _tradeViews[_currentView].HandleInput(gameTime, input, state);
            }
        }


        public override void Draw(SpriteBatch Batch, GameState state)
        {
            if(_tradeViews.ContainsKey(_currentView))
            {
                _tradeViews[_currentView].Draw(Batch, state);
            }
        }


    }
}
