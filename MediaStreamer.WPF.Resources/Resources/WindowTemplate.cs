using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaStreamer.WPF.Resources
{
    public partial class WindowTemplate : ResourceDictionary
    {
        private bool flipWindow = false;
        double workHeight = SystemParameters.WorkArea.Height;
        double workWidth = SystemParameters.WorkArea.Width;

        public WindowTemplate()
        {
            InitializeComponent();
        }

        private void TopTitle_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start("http://professorweb.ru");
        }

        #region "Сворачивание, разворачивание, закрытие, перетаскивание окна"
        private void closeWindow(object sender, RoutedEventArgs e)
        {
            try
            {
                Window win = (Window)((FrameworkElement)sender).TemplatedParent;
                win.Close();
            }
            catch { }
        }

        private void minimizeWindow(object sender, RoutedEventArgs e)
        {
            try
            {
                Window win = (Window)((FrameworkElement)sender).TemplatedParent;
                win.WindowState = WindowState.Minimized;
            }
            catch { }
        }

        private void maximizedWindow(object sender, RoutedEventArgs e)
        {
            try
            {
                Window win = (Window)((FrameworkElement)sender).TemplatedParent;
                Button btn = (Button)sender;

                if (!flipWindow)
                {
                    flipWindow = true;
                    win.Height = workHeight * 0.75;
                    win.Width = workWidth * 0.75;
                    win.Top = workHeight / 8;
                    win.Left = workWidth / 8;
                    btn.Content = Application.Current.FindResource("DataButtonMaximize");
                }
                else
                {
                    flipWindow = false;
                    win.Height = workHeight;
                    win.Width = workWidth;
                    win.Top = 0;
                    win.Left = 0;
                    btn.Content = Application.Current.FindResource("DataButtonMinimize");
                }
            }
            catch { }
        }

        #endregion

        private void timeline_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                FrameworkElement element = (FrameworkElement)sender;
                double x = e.GetPosition(element).X;
                ToolTip tt = (ToolTip)element.ToolTip;
                tt.HorizontalOffset = x - tt.ActualWidth / 2;
            }
            catch { }
        }

    }
}
