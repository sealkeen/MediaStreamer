﻿using MediaStreamer.Domain;
using MediaStreamer.RAMControl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
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
            var args = e.Args;
            if (args != null && args.Count() > 0)
            {
                List<Composition> lst = new List<Composition>();
                foreach (var arg in args)
                {
                    if (File.Exists(arg.Trim('\"')))
                    {
                        Program.DBAccess = new DBRepository() { DB = new MediaStreamer.DataAccess.CrossPlatform.JSONDataContext() };
                        Program.FileManipulator = new IO.FileManipulator(Program.DBAccess);

                        var cmp = Program.FileManipulator.DecomposeAudioFile(arg);
                        lst.Add(cmp);
                    }
                }
                if (lst.Count() > 0)
                {
                    Program.startupFromCommandLine = true;
                    Session.CompositionsVM = new CompositionsViewModel();
                    foreach (var c in lst)
                    {
                        Session.CompositionsVM.CompositionsStore.Queue.AddLast(c);
                    }
                    Program.currentComposition = lst[0];
                }
            }
        }
    }
}
