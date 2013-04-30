using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Media;
using System.IO;
using reWZ;
using reWZ.WZProperties;

namespace MapleSharp.Engines
{
    public static class MusicEngine
    {

        static string current;

        public static void LoadMusic(string name)
        {
            if (name == current)
                return;

            MediaPlayer.Stop(); 

            if (name == "")
                return;

            string[] path = name.Split('/');
            
            if (!Directory.Exists("Music"))
            {
                DirectoryInfo dir = Directory.CreateDirectory("Music"); 
                dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden; 
            }
            //Play Music
            if (!File.Exists("\\Music\\" + path[1] + ".mp3"))
            {
                using (Stream s = File.Create(System.Windows.Forms.Application.StartupPath + @"\Music\" + path[1] + ".mp3"))
                {
                    WZFile soundWz = new WZFile("Sound.wz", WZVariant.GMS, true, WZReadSelection.LowMemory);
                    byte[] sound = ((WZMP3Property)soundWz.ResolvePath(path[0] + ".img/" + path[1])).Value;
                    s.Write(sound, 0, sound.Length);
                }
            }
            Song music = Song.FromUri(name, new Uri("file:///" + System.Windows.Forms.Application.StartupPath + "/Music/" + path[1] + ".mp3"));
            MediaPlayer.Play(music);
            MediaPlayer.IsRepeating = true;
            current = name;
        }

        public static void Dispose()
        {
            MediaPlayer.Stop();
            try
            {
                if (Directory.Exists("Music"))
                    Directory.Delete("Music", true);
            }
            catch (Exception) { }
        }
    }
}
