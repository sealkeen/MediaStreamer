
using MediaStreamer.Domain;
using MediaStreamer.RAMControl;
using MediaStreamer.WPF.Components;
using System;
using System.Threading.Tasks;
//using Xamarin.Forms;
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
            //MediaStreamer.DMEntitiesContext.UseSQLServer = true;
            //Program.DBAccess.LoadingTask = task;

            InitializeComponent();

            var task = Task.Factory.StartNew(() =>
            Program.DBAccess = new DBRepository()
            { DB = new MediaStreamer.DataAccess.CrossPlatform.JSONDataContext() });

            task.Wait();
            Program.DBAccess.DB.EnsureCreated();
            this.windowFrame.Content = new MediaStreamer.WPF.Components.MainPage();

            this.btnDatabase.Click += this.btnDatabase_Click;
            //Session.MainPage.DataBaseClick += this.btnDatabase_Click;
            //Session.MainPage.btnClose.Click += this.btnClose_Click;
            //Session.MainPage.btnMinimize.Click += this.btnMinimize_Click;
        }

        private async void btnDatabase_Click(object sender, RoutedEventArgs e)
        {
            MediaStreamer.IO.FileManipulator fM = new IO.FileManipulator(Program.DBAccess);
            var fullpath = await fM.GetOpenedDatabasePathAsync();
            try
            {
                //Program.DBAccess = new DBRepository() { DB = new MediaStreamer.DMEntities(fullpath) };
                Program.FileManipulator = new MediaStreamer.IO.FileManipulator(Program.DBAccess);
            } catch (Exception ex) {
                Program.SetCurrentStatus(ex.Message);
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
            this.Dispatcher.BeginInvoke(new Action
                (
                    delegate
                    {
                        Session.ChromiumPage = new MediaStreamer.WPF.Components.Web.ChromiumPage();
                        Selector.MainPage.mainFrame.Content = Session.ChromiumPage;
                    }
                )
            ).Wait();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Session.ChromiumPage?.ClosePageResources();
        }
    }
}
