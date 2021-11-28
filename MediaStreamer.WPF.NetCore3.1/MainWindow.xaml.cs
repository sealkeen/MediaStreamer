using System;
using System.Threading.Tasks;
using System.Windows;
using MediaStreamer.Domain;
using MediaStreamer.WPF.Components;

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
            //var task = Task.Factory.StartNew(() => 
            Program.DBAccess = new DBRepository()
            { DB = new MediaStreamer.DMEntitiesContext() }; //);

            //Program.DBAccess.LoadingTask = task;

            InitializeComponent();
            this.Content = new MainPage();
            Session.MainPage.DataBaseClick += this.btnDatabase_Click;
        }

        private async void btnDatabase_Click(object sender, RoutedEventArgs e)
        {
            MediaStreamer.IO.FileManipulator fM = new IO.FileManipulator(Program.DBAccess);
            var fullpath = await fM.GetOpenedDatabasePathAsync();
            try
            {
                Program.DBAccess = new DBRepository() { DB = new MediaStreamer.DMEntitiesContext(fullpath) };
            } catch (Exception ex) {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
