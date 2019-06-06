namespace Core.Common.Items
{
    public class FileData
    {
        public string FileName { get; set; }
        public string[] RawData { get; set; }
        public string[] Observations { get; set; }
        public string[] DataDescription { get; set; }
        public AttributeDescription[] Attributes { get; set; }
        public DataObject[] DataObjects { get; set; }
        public DataObject[] TestObjects { get; set; }
        public string JoinedString { get; set; }
    }
}
