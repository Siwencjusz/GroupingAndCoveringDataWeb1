using System.Data;
using ClosedXML.Excel;
using Core.Common.Interfaces.Excell;
using Core.Common.Items;

namespace BusinessLogicLayer.ExcelWriter
{
    public class PredictionMatrixWriter : IPredictionMatrixWriter
    {
        public Result<bool> AddMatrix(XLWorkbook wb, DataTable coverMatrixDataTable,string sheetName)
        {
            wb.Worksheets.Add(coverMatrixDataTable, sheetName);
            IXLWorksheet ws;
            var result = wb.TryGetWorksheet(sheetName, out ws);
            if (result == false)
            {
                return  new Result<bool>("Nie udało się edytować pliku xls");
            }
            return new Result<bool>(true);
        }
    }
}
