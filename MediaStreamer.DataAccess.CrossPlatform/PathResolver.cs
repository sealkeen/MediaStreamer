using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MediaStreamer.DataAccess.CrossPlatform
{
    public class PathResolver
    {
        public static string GetStandardDatabasePath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                    "MediaStreamer.WPF.NetCore3_1");
        }
        public static string GetStandardDatabasePath_Debug()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "MediaStreamer.WPF.NetCore3_1_Debug");
        }
    }
}
