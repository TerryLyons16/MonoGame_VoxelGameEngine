using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using VoxelRPGGame.MenuSystem;

namespace VoxelRPGGame.GameEngine.World.Textures
{
    /// <summary>
    /// Static class that builds and stores texture atlas
    /// </summary>
    public class TextureAtlas
    {
        private static TextureAtlas textureAtlas = null;
        struct TextureAtlasEntry
        {
            public TextureAtlasEntry(TextureName name, Vector2 tLeft,Vector2 tRight,Vector2 bRight,Vector2 bLeft)
            {
                textureName = name;
                topLeft=tLeft;
                topRight=tRight;
                bottomRight = bRight;
                bottomLeft=bLeft;

                width=topRight.X-topLeft.X;
                height=bottomLeft.Y-topLeft.Y;
            }

            TextureName textureName;
            public Vector2 topLeft,topRight,bottomRight,bottomLeft;
            public float width, height;
            
        }

        private Texture2D _textureAtlas;
        private float width, height;
        private Dictionary<TextureName, TextureAtlasEntry> textureAtlasEntries;


        private TextureAtlas()
        {
            BuildTextureAtlas(ScreenManager.GetInstance().ContentManager);
        }

        public static TextureAtlas GetInstance()
        {
            if (textureAtlas == null)
            {
                textureAtlas=new TextureAtlas();
            }

            return textureAtlas;
        }

        public void BuildTextureAtlas(ContentManager contentManager)
        {
            //TEMP
            _textureAtlas=contentManager.Load<Texture2D>("terrain");
            textureAtlasEntries = new Dictionary<TextureName, TextureAtlasEntry>();

            //NOTE: Some overlapping/bleeding of textures
            /*textureAtlasEntries.Add(TextureName.Dirt,new TextureAtlasEntry(TextureName.Dirt,Vector2.Zero,new Vector2((_textureAtlas.Width/2),0),
                new Vector2((_textureAtlas.Width / 2), _textureAtlas.Height), new Vector2(0, _textureAtlas.Height)));

            textureAtlasEntries.Add(TextureName.Stone, new TextureAtlasEntry(TextureName.Stone,new Vector2((_textureAtlas.Width / 2), 0),new Vector2(_textureAtlas.Width, 0),
             new Vector2(_textureAtlas.Width, _textureAtlas.Height), new Vector2((_textureAtlas.Width / 2), _textureAtlas.Height)));*/


            //TEMP:Nudging everything by 0.5 texels reduces/removes bleeding
            textureAtlasEntries.Add(TextureName.Dirt, new TextureAtlasEntry(TextureName.Dirt, Vector2.Zero, new Vector2((_textureAtlas.Width / 2)-0.5f, 0),
                new Vector2((_textureAtlas.Width / 2)-0.5f, (_textureAtlas.Height/2)-0.5f), new Vector2(0, (_textureAtlas.Height/2)-0.5f)));

            textureAtlasEntries.Add(TextureName.Stone, new TextureAtlasEntry(TextureName.Stone, new Vector2((_textureAtlas.Width / 2)+0.5f, 0), new Vector2(_textureAtlas.Width, 0),
             new Vector2(_textureAtlas.Width, _textureAtlas.Height/2), new Vector2((_textureAtlas.Width / 2)+0.5f, _textureAtlas.Height/2)));

            textureAtlasEntries.Add(TextureName.Grass, new TextureAtlasEntry(TextureName.Grass, new Vector2(0, (_textureAtlas.Height / 2)+0.5f), new Vector2((_textureAtlas.Width / 2) - 0.5f, (_textureAtlas.Height / 2)+0.5f),
               new Vector2((_textureAtlas.Width / 2) - 0.5f, _textureAtlas.Height), new Vector2(0, _textureAtlas.Height)));

            //Load all textures in Textures\Terrain\Tiles, render to an in-memory texture, and use as atlas



            width = _textureAtlas.Width;
            height = _textureAtlas.Height;
        }

        public Vector2[] GetTextureCoordinates(TextureName textureName)
        {
            Vector2[] result = null;

            if (textureAtlasEntries.ContainsKey(textureName))
            {
                result = new Vector2[4];
                result[0] = new Vector2(textureAtlasEntries[textureName].topLeft.X/width,textureAtlasEntries[textureName].topLeft.Y/height);
                result[1] = new Vector2(textureAtlasEntries[textureName].topRight.X / width, textureAtlasEntries[textureName].topRight.Y / height);
                result[2] = new Vector2(textureAtlasEntries[textureName].bottomRight.X / width, textureAtlasEntries[textureName].bottomRight.Y / height); 
                result[3] = new Vector2(textureAtlasEntries[textureName].bottomLeft.X / width, textureAtlasEntries[textureName].bottomLeft.Y / height);
            }


            //If texture does not exist, default to error texture
            return result;
        }

        public Texture2D Atlas
        {
            get
            {
                return _textureAtlas;
            }

        }
    }
}
