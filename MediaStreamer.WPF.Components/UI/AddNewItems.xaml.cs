using MediaStreamer.RAMControl;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace MediaStreamer.WPF.Components
{
    /// <summary>
    /// Логика взаимодействия для AddNewItems.xaml
    /// </summary>
    public partial class AddNewItems : Page
    {
        static AddNewItems _page;

        public static AddNewItems GetPage()
        {
            if (_page == null)
                _page = new AddNewItems();
            return _page;
        }

        private AddNewItems()
        {
            InitializeComponent();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            var text = txtSourceLines.Text;
            int count = 0;
            foreach (var line in text.Split(Environment.NewLine.ToCharArray()))
            {
                if (File.Exists(line.Trim()))
                {
                    Program.FileManipulator.DecomposeAudioFile(line.Trim());
                    count++;
                }
            }
            Selector.CompositionsPage?.List();
            MessageBox.Show($"Files applied: {count}");
        }
    }
}
