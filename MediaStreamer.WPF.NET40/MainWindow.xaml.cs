using System.Windows;
using MediaStreamer.DataAccess.Net40;

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
            MediaStreamer.WPF.Components.Program.DBAccess = new DBAccess { DB = new DMEntities() };
            windowFrame.Content = new MediaStreamer.WPF.Components.MainPage();
        }
    }
}
