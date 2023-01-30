using System;
using System.Collections.Generic;
using System.Text;

namespace Sealkeen.Abstractions
{
    public interface ILogger
    {
        void LogTrace(string message);
        void LogInfo(string message);
        void LogError(string message);
        void LogDebug(string message);
    }
}