using MediaStreamer.Domain;
using MediaStreamer.Logging;
using MediaStreamer.RAMControl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace MediaStreamer.WPF.NetCore3_1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                //var tsk = Task.Factory.StartNew(() => 
                Program._logger = new SimpleLogger()
                //); tsk.Wait()
                ;
                if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
                {
                    Program._logger?.LogTrace("Another instance of the application is already running.");
                    foreach (var proc in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location))
                        .Where(p => p.Handle != System.Diagnostics.Process.GetCurrentProcess().Handle
                        && p.Id != System.Diagnostics.Process.GetCurrentProcess().Id
                        ))
                    {
                        Program._logger?.LogTrace("Killing the process...");
                        proc.CloseMainWindow();
                    }
                }

                // Crazy hack
                var args1 = Environment.GetCommandLineArgs()/*.CreateArgs().*/.Skip(1);
                if (args1 != null && args1.Count() > 0)
                {
                    var arguments1 = args1.Aggregate((a, b) => Path.Combine(a + Environment.NewLine + b));
                    Program._logger?.LogTrace($"cmd arguments 1 : {arguments1}");
                    Program._logger?.LogTrace("Arguments.Count() > 0, ok.");
                    // HandleMultipleArguments(args1);
                }
            } catch (Exception ex) {
                ex.Message.LogStatically();
            }
        }

        //private static void HandleMultipleArguments(IEnumerable<string> args)
        //{
        //    List<Composition> lst = new List<Composition>();
        //    string path = "";
        //    string fullPath = "";
        //    for (int i = 0; i < args.Count(); i++)
        //    {
        //        path += ' ' + args.ElementAt(i);
        //        path += args.ElementAt(i) + '/';
        //        if (File.Exists(path.Trim().Trim('\"')))
        //        {
        //            HandleExistingFile(lst, path);
        //            path = "";
        //        }
        //        else
        //            SimpleLogger.LogStatically($"File {path.Trim().Trim('\"')} doesn't exist.");

        //        if (File.Exists(fullPath.Trim('/')))
        //        {
        //            HandleExistingFile(lst, fullPath);
        //            fullPath = "";
        //        }
        //    }
        //    SetUpPlayingEnvironment(lst);
        //}

        //private static void HandleExistingFile(List<Composition> lst, string path)
        //{
        //    SimpleLogger.LogStatically($"File {path.Trim().Trim('\"')} exists ok.");
        //    Program.DBAccess = new DBRepository() { DB = new MediaStreamer.DataAccess.CrossPlatform.JSONDataContext() };
        //    Program.FileManipulator = new IO.FileManipulator(Program.DBAccess);

        //    var cmp = Program.FileManipulator.DecomposeAudioFile(path.Trim().Trim('\"'));
        //    lst.Add(cmp);
        //}

        //private static void SetUpPlayingEnvironment(List<Composition> lst)
        //{
        //    if (lst.Count() > 0)
        //    {
        //        Program.startupFromCommandLine = true;
        //        Session.CompositionsVM = new CompositionsViewModel();
        //        foreach (var c in lst)
        //        {
        //            Session.CompositionsVM.CompositionsStore.Queue.AddLast(c);
        //        }
        //        Program.currentComposition = lst[0];
        //    }
        //}
    }
}
