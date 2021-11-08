using System.Windows;
using DataBaseResource;
using MediaStreamer.WindowsDesktop.RAMControl;

namespace MediaStreamer.WPF.NET40
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Program.DBAccess = new DBAccess { DB = new DMEntities() };
            InitializeComponent();
            //windowFrame.Content = new MainPage();
        }
    }
}
