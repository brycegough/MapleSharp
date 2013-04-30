using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MapleSharp.Objects.Map
{
    public class MaplePortal
    {
        public Vector2 Location = Vector2.Zero;
        public int toMapId = 999999999;
        public PortalType Type;
        public string PortalName = "";
        public string SpawnName = "";
        public string ScriptName = "";

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Location.X + (int)Constants.Globals.PortalAnimation.Frames[0].Canvas.Origin.X, (int)Location.Y + (int)Constants.Globals.PortalAnimation.Frames[0].Canvas.Origin.Y, Constants.Globals.PortalAnimation.Frames[0].Canvas.Texture.Width, Constants.Globals.PortalAnimation.Frames[0].Canvas.Texture.Height);
            }
        }

        public MaplePortal(string name, string script, string spawn, Vector2 pos, int to, int pt)
        {
            ScriptName = script;
            SpawnName = spawn;
            PortalName = name;
            Location = pos;
            toMapId = to;
            Type = TypeByInt(pt);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Type == PortalType.Normal)
                Constants.Globals.PortalAnimation.Draw(spriteBatch, Location);
            //spriteBatch.DrawString(Constants.Globals.Font, PortalName.ToString(), Location, Color.Black);
        }

        PortalType TypeByInt(int type)
        {
            switch (type)
            {
                case 1:
                case 2:
                    return PortalType.Normal;
                case 0:
                    return PortalType.Spawn;
                case 3:
                    return PortalType.Auto;
                case 9:
                    return PortalType.Teleport;
                default:
                    return PortalType.Normal;
            }
        }

        public enum PortalType
        {
            Normal,
            Spawn,
            Auto,
            Teleport
        }
    }
}
