using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.EnvironmentState;
using VoxelRPGGame.GameEngine.InventorySystem;
using VoxelRPGGame.GameEngine.InventorySystem.Trade;
using VoxelRPGGame.GameEngine.UI.UIComponents;
using VoxelRPGGame.MenuSystem;
using VoxelRPGGame.MenuSystem.MenuElements;

namespace VoxelRPGGame.GameEngine.UI.Inventory
{
    public enum TradeType
    {
        Buy,
        Sell
    }
    public class ShopInventoryListItem : InventoryListItem
    {
        public delegate void OnRequestTrade(ITradeableItem item, int desiredQuantity);
        public event OnRequestTrade OnRequestTradeEvent; 


        TradeType _tradeType;

        protected QuantitySelector _quantitySelector;
        protected TextElement _tradeButton;

        protected int _tradeQuantity = 1;
        protected float _totalPrice = 0;

        protected Color priceColor = Color.White;

        public ShopInventoryListItem(Vector2 positionAbsolute, InventoryView owner, InventoryItem item, TradeType tradeType)
            : base(positionAbsolute, owner, item)
        {
            _tradeType = tradeType;
            _boundingBox = new Rectangle((int)_positionAbsolute.X, (int)_positionAbsolute.Y, 200, 50);
            isActive = true;

            if(InventoryItem.IsStackable)
            {
                _quantitySelector = new QuantitySelector(new Vector2(Position.X + 250, Position.Y), 1);
                _quantitySelector.MinQuantity = 0;
                _quantitySelector.MaxQuantity = InventoryItem.Stock;
                _quantitySelector.OnQuantityChangedEvent += QuantityChanged;
            }

            _tradeButton = new TextElement("" + _tradeType, ScreenManager.GetInstance().DefaultMenuFont, ScreenManager.GetInstance().DefaultMenuFont);
            _tradeButton.Position =  new Vector2(Position.X +375, Position.Y);
            _tradeButton.IsActive = true;
            _tradeButton.OnClickEvent += RequestTrade;

        }

        public void Update(GameTime theTime, GameState state, ITradeInventory buyerInventory)
        {
            Update(theTime, state);

            if(_totalPrice>buyerInventory.Currency)
            {
                priceColor = Color.Red;
            }
            else
            {
                priceColor = Color.White;
            }
        }

        public override void Update(GameTime theTime, GameState state)
        {
            _tradeButton.Update(theTime);

            if(_quantitySelector!=null)
            {
                _quantitySelector.MaxQuantity = InventoryItem.Stock;
                _quantitySelector.Update(theTime, state);
            }

            if (_tradeType == TradeType.Buy)
            {
                _totalPrice = (InventoryItem as ITradeableItem).CustomerBuyPrice * _tradeQuantity;
            }
            else if (_tradeType == TradeType.Sell)
            {
                _totalPrice = (InventoryItem as ITradeableItem).CustomerSellPrice * _tradeQuantity;
            }


            base.Update(theTime, state);
        }

        public override void HandleInput(GameTime gameTime, InputState input, GameState state)
        {
            _tradeButton.HandleInput(gameTime, input);
            if (_quantitySelector != null)
            {
                _quantitySelector.HandleInput(gameTime, input, state);
            }
            base.HandleInput(gameTime, input, state);
        }

        public override void Draw(SpriteBatch Batch, GameState state)
        {
            base.Draw(Batch, state);
            if(InventoryItem is ITradeableItem)
            {

                if (_quantitySelector != null)
                {
                    _quantitySelector.Draw(Batch, state);
                }

                Batch.DrawString(ScreenManager.GetInstance().DefaultMenuFont, _totalPrice+"", new Vector2(Position.X + 200, Position.Y), priceColor);
            }
            _tradeButton.Draw(Batch);

           
        }



        #region Event Handlers

        public void RequestTrade(MenuElement sender)
        {
            if (OnRequestTradeEvent != null)
            {
                if (InventoryItem is ITradeableItem)
                {
                    OnRequestTradeEvent((InventoryItem as ITradeableItem), _tradeQuantity);
                }
            }
        }

        public void QuantityChanged(int quantity)
        {
            _tradeQuantity = quantity;
        }
        #endregion
    }
}
