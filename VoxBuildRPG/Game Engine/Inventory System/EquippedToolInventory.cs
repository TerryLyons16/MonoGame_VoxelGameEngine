using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.EnvironmentState;
using VoxelRPGGame.GameEngine.InventorySystem.Tools;

namespace VoxelRPGGame.GameEngine.InventorySystem
{

    public class EquippedToolInventory:Inventory
    {
            public delegate void ToolAdded();
            public event ToolAdded ToolAddedEvent;
            public delegate void ToolRemoved(ToolAbilityInventory toolAbilities);
            public event ToolRemoved ToolRemovedEvent;
            public delegate void AbilitiesChanged();
            public event AbilitiesChanged AbilitiesChangedEvent;

            protected EquippedItemsInventory _owner;

            protected ToolAbilityInventory _toolAbilities = new ToolAbilityInventory();

        public EquippedToolInventory(EquippedItemsInventory owner):base(1)
        {
            _owner = owner;
        }

        public override InventoryItem AddItem(InventoryItem item)
        {
            if (item is Tools.ToolInventoryItem)
            {
                InventoryItem result = base.AddItem(item);
                if (_items.First.Value == item)//Only fire event if the item was successfully added
                {
                    OnToolAdded();
                }
                return result;
            }
            else
            {
                return item;
            }
        }

        public override InventoryItem RemoveItem(InventoryItem item)
        {
            InventoryItem result = base.RemoveItem(item);

            OnToolRemoved();
            return result;
        }

        public void LoadAbilities(ToolAbilityInventory abilities)
        {
            if(abilities!=null)
            {
                _toolAbilities = abilities;
                OnAbilitiesChanged();
            }
        }
        /// <summary>
        /// Returns true if the position of the selected ability is slots 6-10. Used when deactivating a
        /// primary hand ability if a second hand tool is using the slots 6-10
        /// </summary>
        /// <returns></returns>
        public bool IsActiveAbilityInSecondFive()
        {
            bool result = false;
            if (Abilities != null && Abilities.ActiveAbility != null && ((int)Abilities.ActiveAbility) >= 5)
            {
                result = true;
            }
            return result;
        }

#region Properties

        public EquippedItemsInventory Owner
        {
            get
            {
                return _owner;
            }
        }

        public ToolAbilityInventory Abilities
        {
            get
            {
                return _toolAbilities;
            }
        }

        public ToolType ToolType
        {
            get
            {
                ToolType result = ToolType.NULL;
                if (_items.First != null && _items.First.Value is Tools.ToolInventoryItem)
                {
                    result = (_items.First.Value as Tools.ToolInventoryItem).ToolType;
                }

                return result;
            }
        }

        public EquipConstraint ToolEquipConstraint
        {
            get
            {
                EquipConstraint result = EquipConstraint.NULL;
                if (_items.First != null && _items.First.Value is Tools.ToolInventoryItem)
                {
                    result = (_items.First.Value as Tools.ToolInventoryItem).EquipConstraint;
                }

                return result;
            }
        }

        public bool HasTool
        {
            get
            {
                if (_items.First != null && _items.First.Value is Tools.ToolInventoryItem)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
    }


#endregion

#region Events
        /// <summary>
        /// Fires event if tool was successfully added
        /// </summary>
        public void OnToolAdded()
        {
            if (_items.First != null && _items.First.Value is Tools.ToolInventoryItem)
            {
                if(_toolAbilities!=null)
                {
                    _toolAbilities.Destroy();
                }
                _toolAbilities = new ToolAbilityInventory((_items.First.Value as Tools.ToolInventoryItem).ToolType,this);
                OnAbilitiesChanged();

            }
            if(ToolAddedEvent!=null)
            {
                ToolAddedEvent();
            }
        }

        public void OnToolRemoved()
        {
            if (_toolAbilities != null)
            {
                _toolAbilities.Destroy();
            }

            ToolAbilityInventory abilitiesToSave = _toolAbilities;
            //Clear the tool ability inventory
            _toolAbilities = new ToolAbilityInventory();
            OnAbilitiesChanged();
            //Fire removed event
            if (ToolAddedEvent != null)
            {
                ToolRemovedEvent(abilitiesToSave);
            }

          
        }

        public void OnAbilitiesChanged()
        {
            if(AbilitiesChangedEvent!=null)
            {
                AbilitiesChangedEvent();
            }
        }


        //NOTE: These should be called from the equipped items inventory, which will route positions to the correct tool if multiple equipped
        public void SelectAbility(int abilityPosition)
        {
            if(_items.Count>0&&_toolAbilities!=null)//Only select ability if tool inventory has equipped tool
            {
                _toolAbilities.SelectAbility(abilityPosition);
            }
            //NOTE: Should revert to default if not valid selection , or keep current selection?
        }

        public void DeselectAbility()
        {
            if(_toolAbilities!=null)
            {
                _toolAbilities.DeselectAbility();
            }
        }

        public void ActiveAbilityOnClick(InputState input)
        {
            if (_items.Count > 0 && _toolAbilities != null)//Only select ability if tool inventory has equipped tool
            {
                _toolAbilities.ActiveAbilityOnClick(input);
            }

        }
        //Drag is fired when the previous state is pressed and the current state is pressed
        public void ActiveAbilityOnDrag(InputState input)
        {
            if (_items.Count > 0 && _toolAbilities != null)//Only select ability if tool inventory has equipped tool
            {
                _toolAbilities.ActiveAbilityOnDrag(input);
            }
        }
        //Release is fired when the previous state is pressed and the current state is released
        public void ActiveAbilityOnRelease(InputState input)
        {
            if (_items.Count > 0 && _toolAbilities != null)//Only select ability if tool inventory has equipped tool
            {
                _toolAbilities.ActiveAbilityOnRelease(input);
            }
        }
#endregion
    }
}
