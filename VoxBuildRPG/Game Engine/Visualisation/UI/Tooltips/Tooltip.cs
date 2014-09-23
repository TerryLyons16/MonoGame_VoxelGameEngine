using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  VoxelRPGGame.GameEngine.UI.Tooltips
{
    public enum LeftRightAlignment
    {
        Left,
        Right
    }
    public enum UpDownAlignment
    {
        Up,
        Down
    }
    public abstract class Tooltip:UIElement
    {
        protected LeftRightAlignment _leftRightAlignment = LeftRightAlignment.Left;
        protected UpDownAlignment _upDownAlignment = UpDownAlignment.Up;

    }
}
