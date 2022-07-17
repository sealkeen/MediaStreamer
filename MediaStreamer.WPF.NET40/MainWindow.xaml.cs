using MediaStreamer.DataAccess.CrossPlatform;
using MediaStreamer.DataAccess.Net40;//.SQLite;
using MediaStreamer.Domain;
using MediaStreamer.RAMControl;
using MediaStreamer.WPF.Components;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace MediaStreamer.WPF.Net40
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //TODO: WPFNet40 / Core3.1 – Merge into single project WPF's
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                Selector.MainWindow = this;
                InitializeDataConnections().Wait();
                this.windowFrame.Content = new MainPage();
                //this.DataBaseClick += this.btnDatabase_Click;

                Program.DBAccess.DB.EnsureCreated();
                Program.ApplicationsSettingsContext?.EnsureCreated();

                Program._logger?.LogTrace($"The new position is : {Program.NewPosition}");
            } catch (Exception ex) {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Session.ChromiumPages != null)
                foreach (var page in Session.ChromiumPages.Values)
                    page?.ClosePageResources();
            Program.OnClosing();
        }

        public async Task InitializeDataConnections()
        {
            if (Program.DBAccess == null)
            {
                var task = Task.Factory.StartNew(() =>
                Program.DBAccess = new DBRepository()
                { DB = new MediaStreamer.DataAccess.CrossPlatform.JSONDataContext() });

                task.Wait();
            }
            if (Program.ApplicationsSettingsContext == null)
            {
                var task = Task.Factory.StartNew(() => Program.ApplicationsSettingsContext = new ApplicationSettingsContext());

                task.Wait();
            }
        }

        private async void btnDatabase_Click(object sender, RoutedEventArgs e)
        {
            MediaStreamer.IO.FileManipulator fM = new MediaStreamer.IO.FileManipulator(Program.DBAccess, Program._logger);
            var fullpath = fM.GetOpenedDatabasePathAsync();

            fullpath.Wait();
            try
            {
                Program.DBAccess = new DBRepository() { DB = new DMEntities() };
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }
    }
}