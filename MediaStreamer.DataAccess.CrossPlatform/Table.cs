using System.IO;
using System.Collections.Generic;
using Sealkeen.CSCourse2016.JSONParser.Core;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Concurrent;

namespace MediaStreamer.DataAccess.CrossPlatform
{
    public class Table
    {
        public static bool SizeChanged(ConcurrentDictionary<string, long> sizes, string directory, string tableName)
        {
            if (!sizes.Keys.Any(t => t == tableName))
                return true;

            return sizes[tableName] != GetTableSize(Path.Combine(directory, tableName + ".json"));
        }

        public static long GetTableSize(string path)
        {
            try {
                return new System.IO.FileInfo(path).Length;
            } catch { 
                return 0;
            }
        }

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

        /// <summary>
        /// Loads List of Entities (JItems) into memory.
        /// </summary> /// <param name="dataBasePath"></param> /// <param name="tableName"></param> /// <param name="entities"></param>
        /// <returns>List of Entities (JItems)</returns>

        public static List<JItem> LoadInMemory(string dataBasePath, string tableName)
        {
            List<JItem> result = null;
            var tablePath = Path.Combine(dataBasePath, tableName);

            if (File.Exists(tablePath)) {
                var root = DataBase.LoadFromFileOrCreateRootObject(dataBasePath, tableName);
                result = root.FindPairByKey(Path.GetFileNameWithoutExtension(tableName).ToJString()).Value.Descendants();
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
                if (value == null)
                    piShared.SetValue(entity, default(TValue), null);
                else if (value.GetType() == typeof(string))
                    piShared.SetValue(entity, value.ToString().Trim('\"'), null);
                else {
                    if (Nullable.GetUnderlyingType(piShared.PropertyType) != null)
                        // It's nullable
                        SetReferenceTypeValueFromNonreference(entity, value, piShared);
                    else
                        piShared.SetValue(entity, value, null);
                }
            } catch (NullReferenceException) {
                    piShared.SetValue(entity, default(TValue), null);
            } catch (System.ArgumentException ex) {
                try
                {
                    SetReferenceTypeValueFromNonreference(entity, value, piShared);
                }
                catch (InvalidCastException)
                {
                    // the input string could not be converted to the target type - abort
                    return;
                }
            }
        }

        private static void SetReferenceTypeValueFromNonreference<TEntity, TValue>(TEntity entity, TValue value, PropertyInfo piShared)
        {
            var convertedValue = System.Convert.ChangeType(value,
                Nullable.GetUnderlyingType(piShared.PropertyType));
            piShared.SetValue(entity, convertedValue, null);
        }
    }
}
