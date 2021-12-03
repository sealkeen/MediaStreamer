using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MediaStreamer.DataAccess.Net40.SQLServer;
using MediaStreamer.Domain;
using MediaStreamer.DataAccess.Net40;

namespace MediaStreamer.WPF.Net40.VS2010
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //var file = Plugin.FilePicker.CrossFilePicker.Current.PickFile();

            //MediaStreamer.WPF.Components.Program.DBAccess = new DBRepository { DB = new DMEntities() };
            //this.Content = new MediaStreamer.WPF.Components.MainPage();
            //MediaStreamer.WPF.Components.Session.MainPage.DataBaseClick += this.btnDatabase_Click;
        }

        public IEnumerable<MediaStreamer.DataAccess.Net40.SQLServer.Composition> Compositions;
        private void btnDatabase_Click(object sender, RoutedEventArgs e)
        {
            var ent = new MediaStreamer.DataAccess.Net40.SQLServer.compositionsdbEntities();
            //var ent = new DataBaseResource.DMEntitiesContext();
            //var repository = new DBRepository { DB = ent };
            //var repository = new DBRepository { DB = new DMEntities() };
            DataContext = this;
            var tsk = ent.Compositions.ToList();
            //tsk.Wait();
            Compositions = tsk;//.Result.ToList();
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();

            //MediaStreamer.IO.FileManipulator fM = new MediaStreamer.IO.FileManipulator(MediaStreamer.WPF.Components.Program.DBAccess);
            //var fullpath = fM.GetOpenedDatabasePathAsync();

            //fullpath.Wait();
            //try 
            //{
            //    MediaStreamer.WPF.Components.Program.DBAccess = new DBRepository() { DB = new DMEntities() };
            //}
            //catch (Exception ex)
            //{
            //    MediaStreamer.WPF.Components.Program.SetCurrentStatus(ex.Message);
            //}
        }
    }
}
