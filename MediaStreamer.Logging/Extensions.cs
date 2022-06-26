using Sealkeen.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStreamer.Logging
{
    public static class Extensions
    {
        public static void LogStatically(this string message)
        {
            new SimpleLogger().LogInfo(message);
        }

        public static Action<string> GetLogInfoOrReturnNull(this ILogger logger)
        {
            if (logger == null)
            {
                return null;
            }
            else
                return logger.LogInfo;
        }

        public static Action<string> GetLogErorrOrReturnNull(this ILogger logger)
        {
            if (logger == null)
            {
                return null;
            }
            else
                return logger.LogError;
        }
    }
}
