using System.IO;
using System.Collections.Generic;
using EPAM.CSCourse2016.JSONParser.Library;
using System;
using System.Reflection;

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

        public static List<JItem> LoadInMemory(string dataBasePath, string tableName)
        {
            List<JItem> result = null;
            var tablePath = Path.Combine(dataBasePath, tableName);

            if (File.Exists(tablePath)) {
                var root = DataBase.LoadFromFileOrCreateRootObject(dataBasePath, tableName).Descendants()[0];
                result = root.Descendants();
            } else {
                result = new List<JItem>();
            }
            return result;
        }

        public static void SetProperty<TEntity, TValue>(TEntity entity, string propName, TValue value)
        {
            // Get a type object that represents the Example type.
            Type examType = typeof(TEntity);

            // Change the static property value.
            PropertyInfo piShared = examType.GetProperty(propName);

            if (value.GetType() == typeof(string))
                piShared.SetValue(entity, value.ToString().Trim('\"'), null);
            else
                piShared.SetValue(entity, value, null);
        }
    }
}
