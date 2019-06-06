using System.IO;
using System.Threading.Tasks;
using Core.Common.Interfaces;
using System.Windows.Forms;
namespace BusinessLogicLayer.ExcelWriter
{
    public class SaveFileDialogWrapper : ISaveFileDialog
    {
        private readonly SaveFileDialog _sfd;
        public SaveFileDialogWrapper()
        {
            _sfd = new SaveFileDialog();
        }

        public string FileName
        {
            get { return _sfd.FileName; }
            set
            {
                if (value != null) _sfd.FileName = value;
            }
        }

        public string Filter
        {
            get { return _sfd.Filter; }
            set
            {
                if (value != null) _sfd.Filter = value;
            }
        }

        public string DefaultExt
        {
            get { return _sfd.DefaultExt; }
            set
            {
                if (value != null) _sfd.DefaultExt = value;
            }
        }

        public async Task<bool> ShowDialog(byte[] fileData)
        {
            if (_sfd.ShowDialog() != DialogResult.OK) return false;

            using (var sw = new FileStream(_sfd.FileName, FileMode.OpenOrCreate))
            {
                await sw.WriteAsync(fileData, 0, fileData.Length);
            }

            return true;
        }
    }
}
