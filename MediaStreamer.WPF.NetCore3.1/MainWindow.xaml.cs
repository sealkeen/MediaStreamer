
using MediaStreamer.Domain;
using MediaStreamer.RAMControl;
using MediaStreamer.WPF.Components;
using System;
using System.Collections.Generic;
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
            InitializeComponent();
            if (Program.DBAccess == null)
            {
                var task = Task.Factory.StartNew(() =>
                Program.DBAccess = new DBRepository()
                { DB = new MediaStreamer.DataAccess.CrossPlatform.JSONDataContext() });

                task.Wait();
            }
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
                        var page = new MediaStreamer.WPF.Components.Web.ChromiumPage();
                        if (Session.ChromiumPages == null)
                            Session.ChromiumPages = new Dictionary<int, FirstFMPage>();
                        Session.ChromiumPages.Add(page.PageID, page);
                        Selector.MainPage.mainFrame.Content = Session.ChromiumPages[page.PageID];
                    }
                )
            ).Wait();
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
    }
}
