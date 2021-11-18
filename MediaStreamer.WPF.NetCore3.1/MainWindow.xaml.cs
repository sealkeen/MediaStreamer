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
            Program.DBAccess = new DBRepository() 
            {  DB = new MediaStreamer.DMEntitiesContext() };

            InitializeComponent();
            this.Content = new MainPage();
        }
    }
}
