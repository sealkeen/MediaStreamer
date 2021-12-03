using System;
using System.Threading.Tasks;
using MediaStreamer.WPF.Components;

namespace MediaStreamer.WPF.EventAction
{
    public class MainWindowEventActions
    {
        public MainWindowEventActions()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.ShowDialog();

            //Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            //ofd.ShowDialog();

            //var filedata = Plugin.FilePicker.CrossFilePicker.Current.PickFile();
            //filedata.Wait();
            //System.Diagnostics.Debug.WriteLine(filedata.Result.FilePath);
        }

        [STAThread]
        public async Task ShowDialog()
        {
            //Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            //ofd.ShowDialog();
        }
    }
}
