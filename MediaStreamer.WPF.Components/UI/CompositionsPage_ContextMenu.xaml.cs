using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MediaStreamer.Domain;
using StringExtensions;
using LinqExtensions;
using MediaStreamer.RAMControl;
using System.Threading;
using MediaStreamer.Logging;

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

        //protected void cmiOpenInWinamp_Click(object sender, RoutedEventArgs e)
        //{
        //    cmiPlaySeveral_Click(sender, e);
        //}

        protected void cmiOpenLocation_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = CurrentListView == lstItems ? lstItems.SelectedIndex : lstQuery.SelectedIndex;
            if (selectedIndex >= 0)
            {

                var selectedItem = (CurrentListView == lstItems ? lstItems.SelectedItem : lstQuery.SelectedItem) as IComposition ; 

                if (selectedItem.FilePath.FileExists())
                {
                    selectedItem.FilePath.SelectInExplorer();
                }
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

        //protected void cmiPlaySeveral_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        Program.FileManipulator.PlaySeveralSongs(lstItems?.SelectedItems, typeof(Composition));
        //    }
        //    catch (Exception ex)
        //    {
        //        Program.SetCurrentStatus(ex.Message, true);
        //    }
        //}

        protected void cmiChangeComposition_Click(object sender, RoutedEventArgs e)
        {
            ChangeComposition(CurrentListView ==lstItems ? lstItems.SelectedItems : lstQuery.SelectedItems);
        }
        protected void cmiRename_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedIndex = CurrentListView ==lstItems ? lstItems.SelectedIndex : lstQuery.SelectedIndex ;
                var selectedItems = CurrentListView ==lstItems ? lstItems.SelectedItems : lstQuery.SelectedItems;
                if (selectedIndex >= 0 && selectedItems.Count >= 1)
                {// For every selected element
                    Session.CompositionsVM.CompositionsStore.ChangeCompositionTags(selectedItems);
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
    }
}