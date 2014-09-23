//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;

//using VoxelRPGGame.MenuSystem.MenuElements;
//using VoxelRPGGame.MenuSystem;

//using VoxelRPGGame.GameEngine.Visualisation;

//namespace VoxelRPGGame.GameEngine.Visualisation.Overlay
//{
//    public class RendererTextElement:AbstractDrawable2DGameObject
//    {
//        protected TextElement element;

//        public RendererTextElement(string text, Vector2 position)
//        {
//            element = new TextElement(text);
//            element.Font = ScreenManager.GetInstance().DefaultMenuFont;
//            element.Position = position;
//        }

//        public override void HandleInput(GameTime gameTime, InputState input)
//        {
         
//        }

//        public override void Update()
//        {
           
//        }

//        public override void Draw(SpriteBatch Batch)
//        {
//            element.Draw(Batch);
//        }
//    }
//}
