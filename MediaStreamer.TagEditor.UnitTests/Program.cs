using MediaStreamer.Domain;
using MediaStreamer.IO;
using System;
using MediaStreamer;

namespace MediaStreamer.TagEditor.UnitTests
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Creating repository, initializing DBContext...");
            MediaStreamer.RAMControl.Program.DBAccess = new DBRepository()
            { DB = new MediaStreamer.DMEntities() }; //);

            Console.WriteLine("Creating filemanipulator in RAM memory, initializing ...");
            MediaStreamer.RAMControl.Program.FileManipulator = 
                new MediaStreamer.IO.FileManipulator(MediaStreamer.RAMControl.Program.DBAccess);

            Console.WriteLine("Decomposing local .mpe file ...");
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
