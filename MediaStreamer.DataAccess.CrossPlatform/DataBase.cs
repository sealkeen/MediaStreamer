using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EPAM.CSCourse2016.JSONParser.Library;
using MediaStreamer.Domain;

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

        public static int TryParseInt(JString value)
        {
            int res = 0;
            if (int.TryParse(value.ToString(), out res) == false)
                return 0;
            else
                return res;
        }
        public static int TryParseInt(string value)
        {
            int res = 0;
            if (int.TryParse(value.ToString(), out res) == false)
                return 0;
            else
                return res;
        }

        public static void DeleteTable(string folderPath, string fileName)
        {
            string fullName = System.IO.Path.Combine(folderPath, fileName);
            if (!Directory.Exists(folderPath))
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
        }

        public static JItem LoadFromFileOrCreateRootObject(string folderPath, string fileName)
        {
            JItem result = null;
            string fullName = System.IO.Path.Combine(folderPath, fileName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (File.Exists(fullName))
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

        public static void SetProperty<TEntity, TValue>(TEntity entity, string propName, TValue value)
        {
            // Get a type object that represents the Example type.
            Type examType = typeof(TEntity);

            // Change the static property value.
            PropertyInfo piShared = examType.GetProperty(propName);
            
            piShared.SetValue(entity, value, null);
        }
    }
}
