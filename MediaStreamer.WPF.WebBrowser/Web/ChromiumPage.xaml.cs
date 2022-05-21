using Microsoft.Toolkit.Wpf.UI.Controls;
using System;
using System.Collections.Generic;
using Microsoft.Web.WebView2.Core;
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
using MediaStreamer.RAMControl;

namespace MediaStreamer.WPF.Components.Web
{
    /// <summary>
    /// Interaction logic for ChromiumPage.xaml
    /// </summary>
    public partial class ChromiumPage : FirstFMPage
    {
        public ChromiumPage()
        {
            InitializeComponent();
            //webView.EnsureCoreWebView2Async().Wait();
            //webView.BringIntoView();
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (webView != null //&& webView.CoreWebView2 != null
                    )
                {
                    webView.//CoreWebView2.
                        //Navigate(
                        Source = new Uri(txtAddress.Text)
                        //)
                        ;
                    webView.//CoreWebView2.
                        Navigate(
                        new Uri(txtAddress.Text)
                        )
                        ;
                }
            } catch (Exception ex) {
                Program.SetCurrentStatus($"[{DateTime.Now}] " + ex.Message);
            }
        }

        public override void ClosePageResources()
        {
            webView?.Dispose();
        }
    }
}
