using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSharp.Engines
{
    public static class GameEngine // This engine is a single object that calls the other objects
    {

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, RasterizerState.CullNone, null, Camera.GetMatrix(spriteBatch.GraphicsDevice));
            MapEngine.Draw(spriteBatch);
            spriteBatch.End(); // Finish drawing character/map/etc..

            spriteBatch.Begin();
            InterfaceEngine.Draw(spriteBatch);
            spriteBatch.End(); // Finish drawing user interface..
        }

        public static void Update(int gameTime)
        {
            MapEngine.Update(gameTime);
            InterfaceEngine.Update(gameTime);
        }

        public static void Dispose()
        {
            MapEngine.Dispose();
            MusicEngine.Dispose();
        }
    }
}
