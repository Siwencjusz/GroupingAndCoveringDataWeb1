using System.Data;
using ClosedXML.Excel;
using Core.Common.Items;

namespace Core.Common.Interfaces.Excell
{
    public interface IPredictionMatrixWriter
    {
        Result<bool> AddMatrix(XLWorkbook wb, DataTable coverMatrixDataTable, string sheetName);
    }
}
