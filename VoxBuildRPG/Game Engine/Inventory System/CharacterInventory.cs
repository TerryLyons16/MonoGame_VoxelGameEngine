using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.InventorySystem.Tools;

namespace VoxelRPGGame.GameEngine.InventorySystem
{
    public class CharacterInventory
    {
        protected EquippedItemsInventory _equippedItems; 


        public CharacterInventory()
        {
            _equippedItems = new EquippedItemsInventory(this);
        }
        /// <summary>
        /// Retains a memory of abilities slotted for each tool type 
        /// </summary>
        protected Dictionary<ToolType, ToolAbilityInventory> _storedToolAbilitiesPrimary = new Dictionary<ToolType, ToolAbilityInventory>();
        protected Dictionary<ToolType, ToolAbilityInventory> _storedToolAbilitiesSecondary = new Dictionary<ToolType, ToolAbilityInventory>();
        //Backpacks


        #region Properties

        public EquippedItemsInventory EquippedItems
        {
            get
            {
                return _equippedItems;
            }
        }
        #endregion


        public ToolAbilityInventory GetToolAbilitiesPrimary(ToolType toolType)
        {
            ToolAbilityInventory result = null;

            if (_storedToolAbilitiesPrimary.ContainsKey(toolType))
            {
                result = _storedToolAbilitiesPrimary[toolType];
            }

            return result;
        }

        public ToolAbilityInventory GetToolAbilitiesSecondary(ToolType toolType)
        {
            ToolAbilityInventory result = null;

            if (_storedToolAbilitiesSecondary.ContainsKey(toolType))
            {
                result = _storedToolAbilitiesSecondary[toolType];
            }

            return result;
        }

        #region Event Handlers

        public void SaveToolAbilitiesPrimary(ToolAbilityInventory toolAbilities)
        {
            if (toolAbilities.ToolType != ToolType.NULL)
            {
                if (_storedToolAbilitiesPrimary.ContainsKey(toolAbilities.ToolType))
                {
                    _storedToolAbilitiesPrimary[toolAbilities.ToolType] = toolAbilities;
                }
                else
                {
                    _storedToolAbilitiesPrimary.Add(toolAbilities.ToolType,toolAbilities);
                }
            }
        }

        public void SaveToolAbilitiesSecondary(ToolAbilityInventory toolAbilities)
        {
            if (toolAbilities.ToolType != ToolType.NULL)
            {
                if (_storedToolAbilitiesSecondary.ContainsKey(toolAbilities.ToolType))
                {
                    _storedToolAbilitiesSecondary[toolAbilities.ToolType] = toolAbilities;
                }
                else
                {
                    _storedToolAbilitiesSecondary.Add(toolAbilities.ToolType, toolAbilities);
                }
            }
        }

        #endregion

    }
}
