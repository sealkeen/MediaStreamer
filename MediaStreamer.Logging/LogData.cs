using LinqExtensions;
using ObjectModelExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MediaStreamer.Logging
{
    public class LogData
    {
        public static ConcurrentObservableCollection<LogRecord> LogList { get; set; } = new ConcurrentObservableCollection<LogRecord>();
    }
}
