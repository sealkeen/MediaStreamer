using System;
using System.IO;

namespace MediaStreamer.Logging
{
    public static class SimpleLogger
    {
        /// <summary>
        /// Logs to the Environment's currentDirectory slash log.txt
        /// </summary>
        /// <param name="message"></param>
        public static void LogStatically(this string message)
        {
            try
            {
                var filepath = Path.Combine(Environment.CurrentDirectory, "log.txt");
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
