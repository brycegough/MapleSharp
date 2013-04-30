using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MapleSharp.Objects;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSharp.Constants
{
    public static class Globals
    {

        public static MapleAnimation PortalAnimation;
        public static Dictionary<string, Texture2D> TileCache = new Dictionary<string, Texture2D>();
        public static Dictionary<string, Texture2D> ObjectCache = new Dictionary<string, Texture2D>();
        public static Dictionary<string, Texture2D> BackCache = new Dictionary<string, Texture2D>();
        public static Dictionary<string, Texture2D> LifeCache = new Dictionary<string, Texture2D>();
        public static SpriteFont Font;

        public static MapleCharacter Player;

    }

    public class Game
    {
        public static bool StartFullscreen = true; // Start Maplestory in Fullscreen?
        public static bool CameraLimit = true; // Don't allow the camera past the map edges.. (If false, character will not be followed)
        public static bool CharacterBoundBox = false; // Draw a box around the character? (Removed for now)
        public static bool MuteMusic = false; // This sorta explains itself?
        public static bool DrawFootholds = false; // WARNING: VERY FUCKING LAGGY!

        // 1024 x 768 is MapleStory's maximum resolution, don't bother for 800 x 600 for now..
        public static int ScreenWidth = 1024;
        public static int ScreenHeight = 768;
    }

    public class Physics
    {
        // TODO this.. Don't bother loading them from WZ, just copy paste em

        public static float FallSpeed = 6.7f; // Speed character falls (in px)
        public static float WalkSpeed = 1.25f; // Speed character walks (in px)
        public static float JumpSpeed = 5.55f; // Speed character jumps (in px)
        public static float JumpHeight = 15f; // Height character can jumps (this * JumpSpeed = MaxHeight)
        public static int JumpDelay = 150; // How long player has to wait between jumps (in ms)

        public static float FallMovementPercent = 1.35f;
    }

    public class Controls // todo: this..
    {

    }
}
