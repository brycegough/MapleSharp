using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using reWZ;
using reWZ.WZProperties;

namespace MapleSharp.Objects
{
    public class MapleEquip
    {
        public Dictionary<MapleCharacter.CharacterState, MapleAnimation> ani = new Dictionary<MapleCharacter.CharacterState,MapleAnimation>();
        public Dictionary<MapleCharacter.CharacterState, MapleAnimation> armAni = new Dictionary<MapleCharacter.CharacterState, MapleAnimation>();
        MapleCharacter.CharacterState State = MapleCharacter.CharacterState.Standing;

        public MapleEquip(GraphicsDevice graphics, int id, EquipType type)
        {
            // Load animations..
            WZFile charWz = new WZFile("Character.wz", WZVariant.GMS, true, WZReadSelection.LowMemory);
            string equipPath = EquipTypes[(int)type] + "/" + id.ToString("00000000") + ".img";
            Vector2 extraOffset = Vector2.Zero;

            switch (type)
            {
                case EquipType.Top:
                    extraOffset = new Vector2(2, 25);
                    break;
            }

            foreach (MapleCharacter.CharacterState state in Enum.GetValues(typeof(MapleCharacter.CharacterState))) // Loop through the states and get the frames for each animation..
            {
                Dictionary<int, MapleFrame> eFrames = new Dictionary<int, MapleFrame>();
                Dictionary<int, MapleFrame> aFrames = new Dictionary<int, MapleFrame>();
                string statePath = equipPath + "/" + MapleCharacter.States[(int)state];

                foreach (WZObject frame in charWz.ResolvePath(statePath))
                {
                    foreach (WZObject bit in frame)
                    {
                        if (bit is WZCanvasProperty)
                        {
                            WZPointProperty origin = (WZPointProperty)bit["origin"];
                            switch (bit.Name)
                            {
                                case "mail":
                                    eFrames.Add(int.Parse(frame.Name), new MapleFrame(new MapleCanvas(Tools.BitmapToTexture(graphics, bit.ValueOrDie<System.Drawing.Bitmap>()), new Vector2(-origin.Value.X, -origin.Value.Y)), 100));
                                    break;
                                case "mailArm":
                                    aFrames.Add(int.Parse(frame.Name), new MapleFrame(new MapleCanvas(Tools.BitmapToTexture(graphics, bit.ValueOrDie<System.Drawing.Bitmap>()), new Vector2(-origin.Value.X, -origin.Value.Y)), 100));
                                    break;
                            }

                        }
                    }
                }
                ani.Add(state, new MapleAnimation(eFrames));
                armAni.Add(state, new MapleAnimation(aFrames));
            }
        }

        public void DrawBody(SpriteBatch spriteBatch, Vector2 Position, Vector2 Origin, SpriteEffects effect = SpriteEffects.None)
        {
            if (ani.ContainsKey(State))
            {
                ani[State].Draw(spriteBatch, Position, Origin, effect);
            }
        }

        public void DrawArm(SpriteBatch spriteBatch, Vector2 Position, Vector2 Origin, SpriteEffects effect = SpriteEffects.None)
        {
            if (armAni.ContainsKey(State))
            {
                armAni[State].Draw(spriteBatch, Position, Origin, effect);
            }
        }

        public void Update(int gameTime, MapleCharacter.CharacterState state, int charFrame)
        {
            State = state;
            if (ani.ContainsKey(state) && ani[state].Frames.Count > charFrame)
            {
                ani[state].Frame = charFrame;
                ani[state].Update(gameTime);
            } // wut?o.o
            if (armAni.ContainsKey(state) && armAni[state].Frames.Count > charFrame)
            {
                armAni[state].Frame = charFrame;
                armAni[state].Update(gameTime);
            }
        }

        public enum EquipType
        {
            Top = 0
        }
        string[] EquipTypes = { "Coat" };

    }
}
