using LinqExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MediaStreamer.Logging
{
    public class LogData
    {
        public static ObservableCollection<LogRecord> LogList { get; set; } = new ObservableCollection<LogRecord>();
    }
}
