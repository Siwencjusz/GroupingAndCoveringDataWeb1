using System;
using System.Linq;
using ClosedXML.Excel;
using Core.Common.Interfaces.Excell;
using Core.Common.Items;
using Core.Common.Items.MatrixFeatures;

namespace BusinessLogicLayer.ExcelWriter
{
    public class AttributeWriter : IAttributeWriter
    {
        private const string classlow = "Grupa ocena LOW";
        private const string classhigh = "Grupa ocena HIGH";
        private const string grupaNr = "Grupa nr ";
        private const string factor = "Factor";
        private const string liczbaDecyzjiPozytywnychWGrupie = "Liczba decyzji pozytywnych w grupie";
        private const string obiektyGrupy = "Ilość obiektów w grupie";
        private const string classText = "Klasa";
        private const string id = "DataObjectId";
        private const string wartosc = "Wartość";
        private const string min = "Min";
        private const string max = "Max";
        private const string name = "Raport oceny pokrycia";
        public Result<bool> AddAttributes(XLWorkbook wb, CoverResult dataSet)
        {
            try
            {
                foreach (var setOfDataObjectsGroup in dataSet.AttributesCovers.OrderBy(x=>x.Attribute.Attribute.Id))
                {
                    var name = setOfDataObjectsGroup.Attribute.Attribute.Name;
                    wb.Worksheets.Add(name);
                    IXLWorksheet workSheet = wb.Worksheets.Worksheet(name);
                    FillSheet(workSheet, setOfDataObjectsGroup.ObjectsGroups);
                }
            }
            catch (Exception ex)
            {
                return new Result<bool>(ex);
            }

            return new Result<bool>(true);
        }

        public Result<bool> AddRaport(XLWorkbook wb, CoverSampleResult coverMatrix)
        {
            wb.Worksheets.Add(name);
            IXLWorksheet ws = wb.Worksheets.Worksheet(name);
            ws.Columns().Width = 17;
            var lastcol = 1;

            ws.Cell(1, lastcol).Style.Font.SetBold();
            ws.Cell(1, lastcol).SetValue(nameof(coverMatrix.CoverResult.DataMatrix.LOW));
            lastcol++;

            ws.Cell(1, lastcol).Style.Font.SetBold();
            ws.Cell(1, lastcol).SetValue(nameof(coverMatrix.CoverResult.DataMatrix.HIGH));
            lastcol++;

            ws.Cell(1, lastcol).Style.Font.SetBold();
            ws.Cell(1, lastcol).SetValue(nameof(coverMatrix.CoverResult.HIGHGood));
            lastcol++;

            ws.Cell(1, lastcol).Style.Font.SetBold();
            ws.Cell(1, lastcol).SetValue(nameof(coverMatrix.CoverResult.HIGHBad));
            lastcol++;

            ws.Cell(1, lastcol).Style.Font.SetBold();
            ws.Cell(1, lastcol).SetValue(nameof(coverMatrix.CoverResult.LOWGood));
            lastcol++;

            ws.Cell(1, lastcol).Style.Font.SetBold();
            ws.Cell(1, lastcol).SetValue(nameof(coverMatrix.CoverResult.LOWBad));
            lastcol++;

            ws.Cell(1, lastcol).Style.Font.SetBold();
            ws.Cell(1, lastcol).SetValue(nameof(coverMatrix.CoverResult.Grade));

            lastcol++;
            ws.Cell(1, lastcol).Style.Font.SetBold();
            ws.Cell(1, lastcol).SetValue(nameof(coverMatrix.CoverResult.GoodgGrade));

            var row = 2;
            foreach (var cov in coverMatrix.Samples.OrderByDescending(x => x.Grade))
            {
                lastcol = 1;
                ws.Cell(row, lastcol).SetValue(cov.DataMatrix.LOW);
                ws.Cell(row, ++lastcol).SetValue(cov.DataMatrix.HIGH);
                ws.Cell(row, ++lastcol).SetValue(cov.HIGHGood);
                ws.Cell(row, ++lastcol).SetValue(cov.HIGHBad);
                ws.Cell(row, ++lastcol).SetValue(cov.LOWGood);
                ws.Cell(row, ++lastcol).SetValue(cov.LOWBad);
                ws.Cell(row, ++lastcol).SetValue(cov.Grade);
                ws.Cell(row, ++lastcol).SetValue(cov.GoodgGrade);
                row++;
            }
            return new Result<bool>(true);
        }
        

        private static void FillSheet(IXLWorksheet ws, GroupOfDataObjectsCover[] set)
        {
            ws.Column(1).Width = 34;
            ws.Column(2).Width = 34;
            var lastRow = 1;
            var groupIterator = 1;

            foreach (var groupNotOrd in set.OrderBy(x => x.Group.MaxValue).ToList())
            {
                var group = groupNotOrd.Group.AttributeValues.OrderBy(x =>x.Value);
                ws.Cell(lastRow, 1).Style.Font.SetBold();
                ws.Cell(lastRow, 1).SetValue(grupaNr + groupIterator);

                groupIterator++;

                lastRow++;

                ws.Cell(lastRow, 1).Style.Font.SetBold();
                ws.Cell(lastRow, 1).SetValue(classlow);
                ws.Cell(lastRow, 2).SetValue(groupNotOrd.LOW);
                lastRow++;

                ws.Cell(lastRow, 1).Style.Font.SetBold();
                ws.Cell(lastRow, 1).SetValue(classhigh);
                ws.Cell(lastRow, 2).SetValue(groupNotOrd.HIGH);
                lastRow++;

                ws.Cell(lastRow, 1).Style.Font.SetBold();
                ws.Cell(lastRow, 1).SetValue(factor);
                ws.Cell(lastRow, 2).SetValue(groupNotOrd.Factor);
                lastRow++;

                ws.Cell(lastRow, 1).Style.Font.SetBold();
                ws.Cell(lastRow, 1).SetValue(liczbaDecyzjiPozytywnychWGrupie);
                ws.Cell(lastRow, 2).SetValue(groupNotOrd.Group.Positive);
                lastRow++;

                ws.Cell(lastRow, 1).Style.Font.SetBold();
                ws.Cell(lastRow, 1).SetValue(obiektyGrupy);
                ws.Cell(lastRow, 2).SetValue(groupNotOrd.Group.AttributeValues.Length);
                lastRow += 2;

                ws.Cell(lastRow, 1).Style.Font.SetBold();
                ws.Cell(lastRow, 1).SetValue(id);
                ws.Cell(lastRow, 2).Style.Font.SetBold();
                ws.Cell(lastRow, 2).SetValue(classText);
                ws.Cell(lastRow, 2).Style.Font.SetBold();
                ws.Cell(lastRow, 3).SetValue(wartosc);
                ws.Cell(lastRow, 3).Style.Font.SetBold();


                ws.Cell(lastRow, 5).SetValue(min);
                ws.Cell(lastRow, 5).Style.Font.SetBold();
                ws.Cell(lastRow, 6).SetValue(max);
                ws.Cell(lastRow, 6).Style.Font.SetBold();
                ws.Cell(lastRow + 1, 5).SetValue(groupNotOrd.Group.MinValue);
                ws.Cell(lastRow + 1, 5).Style.Font.SetBold();
                ws.Cell(lastRow + 1, 6).SetValue(groupNotOrd.Group.MaxValue);
                ws.Cell(lastRow + 1, 6).Style.Font.SetBold();


                lastRow++;

                foreach (var dataObject in group)
                {
                    ws.Cell(lastRow, 1).SetValue(dataObject.Id);
                    var cellValue = dataObject.Class;
                    ws.Cell(lastRow, 2).SetValue<double?>(cellValue);
                    var val = dataObject.Value;
                    var nmb = Convert.ToDouble(val);
                    ws.Cell(lastRow, 3).SetValue<double>(nmb);

                    lastRow++;
                }

                lastRow += 2;
            }
        }
    }
}