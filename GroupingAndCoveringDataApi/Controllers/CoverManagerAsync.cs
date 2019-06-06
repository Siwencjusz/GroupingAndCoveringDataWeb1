using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.Interfaces;
using Core.Common.Interfaces.Excell;
using Core.Common.Interfaces.GroupingManager;
using Core.Common.Items;
using GroupingAndCoveringDataApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GroupingAndCoveringDataApi.Controllers
{
    public class CoverManagerAsync
    {
        private const string NieOdnalezionoPodanejMetodyGrupujacej = "Nie odnaleziono podanej metody grupującej";

        private readonly Lazy<IGroupingManager> _groupingManager;
        private readonly Lazy<ITxtExporter> _txtExporter;
        private readonly Lazy<IFileReaderProvider> _fileReaderProvider;
        private readonly Lazy<ICoverMatrixManager> _coverMatrixManager;
        private readonly Lazy<IExcelWriter> _excelWriter;

        public CoverManagerAsync(Lazy<IFileReaderProvider> fileReaderProvider,
            Lazy<IGroupingManager> groupingManager,
            Lazy<ITxtExporter> txtExporter,
            Lazy<ICoverMatrixManager> coverMatrixManager,
            Lazy<IExcelWriter> excelWriter)
        {
            _fileReaderProvider = fileReaderProvider;
            _groupingManager = groupingManager;
            _txtExporter = txtExporter;
            _coverMatrixManager = coverMatrixManager;
            _excelWriter = excelWriter;
        }

        public async Task<IEnumerable<string>> GetGroupingMethodsAsync()
        {
            return await Task.Run(() => _groupingManager.Value.GetGroupingMethods().Select(x => x.MethodName));
        }


        public async Task<Result<IEnumerable<FileContentResult>>> CoverTaskRunner(InputDataModel inputData, CancellationToken token)
        {
            var fileData = await GetFileData(inputData);

            if (fileData.HasErrors())
            {
                return new Result<IEnumerable<FileContentResult>>(fileData.Error);
            }

            var data = CoverInputDataBuilder.Build(fileData.Value, token, inputData.High, inputData.Low,
                inputData.MethodName, inputData.ParamInput, inputData.Step);

            var result = await GetCoverResult(data);

            return result;
        }


        private async Task<Result<IEnumerable<FileContentResult>>> GetCoverResult(CoverInputData data)
        {
            return await await
                Task.Factory.StartNew(async () =>
                    {
                        try
                        {
                            return await GetCoverSampleResult(data);
                        }
                        catch (ThreadAbortException e)
                        {
                            return new Result<IEnumerable<FileContentResult>>(e.Message);
                        }
                        catch (OperationCanceledException e)
                        {
                            return new Result<IEnumerable<FileContentResult>>(e.Message);
                        }
                        catch (Exception e)
                        {
                            return new Result<IEnumerable<FileContentResult>>(e.Message);
                        }
                        finally
                        {
                            await Task.Run(() =>
                            {
                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                                GC.WaitForFullGCApproach();
                                GC.WaitForFullGCComplete();

                            });
                        }
                    }, data.Token,
                    TaskCreationOptions.RunContinuationsAsynchronously, TaskScheduler.Default);
        }

        public async Task<Result<FileData>> GetFileData(InputDataModel inputData)
        {
            var list = new List<string>();
            using (var stream = inputData.File.OpenReadStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    while (reader.Peek() >= 0)
                    {
                        list.Add(await reader.ReadLineAsync());
                    }
                }
            }

            var fileData = _fileReaderProvider.Value.ConvertFile(inputData.File.FileName, list.ToArray());
            return fileData;
        }

        public async Task<Result<IEnumerable<FileContentResult>>> GetCoverSampleResult(CoverInputData data)
        {
            var method = _groupingManager.Value.GetGroupingMethods().FirstOrDefault(x => x.MethodName.Equals(data.MethodName));

            if (method is null)
            {
                return new Result<IEnumerable<FileContentResult>>(NieOdnalezionoPodanejMetodyGrupujacej);
            }

            var progressBarModel = new ProgressBarModel();

            var step = string.IsNullOrEmpty(data.Step) ? 0 : Convert.ToDouble(data.Step);

            var waitRun = Task.Run(() =>
            {
                while (!data.Token.IsCancellationRequested)
                {
                }

                data.Token.ThrowIfCancellationRequested();
            }, data.Token);

            var result = await
                    _coverMatrixManager.Value.GetMatrix(data.InputData, data.DataLow, data.DataHigh, method,
                        data.GroupParameter, step, progressBarModel, waitRun);

            if (data.Token.IsCancellationRequested)
            {
                data.Token.ThrowIfCancellationRequested();

            }
            var dataTxt = $"{DateTime.Now.ToFileTimeUtc()}DataMatrixTxt.txt";
            var testTxt = $"{DateTime.Now.ToFileTimeUtc()}TestTxt.txt";
            var dataMatrixStream = _txtExporter.Value.GetTxtStream(result.Value.CoverResult.DataMatrix.DataTable);
            var testMatrixStream = _txtExporter.Value.GetTxtStream(result.Value.CoverResult.TestMatrix.DataTable);

            var excelName = $"{result.Value.FileName.Replace(" ", "").Replace(".", "").Replace('-', '_').ToUpper()}_SLOW_{result.Value.SLOW}_SHIGH_{result.Value.SHIGH}_STEP_{result.Value.STEP}_METHOD_{result.Value.SelecteMethod.Replace(' ', '_').ToUpper().Replace('Ó', 'O').Replace('Ś', 'S').Replace('Ć', 'C')}_PARAM_{result.Value.SelecteMethodParam}_GRADE_{result.Value.CoverResult.Grade}.xlsx";

            var excelStream = _excelWriter.Value.ExportToStream(
                result.Value,
                result.Value.CoverResult.DataMatrix.DataTable,
                result.Value.CoverResult.TestMatrix.DataTable);

            if (excelStream.HasErrors())
            {
                return new Result<IEnumerable<FileContentResult>>(excelStream.Error);
            }

            progressBarModel.Progress = 100;

            var array = new[] {
                new FileContentResult(testMatrixStream, "application/txt"){FileDownloadName =  testTxt},
                new FileContentResult(dataMatrixStream, "application/txt"){FileDownloadName =  dataTxt},
                new FileContentResult(excelStream.Value,"application/xlsx"){FileDownloadName =  excelName}
                };

            return new Result<IEnumerable<FileContentResult>>(array);
        }
    }
}