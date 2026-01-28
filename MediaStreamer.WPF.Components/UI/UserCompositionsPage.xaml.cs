using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MediaStreamer.Domain;
using MediaStreamer.Logging;
using MediaStreamer.RAMControl;
using MediaStreamer.WPF.Components.Services;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для UserCompositionsPage.xaml
    /// </summary>
    public partial class UserCompositionsPage : FirstFMPage
    {
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

        public override void List()
        {
            GetCompositions();
            this.Dispatcher.BeginInvoke( new Action( delegate {
                lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget(); 
            }));
        }

        public UserCompositionsPage()
        {
            //Compositions = new List<ListenedComposition>();
            InitializeComponent();
            DataContext = this;
            List();
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
        }

        private void buttonListen_Click(object sender, RoutedEventArgs e)
        {
            Program.FileManipulator.DecomposeAudioFile(
                    Program.FileManipulator.OpenAudioFileCrossPlatform(new FilePicker()).Result, Program._logger?.GetLogErorrOrReturnNull()
                );
            List();
        }

        private void buttonNew_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lstItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (null == (((FrameworkElement)e.OriginalSource).DataContext as ListenedComposition))
                return;
            var lcomp = Compositions[lstItems.SelectedIndex];

            Selector.CompositionsPage.PlayTarget(lcomp.Composition);
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            cmiDelete_Click(sender, e);
        }

        private void cmiChangeComposition_Click(object sender, RoutedEventArgs e)
        {
            Selector.CompositionsPage.ChangeComposition(lstItems.SelectedItems);
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
                List();
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message);
            }
        }

        private void cmiQueue_Click(object sender, RoutedEventArgs e)
        {
            Selector.CompositionsPage.QueueSelected(lstItems.SelectedItems);
        }
    }
}
