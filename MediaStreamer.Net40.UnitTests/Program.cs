using System;
using Plugin;

namespace MediaStreamer.Net40.UnitTests
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Picking file from event Action:");
            Console.WriteLine("Awaiting");
            PickFileFromEventAction();
            Console.WriteLine("OK!");

            Console.WriteLine("Picking file from File picker:");
            Console.WriteLine("Awaiting");
            PickFileThroughFilePicker();
            Console.WriteLine("OK!");

            Console.ReadKey();
        }

        public static void PickFileFromEventAction()
        {
            //MediaStreamer.WPF.EventAction.MainWindowEventActions mainWindowEventActions = new WPF.EventAction.MainWindowEventActions();
            //mainWindowEventActions.ShowDialog().Wait();
        }

        static public void PickFileThroughFilePicker()
        {
            var file = Plugin.FilePicker.CrossFilePicker.Current.PickFile();
        }
    }
}
