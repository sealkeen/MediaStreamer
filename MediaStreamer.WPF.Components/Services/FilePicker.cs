using System.Threading.Tasks;
using MediaStreamer.IO;
using Microsoft.Win32;

namespace MediaStreamer.WPF.Components.Services
{
    public class FilePicker : IFilePicker
    {
        public async Task<string> PickFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }
    }
}
