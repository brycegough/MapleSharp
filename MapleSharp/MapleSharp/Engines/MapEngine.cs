using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using reWZ;
using reWZ.WZProperties;
using MapleSharp.Objects;
using Microsoft.Xna.Framework;
using MapleSharp.Objects.Map;
using System.Diagnostics;

namespace MapleSharp.Engines
{
    public static class MapEngine
    {
        public static MapleMap CurrentMap;
        private static GraphicsDevice _device;

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (CurrentMap != null)
                CurrentMap.Draw(spriteBatch);
            if (CurrentMap != null && !Constants.Game.CameraLimit)
            { // Draw the map border if the camera is allowed out of it..
                Texture2D tex = Tools.EmptyTexture(spriteBatch.GraphicsDevice, Color.White);
                spriteBatch.Draw(tex, new Rectangle(CurrentMap.Left, CurrentMap.Top, 1, CurrentMap.Height), Color.White);
                spriteBatch.Draw(tex, new Rectangle(CurrentMap.Left, CurrentMap.Top, CurrentMap.Width, 1), Color.White);
                spriteBatch.Draw(tex, new Rectangle(CurrentMap.Left, CurrentMap.Top + CurrentMap.Height, CurrentMap.Width, 1), Color.White);
                spriteBatch.Draw(tex, new Rectangle(CurrentMap.Left + CurrentMap.Width, CurrentMap.Top, 1, CurrentMap.Height), Color.White);
            }
        }

        public static void Update(int gameTime)
        {
            if (CurrentMap != null)
                CurrentMap.Update(gameTime);
        }

        public static void LoadMap(int id, string spawn = "")
        {
            if (_device != null)
                LoadMap(null, id, spawn);
            else
                throw new Exception("Graphics Device hasn't been loaded, please load atleast 1 map with the GraphicsDevice property set!");
        }

        public static bool MapLoaded = false;
        static Stopwatch sw = new Stopwatch();
        public static void LoadMap(GraphicsDevice graphics, int id, string spawn = "") //TODO: Fade
        {
            if (id == 999999999) // This is used to tell it not to load a new map..
                return;

            sw.Restart();
            MapLoaded = false;

            if (graphics != null)
                _device = graphics;

            if (Constants.Globals.Player == null)
            {
                Constants.Globals.Player = new MapleCharacter(MapleCharacter.CharacterColor.Light, 20000, 33400, _device);
                Constants.Globals.Player.SetTop(graphics, 1040002);
            }

            MapleMap map = new MapleMap();
            map.MapID = id;

            MapleConsole.Write(MapleConsole.LogType.MESSAGE, "Loading Map \"" + map.FormattedMapID + "\".");

            string path = "Map/Map" + map.FormattedMapID.Substring(0, 1) + "/" + map.FormattedMapID + ".img";
            WZFile mapWz = new WZFile("Map.wz", WZVariant.GMS, true, WZReadSelection.LowMemory);
            try { mapWz.ResolvePath(path); } // Make sure the map exists
            catch (Exception) { MapleConsole.Write(MapleConsole.LogType.ERROR, "Map Doesn't Exist!"); return; }
            
            if (!Constants.Game.MuteMusic)
                MusicEngine.LoadMusic(mapWz.ResolvePath(path + "/info/bgm").ValueOrDefault<string>("")); // Load the music

            // TODO: Get map name

            bool boundsDefined = false;
            try
            {
                map.Top = mapWz.ResolvePath(path + "/info/VRTop").ValueOrDefault<int>(0); 
                map.Bottom = mapWz.ResolvePath(path + "/info/VRBottom").ValueOrDefault<int>(0);
                map.Left = mapWz.ResolvePath(path + "/info/VRLeft").ValueOrDefault<int>(0);
                map.Right = mapWz.ResolvePath(path + "/info/VRRight").ValueOrDefault<int>(0);
                boundsDefined = true;
            }
            catch (Exception)
            { // TODO: Find out the coords for maps that don't tell you the camera limit..
                //map.Top = 0;
                //map.Bottom = map.Top + 2009; // I got these values from orbis but I'm not sure whats going to happen on other maps..
            }

            map.Width = map.Right - map.Left;
            map.Height = map.Bottom - map.Top;

            map.HideMinimap = mapWz.ResolvePath(path + "/info/hideMinimap").ValueOrDefault<int>(1) == 1 ? true : false;

            //int miniw = mapWz.ResolvePath(path + "/miniMap/width").ValueOrDefault<int>(0);
            //int minih = mapWz.ResolvePath(path + "/miniMap/height").ValueOrDefault<int>(0);

            // Set the global portal animation if its null
            if (Constants.Globals.PortalAnimation == null)
            {
                Dictionary<int, MapleFrame> frames = new Dictionary<int, MapleFrame>();
                foreach (WZCanvasProperty frame in mapWz.ResolvePath("MapHelper.img/portal/game/pv"))
                {
                    WZPointProperty offset = (WZPointProperty)frame["origin"];
                    frames.Add(int.Parse(frame.Name), new MapleFrame(
                                                        new MapleCanvas(Tools.BitmapToTexture(_device, frame.Value), 
                                                        new Vector2(-offset.Value.X, -offset.Value.Y)), 100)); 
                                                        // wtf.. The portal frame delay isn't in the wz files?!
                }
                Constants.Globals.PortalAnimation = new MapleAnimation(frames);
            }


            #region Load Footholds

            foreach (WZObject fsp in mapWz.ResolvePath(path + "/foothold"))
                foreach (WZObject fs in fsp)
                    foreach (WZObject fh in fs)
                    {
                        map.Footholds.Add(new MapleFoothold(int.Parse(fh.Name), new Vector2(fh["x1"].ValueOrDie<int>(), fh["y1"].ValueOrDie<int>()), new Vector2(fh["x2"].ValueOrDie<int>(), fh["y2"].ValueOrDie<int>()), int.Parse(fsp.Name)));
                    }

            #endregion

            #region Load Backgrounds

            foreach (WZObject bg in mapWz.ResolvePath(path + "/back"))
            {
                if (bg is WZSubProperty && Tools.IsNumeric(bg.Name))
                {
                    try
                    {
                        string fPath = "Back/" + bg["bS"].ValueOrDefault<string>("") + ".img/back/" + bg["no"].ValueOrDefault<int>(0);
                        WZCanvasProperty frame = (WZCanvasProperty)mapWz.ResolvePath(fPath);
                        WZPointProperty origin = (WZPointProperty)frame["origin"];
                        Dictionary<int, MapleFrame> bgs = new Dictionary<int, MapleFrame>();
                        bool isBack = bg["no"].ValueOrDefault<int>(-1) == 0;

                        if (!Constants.Globals.BackCache.ContainsKey(fPath))
                            Constants.Globals.BackCache.Add(fPath, Tools.BitmapToTexture(_device, frame.Value));
                        // TODO: Animated backgrounds..

                        // Again these algorithms are guessed.. Took me hours to get it realistic.. I pretty much just added random shit onto it XD
                        float newX = bg["x"].ValueOrDefault<int>(0) - bg["rx"].ValueOrDefault<int>(0) / 100 * (map.Width + (_device.Viewport.Width / 2)) + (map.Left * 1.65f);
                        float newY = bg["y"].ValueOrDefault<int>(0) - 100 + bg["ry"].ValueOrDefault<int>(0) / 100 * (map.Height + (_device.Viewport.Height / 2)) + map.Top + (map.Height - 300); // 300.. random

                        if (isBack)
                        {
                            newX = map.Left;
                            newY = map.Top;
                        }

                        bgs.Add(0, new MapleFrame(new MapleCanvas(Constants.Globals.BackCache[fPath], new Vector2(-origin.Value.X, -origin.Value.Y)), 100));

                        if (newX + -origin.Value.X > map.BackWidth)
                        {
                            int bW = (int)Math.Ceiling(newX);
                            map.BackWidth = bW + -origin.Value.X;
                        }
                        if (-newY + -origin.Value.Y > map.BackHeight) // Somethings up with this o.o why do we need to make newY negative?
                        {
                            int bH = (int)Math.Ceiling(-newY);
                            map.BackHeight = bH + -origin.Value.Y;
                        }

                        map.Backgrounds.Add(new MapleBackground(int.Parse(bg.Name), new MapleAnimation(bgs), new Vector2(newX, newY), bg["type"].ValueOrDie<int>(), isBack));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            #endregion

            #region Load Tiles/Objects
            foreach (WZObject wLayer in mapWz.ResolvePath(path))
            {
                if (wLayer is WZSubProperty && Tools.IsNumeric(wLayer.Name))
                {
                    string tS = "";
                    List<MapleTile> tiles = new List<MapleTile>();
                    List<MapleObject> objs = new List<MapleObject>();
                    bool hasSetBounds = false;

                    foreach (WZSubProperty item in wLayer)
                    {
                        switch (item.Name)
                        {
                            case "info":
                                if (item.ChildCount > 0)
                                    tS = item["tS"].ValueOrDefault<string>("");
                                break;
                            case "obj":
                                foreach (WZObject wObj in item)
                                {
                                    string oS = wObj["oS"].ValueOrDie<string>();
                                    string l0 = wObj["l0"].ValueOrDie<string>();
                                    string l1 = wObj["l1"].ValueOrDie<string>();
                                    string l2 = wObj["l2"].ValueOrDie<string>();
                                    string objPath = "Obj/" + oS + ".img/" + l0 + "/" + l1 + "/" + l2;
                                    if (l1 == "cherryBlossom")  // Skip this shit.. its all over henesys..
                                        continue;

                                    Dictionary<int, MapleFrame> frames = new Dictionary<int, MapleFrame>();
                                    int z = wObj["z"].ValueOrDefault<int>(0);

                                    foreach (WZObject oF in mapWz.ResolvePath(objPath))
                                    {
                                        if (oF is WZCanvasProperty && Tools.IsNumeric(oF.Name))
                                        {
                                            try
                                            {
                                                WZPointProperty offset = null;
                                                int frameDelay = 150;
                                                
                                                if (!Constants.Globals.ObjectCache.ContainsKey(objPath + "/" + oF.Name))
                                                    Constants.Globals.ObjectCache.Add(objPath + "/" + oF.Name, Tools.BitmapToTexture(_device, ((WZCanvasProperty)oF).Value));
                                                
                                                foreach (WZObject o in oF)
                                                    if (o.Name == "origin")
                                                        offset = (WZPointProperty)o;
                                                    else if (o.Name == "delay")
                                                        frameDelay = o.ValueOrDefault<int>(150);
                                                frames.Add(int.Parse(oF.Name), new MapleFrame(new MapleCanvas(Constants.Globals.ObjectCache[objPath + "/" + oF.Name], (offset != null) ? new Vector2(-offset.Value.X, -offset.Value.Y) : Vector2.Zero), frameDelay));
                                            
                                            }
                                            catch (Exception ex)
                                            {
                                                MapleConsole.Write(MapleConsole.LogType.ERROR, "Error loading object \"" + objPath + "\": " + ex.Message);
                                            }
                                        }
                                    }

                                    objs.Add(new MapleObject(new Vector2(wObj["x"].ValueOrDefault<int>(0), wObj["y"].ValueOrDefault<int>(0)), z, new MapleAnimation(frames), int.Parse(wLayer.Name)));
                                }

                                break;
                            case "tile": 
                                foreach (WZObject wTile in item)
                                {
                                    MapleCanvas tileTex = null;
                                    Vector2 tileLoc = Vector2.Zero;
                                    string tilePath = "Tile/" + tS + ".img/" + wTile["u"].ValueOrDie<string>() + "/" + wTile["no"].ValueOrDefault<int>(0).ToString();
                                    WZCanvasProperty tileCanvas = (WZCanvasProperty)mapWz.ResolvePath(tilePath);
                                    
                                    if (!Constants.Globals.TileCache.ContainsKey(tilePath))
                                        Constants.Globals.TileCache.Add(tilePath, Tools.BitmapToTexture(_device, tileCanvas.Value));

                                    WZPointProperty tOrigin = (WZPointProperty)tileCanvas["origin"];
                                    tileTex = new MapleCanvas(Constants.Globals.TileCache[tilePath], new Vector2(-tOrigin.Value.X, -tOrigin.Value.Y));
                                    tileLoc = new Vector2(wTile["x"].ValueOrDefault<int>(0), wTile["y"].ValueOrDefault<int>(0));

                                    if (!boundsDefined)
                                    { // TODO Fix this..
                                        if (!hasSetBounds)
                                        {
                                            map.Left = (int)tileLoc.X;
                                            map.Right = (int)tileLoc.X + tileTex.Texture.Width;
                                            map.Top = (int)tileLoc.Y;
                                            map.Bottom = (int)tileLoc.Y + tileTex.Texture.Height;
                                            hasSetBounds = true;
                                        }
                                        else
                                        {
                                            if (tileLoc.X < map.Left)
                                                map.Left = (int)tileLoc.X + -tOrigin.Value.X;
                                            if (tileLoc.X + tileTex.Texture.Width > map.Right)
                                                map.Right = (int)tileLoc.X + tileTex.Texture.Width + -tOrigin.Value.X;
                                            if (tileLoc.Y < map.Top)
                                                map.Top = (int)tileLoc.Y + -tOrigin.Value.Y;
                                            if (tileLoc.Y + tileTex.Texture.Height > map.Bottom)
                                                map.Bottom = (int)tileLoc.Y + tileTex.Texture.Height + -tOrigin.Value.Y;
                                        }
                                    }

                                    tiles.Add(new MapleTile(tileTex, tileLoc, int.Parse(wTile.Name)));
                                }
                                break;
                            default:
                                //lolwut o.o
                                MapleConsole.Write(MapleConsole.LogType.WARNING, "Invalid Layer Property \"" + item.Name + "\".");
                                break;
                        }
                    }

                    map.Layers.Add(int.Parse(wLayer.Name),new MapleLayer(int.Parse(wLayer.Name), tS, tiles, objs));
                }

            }
            #endregion

            MapleConsole.Write(MapleConsole.LogType.WARNING, "Left: " + map.Left.ToString() + " - Top: " + map.Top.ToString() + " - Right: " + map.Right.ToString() + " - Bottom: " + map.Bottom.ToString());

            #region Load Portals

            foreach (WZObject portal in mapWz.ResolvePath(path + "/portal"))
            {
                int pType = portal["pt"].ValueOrDefault<int>(-1);
                int pTo = portal["tm"].ValueOrDefault<int>(999999999);

                string script = "";

                try
                {
                    script = portal["script"].ValueOrDefault<string>("");
                }
                catch (Exception) { }

                if (pTo == map.MapID || (pTo == 999999999 && script == ""))
                    pType = 9;
               
                map.Portals.Add(new MaplePortal(portal["pn"].ValueOrDefault<string>(""), script, portal["tn"].ValueOrDefault<string>(""), new Vector2(portal["x"].ValueOrDefault<int>(0), portal["y"].ValueOrDefault<int>(0)), pTo, pType));
            }
            #endregion

            #region Load Life

            WZFile npcWz = new WZFile("Npc.wz", WZVariant.GMS, true, WZReadSelection.LowMemory);
            WZFile mobWz = new WZFile("Mob.wz", WZVariant.GMS, true, WZReadSelection.LowMemory);

            foreach (WZObject life in mapWz.ResolvePath(path + "/life"))
            {
                if (life is WZSubProperty)
                {
                    string type = life["type"].ValueOrDefault<string>("");
                    int lifeId = int.Parse(life["id"].ValueOrDie<string>());

                    if (lifeId == 9300018)
                        continue;

                    switch (type)
                    {
                        case "n": // NPC
                            string lifePath = lifeId.ToString() + ".img";
                            Dictionary<int, MapleFrame> ani = new Dictionary<int, MapleFrame>();

                            try
                            {
                                foreach (WZObject f in npcWz.ResolvePath(lifePath + "/stand"))
                                {
                                    if (f is WZCanvasProperty)
                                    {
                                        if (!Constants.Globals.LifeCache.ContainsKey(lifePath + "/stand/" + f.Name))
                                            Constants.Globals.LifeCache.Add(lifePath + "/stand/" + f.Name, Tools.BitmapToTexture(_device, f.ValueOrDie<System.Drawing.Bitmap>()));

                                        WZPointProperty origin = (WZPointProperty)npcWz.ResolvePath(lifePath + "/stand/" + f.Name)["origin"];

                                        int delay = 100;
                                        try
                                        {
                                            delay = f["delay"].ValueOrDefault<int>(100);
                                        }
                                        catch (Exception) { }

                                        ani.Add(int.Parse(f.Name), new MapleFrame(new MapleCanvas(Constants.Globals.LifeCache[lifePath + "/stand/" + f.Name], new Vector2(-origin.Value.X, -origin.Value.Y)), delay));
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }

                            
                            bool hide = false;

                            try
                            {
                                hide = life["hide"].ValueOrDefault<int>(1) == 0;
                            }
                            catch (Exception) { }

                            int Layer = map.Footholds.Where(o => o.ID == life["fh"].ValueOrDefault<int>(7)).ToArray<MapleFoothold>()[0].Layer;

                            map.Npcs.Add(new MapleNpc(lifeId, new MapleAnimation(ani), new Vector2(life["x"].ValueOrDefault<int>(0), life["cy"].ValueOrDefault<int>(0) + 0), Layer, !hide)); // Why is this 10px off? o.o

                            break;
                        case "m": // Mob

                            break;
                        default: // o.o
                            MapleConsole.Write(MapleConsole.LogType.WARNING, "Invalid Life Type \"" + type + "\".");
                            break;
                    }
                }
            }

            #endregion

            // Load strings from string.wz..
            #region Load Strings



            #endregion

            if (!map.HideMinimap)
            {
                try
                {
                    InterfaceEngine.MiniMap = new MapleMinimap(map, Tools.BitmapToTexture(_device, mapWz.ResolvePath(path + "/miniMap/canvas").ValueOrDie<System.Drawing.Bitmap>()));
                }
                catch (Exception)
                {
                    map.HideMinimap = true;
                }
            }

            CurrentMap = map;
            Camera.Set(map.Left, map.Top); // Set camera inside map bounds.. TODO: Spawn Points

            Vector2 pOrigin = new Vector2(Constants.Globals.PortalAnimation.Frames[0].Canvas.Origin.X / 2, Constants.Globals.PortalAnimation.Frames[0].Canvas.Origin.Y / 2);
            if (spawn != "")
                Constants.Globals.Player.Position = map.Portals.Where(o => o.PortalName == spawn).ToArray<MaplePortal>()[0].Location + pOrigin;
            else
                Constants.Globals.Player.Position = map.Portals.Where(o => o.PortalName == "sp").ToArray<MaplePortal>()[0].Location + pOrigin;

            sw.Stop();
            MapleConsole.Write(MapleConsole.LogType.MESSAGE, "Map loaded in " + sw.ElapsedMilliseconds.ToString() + "ms.");
            Constants.Globals.Player.CenterCamera();
            MapLoaded = true;
        }

        public static void Dispose()
        {
            CurrentMap.Dispose();
        }
    }
}
