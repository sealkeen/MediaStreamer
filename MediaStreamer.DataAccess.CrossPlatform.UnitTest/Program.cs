using System;
using System.IO;
using System.Linq;
using Sealkeen.CSCourse2016.JSONParser.Core;
using MediaStreamer.Domain;
using MediaStreamer.DataAccess.RawSQL;
using MediaStreamer.Logging;

namespace MediaStreamer.DataAccess.CrossPlatform.UnitTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logFile = Path.Combine(Environment.CurrentDirectory + "log.txt");
            ReadonlyDBContext context = new ReadonlyDBContext(@"O:\DB\26.10.2021-3.db3", new SimpleLogger());
            var comps = context.GetCompositions();

           // GetCompositions();
        }

        public static void GetCompositions()
        {
            JSONDataContext context = new JSONDataContext();
            //var comps = context.GetCompositions();
            context.ClearTable("ListenedCompositions");
            ListenedComposition ls = new ListenedComposition() { ListenDate = DateTime.Now, CompositionID = Guid.Empty, UserID = Guid.Empty};
            context.Add(ls);
            var comps = context.GetListenedCompositions();
        }

        // ok / not ok (not all cases checked)
        public static void GetNewID()
        {
            JSONDataContext context = new JSONDataContext();
            var genres = context.GetGenres();

            var maxID = DataBase.GetMaxID<Genre, Int64>(genres.AsQueryable(), "GenreID");
        }

        //OK
        public static void TestGetGenres()
        {
            JSONDataContext context = new JSONDataContext();
            var genres = context.GetGenres();

            foreach (var g in genres)
            {
                Console.WriteLine(g.GenreID + " " + g.GenreName);
            }
            Console.ReadKey();
        }

        //OK
        public static void TestAddGenre()
        {
            JSONDataContext dc = new JSONDataContext();
            dc.EnsureCreated();

            Genre g = new Genre();
            g.GenreID = Guid.NewGuid();
            g.GenreName = "newGenre";

            dc.AddEntity(g);
        }

        //OK
        public static void TestLoadingDB()
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
