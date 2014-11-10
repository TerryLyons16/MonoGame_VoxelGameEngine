using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelRPGGame.GameEngine.InventorySystem;

namespace VoxelRPGGame.GameEngine.UI.Inventory
{
    public static class InventoryItemDisplayUtilities
    {

        public static Color GetItemRarityColour(Rarity itemRarity)
        {
            Color result = Color.White;

            switch (itemRarity)
            {
                case Rarity.Epic:
                    {
                        result = Color.Orange;
                        break;
                    }
                case Rarity.Rare:
                    {
                        result = Color.Green;
                        break;
                    }
                case Rarity.Uncommon:
                    {
                        result = Color.Blue;
                        break;
                    }
                case Rarity.Common:
                    {
                        result = Color.White;
                        break;
                    }
            }

            return result;
        }
    }
}
