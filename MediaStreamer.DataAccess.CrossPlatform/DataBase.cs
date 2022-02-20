using System.Collections.Generic;
using System.IO;
using EPAM.CSCourse2016.JSONParser.Library;

namespace MediaStreamer.DataAccess.CrossPlatform
{
    public class DataBase 
    {

        public static string Coalesce<T>(T line)
        {
            if (line == null)
                return "null";
            else
                return line.ToString();
        }


        public static JItem LoadFromFileOrCreateRootObject(string folderPath, string fileName)
        {
            JItem result = null;
            string fullName = System.IO.Path.Combine(folderPath, fileName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            if (File.Exists(fileName))
            {
                JSONParser parser = new JSONParser(fullName);
                result = parser.Parse();
                if (result.GetType() != typeof(JObject))
                {
                    JObject jObject = new JObject(null);
                    jObject.Add(result);
                    jObject.ToFile(fullName);
                    return jObject;
                } else
                    return result;
            } else {
                JObject jObject = new JObject(null);
                jObject.ToFile(fullName, true);
                return jObject;
            }
        }
    }
}
