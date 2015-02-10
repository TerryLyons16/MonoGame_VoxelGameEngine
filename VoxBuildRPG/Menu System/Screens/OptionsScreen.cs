using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using VoxelRPGGame.MenuSystem.MenuElements;

namespace VoxelRPGGame.MenuSystem.Screens
{
    public class OptionsScreen : MenuScreen
    {
          Texture2D blankTexture;
            SpriteFont font;

        //string menuTitle = "Main Menu";

        //private List<MenuElement> elements = new List<MenuElement>();

        public event EventHandler<EventArgs> ToMenu;
        public event EventHandler<EventArgs> ExitMenu;

        private MenuElement DebugOptions;
        private MenuElement FullscreenOptions;
        

        public OptionsScreen()
            : base("Options")
        {
       





            blankTexture = ScreenManager.GetInstance().ContentManager.Load<Texture2D>("blank");
            //blankTexture = new Texture2D(graphics, 1, 1, false, SurfaceFormat.Color);
            //blankTexture.SetData<Color>(new Color[] { Color.White });


                font = ScreenManager.GetInstance().ContentManager.Load<SpriteFont>("Fonts\\SimulatorFont");
                TEMP_SetSimulatorMenuElements();
 
           

           



            hasFocus = true;
            isActive = true;

            OnTransitionIn();
        }

        private void TEMP_SetSimulatorMenuElements()
        {
            DebugOptions = new ButtonElement();
            DebugOptions.IsActive = true;
            DebugOptions.OnClickEvent += ChangeDebugMenu;
            DebugOptions.ElementText="Debug Menu  :  " + DebugScreen.GetInstance().IsVisible;

            elements.Add(DebugOptions);

            FullscreenOptions = new ButtonElement();
            FullscreenOptions.IsActive = true;
            FullscreenOptions.OnClickEvent += ToggleFullscreen;
            FullscreenOptions.ElementText = "Fullscreen   :  " + ScreenManager.GetInstance().IsFullscreen;

            elements.Add(FullscreenOptions);


            ButtonElement exitMenuButton = new ButtonElement();
            exitMenuButton.IsActive = true;
            exitMenuButton.OnClickEvent += Exit;
            exitMenuButton.ElementText = "Back";

            elements.Add(exitMenuButton);

            foreach (ButtonElement e in elements)
            {
                e.Font = font;
                e.TextColour = Color.White;
                e.Alpha = 1f;
            }

            /* foreach (MenuElement element in elements)
             {
                 element.Highlighted += isHighlighted;
                 element.UnHighlighted += isUnHighlighted;
             }*/


            setElementPositions_Centre(20);
        }

        private void SetElements()
        {
            DebugOptions = new TextElement("Display Debug Menu  :  " + DebugScreen.GetInstance().IsVisible);
            DebugOptions.IsActive = true;
            DebugOptions.OnClickEvent += ChangeDebugMenu;


            elements.Add(DebugOptions);

            FullscreenOptions = new TextElement("Fullscreen   :  " + ScreenManager.GetInstance().IsFullscreen);
            FullscreenOptions.IsActive = true;
            FullscreenOptions.OnClickEvent += ToggleFullscreen;


            elements.Add(FullscreenOptions);


            TextElement exitMenuButton = new TextElement("Back");
            exitMenuButton.IsActive = true;
            exitMenuButton.OnClickEvent += Exit;


            elements.Add(exitMenuButton);

            /* foreach (MenuElement element in elements)
             {
                 element.Highlighted += isHighlighted;
                 element.UnHighlighted += isUnHighlighted;
             }*/


            setElementPositions_Centre(10);
        }

        public override void Draw(SpriteBatch Batch)
        {
            DebugOptions.ElementText = "Display Debug Menu  :  " + DebugScreen.GetInstance().IsVisible;
            FullscreenOptions.ElementText = "Fullscreen   :  " + ScreenManager.GetInstance().IsFullscreen;

            //Adds opacity for screen
            float alpha = 0.0f;

            Batch.Draw(blankTexture, ScreenManager.GetInstance().GraphicsDevice.Viewport.Bounds, Color.Black * alpha);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(ScreenManager.GetInstance().GraphicsDevice.Viewport.Width / 2, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            //Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            //titlePosition.Y -= transitionOffset * 100;

            Batch.DrawString(font, menuTitle, titlePosition, Color.White, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            foreach (MenuElement element in elements)
            {
                element.Draw(Batch);
            }
        }

#region Event Handlers
        public void ChangeDebugMenu(MenuElement element)
        {
            DebugScreen.GetInstance().IsVisible = !DebugScreen.GetInstance().IsVisible;
            DebugOptions.ElementText = "Display Debug Menu  :  " + DebugScreen.GetInstance().IsVisible;
            
        }

        public void ToggleFullscreen(MenuElement element)
        {
            ScreenManager.GetInstance().ToggleFullscreen();
            FullscreenOptions.ElementText = "Fullscreen   :  " + ScreenManager.GetInstance().IsFullscreen;

        }





        public void Exit(MenuElement element)
        {
            
            ExitMenu(this, new EventArgs());


        }
#endregion 
    }


 }

