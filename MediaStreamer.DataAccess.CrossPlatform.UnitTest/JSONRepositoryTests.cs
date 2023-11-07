using Sealkeen.CSCourse2016.JSONParser.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MediaStreamer.DataAccess.CrossPlatform.UnitTest
{
    public class JSONRepositoryTests
    {
        [Fact]
        public void CreateAndOpenJObjectFile()
        {
            // No parent element so first parameter is null
            JObject jObject = new JObject(null, 
                new JKeyValuePair(
                    new JString("Key"), new JString("Value")
                )
            );

            jObject.SaveToFileAndOpenInNotepad("jKeyValuePair.txt");
        }

        // Result in "jKeyValuePair.txt"
        // {"Key":"Value"}

        [Fact]
        public async Task MoveProductionDataToDebugContext()
        {
            JSONDataContext productionContext = new JSONDataContext();
            JSONDataContext debugContext = new JSONDataContext(null, PathResolver.GetStandardDatabasePath() 
                //+ "_Tests"
                + "_Debug"
                );
            debugContext.SaveDelayed = true;
            productionContext.EnsureCreated();
            debugContext.EnsureCreated();
            Assert.True(productionContext.TableInfo.Count > 0); Assert.True(debugContext.TableInfo.Count > 0);

            var comps = await productionContext.GetCompositionsAsync();
            Assert.False(comps.Count == 0);
            var debugComps = debugContext.GetCompositions();
            var debugArts = debugContext.GetArtists();
            var debugAlbums = debugContext.GetAlbums();
            var debugArtistGenres = debugContext.GetArtistGenres();
            var debugGenres = debugContext.GetGenres();
            System.Diagnostics.Debug.WriteLine(
                $"Prod: comps: {debugComps.Count()}, " + $"arts: {debugArts.Count()}"
                + $"albs: {debugAlbums.Count()}" + $"prodAG: {debugArtistGenres.Count()}"
                + $"prodGnr: {debugGenres.Count()}"
            );
            foreach (var cmp in comps)
            {
                var firstCmp = cmp;
                var firstAlb = productionContext.GetAlbums().Where(a => a.AlbumID == firstCmp.AlbumID).FirstOrDefault();
                var firstArt = firstCmp.Artist;
                var firstGenre = productionContext.GetGenres().Where(g => g.GenreID == firstAlb.GenreID).FirstOrDefault();
                var firstArtistGenre = productionContext.GetArtistGenres().Where(
                    g => g.GenreID == firstAlb.GenreID && g.ArtistID == firstArt.ArtistID)
                    .FirstOrDefault();
                if (!debugArts.Any(a => a.ArtistID == firstCmp.ArtistID)) {
                    debugContext.Add(firstArt);
                    //debugContext.SaveChanges();
                }
                if (!debugGenres.Any(g => g.GenreID == firstGenre.GenreID)) {
                    debugContext.Add(firstGenre);
                    //debugContext.SaveChanges();
                }
                if (!debugArtistGenres.Any(ag => ag.GenreID == firstArtistGenre.GenreID && ag.ArtistID == firstArtistGenre.ArtistID)) {
                    debugContext.Add(firstArtistGenre);
                    //debugContext.SaveChanges();
                }
                if (!debugAlbums.Any(a => a.AlbumID == firstCmp.AlbumID))
                {
                    firstAlb.Artist = null;
                    firstAlb.Genre = null;
                    debugContext.Add(firstAlb);
                    //debugContext.SaveChanges();
                }
                if (!debugComps.Any(c => c.CompositionID == firstCmp.CompositionID))
                {
                    firstCmp.Artist = null;
                    firstCmp.Album = null;
                    debugContext.Add(firstCmp);
                    //debugContext.SaveChanges();
                }
            }

            debugContext.SaveChanges();
        }
    }
}
