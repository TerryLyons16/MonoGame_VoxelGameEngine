using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.EnvironmentState;
using VoxelRPGGame.GameEngine.InventorySystem;
using VoxelRPGGame.MenuSystem;

namespace VoxelRPGGame.GameEngine.UI.Inventory
{
    public class AbilityInventorySlot :InventorySlot
    {

        protected int _slotNumber;
        protected bool _isActiveAbility = false;

        public AbilityInventorySlot(Vector2 TEMPposition, InventoryView owner, int number):base(TEMPposition,owner)
        {
            _slotNumber = number;
            IsActive = true;
        }
        public AbilityInventorySlot(Vector2 TEMPposition, InventoryItem item, InventoryView owner, int number)
            : base(TEMPposition, item, owner)
        {
            _slotNumber = number;
        }

        public override void Update(GameTime theTime, GameState state)
        {
            _isActiveAbility = false;
            base.Update(theTime, state);
        }

        public override void Draw(SpriteBatch Batch, GameState state)
        {
            if (_isActiveAbility)
            {
                TEMPCOLOR = Color.Orange;
            }

            else
            {
                TEMPCOLOR = Color.White;
            }


            base.Draw(Batch, state);
            Vector2 slotNumberMeasurements = ScreenManager.GetInstance().DefaultMenuFont.MeasureString("" + _slotNumber);
            Batch.DrawString(ScreenManager.GetInstance().DefaultMenuFont, "" + _slotNumber, PositionAbsolute + new Vector2((Width/2)-slotNumberMeasurements.X/2,Height), Color.White);
        }

        public bool IsActiveAbility
        {
            set
            {
                _isActiveAbility = value;
            }
            get
            {
                return _isActiveAbility;
            }
        }
    }
}
