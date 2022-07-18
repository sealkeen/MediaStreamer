using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MediaStreamer.WPF.Resources
{
    public partial class BlankWindowTemplate : ResourceDictionary
    {
        private bool flipWindow = false;
        double workHeight = SystemParameters.WorkArea.Height;
        double workWidth = SystemParameters.WorkArea.Width;

        public BlankWindowTemplate()
        {
            InitializeComponent();
        }
    }
}
