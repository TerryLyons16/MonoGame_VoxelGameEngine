using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using VoxelRPGGame.GameEngine.World.Textures;
using Microsoft.Xna.Framework;

namespace VoxelRPGGame.GameEngine.World.Geometry
{
    public struct Quad
    {
        Tri upperTri, lowerTri;
        //...
    }

    public struct Tri
    {
        Vector3 vertex1, vertex2, vertex3;
        TextureName texture;
        //...
    }

    public struct TextureVertex
    {
        public VertexPositionNormalTexture vertex;
        public TextureName texture;

        public TextureVertex(VertexPositionNormalTexture vert, TextureName tex)
        {
            vertex = vert;
            texture = tex;
        }
      
    }

    public struct TextureVector
    {
        public TextureVector(Vector3 vec, TextureName tex)
        {
            vector = vec;
            texture = tex;
        }
        public Vector3 vector;
        public TextureName texture;

        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is TextureVector)
            {
                TextureVector vec = (TextureVector)obj;
                if (this.vector == vec.vector && this.texture == vec.texture)
                {
                    result = true;
                }
            }
            return result;
        }
    }

    public static class GeometryServices
    {

        public static List<TextureVertex> BuildQuad(Vector3[] orderedVertices, bool splitFromTopLeft/*Determines whether quad is split into tris from top left corner or top right*/, Vector3 normal, TextureName topTriTexture, TextureName bottomTriTexture)
        {
            //Quad:
            //0-----1
            // |     |
            // |     |
            // 3-----2
            List<TextureVertex> result = new List<TextureVertex>();

            //NOTE: Retrieve texture Coords from texture atlas. Need to check if not null
            Vector2[] textureCoordinatesTopTriangle = TextureAtlas.GetInstance().GetTextureCoordinates(topTriTexture);//new Vector2[4];
            Vector2[] textureCoordinatesBottomTriangle = TextureAtlas.GetInstance().GetTextureCoordinates(bottomTriTexture);//new Vector2[4];

            if (orderedVertices.Length == 4)//Must have 4 vertices for a quad
            {
                //Determine if quad is split 0-2 (top left) of 1-3 (top right)
                if (splitFromTopLeft)
                {
                    for (int i = 0; i <= 2; i++)//Top right triangle 
                    {
                        result.Add(new TextureVertex(new VertexPositionNormalTexture(orderedVertices[i], normal, textureCoordinatesTopTriangle[i]), topTriTexture));
                    }
                    //NOTE: Include all in loop
                    //Bottom left triangle
                    result.Add(new TextureVertex(new VertexPositionNormalTexture(orderedVertices[2], normal, textureCoordinatesBottomTriangle[2]), bottomTriTexture));
                    result.Add(new TextureVertex(new VertexPositionNormalTexture(orderedVertices[3], normal, textureCoordinatesBottomTriangle[3]), bottomTriTexture));
                    result.Add(new TextureVertex(new VertexPositionNormalTexture(orderedVertices[0], normal, textureCoordinatesBottomTriangle[0]), bottomTriTexture));
                }
                else
                {
                    //Top Left triangle
                    result.Add(new TextureVertex(new VertexPositionNormalTexture(orderedVertices[3], normal, textureCoordinatesTopTriangle[3]), topTriTexture));
                    result.Add(new TextureVertex(new VertexPositionNormalTexture(orderedVertices[0], normal, textureCoordinatesTopTriangle[0]), topTriTexture));
                    result.Add(new TextureVertex(new VertexPositionNormalTexture(orderedVertices[1], normal, textureCoordinatesTopTriangle[1]), topTriTexture));

                    //Bottom right triangle
                    for (int i = 1; i <= 3; i++)
                    {
                        result.Add(new TextureVertex(new VertexPositionNormalTexture(orderedVertices[i], normal, textureCoordinatesBottomTriangle[i]), bottomTriTexture));
                    }
                }
            }

            return result;
        }
    }
}
