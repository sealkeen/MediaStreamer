using MediaStreamer.Domain;
using MediaStreamer.WPF.Components;
using Microsoft.Win32;
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
using System.Windows.Shapes;
using MediaStreamer.DataAccess.Net40;//.SQLite;

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
                MediaStreamer.WPF.Components.Program.DBAccess = new DBRepository { DB = new DMEntities() };

                this.windowFrame.Content = new MainPage();
                //this.DataBaseClick += this.btnDatabase_Click;
            } catch (Exception ex) {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        private async void btnDatabase_Click(object sender, RoutedEventArgs e)
        {
            MediaStreamer.IO.FileManipulator fM = new MediaStreamer.IO.FileManipulator(Program.DBAccess);
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