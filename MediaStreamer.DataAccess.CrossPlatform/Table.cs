using EPAM.CSCourse2016.JSONParser.Library;

namespace MediaStreamer.DataAccess.CrossPlatform
{
    public class Table
    {
        public static bool HasFieldValue(JItem jObject, JString field, JString value)
        {
            if (jObject.ContainsKeyAndValueRecursive(field, value))
                return true;
            return false;
        }
    }
}
