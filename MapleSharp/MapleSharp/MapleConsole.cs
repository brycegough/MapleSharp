using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace MapleSharp
{
    public static class MapleConsole
    {

        public enum LogType
        {
            WARNING,
            MESSAGE,
            ERROR
        }

        public static void Write(LogType lt, string location, string message)
        {
            switch (lt)
            {
                case LogType.ERROR:

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[" + location + "] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(message);
                    break;

                case LogType.MESSAGE:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[" + location + "] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(message);
                    break;

                case LogType.WARNING:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("[" + location + "] ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(message);
                    break;
            }
        }

        public static void Write(LogType lt, string message)
        {
            StackFrame frame = new StackFrame(1);
            MethodBase method = frame.GetMethod();
            string loc = method.DeclaringType.Name;
            Write(lt, loc == "Main" ? "MapleSharp" : loc, message);
        }

        public static void SetTitle(string title)
        {
            Console.Title = "[MapleSharp] " + title;
        }

    }
}
