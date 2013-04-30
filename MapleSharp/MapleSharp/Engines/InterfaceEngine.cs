using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapleSharp.Objects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MapleSharp.Objects.Map;

namespace MapleSharp.Engines
{
    public static class InterfaceEngine
    {

        public static MapleMinimap MiniMap;

        public static void Draw(SpriteBatch spriteBatch)
        {

            if (MiniMap != null && MapEngine.CurrentMap != null && !MapEngine.CurrentMap.HideMinimap)
                MiniMap.Draw(spriteBatch);

            MapleCursor.Draw(spriteBatch, Mouse.GetState());
        }

        public static void Update(int gameTime)
        {

            if (MiniMap != null)
                MiniMap.Update(gameTime);

            MapleCursor.Update(gameTime);
        }
    }
}
