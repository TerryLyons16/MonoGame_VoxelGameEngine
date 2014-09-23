using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.EnvironmentState;
using VoxelRPGGame.GameEngine.InventorySystem.Armour;
using VoxelRPGGame.GameEngine.InventorySystem.Tools;

namespace VoxelRPGGame.GameEngine.InventorySystem
{
    /// <summary>
    /// Inventory to hold all of a player's equipped items, including weapons/tools, armour etc.
    /// NOTE: Could probably store all player inventory here (i.e. backpack, unlocked abilities etc)
    /// </summary>
    public class EquippedItemsInventory
    {

        protected EquippedToolInventory _primaryHandSlot = null;
        protected EquippedToolInventory _secondaryHandSlot = null;

        protected ArmourInventory _equippedArmour = null;

        protected CharacterInventory _owner;
        
        public EquippedItemsInventory(CharacterInventory owner)
        {
            _owner = owner;
            _primaryHandSlot = new EquippedToolInventory(this);
            _primaryHandSlot.ToolAddedEvent += OnPrimaryToolAdded;
            _primaryHandSlot.ToolRemovedEvent += _owner.SaveToolAbilitiesPrimary;

            _secondaryHandSlot = new EquippedToolInventory(this);
            _secondaryHandSlot.ToolAddedEvent += OnSecondaryToolAdded;
            _secondaryHandSlot.ToolRemovedEvent += _owner.SaveToolAbilitiesSecondary;

            GameWorldControlState.GetInstance().SelectAbilityEvent += SelectAbility;
            GameWorldControlState.GetInstance().PrimaryHandAbilityOnClickEvent += _primaryHandSlot.ActiveAbilityOnClick;
            GameWorldControlState.GetInstance().SecondaryHandAbilityOnClickEvent += _secondaryHandSlot.ActiveAbilityOnClick;
        }

        /// <summary>
        /// Handles equiping of tools to hands based on the tool's equipConstraints
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="toolSlot"></param>
        /// <returns></returns>
        public InventoryItem AddToolAt(ToolInventoryItem tool,EquippedToolInventory toolSlot)
        {
            InventoryItem result = null;
            if(toolSlot==_primaryHandSlot)
            {
                //Check equip constraints
                switch(tool.EquipConstraint)
                {
                    //Can equip
                    case EquipConstraint.Primary:
                        {
                            result=_primaryHandSlot.AddItem(tool);
                            break;
                        }
                    case EquipConstraint.None:
                         {
                             result=_primaryHandSlot.AddItem(tool);
                            break;
                        }
                        //Can only equip if nothing in secondary hand
                    case EquipConstraint.TwoHanded:
                         {
                             if(!_secondaryHandSlot.HasTool)
                             {
                                 result = _primaryHandSlot.AddItem(tool);
                             }
                             break;
                         }
                    default:
                            {
                                result=tool;
                                break;
                            }
                }
            }
            else if (toolSlot==_secondaryHandSlot)
            {
                //Don't let a tool be equipped to the second hand if there is a two-handed tool in the primary hand
                if (_primaryHandSlot.ToolEquipConstraint != EquipConstraint.TwoHanded)
                {
                    //Check equip constraints
                    switch (tool.EquipConstraint)
                    {
                        //Can equip
                        case EquipConstraint.Secondary:
                            {
                                result = _secondaryHandSlot.AddItem(tool);
                                break;
                            }
                        case EquipConstraint.None:
                            {
                                result = _secondaryHandSlot.AddItem(tool);
                                break;
                            }

                        default:
                            {
                                result = tool;
                                break;
                            }
                    }
                }
            }
            else
            {
                result = null;
            }

            return result;
        }

        #region Properties
        public EquippedToolInventory PrimaryHandInventory
        {
            get 
            {
                return _primaryHandSlot;
            }
        }

        public EquippedToolInventory SecondaryHandInventory
        {
            get
            {
                return _secondaryHandSlot;
            }
        }
        #endregion

        #region Event Handlers

        public void OnPrimaryToolAdded()
        {
            if (_primaryHandSlot != null)
            {
                _primaryHandSlot.LoadAbilities(_owner.GetToolAbilitiesPrimary(_primaryHandSlot.ToolType));
            }
        }

        public void OnSecondaryToolAdded()
        {
            if (_secondaryHandSlot != null)
            {
                _secondaryHandSlot.LoadAbilities(_owner.GetToolAbilitiesSecondary(_secondaryHandSlot.ToolType));
            }

            //If the main hand's selected ability conflicts with the second hand, deactivate it
            if (_primaryHandSlot.IsActiveAbilityInSecondFive())
            {
                _primaryHandSlot.DeselectAbility();
            }
        }

        public void SelectAbility(int abilityPosition)
        {
            //Route the selection to the correct tool
            if (_secondaryHandSlot.HasTool)
            {
                if (abilityPosition <= 5)
                {
                    if (_primaryHandSlot.HasTool)
                    {
                        _primaryHandSlot.SelectAbility(abilityPosition);
                    }
                }
                else
                {
                    _secondaryHandSlot.SelectAbility(abilityPosition - 5);//If abilityPosition is 6, want to select first ability in second hand etc.
                }
            }

            else //If only one eequipped tool, all 10 abilities on it can be accessed
            {
                if(_primaryHandSlot.HasTool)
                {
                    _primaryHandSlot.SelectAbility(abilityPosition);
                }
            }

        }
        #endregion
    }
}
