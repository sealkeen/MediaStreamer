using System;
using System.Collections.Generic;
using System.Text;

namespace MediaStreamer.WPF.NetCore3_1
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class BuildDateTimeAttribute : Attribute
    {
        public string Date { get; set; }
        public BuildDateTimeAttribute(string date)
        {
            Date = date;
        }
    }
}
