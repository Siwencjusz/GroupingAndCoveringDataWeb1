using Core.Common.Items;

namespace Core.Common.Interfaces
{
    public interface IFileReaderProvider
    {
        Result<FileData> ConvertFile(string filename, string[] data);
    }
}
