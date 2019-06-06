namespace Core.Common.Interfaces
{
    public interface IFileChecker
    {
        bool IsFileExsist(string path);
        bool IsFileHasAppriopriateExtension(string path);
    }
}