using System;

namespace MapleSharp
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            MapleConsole.SetTitle("Console");
            MapleConsole.Write(MapleConsole.LogType.MESSAGE, "MapleSharp", "Welcome to MapleSharp - GMS v111");
            
            using (Main game = new Main())
            {
                game.Run();
            }
        }
    }
#endif
}

