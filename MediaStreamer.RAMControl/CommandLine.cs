using MediaStreamer.Domain;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MediaStreamer.RAMControl
{
    public class CommandLine
    {
        public static List<Composition> StartUpFromCommandLine(string[] args)
        {
            Program.startupFromCommandLine = false;
            Program._logger?.LogTrace("Command line startup executed ok.");
            if (args.Count() <= 0)
            {
                Program._logger?.LogTrace("error: Args Count <= 0.");
                return null;
            }

            Program._logger?.LogInfo($"OK: Args Count <{args.Count()}> > 0.");
            Program.startupFromCommandLine = true;
            List<Composition> lst = new List<Composition>();

            Program._logger?.LogTrace("Starting ForEach.");
            foreach (var c in args)
            {
                if (File.Exists(c.Trim().Trim('\"')))
                {
                    Program._logger?.LogInfo($"File {c.Trim().Trim('\"')} exists ok.");
                    HandleExistingFile(lst, c);
                } else {
                    Program._logger?.LogError($"{c.Trim().Trim('\"')} doesn't exist.");
                }
            }

            return lst;
        }

        private static void HandleExistingFile(List<Composition> lst, string path)
        {
            Program._logger?.LogTrace($"File {path.Trim().Trim('\"')} exists ok.");

            if (Program.FileManipulator == null)
            {
                Program._logger?.LogTrace($"Creating file manipulator : {File.Exists(path)}");
                Program.FileManipulator = new MediaStreamer.IO.FileManipulator(Program.DBAccess, Program._logger);
            }
            Program._logger?.LogInfo($"Passing to decompose: {path}, existing : {File.Exists(path)}");
            var cmp = Program.FileManipulator.DecomposeAudioFile(path.Trim().Trim('\"'), Program._logger.LogInfo);

            Program._logger?.LogInfo($"Adding comp to list and it is valid: {cmp != null && cmp.FilePath != null}");
            lst.Add(cmp);
        }

        public static void SetUpPlayingEnvironment(List<Composition> lst)
        {
            if (lst == null || lst.Count() <= 0)
            {
                Program._logger?.LogTrace("CMD valid arguments == 0");
                return;
            }

            if (lst.Count() > 0)
            {
                Program.currentComposition = lst[0];
                Program._logger?.LogTrace($"CMD valid arguments == {lst.Count()}");
                Session.CompositionsVM = new CompositionsViewModel();
                foreach (var c in lst)
                {
                    Session.CompositionsVM.CompositionsStore.Queue.AddLast(c);
                }
            }
        }
    }
}
