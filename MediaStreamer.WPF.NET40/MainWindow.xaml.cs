using System.Windows;
using MediaStreamer.DataAccess.Net40;
using MediaStreamer.WPF.Components;
using MediaStreamer.Domain;

namespace DMultHandler
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
            MediaStreamer.WPF.Components.Program.DBAccess = new DBRepository { DB = new DMEntities() };
            
            this.Content = new MainPage();
        }
    }
}
