using System;
using ClosedXML.Excel;
using Core.Common.Interfaces.Excell;
using Core.Common.Items;

namespace BusinessLogicLayer.ExcelWriter
{
    public class ClassViewer : IClassViewer
    {
        public Result<bool> DrawResultTable(XLWorkbook wb, AttributeGroupsOfObjects[] dataSet)
        {
            return new Result<bool>(true);
            try
            {
                wb.Worksheets.Add("xyz");
                IXLWorksheet workSheet = wb.Worksheets.Worksheet("xyz");
                FillSheet(workSheet, dataSet);
            }
            catch (Exception ex)
            {
                return new Result<bool>(ex);
            }

            return new Result<bool>(true);
        }

        private static void FillSheet(IXLWorksheet ws, AttributeGroupsOfObjects[] dataSet)
        {
            ws.Column(1).Width = 34;
            ws.Column(2).Width = 34;
            var lastRow = 1;
            foreach (var group in dataSet)
            {                                              
                ws.Cell(lastRow, 1).Style.Font.SetBold();
                ws.Cell(lastRow, 1).SetValue(lastRow);
                lastRow++;

            }
        }
    }
}
