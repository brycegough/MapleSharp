using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MapleSharp.Engines;

namespace MapleSharp.Objects.Map
{
    public class MapleBackground
    {
        // TODO Completely fix backgrounds..

        public MapleAnimation Animation;
        public Vector2 Location = Vector2.Zero;
        public BackgroundTypes Type = BackgroundTypes.NoTile;
        public bool IsBack = false;
        public int Index = 0; //TODO: Different speeds for different index's
        public int Width = 0;
        public int Height = 0;

        public MapleBackground(int i, MapleAnimation ani, Vector2 loc, int type, bool isBack = false)
        {
            Width = ani.Frames[0].Canvas.Texture.Width;
            Height = ani.Frames[0].Canvas.Texture.Height;
            Index = i;
            Type = (BackgroundTypes)Enum.ToObject(typeof(BackgroundTypes), type);
            IsBack = isBack;
            Animation = ani;
            Location = loc;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 newLocation = Location;

            if (!IsBack) // These algorithm's are complete guesses of what seems right so no hate.. c:
                newLocation = new Vector2(Location.X - -(Camera.Position.X / 2.5f), Location.Y + (Camera.Position.Y / 4) + (MapEngine.CurrentMap.BackHeight / 4));

           switch(Type)
           {
               case BackgroundTypes.NoTile:
                   Animation.Draw(spriteBatch, newLocation);
                    break;
               case BackgroundTypes.HorizontalTile:
                    for (int w = 0; w < Math.Ceiling((double)MapEngine.CurrentMap.Width / (double)Animation.Frames[0].Canvas.Texture.Width) + 2; w++)
                        Animation.Draw(spriteBatch, newLocation + new Vector2(w * Animation.Frames[0].Canvas.Texture.Width, 0));
                    break;
               case BackgroundTypes.VerticalTile:
                    for (int h = 0; h < Math.Ceiling((double)MapEngine.CurrentMap.Height / (double)Animation.Frames[0].Canvas.Texture.Height) + 2; h++)
                        Animation.Draw(spriteBatch, newLocation + new Vector2(0, h * Animation.Frames[0].Canvas.Texture.Height));
                    break;
               case BackgroundTypes.BothTile:
                   for (int w = 0; w < Math.Ceiling((double)MapEngine.CurrentMap.Width / (double)Animation.Frames[0].Canvas.Texture.Width) + 2; w++)
                       for (int h = 0; h < Math.Ceiling((double)MapEngine.CurrentMap.Height / (double)Animation.Frames[0].Canvas.Texture.Height) + 2; h++)
                           Animation.Draw(spriteBatch, newLocation + new Vector2(w * Animation.Frames[0].Canvas.Texture.Width, h * Animation.Frames[0].Canvas.Texture.Height));
                   break;
               case BackgroundTypes.ScrollTileHorizontal:
                   goto case BackgroundTypes.HorizontalTile;
               case BackgroundTypes.ScrollTileVertical:
                   goto case BackgroundTypes.VerticalTile;
               case BackgroundTypes.ScrollHorizontalTileBoth:
                   goto case BackgroundTypes.BothTile;
               case BackgroundTypes.ScrollVerticalTileBoth:
                   goto case BackgroundTypes.BothTile;
        }
        }

        int move = 0;
        public void Update(int gameTime) 
        {
            Animation.Update(gameTime);
            switch (Type)
            {
                case BackgroundTypes.ScrollTileHorizontal:
                    this.Location.X -= 1;
                    move += 1;
                    if (move >= this.Width)
                    {
                        move -= this.Width;
                        this.Location.X += this.Width - move;
                    }
                    break;
                case BackgroundTypes.ScrollHorizontalTileBoth:
                    goto case BackgroundTypes.ScrollTileHorizontal;
            // TODO: Vertical Scroll
            }
        }


        public enum BackgroundTypes
        {
            NoTile = 0,
            HorizontalTile = 1,
            VerticalTile = 2,
            BothTile = 3,
            ScrollTileHorizontal = 4,
            ScrollTileVertical = 5,
            ScrollHorizontalTileBoth = 6,
            ScrollVerticalTileBoth = 7,
        }

    }
}
