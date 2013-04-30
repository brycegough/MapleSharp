using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSharp.Objects.Map
{
    public class MapleFoothold
    {

        public Vector2 StartPos = Vector2.Zero;
        public Vector2 EndPos = Vector2.Zero;
        public int Layer = 7; // 7 is top layer
        public int ID = -1;

        public MapleFoothold(int id, Vector2 start, Vector2 end, int layer)
        {
            StartPos = start;
            EndPos = end;
            Layer = layer;
            ID = id;
        }

        public bool Intersects(Rectangle rec)
        {
            int BoundHeight = (int)EndPos.Y - (int)StartPos.Y;
            int BoundWidth =(int)EndPos.X - (int)StartPos.X;

            if (rec.Intersects(new Rectangle((int)StartPos.X, (int)StartPos.Y, BoundWidth == 0 ? 1 : BoundWidth, BoundHeight == 0 ? 1 : BoundHeight)))
                return true;
            return rec.Intersects(new Rectangle((int)StartPos.X, (int)StartPos.Y, BoundWidth == 0 ? 1 : BoundWidth, BoundHeight == 0 ? 1 : BoundHeight));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Tools.DrawLine(spriteBatch, StartPos, EndPos, Color.Red);
        }

    }
}
