using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSharp.Objects.Map
{
    public class MapleObject
    {

        public Vector2 Position = Vector2.Zero;
        public int Index = 0;
        MapleAnimation Canvas;
        int lay;

        public MapleObject(Vector2 pos, int zIndex, MapleAnimation can, int layer)
        {
            Position = pos;
            Index = zIndex;
            Canvas = can;
            lay = layer;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Canvas != null)
                Canvas.Draw(spriteBatch, Position);
            //spriteBatch.DrawString(Constants.Globals.Font, Index.ToString() + " - " + lay.ToString(), Position - -Canvas.Frames[0].Canvas.Origin, Color.Black);
        }

        public void Update(int gameTime)
        {
            if (Canvas != null)
                Canvas.Update(gameTime);
        }
    }
}
