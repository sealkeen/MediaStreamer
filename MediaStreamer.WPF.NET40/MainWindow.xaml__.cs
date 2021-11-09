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

namespace FirstFMCourse
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static FirstFMEntities dB;

        public MainWindow()
        {
            InitializeComponent();
            DataBaseResource.ShowGroupMembers();
            //dataGrid.ItemsSource = 
            dB = new FirstFMEntities();
            DataBaseResource.dB = dB;
        }


        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {
            DataBaseResource.PopulateDataBase();
            DataBaseResource.ShowGroupMembers();
        }

        //List<>
        private void buttonBind_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonCompositions_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new FirstFMCourse.ListenedCompositionsPage();
        }

        private void buttonAlbums_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new FirstFMCourse.ListenedAlbumsPage();
        }

        private void buttonArtistGenres_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new FirstFMCourse.ArtistGenresPage();
        }

        private void buttonGroupMembers_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new FirstFMCourse.GroupMembersPage();
        }

        private void buttonArtists_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new FirstFMCourse.ArtistsPage();
        }
    }
}
