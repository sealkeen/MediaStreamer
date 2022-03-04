using System.IO;
using System.Collections.Generic;
using EPAM.CSCourse2016.JSONParser.Library;
using System;
using System.Reflection;

namespace MediaStreamer.DataAccess.CrossPlatform
{
    public class Table
    {
        public static TLinkedTarget GetLinkedEntity<TLinkedTarget>(object id, IEnumerable<TLinkedTarget> target, string propName) 
        {
            TLinkedTarget result = default(TLinkedTarget);
            Type examType = typeof(TLinkedTarget);
            PropertyInfo piShared = null;
            foreach (var v in target)
            {
                piShared = examType.GetProperty(propName);
                var trgtID = piShared.GetValue(v, null);
                if (trgtID.Equals(id))
                {
                    return v;
                }
            }
            return result;
        }

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
            Type examType = typeof(TEntity);    // Get a type object that represents the Example type.
                                                // Change the static property value.
            PropertyInfo piShared = examType.GetProperty(propName);
            try
            {
                if (value.GetType() == typeof(string))
                    piShared.SetValue(entity, value.ToString().Trim('\"'), null);
                else
                    piShared.SetValue(entity, value, null);
            } catch (NullReferenceException) {
                piShared.SetValue(entity, default(TValue), null);
            } catch (System.ArgumentException ex) {
                try
                {
                    var convertedValue = System.Convert.ChangeType(value,
                        Nullable.GetUnderlyingType(piShared.PropertyType));
                    piShared.SetValue(entity, convertedValue, null);
                }
                catch (InvalidCastException)
                {
                    // the input string could not be converted to the target type - abort
                    return;
                }
            }
        }
    }
}
