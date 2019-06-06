using Core.Common.Items;
using DataTable = System.Data.DataTable;

namespace Core.Common.Interfaces
{
    public interface IDataObjectsConverter
    {
        DataTable ConvertToDataTable(DataObject[] objects);
    }
}
