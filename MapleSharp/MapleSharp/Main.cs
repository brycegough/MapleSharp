using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using Apex.GUI;
using reWZ;
using reWZ.WZProperties;
using MapleSharp.Objects;
using MapleSharp.Engines;

namespace MapleSharp
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Stopwatch loadTimer = new Stopwatch(); // Stopwatch for measuring load times
        

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = Constants.Game.ScreenHeight; // Set Height
            graphics.PreferredBackBufferWidth = Constants.Game.ScreenWidth; // Set Width

            if (Constants.Game.StartFullscreen)
                graphics.IsFullScreen = true;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {  
            loadTimer.Restart();

            // Initialize Objects
            
            MapleConsole.Write(MapleConsole.LogType.MESSAGE, "MapleSharp", "Initialized.");
            base.Initialize();
        }

        protected override void LoadContent()
        {
            loadTimer.Restart();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Get Cursor Animations
            WZFile uiWz = new WZFile("UI.wz", WZVariant.GMS, true, WZReadSelection.LowMemory);
            MapleCursor.LoadCursor(GraphicsDevice, (WZSubProperty)uiWz.ResolvePath("Basic.img/Cursor")); // Load the cursor animations c:

            Constants.Globals.Font = Content.Load<SpriteFont>("lolwut");
            
            // TODO: Load character equips'n'shit..
            

            MapEngine.LoadMap(GraphicsDevice, 100000000); // Load a map by id :3
            
            MapleConsole.Write(MapleConsole.LogType.MESSAGE, "MapleSharp", "Content Loaded in " + loadTimer.ElapsedMilliseconds.ToString() + "ms.");
        }

        protected override void UnloadContent()
        {
            GameEngine.Dispose();

            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Escape))
                this.Exit();
            if (!Constants.Game.CameraLimit)
            {
                if (keyboard.IsKeyDown(Keys.D))
                    Camera.MoveX(10);
                if (keyboard.IsKeyDown(Keys.A))
                    Camera.MoveX(-10);
                if (keyboard.IsKeyDown(Keys.W))
                    Camera.MoveY(-10);
                if (keyboard.IsKeyDown(Keys.S))
                    Camera.MoveY(10);
            }

            GameEngine.Update(gameTime.ElapsedGameTime.Milliseconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            GameEngine.Draw(spriteBatch);

            spriteBatch.Begin();
            spriteBatch.DrawString(Constants.Globals.Font, Constants.Globals.Player.Layer.ToString(), new Vector2(10, 10), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
