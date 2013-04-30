using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSharp.Objects.Map
{
    public class MapleLayer
    {

        public string TileSet = "";
        public List<MapleTile> Tiles;
        public List<MapleObject> Objects;
        public int LayerIndex;

        public MapleLayer(int index, string tileSet, List<MapleTile> tiles, List<MapleObject> objs)
        {
            TileSet = tileSet;
            Tiles = tiles;
            Objects = objs;
            LayerIndex = index;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (MapleObject obj in Objects.OrderBy(o => o.Index))
                obj.Draw(spriteBatch);
            foreach (MapleTile tile in Tiles.OrderBy(o => o.Index))
                tile.Draw(spriteBatch);
        }

        public void Update(int gameTime)
        {
            foreach (MapleObject obj in Objects)
                obj.Update(gameTime);
        }
    }
}
