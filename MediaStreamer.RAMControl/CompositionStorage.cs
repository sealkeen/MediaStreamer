using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MediaStreamer.Domain;
using ObjectModelExtensions;

namespace MediaStreamer.RAMControl
{
    public class CompositionStorage
    {
        public CompositionStorage()
        {
            Compositions = new List<IComposition>();

            if(Queue == null)
                Queue = new ObservableLinkedList<Composition>();
        }

        public IList<IComposition> Compositions { get; set; }
        public ObservableLinkedList<Composition> Queue { get; set; }

        public void ChangeCompositionTags(IList selectedItems)
        {
            try
            {
                foreach (var song in selectedItems)
                {
                    IComposition currentComp = (IComposition)selectedItems[selectedItems.IndexOf(song)];

                    RenameCompositionFile(currentComp);
                }

                Program.DBAccess.DB.SaveChangesTo("Compositions");
            }
            catch (Exception ex)
            {
                Program._logger?.LogError("RenameCompositionFiles" + ex.Message);
            }
        }


        /// <summary>
        /// TODO: Instead of 1 do 2.
        /// [1] Fix "Rename to Standard" check menu item button -> enable renaming the file to match pattern "Artist – Title (Year if exists)".
        /// [2] The previous is foxed, test rename further.
        /// </summary>
        /// <param name="compositions"></param>
        protected static void RenameCompositionFile(IComposition composition)
        {
            Artist artist = composition.Artist;

            if (composition.Artist == null)
            {
                artist = Program.DBAccess.DB.GetArtists().Where(x => x.ArtistID == composition.ArtistID).First();
            }

            composition.FilePath = RenameFileKeepExtension(composition.FilePath,
                artist.ArtistName + " - " + composition.CompositionName
            );
        }

        public static string RenameFileKeepExtension(string filePath, string newFileName)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (string.IsNullOrWhiteSpace(newFileName))
                throw new ArgumentException("New file name cannot be null or empty", nameof(newFileName));

            string directory = Path.GetDirectoryName(filePath);
            string extension = Path.GetExtension(filePath);

            string newFilePath = Path.Combine(directory, newFileName + extension);

            File.Move(filePath, newFilePath);

            return newFilePath;
        }
    }
}
