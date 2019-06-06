using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.Items;
using ClosedXML.Excel;
using Core.Common.Interfaces;
using Core.Common.Interfaces.Excell;


namespace BusinessLogicLayer.ExcelWriter
{
    public class ExcelWriter : IExcelWriter
    {
        private const string MacierzPredykcjiTestowe = "Macierz Predykcji testowe";
        private const string MacierzPredykcjiTreningowe = "Macierz Predykcji treningowe";
        private const string Obiekty = "Obiekty";
        private const string NieZapisanoPliku = "Nie zapisano pliku";
        private const string Illegal = "\"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?";
        private const string NewSfdDefaultExt = ".xlsx";
        private const string XlsDocumentsXlsxXlsx = "XLS Documents (.xlsx)|*.xlsx";
        private const char Dot = '.';
        private const char NewChar = '_';
        private const char Percent = '%';
        private const char Dash = '-';
        private const char Separator = ',';
        private IPredictionMatrixWriter _predictionMatrixWriter { get; set; }
        private IAttributeWriter _attributeWriter { get; set; }
        private ISaveFileDialog _newSFD { get; set; }
        public IDataObjectsConverter _dataObjectsConverter { get; }

        public ExcelWriter(
            IPredictionMatrixWriter predictionMatrixWriter,
            IAttributeWriter attributeWriter,
            ISaveFileDialog sfd,
            IDataObjectsConverter dataObjectsConverter)
        {
            _predictionMatrixWriter = predictionMatrixWriter;
            _attributeWriter = attributeWriter;
            _newSFD = sfd;
            _dataObjectsConverter = dataObjectsConverter;
        }
        public ExcelWriter(
            IPredictionMatrixWriter predictionMatrixWriter,
            IAttributeWriter attributeWriter,
            IDataObjectsConverter dataObjectsConverter)
        {
            _predictionMatrixWriter = predictionMatrixWriter;
            _attributeWriter = attributeWriter;
            _dataObjectsConverter = dataObjectsConverter;
        }
        public async Task<Result<bool>> ExportToExcel(string fileDataFileName,
            CoverSampleResult coverSample,
            DataTable dataMatrix,
            DataTable testMatrix)
        {
            var coverMatrix = coverSample.CoverResult;

            var excelStream = ExportToStream(coverSample, dataMatrix, testMatrix);

            var suffix = $"{coverSample.FileName}_SLOW_{coverSample.SLOW}_SHIGH_{coverSample.SHIGH}_LOW_{coverMatrix.DataMatrix.LOW}_HIGH_{coverMatrix.DataMatrix.HIGH}_STEP_{coverSample.STEP}_METHOD_{coverSample.SelecteMethod.Replace(' ', NewChar).ToUpper()}_PARAM_{coverSample.SelecteMethodParam}_GRADE_{coverSample.CoverResult.Grade}";

            var sfdResult = await SaveFileDialog(fileDataFileName, suffix, excelStream.Value);

            string fileName;
            if (sfdResult.Value)
            {
                fileName = _newSFD.FileName;
            }
            else
            {
                return new Result<bool>(NieZapisanoPliku);
            }

            return SaveAndOpenFile(fileName);
        }

        public Result<byte[]> ExportToStream(CoverSampleResult coverSample,
            DataTable dataMatrix,
            DataTable testMatrix)
        {
            var coverMatrix = coverSample.CoverResult;

            using (var wb = new XLWorkbook())
            {
                var dataTable =
                    _dataObjectsConverter.ConvertToDataTable(coverMatrix.DataObjects.Concat(coverMatrix.TestObjects)
                        .ToArray());

                var result = AddDataTableToExcel(dataTable, wb, Obiekty);
                if (result.HasErrors())
                {
                    return new Result<byte[]>(result.Error);
                }

                result = _attributeWriter.AddRaport(wb, coverSample);
                if (result.HasErrors())
                {
                    return new Result<byte[]>(result.Error);
                }

                result = AddDataTableToExcel(dataMatrix, wb, MacierzPredykcjiTreningowe);
                if (result.HasErrors())
                {
                    return new Result<byte[]>(result.Error);
                }

                result = _attributeWriter.AddAttributes(wb, coverMatrix);
                if (result.HasErrors())
                {
                    return new Result<byte[]>(result.Error);
                }

                result = AddDataTableToExcel(testMatrix, wb, MacierzPredykcjiTestowe);
                if (result.HasErrors())
                {
                    return new Result<byte[]>(result.Error);
                }

                var excelStream = new MemoryStream();
                wb.SaveAs(excelStream);

                return new Result<byte[]>(excelStream.ToArray());
            }
        }

        private static Result<bool> SaveAndOpenFile(string fileName)
        {
            try
            {
                System.Diagnostics.Process.Start(fileName);
            }
            catch (Exception e)
            {
                return new Result<bool>(e);
            }
            return new Result<bool>(true);
        }

        private Result<bool> AddDataTableToExcel(DataTable dataTable, XLWorkbook wb, string title)
        {
            return _predictionMatrixWriter.AddMatrix(wb, dataTable, title);
        }

        private async Task<Result<bool>> SaveFileDialog(string fileDataFileName, string suffix, byte[] excelStream)
        {
            try
            {
                _newSFD.FileName = fileDataFileName + suffix;
                _newSFD.FileName = _newSFD.FileName.Replace(Dot, NewChar).ToUpper().Replace(Dash, NewChar).Replace(Separator, NewChar).Replace(Dot, NewChar)
                    .Replace(Percent, NewChar).Replace(Dash, NewChar);
                string invalid = _newSFD.FileName;

                string _Illegal;

                foreach (char c in invalid)
                {
                    _Illegal = Illegal.Replace(c.ToString(), string.Empty);
                }

                _newSFD.FileName = invalid;
                _newSFD.DefaultExt = NewSfdDefaultExt;
                _newSFD.Filter = XlsDocumentsXlsxXlsx;
                var sfdResult = await _newSFD.ShowDialog(excelStream);
                return new Result<bool>(sfdResult);
            }
            catch (Exception e)
            {
                return new Result<bool>(e);
            }

        }
    }
}
