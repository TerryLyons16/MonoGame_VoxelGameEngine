using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.EnvironmentState;
using VoxelRPGGame.GameEngine.InventorySystem.Trade;
using VoxelRPGGame.MenuSystem.MenuElements;

namespace VoxelRPGGame.GameEngine.UI.Inventory.Trade
{
    public class ShopInterface: UIElement
    {
        Dictionary<TradeType, ShopInventroyListView> _tradeViews = new Dictionary<TradeType, ShopInventroyListView>();

        protected TradeType _currentView = TradeType.Buy;//Set default view to be displayed

        protected ITradeInventory _shopInventory;
        protected ITradeInventory _playerInventory;
        protected MenuElement _buyTab;
        protected MenuElement _sellTab;

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


            _buyTab = new TextElement("Buy")
            {
                IsActive = true,
                Position = new Vector2(this.Position.X, this.Position.Y)
            };
            _buyTab.OnClickEvent += SwitchToBuyMenu;

            _sellTab = new TextElement("Sell")
            {
                IsActive = true,
                Position = new Vector2(_buyTab.Position.X + _buyTab.BoundingBox.Width + 20, _buyTab.Position.Y)
            };
            _sellTab.OnClickEvent += SwitchToSellMenu;

            Vector2 listsPosition = new Vector2(_positionAbsolute.X,_positionAbsolute.Y+_buyTab.BoundingBox.Height+20);

            _tradeViews.Add(TradeType.Buy, new ShopInventroyListView(shop, player, listsPosition, TradeType.Buy));
            _tradeViews.Add(TradeType.Sell, new ShopInventroyListView(player, shop, listsPosition, TradeType.Sell));

            

        }

        public override void Update(GameTime theTime, GameState state)
        {
            if (_buyTab!=null)
            {
                _buyTab.Update(theTime);
            }
            if(_sellTab!=null)
            {
                _sellTab.Update(theTime);
            }

            foreach (KeyValuePair<TradeType, ShopInventroyListView> pair in _tradeViews)
            {
                _tradeViews[pair.Key].Update(theTime, state);
            }
        }

        //  public virtual void HandleInput(GameTime gameTime, InputState input) { }

        //Handles user input. This is separate to Update, as a screen can still be updated even if it cannot handle input  
        public override void HandleInput(GameTime gameTime, InputState input, GameState state)
        {
            if (_buyTab != null)
            {
                _buyTab.HandleInput(gameTime, input);
            }
            if (_sellTab != null)
            {
                _sellTab.HandleInput(gameTime, input);
            }

            if (_tradeViews.ContainsKey(_currentView))
            {
                _tradeViews[_currentView].HandleInput(gameTime, input, state);
            }
        }


        public override void Draw(SpriteBatch Batch, GameState state)
        {
            if (_buyTab != null)
            {
                _buyTab.Draw(Batch);
            }
            if (_sellTab != null)
            {
                _sellTab.Draw(Batch);
            }

            if(_tradeViews.ContainsKey(_currentView))
            {
                _tradeViews[_currentView].Draw(Batch, state);
            }
        }


#region Event Handlers

        public void SwitchToBuyMenu(MenuElement element)
        {
            SwitchToTradeMenu(TradeType.Buy);
        }

        public void SwitchToSellMenu(MenuElement element)
        {
            SwitchToTradeMenu(TradeType.Sell);
        }

        public void SwitchToTradeMenu(TradeType menu)
        {
            if (_tradeViews.ContainsKey(menu))
            {
                _currentView = menu;
            }
        }

#endregion


    }
}
