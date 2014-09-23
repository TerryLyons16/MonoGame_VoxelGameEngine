using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using VoxelRPGGame.MenuSystem;
using VoxelRPGGame.GameEngine.World.Textures;

namespace VoxelRPGGame.GameEngine.World.Voxels
{
    public class DirtBlock: AbstractBlock
    {

        public DirtBlock(/*Vector3 centre,*/ BlockShape shape, Direction facing) :
            base(/*centre,*/ shape, MaterialType.Dirt, facing)
        {
            //Note: texture loading should be done centrally
            //Texture2D tex = ScreenManager.GetInstance().ContentManager.Load<Texture2D>("dirt");//dirt2

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                BlockTextures.Add(direction, TextureName.Dirt);
            }
            BlockTextures[Direction.Up] = TextureName.Grass;
            SetFaces();
            GetCubeWireframe();
        }

    }
}
