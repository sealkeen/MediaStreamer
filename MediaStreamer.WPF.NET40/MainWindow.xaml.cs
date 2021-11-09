using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Timers;
using System.Windows.Forms;
using MediaStreamer.WindowsDesktop.DataAccess.Net40;

namespace DMultHandler
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MediaStreamer.WindowsDesktop.Program.DBAccess = new DBAccess { DB = new DMEntities() };
            MediaStreamer.WindowsDesktop.Program.FileManipulator = 
                new MediaStreamer.IO.FileManipulator(MediaStreamer.WindowsDesktop.Program.DBAccess);
            //MediaStreamer.WindowsDesktop.Session.MainPage = this;

            windowFrame.Content = new MediaStreamer.WindowsDesktop.MainPage();
        }
    }
}
