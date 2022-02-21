using System;
using System.IO;
using EPAM.CSCourse2016.JSONParser.Library;
using MediaStreamer.Domain;

namespace MediaStreamer.DataAccess.CrossPlatform.UnitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            JSONDataContext context = new JSONDataContext();
            var genres = context.GetGenres();

            foreach (var g in genres)
            {
                Console.WriteLine(g.GenreID + g.GenreName);
            }
            Console.ReadKey();
        }

        static void TestGetGenres()
        {
            
        }

        //OK
        static void TestAddGenre()
        {
            JSONDataContext dc = new JSONDataContext();
            dc.EnsureCreated();

            Genre g = new Genre();
            g.GenreID = 0;
            g.GenreName = "new Genre";

            dc.AddEntity(g);
        }

        //OK
        static void TestLoadingDB()
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
