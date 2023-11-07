using MediaStreamer.DataAccess.CrossPlatform;
using MediaStreamer.DataAccess.NetStandard;
using MediaStreamer.Domain;
using MediaStreamer.RAMControl;
using MediaStreamer.WPF.Components;
using StringExtensions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
            try
            {
                Task.Run(() => InitializeDataConnections(
                    afterLoad : () => {
                        windowFrame.Content = new MediaStreamer.WPF.Components.MainPage();
                    }
                ));

                Selector.MainWindow = this;

                ResolveCMDParamFilePaths();
            }
            catch (Exception ex) {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        public async Task InitializeDataConnections(Action afterLoad)
        {
            if (Program.DBAccess == null) {
                DMEntitiesContext.UseSQLServer = true;
                var context = //new DMEntitiesContext();
                    new JSONDataContext(Program.SetCurrentStatus);
                Program.DBAccess = new DBRepository() { DB = context };
            }
            if (Program.ApplicationsSettingsContext == null) {
                Program.ApplicationsSettingsContext = await ApplicationSettingsContext.GetInstanceAsync();
            }
            Program.DBAccess.DB.EnsureCreated();
            Program.ApplicationsSettingsContext.EnsureCreated();
            Program._logger?.LogTrace($"The new position is : {Program.NewPosition}");
            await this.Dispatcher.BeginInvoke( afterLoad );
        }

        private static void ResolveCMDParamFilePaths()
        {
            try
            {
                string cmd = "Arguments: " + Environment.NewLine;
                foreach (var l in Environment.GetCommandLineArgs().Skip(1)) cmd += l + Environment.NewLine;
                Program._logger?.LogTrace(cmd);
                var cmdList = CommandLine.StartUpFromCommandLine( Environment.GetCommandLineArgs().Skip(1).ToArray() );
                if (cmdList != null && cmdList.Count > 0)
                {
                    if (cmdList[0]?.FilePath == null)
                        Program._logger?.LogTrace("FATAL: CMD [0] FILEPATH IS NULL");
                    Program._logger?.LogTrace($"OK: CMD list is fine. Composition: {cmdList[0].FilePath}");
                    Program.currentComposition = cmdList[0];
                }
                else
                    Program._logger?.LogTrace("FATAL: CMD list is null");
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
                Program.DBAccess?.DB.EnsureCreated();
            })
            );

            UpdatePageViews();
            await tsk;
            Program.FileManipulator = new MediaStreamer.IO.FileManipulator(Program.DBAccess, Program._logger);
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
            Program.FileManipulator = new MediaStreamer.IO.FileManipulator(Program.DBAccess, Program._logger);
            await Selector.CompositionsPage?.ListAsync();
        }

        private async void btnSQLite_Click(object sender, RoutedEventArgs e)
        {
            Selector.MainPage.SetFrameContent(Selector.LoadingPage ?? (Selector.LoadingPage = new LoadingPage()));
            DMEntitiesContext.UseSQLServer = false;
            var tsk = Task.Factory.StartNew(new Action(delegate
            {
                Program.DBAccess = new DBRepository() { DB = new DMEntitiesContext() };
                Program.DBAccess?.DB.EnsureCreated();
            })
            );
            UpdatePageViews();
            await tsk;
            Program.FileManipulator = new MediaStreamer.IO.FileManipulator(Program.DBAccess, Program._logger);
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

        private void btnSQLJSONClear_Click(object sender, RoutedEventArgs e)
        {
            Program.DBAccess?.DB.Clear();
        }

        private void btnSQLiteNavigate_Click(object sender, RoutedEventArgs e)
        {
            MediaStreamer.DataAccess.NetStandard.PathResolver.GetStandardDatabasePath().ExplorePath();
        }

        private void btnLogs_Click(object sender, RoutedEventArgs e)
        {
            Session.MainPage?.SetFrameContent(LoggerPage.GetInstance());
        }

        private void btnClearData_Click(object sender, RoutedEventArgs e)
        {
            var ok = MessageBox.Show($"Clear all data from: {Program.DBAccess?.DB.ToString()}", "Dateting all data", MessageBoxButton.YesNoCancel);
            if (ok == MessageBoxResult.Yes)
            {
                Program.DBAccess?.DB.Clear();
            }
        }
            
        private void btnAddRawData_Click(object sender, RoutedEventArgs e)
        {
            Session.MainPage.SetFrameContent(AddNewItems.GetPage());
        }

        private async void btnSQLiteLoadFile_Click(object sender, RoutedEventArgs e)
        {
            Selector.MainPage.SetFrameContent(Selector.LoadingPage ?? (Selector.LoadingPage = new LoadingPage()));
            //var tsk = Task.Factory.StartNew(new Action(async delegate
            //{
            var path = await Program.FileManipulator.GetOpenedDatabasePathAsync();
            if (!File.Exists(path))
                return;
            Program.DBAccess = new DBRepository() { DB = new DataAccess.RawSQL.ReadonlyDBContext(path, Program._logger) };
            //})
            //);
            UpdatePageViews();
            //await tsk;
            Program.FileManipulator = new MediaStreamer.IO.FileManipulator(Program.DBAccess, Program._logger);
            Selector.CompositionsPage?.ListAsync();
        }

        public Style BlankStyle { get; set; }
        public const int delay = 500;
        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Program.mePlayer != null)
                    Program.NewPosition = Program.mePlayer.Position;
                else
                    Program.NewPosition = TimeSpan.FromSeconds(0.0);
                if (this.Style != BlankStyle)
                {
                    InitializeAndSetEmptyStyle();

                    this.Style = BlankStyle;
                }
                else
                {
                    this.Style = (Style)FindResource("MainWindowStyle");
                }
                InitializeComponent();
                Selector.Rerender();
                Program.OnClosing();
                //this.Close();

            } catch (Exception ex) {

                Program._logger?.LogError($" UI Reload failed. ");
                Program.SetCurrentStatus(" while changing the window style: " + ex.Message);
            } finally {
                // Hacking the UI reload (reseting the player position)
                Task.Factory.StartNew(new Action(delegate
                {
                    Thread.Sleep(delay);
                    Selector.MainWindow.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        if(Program.mePlayer != null)
                            Program.mePlayer.Position = Program.NewPosition + TimeSpan.FromMilliseconds(delay);
                    }));
                }));
            }
        }
            
        private void InitializeAndSetEmptyStyle()
        {
            if (BlankStyle == null)
                BlankStyle = new Style
                {
                    TargetType = typeof(Window)
                };
        }
    }
}
