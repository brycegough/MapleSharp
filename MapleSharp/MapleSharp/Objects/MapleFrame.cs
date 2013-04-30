using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MapleSharp.Objects
{
    public class MapleFrame
    {

        public MapleCanvas Canvas;
        public int Delay;

        public MapleFrame(MapleCanvas c, int del)
        {
            Canvas = c;
            Delay = del;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 loc, Vector2 origin, SpriteEffects effect = SpriteEffects.None)
        {
            Canvas.Draw(spriteBatch, loc, origin, effect);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 loc, SpriteEffects effect = SpriteEffects.None)
        {
            this.Draw(spriteBatch, loc, Vector2.Zero, effect);
        }

    }
}
