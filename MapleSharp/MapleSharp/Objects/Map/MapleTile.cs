using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MapleSharp.Objects.Map
{
    public class MapleTile
    {

        public MapleCanvas Canvas = null;
        public Vector2 Location = Vector2.Zero;
        public int Index;

        public MapleTile(MapleCanvas tex, Vector2 loc, int index) 
        {
            Canvas = tex;
            Location = loc;
            Index = index;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Canvas != null)
                Canvas.Draw(spriteBatch, Location);
        }

        // TODO: Maybe Update? If there are animated tiles..
    }
}
