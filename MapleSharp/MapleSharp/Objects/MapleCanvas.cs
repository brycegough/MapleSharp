using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSharp.Objects
{
    public class MapleCanvas
    {

        public Vector2 Origin = Vector2.Zero;
        public Texture2D Texture;

        public MapleCanvas(Texture2D tex, Vector2 ori)
        {
            Origin = ori;
            Texture = tex;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 Location, Vector2 origin, SpriteEffects effect = SpriteEffects.None)
        {
            if (this.Texture != null)
                spriteBatch.Draw(this.Texture, new Rectangle((int)Location.X + (int)Origin.X + (int)origin.X, (int)Location.Y + (int)Origin.Y + (int)origin.Y, this.Texture.Width, this.Texture.Height), null, Color.White, 0, Vector2.Zero, effect, 0);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 Location, SpriteEffects effect = SpriteEffects.None)
        {
            this.Draw(spriteBatch, Location, Vector2.Zero, effect);
        }

    }
}
