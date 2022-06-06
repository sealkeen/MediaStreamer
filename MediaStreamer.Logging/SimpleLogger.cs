using StringExtensions;
using System;
using System.IO;
using System.Threading.Tasks;

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
                CreateLogRecord(message);
            }
            catch (Exception ex) {
                try {
                    Directory.CreateDirectory(filepath.GetDirectoryOf());
                } catch (Exception exInner) {
                    Console.WriteLine($"[{DateTime.Now}] Logger error: " + exInner.Message);
                }
                Console.WriteLine($"[{DateTime.Now}] Logger error: " + ex.Message);
            }
        }

        public static void CreateLogRecord(string message)
        {
            try {
#if !NET40
            //Task.Factory.StartNew(new Action(
            //    delegate
            //    {
                    LogRecord lR = new LogRecord(DateTime.Now, message);
                    LogData.LogList.Add(lR);
            //    }
            //));
#endif          
            } catch(Exception ex) {
                
            }
        }
    }
}
