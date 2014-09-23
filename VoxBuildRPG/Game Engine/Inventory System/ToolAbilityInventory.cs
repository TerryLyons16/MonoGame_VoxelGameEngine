using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.InventorySystem.Tools;
using VoxelRPGGame.GameEngine.InventorySystem.Abilities;
using VoxelRPGGame.GameEngine.EnvironmentState;

namespace VoxelRPGGame.GameEngine.InventorySystem
{
    /// <summary>
    /// Stores a tool and its equipped abilities.
    /// Used to determine what player is holding and can use
    /// </summary>
    public class ToolAbilityInventory: ArrayInventory
    {
        protected ToolType _toolType = ToolType.NULL; //Allowed type of tool
        protected int ? _activeAbility = null;

        protected EquippedToolInventory tempOwner;

        public ToolAbilityInventory()
            : base(10)
        {
         //   _items = new LinkedList<InventoryItem/*AbilityInventoryItem*/>();
        }

        public ToolAbilityInventory(ToolType toolType,EquippedToolInventory owner):base(10)
        {
           
            tempOwner=owner;
            _toolType = toolType;
          //  _items = new LinkedList<InventoryItem/*AbilityInventoryItem*/>();
        }

        public override InventoryItem AddItemAt(InventoryItem item, int position)
        {
            InventoryItem result = item;

            if (_toolType != ToolType.NULL && item is AbilityInventoryItem)//Only allow AbilityInventoryItems to be added
            {
                if (((AbilityInventoryItem)item).AllowedTools.Contains(_toolType))//Only add the item if it can be used by the tool
                {
                    result= base.AddItemAt(item, position);
                }

                if(_items.Contains(item))
                {
                    item.RemoveFromInventoryRequest += OnRemoveAbility;
                }
            }
            return result;
        }
        public override InventoryItem AddItem(InventoryItem item)
        {
            InventoryItem result = item;
            if (_toolType!= ToolType.NULL&&item is AbilityInventoryItem)//Only allow AbilityInventoryItems to be added
            {
                if (((AbilityInventoryItem)item).AllowedTools.Contains(_toolType))//Only add the item if it can be used by the tool
                {
                    result= base.AddItem(item);
                }
            }
            if (_items.Contains(item))
            {
                item.RemoveFromInventoryRequest += OnRemoveAbility;
            }
            return result;
        }

        public ToolType ToolType
        {
            get
            {
                return _toolType;
            }

        }


        public void SelectAbility(int abilityPosition)
        { 

            if (_items.Length >= (abilityPosition-1 )&& _items.ToList<InventoryItem>()[(int)abilityPosition-1] != null)//Ensure that the selected position is valid
           {
               _activeAbility = abilityPosition-1;//Selections are 1-10, but positions are 0-9
           }
            else
            {
                _activeAbility = null;
            }
            //NOTE: Should revert to default if not valid selection , or keep current selection?
        }

        public void DeselectAbility()
        {
            _activeAbility = null;
        }

        public void ActiveAbilityOnClick(InputState input)
        {
            if (_activeAbility != null&&_items.Length>0 && _items[(int)_activeAbility] != null && _items[(int)_activeAbility] is AbilityInventoryItem)
            {
                ((AbilityInventoryItem)_items.ToList<InventoryItem>()[(int)_activeAbility]).OnClick(input);
            }

        }
        //Drag is fired when the previous state is pressed and the current state is pressed
        public void ActiveAbilityOnDrag(InputState input)
        {
            if (_activeAbility != null && _items.Length > 0 && _items[(int)_activeAbility] != null && _items[(int)_activeAbility] is AbilityInventoryItem)
            {
                ((AbilityInventoryItem)_items.ToList<InventoryItem>()[(int)_activeAbility]).OnDrag(input);
            }
        }
        //Release is fired when the previous state is pressed and the current state is released
        public void ActiveAbilityOnRelease(InputState input)
        {
            if (_activeAbility != null && _items.Length > 0 && _items[(int)_activeAbility] != null && _items[(int)_activeAbility] is AbilityInventoryItem)
            {
                ((AbilityInventoryItem)_items.ToList<InventoryItem>()[(int)_activeAbility]).OnRelease(input);
            }
        }

        //Removes event handlers
        public void Destroy()
        {
            tempOwner = null;
        }
      

        public int ? ActiveAbility
        {
            get
            {
                return _activeAbility;
            }
        }

        public InventoryItem OnRemoveAbility(InventoryItem ability)
        {
            //Set the active ability to null if it is removed
            if(_activeAbility!=null&&_items[(int)_activeAbility]==null)
            {
                _activeAbility = null;
            }

            return ability;
        }
    }
}
