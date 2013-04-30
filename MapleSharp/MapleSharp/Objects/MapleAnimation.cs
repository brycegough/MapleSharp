using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MapleSharp.Objects
{
    public class MapleAnimation
    {

        public Dictionary<int, MapleFrame> Frames = new Dictionary<int, MapleFrame>();
        public int Frame = 0;
        public int Interval = 200;
        public bool Reverse = false;

        public MapleAnimation(Dictionary<int, MapleFrame> frames, bool rev = false)
        {
            Frames = frames;
            Reverse = rev;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 Location, Vector2 origin, SpriteEffects effect = SpriteEffects.None)
        {
            if (Frames.Count > 0)
            {
                if (Frames.ContainsKey(Frame))
                {
                    Frames[Frame].Draw(spriteBatch, Location, origin, effect);
                } // What else could we do? o.o Maybe draw the first frame to avoid flashing..
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 Location, SpriteEffects effect = SpriteEffects.None)
        {
            this.Draw(spriteBatch, Location, Vector2.Zero, effect);
        }

        int totGameTime = 0;
        bool reversing = false;
        public void Update(int gameTime)
        {
            totGameTime += gameTime;
            if (Frames.ContainsKey(Frame) && Frames.Count > 1)
            {
                if (totGameTime >= Frames[Frame].Delay)
                {
                    totGameTime = 0;
                    if ((Frame + 1) >= Frames.Count)
                        if (!Reverse)
                            Frame = 0;
                        else
                        {
                            reversing = true;
                            Frame--;
                        }
                    else if ((Frame - 1) < 0 && Reverse)
                    {
                        reversing = false;
                        Frame++;
                    }
                    else if (Reverse)
                        if (!reversing)
                            Frame++;
                        else
                            Frame--;
                    else
                        Frame++;
                }
            }
        }
    }
}
