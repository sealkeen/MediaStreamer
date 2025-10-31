using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;
using MediaStreamer.RAMControl;
using System.Linq;
using MediaStreamer.Logging;
using System.Windows.Input;
using System.Windows.Controls;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для Compositions.xaml
    /// </summary>
    public partial class CompositionsPage : FirstFMPage
    {
        private async void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Action<string>? logMethod = Console.WriteLine;
            // TODO: Add logger, logging
            if(Program._logger != null) 
                logMethod= Program._logger.LogInfo;

            Session.MainPageVM.SetSkip(Session.MainPageVM.GetSkip() - Session.MainPageVM.GetTake(), logMethod);
            if (Session.MainPageVM.GetSkip() < 0)
                Session.MainPageVM.SetSkip(0, logMethod);

            Session.CompositionsVM.CompositionsStore.Compositions = await Program.DBAccess.DB.GetICompositionsAsync
                (Session.MainPageVM.GetSkip(), Session.MainPageVM.GetTake());
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();

            _ = Task.Factory.StartNew(() =>
            {
                Session.MainPageVM.SetTotal(Program.DBAccess.DB.GetCompositions().Count());
                Session.MainPageVM.UpdateBindingExpression();
            }
            );
        }

        private void lstQuery_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            CurrentListView = sender as Control;
        }

        private async void btnBegin_Click(object sender, RoutedEventArgs e)
        {
            Session.MainPageVM.SetSkip(0, Program._logger.LogInfo);

            Session.CompositionsVM.CompositionsStore.Compositions = await Program.DBAccess.DB.GetICompositionsAsync
                (0, Session.MainPageVM.GetTake());

            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();

            _ = Task.Factory.StartNew(() =>
                {
                    Session.MainPageVM.SetTotal(Program.DBAccess.DB.GetCompositions().Count());
                    Session.MainPageVM.UpdateBindingExpression();
                }
            );
        }

        private async void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            Session.MainPageVM.SetSkip(Session.MainPageVM.GetLastPageIndex() * Session.MainPageVM.GetTake(), Program._logger.LogInfo);
            
            Session.CompositionsVM.CompositionsStore.Compositions = await Program.DBAccess.DB.GetICompositionsAsync
                (Session.MainPageVM.GetLastPageIndex(), Session.MainPageVM.GetTake());
            
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();

            _ = Task.Factory.StartNew(() =>
                {
                    Session.MainPageVM.SetTotal(Program.DBAccess.DB.GetCompositions().Count());
                    Session.MainPageVM.UpdateBindingExpression();
                }
            );
        }

        private async void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Session.MainPageVM.SetSkip(Session.MainPageVM.GetSkip() + Session.MainPageVM.GetTake(), Program._logger.LogInfo);
            Session.CompositionsVM.CompositionsStore.Compositions = await Program.DBAccess.DB.GetICompositionsAsync
                (Session.MainPageVM.GetSkip(), Session.MainPageVM.GetTake());
            lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();

            _ = Task.Factory.StartNew(() =>
                {
                    Session.MainPageVM.SetTotal(Program.DBAccess.DB.GetCompositions().Count());
                    Session.MainPageVM.UpdateBindingExpression();
                }
            );
        }

        protected async void buttonNewComp_Click(object sender, RoutedEventArgs e)
        {
            var tsk = await Program.FileManipulator.OpenAudioFileCrossPlatform();

            Program.FileManipulator.DecomposeAudioFile(tsk, Program._logger?.GetLogErorrOrReturnNull());
            ReList();
        }

        protected void buttonNewRange_Click(object sender, RoutedEventArgs e)
        {
            Program.FileManipulator.DecomposeAudioFiles(Program.FileManipulator.OpenAudioFilesCrossPlatform(), Program.SetCurrentStatus);
            ReList();
        }
    }
}
