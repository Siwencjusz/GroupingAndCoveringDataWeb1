using System.IO;
using Core.Common.Interfaces;

namespace BusinessLogicLayer.Reader
{
    public class FileReader : IFileReader
    {
        public string[] GetFileContent(string path)
        {
            return File.ReadAllLines(path);
        }
    }
}
