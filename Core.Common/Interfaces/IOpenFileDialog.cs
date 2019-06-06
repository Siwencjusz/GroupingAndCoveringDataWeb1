namespace Core.Common.Interfaces
{
    public interface IOpenFileDialog
    {
        string FileName { get; }
        string SafeFileName { get; }
        string Filter { get; set; }
        bool ShowDialog();
    }
}