using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using reWZ;
using Microsoft.Xna.Framework;
using reWZ.WZProperties;
using MapleSharp.Engines;
using MapleSharp.Objects.Map;
using Microsoft.Xna.Framework.Input;

namespace MapleSharp.Objects
{
    public class MapleCharacter
    {
        // TODO Add equips/face/effects

        public Vector2 Position = Vector2.Zero;
        public MapleFoothold Foothold;
        public int Layer;

        MapleCanvas Head;
        Dictionary<CharacterState, MapleAnimation> Body = new Dictionary<CharacterState,MapleAnimation>();
        Dictionary<CharacterState, MapleAnimation> Arm = new Dictionary<CharacterState,MapleAnimation>();
        //TODO remember to add hands..

        CharacterColor SkinColor;
        CharacterState State = CharacterState.Standing;
        CharacterDirection Direction = CharacterDirection.Right;
        Rectangle Bounds = new Rectangle(0, 0, 39,63);
        Dictionary<string, Vector2> Navel = new Dictionary<string, Vector2>();

        int Jump = 0;
        int jumpDelay = 0;

        Rectangle FeetCollision = Rectangle.Empty;
        Rectangle LeftCollision = Rectangle.Empty;
        Rectangle RightCollision = Rectangle.Empty;
        Vector2 NeckPosition = Vector2.Zero;

        Dictionary<string, MapleAnimation> Hair = new Dictionary<string, MapleAnimation>();
        Dictionary<string, MapleAnimation> HatHair = new Dictionary<string, MapleAnimation>();
        MapleAnimation Face;

        MapleEquip Top;

        public MapleCharacter(CharacterColor c, int face, int hair, GraphicsDevice graphics)
        {
            try
            {
                SkinColor = c;
                WZFile charWz = new WZFile("Character.wz", WZVariant.GMS, true, WZReadSelection.LowMemory);
                string path = ((int)SkinColor).ToString("00000000") + ".img";

                WZCanvasProperty h = (WZCanvasProperty)charWz.ResolvePath(((int)SkinColor).ToString("00010000") + ".img/front/head");
                WZPointProperty o = (WZPointProperty)h["origin"];
                Head = new MapleCanvas(Tools.BitmapToTexture(graphics, h.Value), new Vector2(-o.Value.X, -o.Value.Y));
                Bounds.X = -o.Value.X;
                Bounds.Y = -o.Value.Y;
                WZPointProperty neckPos = (WZPointProperty)h["map"]["neck"];
                NeckPosition = new Vector2(neckPos.Value.X, neckPos.Value.Y);

                foreach (CharacterState state in Enum.GetValues(typeof(CharacterState))) // Loop through the states and get the frames for each animation..
                {
                    Dictionary<int, MapleFrame> bodyState = new Dictionary<int, MapleFrame>();
                    Dictionary<int, MapleFrame> armState = new Dictionary<int, MapleFrame>();

                    foreach (WZObject frame in charWz.ResolvePath(path + "/" + States[(int)state]))
                        if (Tools.IsNumeric(frame.Name))
                        {
                            int delay = frame["delay"].ValueOrDefault<int>(250);
                            Vector2 bodyNavel = Vector2.Zero;
                            Vector2 frameNeckPos = Vector2.Zero;
                            bool ani = Ani[(int)state];

                            foreach (WZObject part in frame)
                            {
                                WZObject p = part;

                                if (p is WZCanvasProperty)
                                {
                                    Texture2D PartTexture = Tools.BitmapToTexture(graphics, p.ValueOrDie<System.Drawing.Bitmap>());
                                    WZPointProperty origin = (WZPointProperty)p["origin"];

                                    switch (p.Name)
                                    {
                                        case "body":
                                            WZPointProperty navel = (WZPointProperty)p["map"]["navel"];
                                            WZPointProperty neck = (WZPointProperty)p["map"]["neck"];
                                            Navel.Add(States[(int)state] + frame.Name, new Vector2(-navel.Value.X, -navel.Value.Y));
                                            bodyNavel = new Vector2(navel.Value.X, navel.Value.Y);
                                            frameNeckPos = new Vector2(-neck.Value.X, -neck.Value.Y);
                                            Vector2 BodyLoc = new Vector2(-origin.Value.X + frameNeckPos.X + NeckPosition.X, -origin.Value.Y + frameNeckPos.Y + NeckPosition.Y);
                                            bodyState.Add(bodyState.Count, new MapleFrame(new MapleCanvas(PartTexture, BodyLoc), delay));
                                            break;
                                        case "arm":
                                            WZPointProperty armNavel = (WZPointProperty)p["map"]["navel"];
                                            Vector2 ArmLoc = new Vector2((-origin.Value.X + -armNavel.Value.X - -bodyNavel.X) + frameNeckPos.X + NeckPosition.X, (-origin.Value.Y + -armNavel.Value.Y - -bodyNavel.Y) + frameNeckPos.Y + NeckPosition.Y);
                                            armState.Add(armState.Count, new MapleFrame(new MapleCanvas(PartTexture, ArmLoc), delay));
                                            break;
                                    }
                                }
                            }
                            if (ani == false)
                                break;
                        }
                    Body.Add(state, new MapleAnimation(bodyState, true));
                    Arm.Add(state, new MapleAnimation(armState));

                    if (Hair.ContainsKey(HairStates[(int)state]))
                            continue;

                    // Hair
                    string hairPath = "Hair/" + hair.ToString("00000000") + ".img";
                    Dictionary<int, MapleFrame> hf = new Dictionary<int, MapleFrame>();
                    Dictionary<int, MapleFrame> hfh = new Dictionary<int, MapleFrame>();

                    WZCanvasProperty hairCanvas = (WZCanvasProperty)charWz.ResolvePath(hairPath + "/" + HairStates[(int)state] + "/hairOverHead");
                    WZPointProperty hairOrigin = (WZPointProperty)hairCanvas["origin"];
                    WZCanvasProperty hhatCanvas = (WZCanvasProperty)charWz.ResolvePath(hairPath + "/" + HairStates[(int)state] + "/hair");
                    WZPointProperty hhatOrigin = (WZPointProperty)hhatCanvas["origin"];

                    hf.Add(hf.Count, new MapleFrame(new MapleCanvas(Tools.BitmapToTexture(graphics, hairCanvas.Value), new Vector2(-hairOrigin.Value.X - 2, -hairOrigin.Value.Y - 5)), 100));
                    hfh.Add(hfh.Count, new MapleFrame(new MapleCanvas(Tools.BitmapToTexture(graphics, hhatCanvas.Value), new Vector2(-hhatOrigin.Value.X - 2, -hhatOrigin.Value.Y - 5)), 100));

                    Hair.Add(HairStates[(int)state], new MapleAnimation(hf));
                    HatHair.Add(HairStates[(int)state], new MapleAnimation(hfh));
                }

                // Face
                string facePath = "Face/" + face.ToString("00000000") + ".img";
                Dictionary<int, MapleFrame> ff = new Dictionary<int, MapleFrame>();
                foreach (WZObject fs in charWz.ResolvePath(facePath + "/default")) // TODO Other expressions
                {
                    if (fs is WZCanvasProperty && fs.Name == "face")
                    {
                        WZPointProperty faceOrigin = (WZPointProperty)fs["origin"];

                        ff.Add(ff.Count, new MapleFrame(new MapleCanvas(Tools.BitmapToTexture(graphics, fs.ValueOrDie<System.Drawing.Bitmap>()), new Vector2(-faceOrigin.Value.X - 3, -faceOrigin.Value.Y + 7)), 0)); 
                    }
                }
                Face = new MapleAnimation(ff);
            }
            catch (Exception ex)
            {
                MapleConsole.Write(MapleConsole.LogType.ERROR, "Error initializing character: " + ex.Message);
                MapleConsole.Write(MapleConsole.LogType.WARNING, "StackTrace", ex.StackTrace);
            }
        }

        public void SetTop(GraphicsDevice g, int id) // TODO add rest of equip types..
        {
            Top = new MapleEquip(g, id, MapleEquip.EquipType.Top);
        }

        public void Draw(SpriteBatch spriteBatch)
        { 
            DrawCharacter(spriteBatch, Direction, Position);
            
            if (Constants.Game.CharacterBoundBox)
            {
                Texture2D tex = Tools.EmptyTexture(spriteBatch.GraphicsDevice, Color.White);
                spriteBatch.Draw(tex, new Rectangle((int)Position.X + Bounds.X, (int)Position.Y + Bounds.Y, Bounds.Width, 1), Color.Black);
                spriteBatch.Draw(tex, new Rectangle((int)Position.X + Bounds.X, (int)Position.Y + Bounds.Y, 1, Bounds.Height), Color.Black);
                spriteBatch.Draw(tex, new Rectangle((int)Position.X + Bounds.X + Bounds.Width, (int)Position.Y + Bounds.Y, 1, Bounds.Height), Color.Black);
                spriteBatch.Draw(tex, new Rectangle((int)Position.X + Bounds.X, (int)Position.Y + Bounds.Height + Bounds.Y, Bounds.Width, 1), Color.Black);
                spriteBatch.Draw(tex, FeetCollision, Color.Red);
            }
        }

        public void DrawCharacter(SpriteBatch spriteBatch, CharacterDirection dir, Vector2 pos)
        {
            Vector2 AllOffset = Vector2.Zero;
            if (State == CharacterState.Prone)
            {
                AllOffset += new Vector2(-38, 26);
            }

            if (dir == CharacterDirection.Right)
                AllOffset.X = -AllOffset.X;

            if (dir == CharacterDirection.Left)
            {
                if (Body != null && Body.ContainsKey(State))
                    Body[State].Draw(spriteBatch, pos, AllOffset);
                if (Head != null)
                    Head.Draw(spriteBatch, pos, AllOffset);
                if (Face != null)
                    Face.Draw(spriteBatch, pos, AllOffset);
                if (Hair != null)
                    Hair[HairStates[(int)State]].Draw(spriteBatch, pos, AllOffset);

                // Draw Equip Body
                //if (Top != null)
                    //Top.DrawBody(spriteBatch, pos, Navel[States[(int)State] + Body[State].Frame.ToString()]); // wtf.. this doesn't work? todo this

                // Draw player arm (has to be in the middle of equip back and front
                if (Arm != null && Arm.ContainsKey(State))
                    Arm[State].Draw(spriteBatch, pos, AllOffset);

                // Draw Equip Arms
                //if (Top != null)
                    //Top.DrawArm(spriteBatch, pos, Navel[States[(int)State] + Body[State].Frame.ToString()]);
            }
            else
            {
                if (Body != null && Body.ContainsKey(State))
                {
                    MapleCanvas bodyCanvas = Body[State].Frames[Body[State].Frame].Canvas;
                    Vector2 BodyOffset = new Vector2(-bodyCanvas.Origin.X - bodyCanvas.Texture.Width, bodyCanvas.Origin.Y);
                    Body[State].Draw(spriteBatch, pos, (BodyOffset + AllOffset) - bodyCanvas.Origin, SpriteEffects.FlipHorizontally);
                }
                if (Head != null)
                {
                    Vector2 HeadOffset = new Vector2(-Head.Origin.X - Head.Texture.Width, Head.Origin.Y);
                    Head.Draw(spriteBatch, pos, (HeadOffset + AllOffset) - Head.Origin, SpriteEffects.FlipHorizontally);
                }
                if (Face != null)
                {
                    MapleCanvas FaceCanvas = Face.Frames[Face.Frame].Canvas;
                    Vector2 FaceOffset = new Vector2(-FaceCanvas.Origin.X - FaceCanvas.Texture.Width, FaceCanvas.Origin.Y);
                    Face.Draw(spriteBatch, pos, (FaceOffset + AllOffset) - FaceCanvas.Origin, SpriteEffects.FlipHorizontally);
                }
                if (Hair != null)
                {
                    MapleCanvas HairCanvas = Hair[HairStates[(int)State]].Frames[Hair[HairStates[(int)State]].Frame].Canvas;
                    Vector2 HairOffset = new Vector2(-HairCanvas.Origin.X - HairCanvas.Texture.Width, HairCanvas.Origin.Y);
                    Hair[HairStates[(int)State]].Draw(spriteBatch, pos, (HairOffset + AllOffset) - HairCanvas.Origin, SpriteEffects.FlipHorizontally);
                }
                if (Arm != null && Arm.ContainsKey(State))
                {
                    MapleCanvas armCanvas = Arm[State].Frames[Arm[State].Frame].Canvas;
                    Vector2 ArmOffset = new Vector2(-armCanvas.Origin.X - armCanvas.Texture.Width, armCanvas.Origin.Y);
                    Arm[State].Draw(spriteBatch, pos, (ArmOffset + AllOffset) - armCanvas.Origin, SpriteEffects.FlipHorizontally);
                }
            }
        }


        private KeyboardState oldState;
        public void Update(int gameTime) //TODO Camera following // TODO Make the physics more like real maple -_- // TODO Ladders/Ropes
        {
            if (!Constants.Game.CameraLimit)
                if (Mouse.GetState().RightButton == ButtonState.Pressed)
                    this.Position = new Vector2(Mouse.GetState().X + Camera.Position.X, Mouse.GetState().Y + Camera.Position.Y);

            if (Body != null && Body.ContainsKey(State))
                Body[State].Update(gameTime);
            if (Arm != null && Arm.ContainsKey(State))
                Arm[State].Frame = Body[State].Frame;
            if (Top != null)
            {
                Top.ani[State].Frame = Body[State].Frame;
                Top.armAni[State].Frame = Body[State].Frame;
            }

            LeftCollision = new Rectangle((int)this.Position.X - 1, (int)this.Position.Y + Bounds.Y + Bounds.Height - 7, 1, 6);
            RightCollision = new Rectangle((int)this.Position.X + 1, (int)this.Position.Y + Bounds.Y + Bounds.Height - 7, 1, 6);
            FeetCollision = new Rectangle((int)this.Position.X, (int)this.Position.Y + Bounds.Y + Bounds.Height - 7, 1, 8);

            Foothold = null;
            bool wallLeft = false;
            bool wallRight = false;
            foreach (MapleFoothold fh in MapEngine.CurrentMap.Footholds) // TODO: Slopes.
            {
                if (fh.Intersects(FeetCollision))
                {
                    Foothold = fh;
                    Layer = fh.Layer;
                }

                if (fh.Intersects(LeftCollision))
                    wallLeft = true;
                if (fh.Intersects(RightCollision))
                    wallRight = true;
            }
            bool touchingFoothold = (Foothold != null);

            if (touchingFoothold)
            {
                this.Position.Y = (Foothold.StartPos.Y - this.Bounds.Height) + -(int)Head.Origin.Y;
                FeetCollision.Y = (int)this.Position.Y + Bounds.Y + Bounds.Height - 7;
            }

            if (!touchingFoothold && Jump <= 0)
            {
                this.Position.Y += Constants.Physics.FallSpeed;
            }
            else if (Jump > 0 && Jump < Constants.Physics.JumpHeight)
            {
                Jump++; // TODO Jump velocity.... get rid of this quick fix XD
                this.Position.Y -= Constants.Physics.JumpSpeed;
            }
            else if (Jump >= Constants.Physics.JumpHeight)
            {
                jumpDelay = Constants.Physics.JumpDelay;
                Jump = 0;
            }

            if (this.Position.Y > MapEngine.CurrentMap.Bottom)
                    this.Position.Y = MapEngine.CurrentMap.Top;

            KeyboardState keyboard = Keyboard.GetState();

            this.State = CharacterState.Standing;

            if ((keyboard.IsKeyUp(Keys.LeftAlt) || keyboard.IsKeyUp(Keys.RightAlt)) && (oldState.IsKeyDown(Keys.LeftAlt) || oldState.IsKeyDown(Keys.RightAlt)))
                jumpDelay = 0;

            if (keyboard.IsKeyDown(Keys.Down) && touchingFoothold)
                this.State = CharacterState.Prone;

            if (this.State != CharacterState.Prone)
            {
                if (keyboard.IsKeyDown(Keys.Right) && this.Position.X < MapEngine.CurrentMap.Right && !wallRight)
                {
                    // TODO: Check edges
                    Position.X += Constants.Physics.WalkSpeed * ((Jump > 0 || !touchingFoothold) ? Constants.Physics.FallMovementPercent : 1);
                    this.State = CharacterState.Walking;
                    this.Direction = CharacterDirection.Right;
                }
                if (keyboard.IsKeyDown(Keys.Left) && this.Position.X > MapEngine.CurrentMap.Left && !wallLeft)
                {
                    Position.X -= Constants.Physics.WalkSpeed * ((Jump > 0 || !touchingFoothold) ? Constants.Physics.FallMovementPercent : 1);
                    this.State = CharacterState.Walking;
                    this.Direction = CharacterDirection.Left;
                }
                if (keyboard.IsKeyDown(Keys.Up))
                {
                    if (MapEngine.MapLoaded)
                        foreach (MaplePortal portal in MapEngine.CurrentMap.Portals)
                        {
                            if (portal.Bounds.Intersects(FeetCollision))
                            {
                                MapEngine.LoadMap(portal.toMapId, portal.SpawnName);
                                break;
                            }
                        }
                }
                if ((keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt)) && Jump <= 0 && touchingFoothold && jumpDelay <= 0)
                {
                    Jump = 1;
                    this.Position.Y -= 1;
                }
            }
            else if ((keyboard.IsKeyDown(Keys.LeftAlt) && oldState.IsKeyUp(Keys.LeftAlt)) || (keyboard.IsKeyDown(Keys.RightAlt) && oldState.IsKeyUp(Keys.RightAlt)))
                this.Position.Y += 10; // Place under foothold?

            if (jumpDelay > 0)
                jumpDelay -= gameTime;

            if (Jump > 0)
                State = CharacterState.Jump;
            oldState = keyboard;

            if (Constants.Game.CameraLimit)
            {
                if ((this.Position.X < Camera.Position.X + (Constants.Game.ScreenWidth / 3) && Direction == CharacterDirection.Left))
                    Camera.SetX((int)this.Position.X - (Constants.Game.ScreenWidth / 3));
                else if ((this.Position.X > Camera.Position.X + Constants.Game.ScreenWidth - (Constants.Game.ScreenWidth / 3) && Direction == CharacterDirection.Right))
                    Camera.SetX((int)this.Position.X - Constants.Game.ScreenWidth + (Constants.Game.ScreenWidth / 3));
                if ((this.Position.Y < Camera.Position.Y + (Constants.Game.ScreenHeight / 3)))
                    Camera.SetY((int)this.Position.Y - (Constants.Game.ScreenHeight / 3));
                if ((this.Position.Y > Camera.Position.Y + Constants.Game.ScreenHeight - (Constants.Game.ScreenHeight / 3)))
                    Camera.SetY((int)this.Position.Y - Constants.Game.ScreenHeight + (Constants.Game.ScreenHeight / 3));
            }
        }

        public void CenterCamera()
        {
            Camera.Set((int)this.Position.X - (Constants.Game.ScreenWidth / 2), (int)this.Position.Y - (Constants.Game.ScreenHeight / 2));
        }

        public enum CharacterDirection
        {
            Left,
            Right
        }

        public enum CharacterColor
        {
            Light = 2000,
            Tanned = 2001,
            Dark = 2002,
            Pale = 2003,
            Ghost = 2004,
            Green = 2005,
            White = 2009,
            // TODO: add the rest..
        }

        public enum CharacterState // Numbers corrospond to the string[] States..
        { // TODO add rest of states
            Standing = 0,
            Walking = 1,
            Prone = 2,
            Jump = 3
        }
        public static string[] States = { "stand1", "walk1", "proneStab", "jump" };// This my children, I call... Getting lazy.
        string[] HairStates = { "default", "default", "default", "default" };
        bool[] Ani = { true, true, false, true };
    }
}
