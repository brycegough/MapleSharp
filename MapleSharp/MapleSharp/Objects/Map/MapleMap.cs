using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MapleSharp.Objects.Map
{
    public class MapleMap
    {
        public int MapID = 999999999;
        public string MapName = "Unknown";
        public bool HideMinimap = false;

        public int Left = 0;
        public int Right = 0;
        public int Top = 0;
        public int Bottom = 0;

        public SortedDictionary<int, MapleLayer> Layers = new SortedDictionary<int, MapleLayer>();
        public List<MaplePortal> Portals = new List<MaplePortal>();
        public List<MapleBackground> Backgrounds = new List<MapleBackground>();
        public List<MapleNpc> Npcs = new List<MapleNpc>();
        public List<MapleFoothold> Footholds = new List<MapleFoothold>();

        public int Width = 0;
        public int Height = 0;
        public int BackWidth = 0;
        public int BackHeight = 0;
        

        public string FormattedMapID 
        {
            get
            {
                return MapID.ToString("000000000");
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (MapleBackground bg in Backgrounds)
                bg.Draw(spriteBatch);
            foreach (MapleLayer layer in Layers.Values)
            {
                layer.Draw(spriteBatch);
                foreach (MapleNpc npc in Npcs.Where(o => o.Layer == layer.LayerIndex))
                    npc.Draw(spriteBatch);
                if (layer.LayerIndex == Constants.Globals.Player.Layer)
                    Constants.Globals.Player.Draw(spriteBatch); // TODO: Draw other characters if multiplayer
            }
            foreach (MaplePortal portal in Portals)
                portal.Draw(spriteBatch);
            

            if (Constants.Game.DrawFootholds)
                foreach (MapleFoothold f in Footholds)
                    f.Draw(spriteBatch);
        }

        public void Update(int gameTime)
        {
            if (Constants.Globals.Player != null)
                Constants.Globals.Player.Update(gameTime);

            foreach (MapleBackground bg in Backgrounds)
                bg.Update(gameTime);
            if (Portals.Count > 0)
                Constants.Globals.PortalAnimation.Update(gameTime);
            foreach (MapleLayer layer in Layers.Values)
                layer.Update(gameTime);
            foreach (MapleNpc npc in Npcs)
                npc.Update(gameTime);
        }

        public void Dispose()
        {
            //...Nothing to dispose..
        }
    }
}
