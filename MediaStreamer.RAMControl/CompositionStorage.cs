using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MediaStreamer.IO;
using MediaStreamer.Domain;

namespace MediaStreamer.RAMControl
{
    public class CompositionStorage
    {
        public static List<Composition> Compositions { get; set; }
        public static LinkedList<Composition> Queue { get; set; }

        public static void ChangeCompositionTags(IList selectedItems)
        {
            IComposition currentComp;
            List<Composition> compositions = new List<Composition>();
            foreach (var song in selectedItems)
            {
                currentComp = CompositionStorage.Compositions[selectedItems.IndexOf(song)];
                string filePath = currentComp.FilePath;
                if (filePath != null)
                {
                    compositions?.Add(currentComp.GetInstance());
                }
            }

            FileInfo fileInfo;
            foreach (var comp in compositions)
            {
                fileInfo = new FileInfo(comp.FilePath);
                string newName = fileInfo?.DirectoryName + "\\" +
                    comp.Artist.ArtistName + " – " + comp.CompositionName;
                if (comp.Album.Year != null)
                {
                    if (comp?.Album?.Year < 2100 && comp.Album.Year > 1900)
                        newName += $" ({comp?.Album?.Year})";
                }

                //newName += fileInfo.Extension;
                //if (fileInfo.FullName.FileExists())
                //    System.IO.File.Move(fileInfo.FullName, newName);
                //comp.FilePath = newName;

                TimeSpan tS;
                if (comp.Duration != null)
                    tS = TimeSpan.FromSeconds(comp.Duration.Value);
                else
                    tS = TimeSpan.FromSeconds(int.MinValue);

                Program.DBAccess.AddComposition(comp.Artist.ArtistName,
                    comp.CompositionName, comp.Album.AlbumName, ((long)tS.TotalSeconds), comp.FilePath);
            }
        }
    }
}
