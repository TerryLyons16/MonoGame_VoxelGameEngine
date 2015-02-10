using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace VoxelRPGGame.MenuSystem.Screens
{
    public class DebugScreen: AbstractScreen
    {
        private static DebugScreen debugScreen = null;

        //-------Frames per Second Counter--------
        private int totalFrames = 0, totalUpdates = 0;
        private float elapsedTime = 0.0f;
        private int fps = 0, ups = 0;

        public int PolysDrawn { get; set; }
        public int VertsDrawn { get; set; }

        private Dictionary<string, string> debugMenu = new Dictionary<string, string>();
        //----------------------------------------

        private DebugScreen()
        {
            debugMenu.Add("FPS: ",""+ fps);
            debugMenu.Add("Updates per Second: ", ""+ups);
            debugMenu.Add("Polygons Drawn: ", "" + PolysDrawn);
            debugMenu.Add("Vertices Drawn: ", "" + PolysDrawn);
            debugMenu.Add("IsMouseVisible: ", ""+false);
            debugMenu.Add("Mouse Position:", "");
            debugMenu.Add("Camera Rotation:", "");
            debugMenu.Add("Mouse Delta:", "");
            isVisible = false;
            isActive = true;
            hasFocus = true;
            PolysDrawn = 0;
            VertsDrawn = 0;
        }

        public static DebugScreen GetInstance()
        {
            if (debugScreen == null)
            {
                debugScreen = new DebugScreen();
                
            }

            return debugScreen;
        }


        public override void Update(GameTime theTime)
        {
            totalUpdates++;

            elapsedTime += (float)theTime.ElapsedGameTime.TotalMilliseconds;

            // 1 Second has passed
            if (elapsedTime >= 1000.0f)
            {
                fps = totalFrames;
                debugMenu["FPS: "] = ""+fps;

                ups = totalUpdates;
                debugMenu["Updates per Second: "] = ""+ups;
                totalFrames = 0;
                elapsedTime = 0;
                totalUpdates = 0;
            }

            debugMenu["IsMouseVisible: "]=""+ScreenManager.GetInstance().Game.IsMouseVisible;

            debugMenu["Polygons Drawn: "]=""+PolysDrawn;
            debugMenu["Vertices Drawn: "] = "" + VertsDrawn;


          

        }

        public override void HandleInput(Microsoft.Xna.Framework.GameTime gameTime, InputState input)
        {
           debugMenu["Mouse Position:"]=input.CurrentMouseState.X+","+ input.CurrentMouseState.Y;
        }

        public override void Draw(SpriteBatch Batch)
        {
            debugMenu["Polygons Drawn: "] = "" + PolysDrawn;
            debugMenu["Vertices Drawn: "] = "" + VertsDrawn;

            totalFrames++;

            float yPosition = 20.0f;

            foreach (KeyValuePair<string, string> pair in debugMenu)
            {
                Batch.DrawString(ScreenManager.GetInstance().DefaultMenuFont, string.Format(pair.Key + pair.Value), new Vector2(10.0f, yPosition), Color.White);
                yPosition += 20.0f;
            }

             PolysDrawn =0;
             VertsDrawn = 0;
        }


        public void SetDebugListing(string name, string value)
        {
            if (!debugMenu.Keys.Contains(name))
            {
                debugMenu.Add(name, value);
            }

            else
            {
                debugMenu[name] = value;

            }

        }

        public void RemoveDebugListing(string name)
        {
            if (debugMenu.Keys.Contains(name))
            {
                debugMenu.Remove(name);
            }
        }

    }
}
