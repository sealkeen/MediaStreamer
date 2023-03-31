using MediaStreamer.Domain;
using Sealkeen.CSCourse2016.JSONParser.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediaStreamer.DataAccess.CrossPlatform
{
    public class ApplicationSettingsContext : IApplicationsSettingsContext
    {
        public ApplicationSettingsContext()
        {
            FolderName = PathResolver.GetStandardDatabasePath();
            PlayerStates = new List<PlayerState>();
            DBPaths = new List<DBPath>();
        }

        public string FolderName { get; set; }
        public virtual List<PlayerState> PlayerStates { get; set; }
        public virtual List<DBPath> DBPaths { get; set; }

        public void EnsureCreated()
        {
            DBPaths = GetDBPaths().ToList();
            PlayerStates = GetPlayerStates().ToList();
        }

        public void Add(DBPath path)
        {
            string genresDB = Path.Combine(FolderName, "DBPaths.json");

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "Genres.json");
            JItem itemsCollection = null;
            if (root != null)
                itemsCollection = root.FindPairByKey("Genres".ToJString()).GetPairedValue();
            else
                itemsCollection = new JArray(root);

            JObject jAG = new JObject(itemsCollection);

            //Properties
            List<JKeyValuePair> list = new List<JKeyValuePair>();
            list.Add(new JKeyValuePair(Key.DBPathID.ToJString(), DataBase.Coalesce(path.DBPathID).ToSingleValue(), jAG));
            list.Add(new JKeyValuePair(Key.DataSource, DataBase.Coalesce(path.DataSource), jAG));

            // Already Exists, return 
            if (DBPaths.Where( c => c.DataSource == path.DataSource).Count() != 0 )
                return;

            jAG.AddPairs(list);
            itemsCollection.Add(jAG);
            root.ToFile(genresDB);
        }


        public void Add(PlayerState playerState)
        {
            string listenedDB = Path.Combine(FolderName, "PlayerStates.json");

            var root = DataBase.LoadFromFileOrCreateRootObject(FolderName, "PlayerStates.json");

            JItem itemsCollection = null;
            if (root != null)
                itemsCollection = root.FindPairByKey("PlayerStates".ToJString()).GetPairedValue();
            else
                itemsCollection = new JArray(root);

            JObject jLS = new JObject(itemsCollection);

            //Properties
            List<JKeyValuePair> list = new List<JKeyValuePair>();
            list.Add(new JKeyValuePair(Key.StateID.ToString(), playerState.StateID.ToString(), jLS));
            list.Add(new JKeyValuePair(Key.StateTime.ToString(), playerState.StateTime.ToString(), jLS));
            list.Add(new JKeyValuePair(Key.VolumeLevel.ToString(), playerState.VolumeLevel.ToString(), jLS));

            jLS.AddPairs(list);
            itemsCollection.Add(jLS);
            root.ToFile(listenedDB);
        }

        public IQueryable<DBPath> GetDBPaths()
        {
            var jCompositions = Table.LoadInMemory(FolderName, "DBPaths.json");

            DBPaths = new List<DBPath>();
            foreach (var jComposition in jCompositions)
            {
                DBPath received = new DBPath();
                var fields = jComposition.DescendantPairs();
                foreach (var kv in fields)
                {
                    switch (kv.Key.ToString().Trim('\"'))
                    {
                        case Key.DBPathID:
                            Table.SetProperty(received, Key.DBPathID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.DataSource:
                            Table.SetProperty(received, Key.DataSource, kv.GetPairedValue().AsUnquoted());
                            break;
                    }
                }
                DBPaths.Add(received);
            }

            return DBPaths.AsQueryable();
        }

        public IQueryable<PlayerState> GetPlayerStates()
        {
            var jCompositions = Table.LoadInMemory(FolderName, "PlayerStates.json");

            PlayerStates = new List<PlayerState>();
            foreach (var jComposition in jCompositions)
            {
                PlayerState received = new PlayerState();
                var fields = jComposition.DescendantPairs();
                foreach (var kv in fields)
                {
                    switch (kv.Key.ToString().Trim('\"'))
                    {
                        case Key.StateID:
                            Table.SetProperty(received, Key.StateID, Guid.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.StateTime:
                            Table.SetProperty(received, Key.StateTime, DateTime.Parse(kv.Value.AsUnquoted()));
                            break;
                        case Key.VolumeLevel:
                            Table.SetProperty(received, Key.VolumeLevel, double.Parse(kv.Value.AsUnquoted()));
                            break;
                    }
                }
                PlayerStates.Add(received);
            }

            return PlayerStates.AsQueryable();
        }

        public double? GetCachedVolumeLevel()
        {
            if (PlayerStates == null || PlayerStates.Count <= 0
                )
                return null;
            // Volume level is double from 0 to 1
            var level = PlayerStates.Last().VolumeLevel;

            return level > 1 ? 1 : level; 
        }

        public IQueryable<string> GetDataSources()
        {
            if (PlayerStates == null || PlayerStates.Count <= 0)
                return null;

            List<string> result = new List<string>();
            foreach (var src in DBPaths)
            {
                result.Add(src.DataSource);
            }
            return result.AsQueryable();
        }

        public void ClearPlayerStates()
        {
            PlayerStates = null;
            DataBase.DeleteTable(FolderName, "PlayerStates.json");
        }

        private static ApplicationSettingsContext _instance;
        public static async Task<ApplicationSettingsContext> GetInstanceAsync()
        {
            if (_instance == null)
            {
                _instance = new ApplicationSettingsContext();
            }
            return _instance;
        }
    }
}