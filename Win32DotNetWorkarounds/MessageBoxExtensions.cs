using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Win32DotNetWorkarounds
{
    public static class MessageBoxExtensions
    {
        public static void ShowMessageBoxInDebug(this string message)
        {
//#if DEBUG
            MessageBox.Show(message);
            MediaStreamer.Logging.SimpleLogger.LogStatically(message);
//#endif
        }
    }
}
