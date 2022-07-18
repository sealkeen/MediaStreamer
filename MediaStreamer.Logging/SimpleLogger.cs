using Sealkeen.Abstractions;
using StringExtensions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MediaStreamer.Logging
{
    public class SimpleLogger : ILogger
    {
        public SimpleLogger(string file = null) 
        {
            try
            {
                _sw = new StreamWriter(filepath, true);
            }
            catch (IOException)
            {
                filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MediaStreamer.WPF.NetCore3_1", "Logs"
                    , DateTime.Now.ToString("log_dd-mm-yyyy_hh-mm-ss.fff") + ".txt"
                    );
                _sw = new StreamWriter(filepath, true);
            }
            finally {
                if (_sw != null && File.Exists(filepath))
                {
                    LogDebug("Opening file stream: " + filepath + "...");
                }
            }
        }
        ~SimpleLogger() => _sw?.Close();

        public string filepath = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MediaStreamer.WPF.NetCore3_1", "Logs", "log.txt");
        private StreamWriter _sw;

        public LogLevel LogLevel { get; set; } = LogLevel.Trace;

        public LogLevel GetLogLevel()
        {
            return this.LogLevel;
        }
        public void LogTrace(string message)
        {
            if (LogLevel <= LogLevel.Trace)
                Log("[trace] " + message);
        }
        public void LogDebug(string message)
        {
            if (LogLevel <= LogLevel.Debug)
                Log("[debug] " + message);
        }
        public void LogInfo(string message)
        {
            if(LogLevel <= LogLevel.Info)
                Log(message);
        }
        public void LogError(string message)
        {
            if (LogLevel <= LogLevel.Error)
                Log("[error] " + message);
        }

        /// <summary>
        /// Logs to the Environment's currentDirectory slash log.txt
        /// </summary>
        /// <param name="message"></param>
        private void Log(string message)
        {
            try
            {
                //overwrite

                _sw.WriteLine($"[{DateTime.Now}] " + message);
                Console.WriteLine($"[{DateTime.Now}] " + message);
                CreateLogRecord(message);
                _sw.Flush();
            }
            catch (Exception ex)
            {
                try
                {
                    Directory.CreateDirectory(filepath.GetDirectoryOf());
                }
                catch (Exception exInner)
                {
                    Console.WriteLine($"[{DateTime.Now}] Logger error: " + exInner.Message);
                }
                Console.WriteLine($"[{DateTime.Now}] Logger error: " + ex.Message);
            }
        }
        public void CreateLogRecord(string message)
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
