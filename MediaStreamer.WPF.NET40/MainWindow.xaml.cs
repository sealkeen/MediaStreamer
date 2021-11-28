using System.Windows;
using MediaStreamer.IO;
using MediaStreamer.DataAccess.Net40;
using MediaStreamer.WPF.Components;
using MediaStreamer.Domain;
using System;

namespace DMultHandler
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
            MediaStreamer.WPF.Components.Program.DBAccess = new DBRepository { DB = new DMEntities() };
            
            this.Content = new MainPage();
            Session.MainPage.DataBaseClick += this.btnDatabase_Click;
        }

        private async void btnDatabase_Click(object sender, RoutedEventArgs e)
        {
            MediaStreamer.IO.FileManipulator fM = new MediaStreamer.IO.FileManipulator(Program.DBAccess);
            var fullpath = await fM.GetOpenedDatabasePathAsync();

            try 
            {
                Program.DBAccess = new DBRepository() { DB = new DMEntities(fullpath) };
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }
    }
}
