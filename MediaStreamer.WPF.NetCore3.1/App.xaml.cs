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
                KillExistingProgramWindow();

                // Crazy hack
                var args1 = Environment.GetCommandLineArgs()/*.CreateArgs().*/.Skip(1);
                if (args1 != null && args1.Count() > 0)
                {
                    var arguments1 = args1.Aggregate((a, b) => Path.Combine(a + Environment.NewLine + b));
                    Program._logger?.LogTrace($"cmd arguments 1 : {arguments1}");
                    Program._logger?.LogTrace("Arguments.Count() > 0, ok.");
                }
            }
            catch (Exception ex) {
                Program._logger?.LogError(ex.ToString()+ ex.Message);
            }
        }

        private static void KillExistingProgramWindow()
        {
            try {
                if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
                {
                    Program._logger?.LogDebug("Another instance of the application is already running.");
                    foreach (var proc in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location))
                        .Where(p => p.Handle != System.Diagnostics.Process.GetCurrentProcess().Handle
                        && p.Id != System.Diagnostics.Process.GetCurrentProcess().Id
                        ))
                    {
                        Program._logger?.LogDebug("Killing the process...");
                        proc.CloseMainWindow();
                    }
                    Program._logger.LogDebug("Waiting for another instance to shut down...");
                    System.Threading.Thread.Sleep(450);
                }
            } catch { 

            }
        }
    }
}
