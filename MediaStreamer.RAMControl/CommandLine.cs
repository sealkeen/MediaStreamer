using MediaStreamer.Domain;
using MediaStreamer.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace MediaStreamer.RAMControl
{
    public class CommandLine
    {
        public static List<Composition> StartUpFromCommandLine(string[] args)
        {
            Program.startupFromCommandLine = false;
            "Command line startup executed ok.".LogStatically();
            if (args.Count() <= 0)
            {
                "FATAL: Args Count <= 0.".LogStatically();
                return null;
            }
            $"OK: Args Count <{args.Count()}> > 0.".LogStatically();
            Program.startupFromCommandLine = true;
            List<Composition> lst = new List<Composition>();


            "Starting ForEach.".LogStatically();
            foreach (var c in args)
            {
                if (File.Exists(c.Trim().Trim('\"')))
                {
                    SimpleLogger.LogStatically($"File {c.Trim().Trim('\"')} exists ok.");
                    HandleExistingFile(lst, c);
                } else {
                    SimpleLogger.LogStatically($"FATAL: {c.Trim().Trim('\"')} doesn't exist.");
                }
            }

            return lst;
        }

        private static void HandleExistingFile(List<Composition> lst, string path)
        {
            SimpleLogger.LogStatically($"File {path.Trim().Trim('\"')} exists ok.");

            if (Program.FileManipulator == null)
            {
                $"Creating file manipulator : {File.Exists(path)}".LogStatically();
                Program.FileManipulator = new MediaStreamer.IO.FileManipulator(Program.DBAccess);
            }
            $"Passing to decompose: {path}, existing : {File.Exists(path)}".LogStatically();
            var cmp = Program.FileManipulator.DecomposeAudioFile(path.Trim().Trim('\"'), MediaStreamer.Logging.SimpleLogger.LogStatically);

            $"Adding comp to list and it is valid: {cmp != null && cmp.FilePath != null}".LogStatically();
            lst.Add(cmp);
        }

        public static void SetUpPlayingEnvironment(List<Composition> lst)
        {
            if (lst == null || lst.Count() <= 0)
            {
                "CMD valid arguments == 0".LogStatically();
                return;
            }
            if (lst.Count() > 0)
            {
                Program.currentComposition = lst[0];
                $"CMD valid arguments == {lst.Count()}".LogStatically();
                Session.CompositionsVM = new CompositionsViewModel();
                foreach (var c in lst)
                {
                    Session.CompositionsVM.CompositionsStore.Queue.AddLast(c);
                }
            }
        }
    }
}
