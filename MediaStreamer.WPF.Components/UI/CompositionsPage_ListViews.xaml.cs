using LinqExtensions;
using MediaStreamer.Domain;
using MediaStreamer.RAMControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для Compositions.xaml
    /// </summary>
    public partial class CompositionsPage : FirstFMPage
    {
	    private LinkedList<object> _selectedItems = new LinkedList<object>();
        protected void lstItems_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (lstItems.SelectedItems.Count <= 0)
                return;

            _selectedItems.Clear();
            foreach (object i in lstItems.SelectedItems)
                _selectedItems.Enqueue(i);

            var tsk = Task.Factory.StartNew(() =>
                    lstItems_MouseLeftButtonDown(sender, e.ButtonState, e.Source, e.OriginalSource, _selectedItems)
            );
        }
        private void lstItems_MouseLeftButtonDown(object sender, 
            MouseButtonState state, 
            object source, 
            object originalSource, 
            IEnumerable SelectedItems)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(
                    new Action(delegate
                    {
                        if (null == (((FrameworkElement)originalSource).DataContext as Composition))
                            return;

                        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) ||
                        Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)
                        )
                            return;

                    //lstItems.SelectedItems.Add((((FrameworkElement)e.OriginalSource).DataContext as Composition));
                    if (state == MouseButtonState.Pressed)
                        {
                            if (source != null)
                            {
                                DataObject dataObject = new DataObject(SelectedItems);
                                DragDrop.DoDragDrop(lstItems, dataObject, DragDropEffects.Move);
                            }
                        }
                    })
                );
            } catch(Exception ex){
                Program._logger?.LogTrace("lstItems_MouseLeftButtonDown :" + ex.Message);
            }
        }
        private async void lstItems_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                //HandleFileOpen(files[0]);

                List<string> lstFiles = new List<string>(files);
                //var tsk = await 
                //Task.Factory.StartNew( () =>
                Program.FileManipulator.DecomposeAudioFiles(lstFiles, Program.SetCurrentStatus) 
                //    )
                ;
                ReList();
            }
        }
        protected void lstItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtArtistName.Text = Session.CompositionsVM.CompositionsStore.Compositions[lstItems.SelectedIndex].Artist?.ArtistName;
                txtAlbumName.Text = Session.CompositionsVM.CompositionsStore.Compositions[lstItems.SelectedIndex].Album?.AlbumName;
            }
            catch (Exception ex)
            {
                //Program.SetCurrentStatus(ex.Message);
            }
        }

        public void lstItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if ((Program.mePlayer != null) && (Program.mePlayer.Source != null)) {
            if (null == (((FrameworkElement)e.OriginalSource).DataContext as Composition))
            {
                return;
            }

            PlaySelectedTarget();
        }

        protected void lstItems_OnColumnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _orderByDescending = !_orderByDescending;
                string columnName = ((GridViewColumnHeader)e.OriginalSource).Column.Header.ToString();
                switch (columnName)
                {
                    case "Composition":
                        Session.CompositionsVM.CompositionsStore.Compositions = Program.DBAccess.DB.GetICompositions().OrderByWithDirection(x => x.CompositionName, _orderByDescending).ToList();
                        break;
                    case "Artist":
                        Session.CompositionsVM.CompositionsStore.Compositions = Program.DBAccess.DB.GetICompositions().OrderByWithDirection(x => x.Artist.ArtistName, _orderByDescending).ToList();
                        break;
                    case "Duration (sec)":
                        Session.CompositionsVM.CompositionsStore.Compositions = Program.DBAccess.DB.GetICompositions().OrderByWithDirection(x => x.Duration, _orderByDescending).ToList();
                        break;
                    case "File Path":
                        Session.CompositionsVM.CompositionsStore.Compositions = Program.DBAccess.DB.GetICompositions().OrderByWithDirection(x => x.FilePath, _orderByDescending).ToList();
                        break;
                }
                lstItems.GetBindingExpression(System.Windows.Controls.ListView.ItemsSourceProperty).UpdateTarget();
            }
            catch (Exception ex)
            {
                Program.SetCurrentStatus("CompositionsPage.ListView_OnColumnClick :" + ex.Message, true);
            }
        }

        public void lstItems_TryToSelectItem(IComposition comp)
        {
            try
            {
                if (comp == null)
                    return;
                var found = Session.CompositionsVM.CompositionsStore.Compositions.Where(c => c.CompositionID == comp.CompositionID);
                if (found.Count() > 0)
                {
                    lstItems.SelectedItem = found.First();
                }
            }
            catch (Exception ex)
            {
                Program._logger?.LogTrace("TryToSelectItem :" + ex.Message);
            }
        }

        public bool lstItems_OwnsFocus()
        {
            if (lstItems.IsKeyboardFocusWithin || lstQuery.IsKeyboardFocusWithin)
                return true;
            return false;
        }

        private void lstItems_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((sender as ListView).SelectedItems.Count >= 1)
                {
                    if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        var sb = new StringBuilder();
                        var selectedItems = lstItems.SelectedItems;

                        foreach (var item in selectedItems)
                        {
                            sb.Append((item as IComposition).FilePath);
                            sb.Append(Environment.NewLine);
                        }
                        Clipboard.SetDataObject(sb.ToString().TrimEnd());
                    }
                }
            }
            catch (Exception ex) {
                Program._logger?.LogError(ex.Message);
            }
        }
    }
}