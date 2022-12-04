using MediaStreamer.Domain;
using MediaStreamer.IO;
using System;
using MediaStreamer;
using MediaStreamer.DataAccess.NetStandard;

namespace MediaStreamer.TagEditor.UnitTests
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Creating repository, initializing DBContext...");
            MediaStreamer.RAMControl.Program.DBAccess = new DBRepository()
            { DB = new DMEntities() }; //);

            Console.WriteLine("Creating filemanipulator in RAM memory, initializing ...");
            MediaStreamer.RAMControl.Program.FileManipulator = 
                new MediaStreamer.IO.FileManipulator(MediaStreamer.RAMControl.Program.DBAccess, null);

            Console.WriteLine("Decomposing local .mp3 file ...");
            MediaStreamer
                .RAMControl
                .Program
                .FileManipulator
                .DecomposeAudioFile(@"C:\Users\Sealkeen\Music\Crywolf – The Moon Is Falling Down by Crywolf.mp3",
                ShowError);
            Console.ReadKey();
        }

        public static void ShowError(string message)
        {
            Console.WriteLine(message);
        }
    }
}
