using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using MediaStreamer.FileTypes;
using System.Linq;
using MediaStreamer.IO;

namespace DMultHandler
{
    /// <summary>
    /// Логика взаимодействия для UserCompositionsPage.xaml
    /// </summary>
    public partial class UserCompositionsPage : FirstFMPage
    {
        public bool lastDataLoadWasPartial = false;
        public List<ListenedComposition> Compositions { get; set; }
        public void GetCompositions(Action<string> errorAction = null)
        {
            try
            {
                Compositions = Program.DBAccess.GetCurrentUsersListenedCompositions(SessionInformation.CurrentUser).ToList();
                lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
            }
            catch (Exception ex)
            {
                errorAction?.Invoke(ex.Message);
            }
        }

        public void ListCompositions()
        {
            GetCompositions();
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }

        public UserCompositionsPage()
        {
            //Compositions = new List<ListenedComposition>();
            InitializeComponent();
            DataContext = this;
            ListCompositions();
        }

        private void buttonListen_Click(object sender, RoutedEventArgs e)
        {
            Program.FileManipulator.DecomposeAudioFile(Program.FileManipulator.OpenAudioFileInMSWindows());
            ListCompositions();
        }

        private void buttonNew_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lstItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (null == (((FrameworkElement)e.OriginalSource).DataContext as ListenedComposition))
                return;
            var lcomp = Compositions[lstItems.SelectedIndex];

            Session.CompositionsPage.PlayTarget(lcomp.Composition);
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            cmiDelete_Click(sender, e);
        }

        private void cmiChangeComposition_Click(object sender, RoutedEventArgs e)
        {
            Session.CompositionsPage.ChangeComposition(lstItems.SelectedItems);
        }

        private void cmiPlaySeveral_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Program.FileManipulator.PlaySeveralSongs(lstItems.SelectedItems, typeof(ListenedComposition), Program.SetCurrentStatus);
                lstItems.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        private void cmiDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = lstItems.SelectedIndex;
                if (index != -1)
                {
                    Program.DBAccess.DeleteListenedComposition(Compositions[index]);
                    Program.DBAccess.DB.SaveChanges();
                }
                ListCompositions();
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        private void cmiQueue_Click(object sender, RoutedEventArgs e)
        {
            Session.CompositionsPage.QueueSelected(lstItems.SelectedItems);
        }
    }
}
