using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using SysDraw = System.Drawing;
using Microsoft.Xna.Framework;

namespace MapleSharp.Objects.Map
{
    public class MapleMinimap
    {

        MapleMap _map;
        Texture2D _texture;

        public MapleMinimap(MapleMap map, Texture2D mini) 
        {
            _map = map;
            _texture = mini;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(_texture, new Vector2(10, 10), Color.White);
        }

        public void Update(int gameTime)
        {

        }

    }
}
