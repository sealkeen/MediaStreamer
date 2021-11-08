using System.Windows;
using MediaStreamer.WindowsDesktop;
using DMEntitiesDataLibrary;
using DMEntitiesDataLibrary.Models;

namespace MediaStreamer.WPF.NetCore3_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            MediaStreamer.WindowsDesktop.RAMControl.Program.DBAccess = new DMEntitiesDataLibrary.NetCoreDBRepository();
            MediaStreamer.WindowsDesktop.RAMControl.Program.DBAccess.DB = new DMEntitiesDataLibrary.DMEntitiesContext();

            Program.DBAccess = new DMEntitiesDataLibrary.NetCoreDBRepository();
            Program.DBAccess.DB = new DMEntitiesDataLibrary.DMEntitiesContext();
            //DMEntitiesDataLibrary.DBAccess.dB = new DMEntitiesDataLibrary.DMEntitiesContext();
            //Program.DBAccess = new DMEntitiesDataLibrary.DBAccess();
            //Program.DBAccess.DB = new DMEntitiesDataLibrary.DMEntitiesContext();
            InitializeComponent();
            windowFrame.Content = new MainPage();
        }
    }
}
