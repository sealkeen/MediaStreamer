using MediaStreamer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaStreamer.DataAccess.CrossPlatform.Extensions
{
    public static class JSONDataContextExtensions
    {
        public static void AddRange(this JSONDataContext context, IEnumerable<Composition> newCompositions)
        {
            if (context.SaveDelayed) {
                foreach (var cmp in newCompositions) context.Compositions.Add(cmp);
                return;
            }

            var cTable = CrossTable.Load(context.FolderName, "Compositions");

            foreach (var comp in newCompositions) {
                if (comp == null)
                    continue;

                // Already Exists, return 
                if (context.Compositions.Where(c =>
                        c.CompositionName == comp.CompositionName &&
                        c.FilePath == comp.FilePath).Count() != 0)
                    continue;

                CrossTable.AddNewObjectToCollection(comp.GetPropList(), cTable.Items);
                context.Compositions.Add(comp);
            }

            cTable.Root.ToFile(cTable.FilePath);
        }


        public static void AddRange(this JSONDataContext context, IEnumerable<Artist> newArtists)
        {
            if (context.SaveDelayed) {
                foreach (var cmp in newArtists) context.Artists.Add(cmp);
                return;
            }

            var cTable = CrossTable.Load(context.FolderName, "Artists");

            foreach (var art in newArtists) {
                if (art == null)
                    continue;

                // Already Exists, return 
                if (context.Artists.Where(c =>
                        c.ArtistName.Trim().ToLower() == art.ArtistName.Trim().ToLower()
                    ).Count() != 0)
                    continue;

                CrossTable.AddNewObjectToCollection(art.GetPropList(), cTable.Items);
                context.Artists.Add(art);
            }

            cTable.Root.ToFile(cTable.FilePath);
        }
    }
}
