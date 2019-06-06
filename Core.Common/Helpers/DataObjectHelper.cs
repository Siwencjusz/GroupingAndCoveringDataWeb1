using System.Linq;
using Core.Common.Items;

namespace Core.Common.Helpers
{
    public static class DataObjectHelper
    {
        public static DataObject SchemeObject { get; set; }

        public static DataObject Clone()
        {
            return new DataObject(FillAttribute(SchemeObject.Class), CloneAttributes(SchemeObject.Attributes));
        }

        private static RowColumnObject[] CloneAttributes(RowColumnObject[] x)
        {
            return x.AsParallel().Select(FillAttribute).ToArray();
            
        }

        private static RowColumnObject FillAttribute(RowColumnObject attribute)
        {
            return new RowColumnObject
            {
                Id = attribute.Id,
                Type = attribute.Type,
                Name = attribute.Name,
                IsClass = attribute.IsClass,
                Precision = attribute.Precision,
            };
        }
    }
}