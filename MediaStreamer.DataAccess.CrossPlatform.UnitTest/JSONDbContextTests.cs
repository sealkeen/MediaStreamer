using System;
using System.IO;
using System.Linq;
using Sealkeen.CSCourse2016.JSONParser.Core;
using MediaStreamer.Domain;
using MediaStreamer.DataAccess.RawSQL;
using MediaStreamer.Logging;
using Xunit;
using Xunit.Sdk;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MediaStreamer.DataAccess.CrossPlatform.UnitTest
{
    public class JSONDbContextTests
    {
#if NOT
        public static void Main(string[] args)
#else
        [Theory, InlineData( null )]
        public void NotMain(string[] args)
#endif
        {
            var logFile = Path.Combine(Environment.CurrentDirectory + "log.txt");
            ReadonlyDBContext context = new ReadonlyDBContext(@"O:\DB\26.10.2021-3.db3", new SimpleLogger());
            var comps = context.GetCompositions();

            //GetCompositions();
        }

        [Fact]
        public void GetCompositions()
        {
            JSONDataContext context = new JSONDataContext();
            //var comps = context.GetCompositions();
            context.ClearTable("ListenedCompositions");
            ListenedComposition ls = new ListenedComposition() { ListenDate = DateTime.Now, CompositionID = Guid.Empty, UserID = Guid.Empty};
            context.Add(ls);
            var comps = context.GetListenedCompositions();
        }

        // ok / not ok (not all cases checked)
        [Fact]
        public void GetNewID()
        {
            JSONDataContext context = new JSONDataContext();
            var genres = context.GetGenres();

            var maxID = DataBase.GetMaxID<Genre, Int64>(genres.AsQueryable(), "GenreID");
        }

        //OK
        [Fact]
        public void TestGetGenres()
        {
            JSONDataContext context = new JSONDataContext();
            context.EnsureCreated();
            var genres = context.GetGenres();

            Assert.NotNull(genres);
            foreach (var g in genres)
                Debug.WriteLine(g.GenreID + " " + g.GenreName);
            
            Assert.True(context.TableInfo.Count > 0);
        }

        //OK
        [Fact] //(Skip = "No add for the production DB")]
        public void TestAddGenre()
        {
            JSONDataContext dc = new JSONDataContext(null, PathResolver.GetStandardDatabasePath_Debug());
            dc.EnsureCreated();

            Genre g = new Genre() { GenreID = Guid.NewGuid(), GenreName = "newGenre"};

            dc.AddEntity(g);
        }

        //OK
        [Fact]
        public void TestLoadingDB()
        {
            JSONParser jSONParser = new JSONParser("Compositions/Genres.json");
            JItem jItem = jSONParser.Parse();

            Console.WriteLine(jItem.ToString());

            var descendant = jItem.Descendants()[0];
            var descendant1 = descendant.Descendants()[0];

            jItem = DataBase.LoadFromFileOrCreateRootObject("Compositions", "Artists.json");

            Console.WriteLine(jItem.ToString());

            descendant = jItem.Descendants()[0];
            descendant1 = descendant.Descendants()[0];
        }
    }
}
