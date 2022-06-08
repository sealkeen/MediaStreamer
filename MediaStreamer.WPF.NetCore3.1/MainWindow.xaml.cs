
using MediaStreamer.Domain;
using MediaStreamer.IO;
using MediaStreamer.Logging;
using MediaStreamer.RAMControl;
using MediaStreamer.WPF.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using StringExtensions;
using System.Windows;
using MediaStreamer.DataAccess.NetStandard;

namespace MediaStreamer.WPF.NetCore3_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //TODO: WPFNet40 / Core3.1 – Merge into single project WPF's
        public MainWindow()
        {
            InitializeComponent();
            if (Program.DBAccess == null)
            {
                var task = Task.Factory.StartNew(() =>
                Program.DBAccess = new DBRepository()
                { DB = new MediaStreamer.DataAccess.CrossPlatform.JSONDataContext() });

                task.Wait();
            }
            Program.DBAccess.DB.EnsureCreated();
            $"The new position is : {Program.NewPosition}".LogStatically();

            ResolveCMDParamFilePaths();

            this.windowFrame.Content = new MediaStreamer.WPF.Components.MainPage();

            //Session.MainPage.DataBaseClick += this.btnDatabase_Click;
            //Session.MainPage.btnClose.Click += this.btnClose_Click;
            //Session.MainPage.btnMinimize.Click += this.btnMinimize_Click;
        }

        private static void ResolveCMDParamFilePaths()
        {
            try
            {
                string cmd = "Arguments: " + Environment.NewLine;
                foreach (var l in Environment.GetCommandLineArgs().Skip(1)) cmd += l + Environment.NewLine;
                cmd.LogStatically();
                var cmdList = CommandLine.StartUpFromCommandLine( Environment.GetCommandLineArgs().Skip(1).ToArray() );
                if (cmdList != null && cmdList.Count > 0)
                {
                    if (cmdList[0]?.FilePath == null)
                        "FATAL: CMD [0] FILEPATH IS NULL".LogStatically();
                    $"OK: CMD list is fine. Composition: {cmdList[0].FilePath}".LogStatically();
                    Program.currentComposition = cmdList[0];
                }
                else
                    "FATAL: CMD list is null".LogStatically();
                CommandLine.SetUpPlayingEnvironment(cmdList);
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus("ResolveCMDParamFilePaths() error: " + ex.Message);
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.Close();
            App.Current.Shutdown();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        private void btnBrowser_Click(object sender, RoutedEventArgs e)
        {
            //this.Dispatcher.BeginInvoke(new Action
            //    (
            //        delegate
            //        {
            //            var page = new MediaStreamer.WPF.Components.Web.ChromiumPage();
            //            if (Session.ChromiumPages == null)
            //                Session.ChromiumPages = new Dictionary<int, FirstFMPage>();
            //            Session.ChromiumPages.Add(page.PageID, page);
            //            Selector.MainPage.mainFrame.Content = Session.ChromiumPages[page.PageID];
            //        }
            //    )
            //).Wait();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(Session.ChromiumPages != null)
                foreach(var page in Session.ChromiumPages.Values)
                    page?.ClosePageResources();
            Program.OnClosing();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Selector.MainPage.StatusPage_PreviewKeyDown(sender, e);
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            var linkTimeLocal = getAssemblyBuildDateTime();
            MessageBox.Show("Program Version: " + linkTimeLocal.ToString());
        }

        private static DateTime? getAssemblyBuildDateTime()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var attr = Attribute.GetCustomAttribute(assembly, typeof(BuildDateTimeAttribute)) as BuildDateTimeAttribute;
            if (DateTime.TryParse(attr?.Date, out DateTime dt))
                return dt;
            else
                return null;
        }

        private void btnCMD_Click(object sender, RoutedEventArgs e)
        {
            string cmd = "CMD Arguments: ";
            foreach (var c in Environment.GetCommandLineArgs())
            {
                cmd += Environment.NewLine + c;
            }

            cmd += Environment.NewLine + Environment.NewLine + "Startup from command line: " + Environment.NewLine + Program.startupFromCommandLine;

            MessageBox.Show(cmd);
        }

        private async void btnSQLServer_Click(object sender, RoutedEventArgs e)
        {
            Selector.MainPage.SetFrameContent(Selector.LoadingPage ?? (Selector.LoadingPage = new LoadingPage()));
            DMEntitiesContext.UseSQLServer = true;
            var tsk = Task.Factory.StartNew(new Action( delegate {
                Program.DBAccess = new DBRepository() { DB = new DMEntitiesContext() };
                Program.DBAccess.DB.EnsureCreated();
            })
            );

            UpdatePageViews();
            await tsk;
            Program.FileManipulator = new MediaStreamer.IO.FileManipulator(Program.DBAccess);
            await Selector.CompositionsPage?.ListAsync();
        }

        private async void btnSQLJson_Click(object sender, RoutedEventArgs e)
        {
            Selector.MainPage.SetFrameContent(Selector.LoadingPage ?? (Selector.LoadingPage = new LoadingPage()));
            var tsk = Task.Factory.StartNew( () =>
                Program.DBAccess = new DBRepository() { DB = 
                    new MediaStreamer.DataAccess.CrossPlatform.JSONDataContext() 
                }
            );

            UpdatePageViews();
            await tsk;
            Program.FileManipulator = new MediaStreamer.IO.FileManipulator(Program.DBAccess);
            await Selector.CompositionsPage?.ListAsync();
        }

        private async void btnSQLite_Click(object sender, RoutedEventArgs e)
        {
            Selector.MainPage.SetFrameContent(Selector.LoadingPage ?? (Selector.LoadingPage = new LoadingPage()));
            DMEntitiesContext.UseSQLServer = false;
            var tsk = Task.Factory.StartNew(new Action(delegate
            {
                Program.DBAccess = new DBRepository() { DB = new DMEntitiesContext() };
                Program.DBAccess.DB.EnsureCreated();
            })
            );
            UpdatePageViews();
            await tsk;
            Program.FileManipulator = new MediaStreamer.IO.FileManipulator(Program.DBAccess);
            await Selector.CompositionsPage?.ListAsync();
        }

        private static void UpdatePageViews()
        {
            Selector.CompositionsPage = new CompositionsPage();
            Session.ArtistsVM = new ArtistsViewModel();
            Session.AlbumsVM = new AlbumsViewModel();
            Session.GenresVM = new GenresViewModel();
        }

        private void btnSQLJsonNavigate_Click(object sender, RoutedEventArgs e)
        {
            MediaStreamer.DataAccess.CrossPlatform.PathResolver.GetStandardDatabasePath().ExplorePath();
        }

        private void btnSQLiteNavigate_Click(object sender, RoutedEventArgs e)
        {
            MediaStreamer.DataAccess.NetStandard.PathResolver.GetStandardDatabasePath().ExplorePath();
        }

        private void btnLogs_Click(object sender, RoutedEventArgs e)
        {
            Session.MainPage.SetFrameContent(LoggerPage.GetInstance());
        }
    }
}
