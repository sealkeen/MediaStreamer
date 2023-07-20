using Sealkeen.CSCourse2016.JSONParser.Core;

namespace MediaStreamer.DataAccess.CrossPlatform
{
    public class Table
    {
        public JArray Items { get; set; }
        public JItem Root { get;set; }
        public string FilePath { get; set; }
    }
}
