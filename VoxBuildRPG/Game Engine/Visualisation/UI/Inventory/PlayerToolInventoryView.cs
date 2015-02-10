using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.EnvironmentState;
using VoxelRPGGame.GameEngine.InventorySystem;

namespace VoxelRPGGame.GameEngine.UI.Inventory
{
    public class PlayerToolInventoryView : UIElement
    {
        protected InventoryGridView _primaryHand;
        protected AbilityInventoryGridView _primaryHandAbilities;
        protected InventoryGridView _secondaryHand;
        protected AbilityInventoryGridView _secondaryHandAbilities;

        private Vector2 _positionAbsolute;//Position on screen it is drawn at

        protected EquippedItemsInventory _equippedItemsInventory;//NOTE: This may be changed to a CharacterInventory, with the view including the backpack inventories

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
                float result = 0.0f;

                if(_secondaryHand!=null)
                {
                    result = (_secondaryHand.Position.X + _secondaryHand.Width) - _positionAbsolute.X;
                }
                return result;
            }
        }
        public override float Height
        {
            get
            {
                float result = 0.0f;

                if (_secondaryHand != null)
                {
                    result = (_secondaryHand.Position.Y + _secondaryHand.Height) - _positionAbsolute.Y;
                }
                return result;
            }
        }

        public PlayerToolInventoryView(EquippedItemsInventory playerEquipmentInventory, Vector2 positionRelative,Vector2 parentPosition)
        {
            isVisible = true;
            IsActive = true;
            _positionRelative = positionRelative;
            _positionAbsolute = positionRelative + parentPosition;

            hasFocus = true;

            _equippedItemsInventory = playerEquipmentInventory;
            _primaryHand = new EquippedToolInventoryGridView(_equippedItemsInventory.PrimaryHandInventory, 1, Vector2.Zero, _positionAbsolute);
            _equippedItemsInventory.PrimaryHandInventory.ToolAddedEvent += OnToolAdded;
            _equippedItemsInventory.PrimaryHandInventory.ToolRemovedEvent += OnToolRemoved;

            ((EquippedToolInventory)_primaryHand.InventoryModel).AbilitiesChangedEvent += OnPrimaryToolAbilitiesChanged;
            _primaryHandAbilities = new AbilityInventoryGridView(_equippedItemsInventory.PrimaryHandInventory.Abilities, _equippedItemsInventory.PrimaryHandInventory.Abilities.MaxCapacity, (_primaryHand.Position + new Vector2(_primaryHand.Width + 15, 0)) - _positionAbsolute, _positionAbsolute, 5, false);

            _secondaryHandAbilities = new AbilityInventoryGridView(_equippedItemsInventory.SecondaryHandInventory.Abilities, _equippedItemsInventory.SecondaryHandInventory.Abilities.MaxCapacity, (_primaryHandAbilities.Position + new Vector2(_primaryHandAbilities.Width + 5, 0)) - _positionAbsolute, _positionAbsolute, 5, true);
            _secondaryHand = new EquippedToolInventoryGridView(playerEquipmentInventory.SecondaryHandInventory, 1, (_secondaryHandAbilities.Position + new Vector2(_secondaryHandAbilities.Width + 15, 0)) - _positionAbsolute, _positionAbsolute);
            ((EquippedToolInventory)_secondaryHand.InventoryModel).AbilitiesChangedEvent += OnSecondaryToolAbilitiesChanged;

            _equippedItemsInventory.SecondaryHandInventory.ToolAddedEvent += OnToolAdded;
            _equippedItemsInventory.SecondaryHandInventory.ToolRemovedEvent += OnToolRemoved;
        }


        public override void HandleInput(GameTime gameTime, InputState input, GameState state)
        {
            if (_primaryHand != null)
            {
                _primaryHand.HandleInput(gameTime, input, state);
            }

            if (_primaryHandAbilities != null)
            {
                _primaryHandAbilities.HandleInput(gameTime, input, state);
            }

            if (_secondaryHand != null)
            {
                _secondaryHand.HandleInput(gameTime, input, state);
            }

            if (_secondaryHandAbilities != null)
            {
                _secondaryHandAbilities.HandleInput(gameTime, input, state);
            }
           
        }

        public override void Update(GameTime theTime, GameState state, Vector2 parentPosition)
        {
            _positionAbsolute = _positionRelative + parentPosition;

            if (_primaryHand != null)
            {
                _primaryHand.Update(theTime, state, _positionAbsolute);
            }

            if (_primaryHandAbilities != null)
            {
                _primaryHandAbilities.Update(theTime, state, _positionAbsolute);
            }

            if (_secondaryHand != null)
            {
                _secondaryHand.Update(theTime, state, _positionAbsolute);
            }

            if (_secondaryHandAbilities != null)
            {
                _secondaryHandAbilities.Update(theTime, state, _positionAbsolute);
            }
        }

        public override void Update(GameTime theTime, GameState state)
        {
           if(_primaryHand!=null)
           {
               _primaryHand.Update(theTime, state);
           }

           if (_primaryHandAbilities != null)
           {
               _primaryHandAbilities.Update(theTime, state);
           }

           if (_secondaryHand != null)
           {
               _secondaryHand.Update(theTime, state);
           }

           if (_secondaryHandAbilities != null)
           {
               _secondaryHandAbilities.Update(theTime, state);
           }
        }

        public override void Draw(SpriteBatch Batch, GameState state)
        {
            if (_primaryHand != null)
            {
                _primaryHand.Draw(Batch, state);
            }

            if (_primaryHandAbilities != null)
            {
                _primaryHandAbilities.Draw(Batch, state);
            }

            if (_secondaryHand != null)
            {
                _secondaryHand.Draw(Batch, state);
            }

            if (_secondaryHandAbilities != null)
            {
                _secondaryHandAbilities.Draw(Batch, state);
            }
        }



#region Event Handlers
        
        /// <summary>
        /// When the tool is added, its abilityInventory changes. Reattach its abilityInventory to the view
        /// </summary>
        public void OnPrimaryToolAbilitiesChanged()
        {
            _primaryHandAbilities.ReattachModel(_equippedItemsInventory.PrimaryHandInventory.Abilities);
        }

        public void OnSecondaryToolAbilitiesChanged()
        {
            _secondaryHandAbilities.ReattachModel(_equippedItemsInventory.SecondaryHandInventory.Abilities);
        }

        public void OnToolAdded()
        {
            //NOTE: Need to do the same on tool removed

            //Reassign the display of tool abilities
            if( _equippedItemsInventory.SecondaryHandInventory.HasTool)
            {
                _primaryHandAbilities.RestrictDisplayTo(5);
                _secondaryHandAbilities.RestrictDisplayTo(5);
            }
            else
            {
                _primaryHandAbilities.RestrictDisplayTo(10);
                _secondaryHandAbilities.RestrictDisplayTo(0);
            }

        }

        public void OnToolRemoved(ToolAbilityInventory toolAbilities)
        {
            //NOTE: Need to do the same on tool removed

            //Reassign the display of tool abilities
            if (_equippedItemsInventory.SecondaryHandInventory.HasTool)
            {
                _primaryHandAbilities.RestrictDisplayTo(5);
                _secondaryHandAbilities.RestrictDisplayTo(5);
            }
            else
            {
                _primaryHandAbilities.RestrictDisplayTo(10);
                _secondaryHandAbilities.RestrictDisplayTo(0);
            }

        }

#endregion
    }
}
