using MediaStreamer.Logging;
using System;
using System.Collections.Generic;
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

namespace MediaStreamer.WPF.Components.UI
{
    /// <summary>
    /// Singleton pattern LoggerPage.xaml
    /// </summary>
    public partial class LoggerPage : Page
    {
        private static LoggerPage _loggerPage;
        private LoggerPage()
        {
            InitializeComponent();
            lstOutput.ItemsSource = LogData.LogList;
        }

        public static LoggerPage GetInstance() 
        {
            if(_loggerPage == null)
                return _loggerPage = new LoggerPage();
            return _loggerPage;
        }

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                var sb = new StringBuilder();
                var selectedItems = lstOutput.SelectedItems;

                foreach (var item in selectedItems)
                {
                    sb.Append(
                        "[" + LogData.LogList[lstOutput.Items.IndexOf(item)].CreationDate.ToString() + "] " +
                        LogData.LogList[lstOutput.Items.IndexOf(item)].Message.ToString()
                        + Environment.NewLine);
                }
                Clipboard.SetDataObject(sb.ToString());
            }
        }
    }
}
