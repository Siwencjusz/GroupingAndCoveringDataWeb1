using System.Threading.Tasks;

namespace Core.Common.Interfaces
{
    public interface ISaveFileDialog
    {
        string FileName { get; set; }
        string Filter { get; set; }
        string DefaultExt { get; set; }
        Task<bool> ShowDialog(byte[] fileData);
    }
}
