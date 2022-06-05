using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStreamer.Logging
{
    public class LogRecord
    {
        public LogRecord(DateTime date, string text)
        {
            CreationDate = date; Message = text;
        }

        public DateTime CreationDate { get; set; }
        public string Message { get; set; }
    }
}
