using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EPAM.CSCourse2016.JSONParser.Library;
using StringExtensions;

namespace MediaStreamer.DataAccess.CrossPlatform
{
    public class DataBase 
    {
        public static TFieldType GetMaxID<TEntity, TFieldType>(IQueryable<TEntity> entities, string keyPropertyName)
        {
            // Get a type object that represents the Example type.
            Type examType = typeof(TEntity);

            // Change the static property value.
            PropertyInfo[] properties = examType.GetProperties();

            TFieldType result = default(TFieldType);
            TFieldType fromData = default(TFieldType);
            var maxID = properties.Where(p => p.Name == keyPropertyName).Max();
            foreach (var e in entities)
            {
                if (maxID != null)
                {
                    if ((fromData = (TFieldType)maxID.GetValue(e, null)) != null)
                    {
                        if (Comparer<TFieldType>.Default.Compare(result, fromData) < 0) {
                            result = (TFieldType)maxID.GetValue(e, null);
                        }
                    }
                }
            }
            return result;
        }

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
            if (int.TryParse(value.ToString().Trim('\"'), out res) == false)
                return 0;
            else
                return res;
        }

        public static int? TryParseNullableInt(string value)
        {
            //int res = 0;
            //if (int.TryParse(value.ToString().Trim('\"'), out res) == false)
            //    res = 0;
            int? res = ToNullableStringExtension.ToNullable<int>(value.Trim('\"'));
            return res;
        }

        public static long? TryParseNullableLong(string value)
        {
            //int res = 0;
            //if (int.TryParse(value.ToString().Trim('\"'), out res) == false)
            //    res = 0;
            long? res = ToNullableStringExtension.ToNullable<long>(value.Trim('\"'));
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
                
                if (!result.IsCollection())
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
