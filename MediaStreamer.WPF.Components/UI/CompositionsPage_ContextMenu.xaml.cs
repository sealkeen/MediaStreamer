using MediaStreamer.Domain;
using MediaStreamer.RAMControl;
using StringExtensions;
using System;
using System.Diagnostics;
using System.Windows;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для Compositions.xaml
    /// </summary>
    public partial class CompositionsPage : FirstFMPage
    {
        protected void cmiEnQueue_Click(object sender, RoutedEventArgs e)
        {
            QueueSelected();
        }

        protected void cmiMoveToEnd_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = CurrentListView == lstItems
                ? lstItems.SelectedIndex : lstQuery.SelectedIndex;
            if (selectedIndex >= 0)
            {
                var selectedItem = (CurrentListView == lstItems ? lstItems.SelectedItem : lstQuery.SelectedItem) as IComposition;

                Program.DBAccess.MoveCompositionTo(selectedItem.CompositionID, Domain.Enums.EMoveDirection.End);
            }
        }

        protected void cmiOpenLocation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedIndex = CurrentListView == lstItems ? lstItems.SelectedIndex : lstQuery.SelectedIndex;
                if (selectedIndex >= 0)
                {
                    var selectedItem = (CurrentListView == lstItems ?
                        lstItems.SelectedItem : lstQuery.SelectedItem) as IComposition;
                    if (selectedItem.FilePath.FileExists())
                    {
                        selectedItem.FilePath.SelectInExplorer();
                    }
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message, true);
            }
            finally
            {
                ReList();
            }
        }

        protected void cmiPush_Click(object sender, RoutedEventArgs e)
        {
            QueueSelected(false);
        }

        protected void cmiUpdate_Click(object sender, RoutedEventArgs e)
        {
            ReList();
        }

        protected void cmiChangeComposition_Click(object sender, RoutedEventArgs e)
        {
            ChangeComposition(CurrentListView ==lstItems ? lstItems.SelectedItems : lstQuery.SelectedItems);
        }

        protected async void cmiRename_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedIndex = CurrentListView ==lstItems ? lstItems.SelectedIndex : lstQuery.SelectedIndex ;
                var selectedItems = CurrentListView ==lstItems ? lstItems.SelectedItems : lstQuery.SelectedItems;
                if (selectedIndex >= 0 && selectedItems.Count >= 1)
                { // For every selected element
                    Session.CompositionsVM.CompositionsStore.ChangeCompositionTags(selectedItems);
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message, true);
            }
            finally
            {
                Session.CompositionsVM.CompositionsStore.Compositions = await GetICompositions();
                lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
                lastDataLoadWasPartial = false;
            }
        }

        protected void cmiDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedIndex = CurrentListView ==lstItems ? lstItems.SelectedIndex : lstQuery.SelectedIndex;
                var selectedItem = CurrentListView ==lstItems ? lstItems.SelectedItem : lstQuery.SelectedItem;
                if (selectedIndex >= -1)
                {
                    Program.DBAccess?.DeleteComposition((selectedItem as IComposition).GetInstance());
                }
                ReList();
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus($"Delete composition violation: {ex.Message}", true);
            }
        }

        private void cmiPlayInAimp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedIndex = CurrentListView == lstItems ? lstItems.SelectedIndex : lstQuery.SelectedIndex;
                var selectedItem = CurrentListView == lstItems ? lstItems.SelectedItem : lstQuery.SelectedItem;

                if (selectedIndex >= -1)
                {
                    var instance = (selectedItem as IComposition).GetInstance();
                    ProcessStartInfo psi = new ProcessStartInfo() // $"C:\Program Files\AIMP\AIMP.exe" /ADD_PLAY "D:\Music\MySong.mp3"
                    {
                        FileName = @"C:/Program Files/AIMP/AIMP.exe",
                        Arguments = $"/ADD_PLAY \"{instance.FilePath}\"",
                    };

                    psi.UseShellExecute = true;

                    Process.Start(psi);
                }
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus(ex.Message, true);
            }
        }
    }
}