using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Gdi = System.Drawing;
using System.IO;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework;
using MapleSharp.Objects.Map;

namespace MapleSharp
{
    public static class Tools
    {

        public static Texture2D BitmapToTexture(GraphicsDevice device, Gdi.Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png); // Save the bitmap to memory
            bitmap.Dispose(); // Dispose the bitmap object

            Texture2D tex = Texture2D.FromStream(device, ms); // Load the texture from the bitmap in memory
            ms.Close(); // Close the memorystream
            ms.Dispose(); // Dispose the memorystream
            return tex; // return the texture
        }

        public static Texture2D EmptyTexture(GraphicsDevice device, Color c)
        {
            Texture2D tex = new Texture2D(device, 1, 1);
            tex.SetData<Color>(new Color[] {c});
            return tex;
        }

        public static bool IsNumeric(string txt)
        {
            double output;
            return double.TryParse(txt, out output);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 StartPoint, Vector2 EndPoint, Color c)
        {
            float angle = (float)Math.Atan2(EndPoint.Y - StartPoint.Y, EndPoint.X - StartPoint.X);
            float length = Vector2.Distance(StartPoint, EndPoint);

            spriteBatch.Draw(Tools.EmptyTexture(spriteBatch.GraphicsDevice, c), new Rectangle((int)StartPoint.X, (int)StartPoint.Y, (int)length, 1), null, c, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
