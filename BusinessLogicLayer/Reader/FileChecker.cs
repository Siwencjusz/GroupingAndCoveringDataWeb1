using System.IO;
using System.Linq;
using Core.Common.Interfaces;

namespace BusinessLogicLayer.Reader
{
    public class FileChecker : IFileChecker
    {
        private const char Separator = '.';
        private const string DataFilter = "FileData Files(*.txt, *.tab) | *.txt; *.tab";

        public bool IsFileExsist(string path)
        {
            return File.Exists(path);
        }

        public bool IsFileHasAppriopriateExtension(string path)
        {
            return DataFilter.Contains(path.Split(Separator).Last());
        }
    }
}
