using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MapleSharp.Engines;

namespace MapleSharp
{
    public static class Camera
    {

        private static float _zoom = 1f;  // Default Camera Zoom 
        private static Vector2 _pos = Vector2.Zero;
        private static float _rotation = 0.0f;

        // Sets and gets zoom
        static float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        // Sets and Gets the rotation for the camera
        static float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        // Gets and sets position
        public static Vector2 Position
        {
            get { return _pos; }
        }

        public static void Set(int x, int y)
        {
            SetX(x);
            SetY(y);
        }

        public static void SetX(int amount)
        {
            if (MapEngine.CurrentMap.Left > amount && Constants.Game.CameraLimit)
                _pos = new Vector2(MapEngine.CurrentMap.Left, Position.Y);
            else if (MapEngine.CurrentMap.Right < amount + Constants.Game.ScreenWidth && Constants.Game.CameraLimit)
                _pos = new Vector2(MapEngine.CurrentMap.Right - Constants.Game.ScreenWidth, Position.Y);
            else
                _pos = new Vector2(amount, Position.Y);
        }

        public static void MoveX(int amount)
        {
            SetX((int)Position.X + amount);
        }

        public static void SetY(int amount)
        {
            if (MapEngine.CurrentMap.Top > amount && Constants.Game.CameraLimit)
                _pos = new Vector2(Position.X, MapEngine.CurrentMap.Top);
            else if (MapEngine.CurrentMap.Bottom < amount + Constants.Game.ScreenHeight && Constants.Game.CameraLimit)
                _pos = new Vector2(Position.X, MapEngine.CurrentMap.Bottom - Constants.Game.ScreenHeight);
            else
                _pos = new Vector2(Position.X, amount);
        }

        public static void MoveY(int amount)
        {
            SetY((int)Position.Y + amount);
        }

        public static Matrix GetMatrix(GraphicsDevice graphicsDevice)
        {
            return Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                   Matrix.CreateTranslation(new Vector3(0, 0, 0));
        }
    }
}
