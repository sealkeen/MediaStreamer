using Sealkeen.CSCourse2016.JSONParser.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace MediaStreamer.DataAccess.CrossPlatform
{
    public class CrossTable
    {
        public static void MapNavigation<TSource, TTarget>(TSource sourceTable, TTarget targetTable, string sourceProperty)
        {
            if (sourceTable == null || targetTable == null || string.IsNullOrEmpty(sourceProperty))
            {
                return;
            }
        }

        public static void AddNewObjectToCollection(LinkedList<JKeyValuePair> @props, JCollection @collection)
        {
            JObject jObj = new JObject(@collection);
            // TODO: Update to IEnumerable in 1.0.1-beta of JSON-Parsing-Compiler
            jObj.AddPairs(@props.ToList());
            @collection.Add(jObj);
        }

        private static JArray GetObjectsArray(JItem root, string tableName)
        {
            JArray itemsCollection = null;
            if (root != null)
                itemsCollection = (JArray)root.FindPairByKey(tableName.ToJString()).GetPairedValue();
            else
                itemsCollection = new JArray(root);

            return itemsCollection;
        }

        public static Table Load(string folderPath, string tableName)
        {
            var root = DataBase.LoadFromFileOrCreateRootObject(folderPath, $"{tableName}.json");
            Table result = new Table() {
                FilePath = Path.Combine(folderPath, $"{tableName}.json"),
                Root = root,
                Items = GetObjectsArray(root, $"{tableName}")
            };
            return result;
        }

        public static bool SizeChanged(ConcurrentDictionary<string, long> sizes, string directory, string tableName)
        {
            if (!sizes.Keys.Any(t => t == tableName))
                return true;

            return sizes[tableName] != GetTableSize(Path.Combine(directory, tableName + ".json"));
        }

        /// <returns>True if Table modify date has changed since the last data load. False otherwise.</returns>
        public static bool UpdateOccured(ConcurrentDictionary<string, DateTime> dates, string directory, string tableName)
        {
            if (!dates.Keys.Any(t => t == tableName))
                return true;

            return dates[tableName] != GetTableUpdateTime(Path.Combine(directory, tableName + ".json"));
        }

        public static DateTime GetTableUpdateTime(string path)
        {
            try
            {
                return new System.IO.FileInfo(path).LastWriteTime;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public static long GetTableSize(string path)
        {
            try
            {
                return new System.IO.FileInfo(path).Length;
            }
            catch
            {
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

        /// <summary>
        /// Loads List of Entities (JItems) into memory as JItems.
        /// </summary> /// <param name="dataBasePath"></param> /// <param name="tableName"></param> /// <param name="entities"></param>
        /// <returns>List of Entities (JItems)</returns>
        public static List<JItem> LoadAllEntities(string dataBasePath, string tableName)
        {
            List<JItem> result = null;
            var tablePath = Path.Combine(dataBasePath, tableName);

            if (File.Exists(tablePath))
            {
                var root = DataBase.LoadFromFileOrCreateRootObject(dataBasePath, tableName);
                result = root.FindPairByKey(Path.GetFileNameWithoutExtension(tableName).ToJString()).Value.Descendants();
            }
            else
            {
                result = new List<JItem>();
            }
            return result;
        }
    }
}
