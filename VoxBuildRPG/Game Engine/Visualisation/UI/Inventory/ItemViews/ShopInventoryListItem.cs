using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.EnvironmentState;
using VoxelRPGGame.GameEngine.InventorySystem;
using VoxelRPGGame.GameEngine.InventorySystem.Trade;
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

        protected TextElement _tradeButton;

        protected int _tradeQuantity = 1;

        public ShopInventoryListItem(Vector2 positionAbsolute, InventoryView owner, InventoryItem item, TradeType tradeType)
            : base(positionAbsolute, owner, item)
        {
            _tradeType = tradeType;
            _boundingBox = new Rectangle((int)_positionAbsolute.X, (int)_positionAbsolute.Y, 200, 50);
            isActive = true;

            _tradeButton = new TextElement("" + _tradeType);
            _tradeButton.Position =  new Vector2(Position.X + Width, Position.Y);
            _tradeButton.IsActive = true;
            _tradeButton.OnClickEvent += RequestTrade;

        }

        public override void Update(GameTime theTime, GameState state)
        {
            _tradeButton.Update(theTime);
            base.Update(theTime, state);
        }

        public override void HandleInput(GameTime gameTime, InputState input, GameState state)
        {
            _tradeButton.HandleInput(gameTime, input);
            base.HandleInput(gameTime, input, state);
        }

        public override void Draw(SpriteBatch Batch, GameState state)
        {
            base.Draw(Batch, state);
            Color nameColor = Color.White;
            //  Batch.Draw(_slotTexture, _boundingBox, TEMPCOLOR);

            _tradeButton.Draw(Batch);

          //  Batch.DrawString(ScreenManager.GetInstance().DefaultMenuFont, "" + _tradeType, new Vector2(Position.X + Width, Position.Y), nameColor);
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
        #endregion
    }
}
