using System;
using System.IO;

namespace MediaStreamer.Logging
{
    public static class SimpleLogger
    {
        public static string filepath = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MediaStreamer.WPF.NetCore3_1", "Logs", "log.txt");
        /// <summary>
        /// Logs to the Environment's currentDirectory slash log.txt
        /// </summary>
        /// <param name="message"></param>
        public static void LogStatically(this string message)
        {
            try
            {
                //overwrite
                StreamWriter sw = new StreamWriter(filepath, true);
                sw.WriteLine($"[{DateTime.Now}] " + message);
                Console.WriteLine($"[{DateTime.Now}] " + message);
                sw.Close();
            }
            catch (Exception ex) {
                Console.WriteLine($"[{DateTime.Now}] Logger error: " + ex.Message);
            }
        }
    }
}
