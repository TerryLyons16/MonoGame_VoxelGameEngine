using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.InventorySystem.Tools;

namespace VoxelRPGGame.GameEngine.InventorySystem.Abilities
{
    public enum AbilityUsageMode
    {
        NULL,
        /// <summary>
        /// One use and then deactivates
        /// </summary>
        Single,
        /// <summary>
        /// Multiple uses dictated by _numUses
        /// </summary>
        Multi,
        /// <summary>
        /// Continuous Use
        /// </summary>
        Sustained
    }

    //NOTE: Add way to determine what tools an ability can be used with
    //Also need an ability to be able to get the stats of the tool that it is being used with

    public abstract class AbilityInventoryItem: InventoryItem
    {

        protected AbilityUsageMode _abilityUsageMode=AbilityUsageMode.NULL;
        protected int _numUses; //Number of times the ability can be used if Usage mode is multi
        protected ToolInventoryItem _ActiveTool;

        protected List<ToolType> _allowedTools;

        public AbilityInventoryItem(string iconLocation):base(iconLocation)
        {
            _allowedTools = new List<ToolType>();

        }

        //Click is fired when the previous state is released and the current state is pressed
        public abstract void OnClick(InputState input);
        //Drag is fired when the previous state is pressed and the current state is pressed
        public abstract void OnDrag(InputState input);
        //Release is fired when the previous state is pressed and the current state is released
        public abstract void OnRelease(InputState input);

        /// <summary>
        /// Can toggle multiple functions by having logic to store the state of the selected ability,
        /// then switching state if onSelect runs again (can also store a list of icons and toggle the _iconFileLocation
        /// of InventoryItem to display the currently selected state)
        /// </summary>
        public abstract void OnSelect();
        public abstract void OnDeselect();



        public List<ToolType> AllowedTools
        {
            get
            {
                return _allowedTools;
            }
        }
    }
}
