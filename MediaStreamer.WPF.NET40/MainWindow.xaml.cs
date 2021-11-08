using System.Windows;
using MediaStreamer;
using MediaStreamer.Domain;
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
            MediaStreamer.WindowsDesktop.RAMControl.Program.DBAccess = 
                new DBAccess { DB = new MediaStreamer.DMEntities() };
            InitializeComponent();
            //windowFrame.Content = new MainPage();
        }
    }
}
