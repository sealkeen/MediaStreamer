using MediaStreamer.DataAccess.RawSQL;
using MediaStreamer.Domain;
using MediaStreamer.Logging;
using Sealkeen.CSCourse2016.JSONParser.Core;
using Sealkeen.Linq.Extensions;
using System.Diagnostics;
using Xunit;

namespace MediaStreamer.DataAccess.CrossPlatform.UnitTest
{
    public class JSONDbContextTests
    {
#if NOT
        public static void Main(string[] args)
#else
        [Theory, InlineData( null )]
        public async Task NotMain(string[] args)
#endif
        {
            var logFile = Path.Combine(Environment.CurrentDirectory + "log.txt");
            ReadonlyDBContext context = new ReadonlyDBContext(@"O:\DB\26.10.2021-3.db3", new SimpleLogger());
            var comps = await context.GetCompositions().CreateListAsync();

            comps.FindLast( c => true );
        }

        [Fact]
        public async Task GetListenedCompositions_ShouldNot_BeEmpty()
        {
            JSONDataContext context = new JSONDataContext();
            context.ClearTable("ListenedCompositions");
            ListenedComposition ls = new ListenedComposition() { ListenDate = DateTime.Now, CompositionID = Guid.Empty, UserID = Guid.Empty};
            context.Add(ls);
            var comps = await context.GetListenedCompositions().CreateListAsync();

            comps.FindLast(c => true);

            Assert.NotEmpty(comps);
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
