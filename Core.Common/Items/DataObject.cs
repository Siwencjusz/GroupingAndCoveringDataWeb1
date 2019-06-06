using Core.Common.Helpers;

namespace Core.Common.Items
{
    public class DataObject
    {

        public static DataObject SchemeObject
        {
            get
            {
                return DataObjectHelper.Clone();
            }
            set
            {
                DataObjectHelper.SchemeObject = value;
            }
        }

        public DataObject(
            RowColumnObject rowColumnObject, 
            RowColumnObject[] objectAttributes)
        {
            Attributes = objectAttributes;
            Id = 0;
            Class = rowColumnObject;
        }

        public long Id { get; set; }
        public RowColumnObject[] Attributes { get; set; }
        public RowColumnObject Class { get; set; }
    }
}
