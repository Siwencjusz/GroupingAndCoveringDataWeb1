using System.Windows.Forms;
using Core.Common.Interfaces;

namespace BusinessLogicLayer.Reader
{
    public class OpenFileDialogWrapper : IOpenFileDialog
    {
        private const string DataFilter = "FileData Files(*.txt, *.tab) | *.txt; *.tab";
        private readonly OpenFileDialog _ofd;

        public OpenFileDialogWrapper()
        {
            _ofd = new OpenFileDialog {Filter = DataFilter};
        }

        public string FileName => _ofd.FileName;
        public string SafeFileName => _ofd.SafeFileName;
        public string Filter
        {
            get
            {
                return _ofd.Filter;
            }
            set
            {
                _ofd.Filter = value;
            }
        }
        public bool ShowDialog()
        {
            return _ofd.ShowDialog() == DialogResult.OK;
        }
    }
}
