﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelRPGGame.GameEngine.InventorySystem.Tools
{
    public abstract class WeaponInventoryItem:ToolInventoryItem
    {
       

        protected float _range;//? Is this correct?

        public WeaponInventoryItem(string name,ToolType type,string iconLocation,EquipConstraint tempEquipConstraint)
            : base(name,type, iconLocation, tempEquipConstraint)
        {

        }
    }
}
