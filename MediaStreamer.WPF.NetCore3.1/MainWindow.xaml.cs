using System.Windows;
using MediaStreamer.Domain;
using MediaStreamer.WindowsDesktop;
using MediaStreamer.Models;

namespace MediaStreamer.WPF.NetCore3_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            MediaStreamer.WindowsDesktop.RAMControl.Program.DBAccess = new MediaStreamer.NetCoreDBRepository();
            MediaStreamer.WindowsDesktop.RAMControl.Program.DBAccess.DB = new MediaStreamer.DMEntitiesContext();

            Program.DBAccess = new MediaStreamer.NetCoreDBRepository();
            Program.DBAccess.DB = new MediaStreamer.DMEntitiesContext();
            //DMEntitiesDataLibrary.DBAccess.dB = new DMEntitiesDataLibrary.DMEntitiesContext();
            //Program.DBAccess = new DMEntitiesDataLibrary.DBAccess();
            //Program.DBAccess.DB = new DMEntitiesDataLibrary.DMEntitiesContext();
            InitializeComponent();
            windowFrame.Content = new MainPage();
        }
    }
}
