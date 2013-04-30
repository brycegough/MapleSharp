using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using reWZ;
using reWZ.WZProperties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MapleSharp.Objects
{
    public static class MapleCursor
    {

        static Dictionary<int, MapleAnimation> cursorFrames = new Dictionary<int, MapleAnimation>();
        static int curAni = 0;

        public static void LoadCursor(GraphicsDevice graphics, WZSubProperty cursor)
        {
            foreach (WZSubProperty ani in cursor)
            {
                Dictionary<int, MapleFrame> frames = new Dictionary<int, MapleFrame>();

                foreach (WZObject frame in ani)
                    if (frame is WZCanvasProperty)
                    {
                        Texture2D texture = Tools.BitmapToTexture(graphics, ((WZCanvasProperty)frame).Value);
                        Vector2 offset = new Vector2(0, 0) ;
                        int aniDelay = 100;

                        foreach (WZObject offs in frame)
                            if (offs is WZPointProperty && offs.Name == "origin")
                                offset = new Vector2(((WZPointProperty)offs).Value.Y, ((WZPointProperty)offs).Value.X);
                            else if (offs is WZInt32Property && offs.Name == "delay")
                                aniDelay = ((WZInt32Property)offs).Value;

                        frames.Add(int.Parse(frame.Name), new MapleFrame(new MapleCanvas(texture,offset), aniDelay));
                    }

                cursorFrames.Add(int.Parse(ani.Name), new MapleAnimation(frames));
            }
        }

        public static void Draw(SpriteBatch spriteBatch, MouseState mouse)
        {
            Vector2 pos = new Vector2(mouse.X, mouse.Y);

            if (mouse.LeftButton == ButtonState.Released)
                curAni = 0;
            else
                curAni = 12;

            cursorFrames[curAni].Draw(spriteBatch, pos);
        }

        public static void Update(int gameTime)
        {
            cursorFrames[curAni].Update(gameTime);
        }

    }
}
