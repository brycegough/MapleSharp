using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MapleSharp.Engines;

namespace MapleSharp.Objects.Map
{
    public class MapleNpc
    {
        public MapleAnimation Animation;
        public Vector2 Location = Vector2.Zero;
        public string Name = "";
        public int ID = -1;
        public bool Hidden = false;
        public int Layer = 7;
        
        public MapleNpc(int id, MapleAnimation ani, Vector2 loc, int layer, bool hide = false)
        {
            ID = id;
            Animation = ani;
            Location = loc;
            Hidden = hide;
            Layer = layer;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Hidden)
            {
                Animation.Draw(spriteBatch, Location);
                //TODO NPC NAME
                //spriteBatch.DrawString(Constants.Globals.Font, this.Layer.ToString(), this.Location + this.Animation.Frames[0].Canvas.Origin, Color.Black);
            }
        }

        public void Update(int gameTime)
        {
            if (!Hidden)
            {
                Animation.Update(gameTime);
            }
        }

    }
}
