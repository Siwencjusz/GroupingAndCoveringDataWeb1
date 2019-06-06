using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.Interfaces;
using Core.Common.Interfaces.Excell;
using Core.Common.Interfaces.GroupingManager;
using Core.Common.Items;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GroupingAndCoveringDataApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileConverterController : ControllerBase
    {
        private const string NieOdnalezionoPodanejMetodyGrupujacej = "Nie odnaleziono podanej metody grupującej";

        private readonly IGroupingManager _groupingManager;
        private readonly ITxtExporter _txtExportert;
        private readonly IFileReaderProvider _fileReaderProvider;
        private readonly ICoverMatrixManager _coverMatrixManager;
        private readonly IExcelWriter _excelWriter;
        private Thread _thread;
        private CancellationTokenSource _cmdRunCancelationToken;

        public FileConverterController(
            IFileReaderProvider fileReaderProvider,
            IGroupingManager groupingManager,
            ITxtExporter txtExportert,
            ICoverMatrixManager coverMatrixManager,
            IExcelWriter excelWriter)
        {
            _fileReaderProvider = fileReaderProvider;
            _groupingManager = groupingManager;
            _txtExportert = txtExportert;
            _coverMatrixManager = coverMatrixManager;
            _excelWriter = excelWriter;
        }
        ~FileConverterController()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        // GET api/values
        [HttpGet]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string[]>> GetMethods()
        {
            return Ok(await Task.Run(() => _groupingManager.GetGroupingMethods().Select(x => x.MethodName)));
        }
        public class InputData
        {
            [Required]
            public FileData File { get; set; }
            [Required]
            public double Low { get; set; }
            [Required]
            public double High { get; set; }
            [Required]
            public string MethodName { get; set; }
            [Required]
            public double ParamInput { get; set; }
            public double? Step { get; set; }
        }
        public class FileInput
        {
            [Required]
            public IFormFile File { get; set; }
        }

        // POST api/values
        [HttpPost("ConvertFile")]
        [ProducesResponseType(typeof(FileData), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FileData>> ConvertFile([FromForm]FileInput file)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Keys.First());
            }

            var list = new List<string>();
            using (var stream = file.File.OpenReadStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    while (reader.Peek() >= 0)
                    {
                        list.Add(await reader.ReadLineAsync());
                    }

                }
            }

            var fileData = _fileReaderProvider.ConvertFile(file.File.FileName, list.ToArray());

            if (fileData.HasErrors())
            {
                return BadRequest(fileData.Error);
            }
            return Ok(fileData);
        }

        // POST api/values
        [HttpPost("GenerateCover")]
        [ProducesResponseType(typeof(ActionResult<FileContentResult[]>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CoverSampleResult>> GenerateCover([FromBody] InputData inputData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Keys.First());
            }

            _cmdRunCancelationToken = new CancellationTokenSource();
            var token = _cmdRunCancelationToken.Token;

            var covResult = await await
                Task.Factory.StartNew(async
                        () => await GetCoverSampleResult(inputData), token,
                    TaskCreationOptions.RunContinuationsAsynchronously, TaskScheduler.Default);

            if (covResult.HasErrors())
            {
                return BadRequest(covResult.Error);
            }
            return Ok(covResult);
        }

        // POST api/values
        [HttpPost("GenerateCoverExcel")]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CoverSampleResult>> GenerateCoverExcel([FromBody] InputData inputData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Keys.First());
            }

            _cmdRunCancelationToken = new CancellationTokenSource();
            var token = _cmdRunCancelationToken.Token;

            var covResult = await await
                Task.Factory.StartNew(async
                        () => await GetCoverSampleResult(inputData), token,
                    TaskCreationOptions.RunContinuationsAsynchronously, TaskScheduler.Default);

            if (covResult.HasErrors())
            {
                return BadRequest(covResult.Error);
            }

            var file = covResult.Value.ToArray().Last();
            Response.Headers.Add("Content-Disposition", "inline; filename=" + file.FileDownloadName);
            //var fileTxt2 = result.Value.Skip(1).First().FileDownloadName;
            //Response.Headers.Add("Content-Disposition", "inline; filename=" + fileTxt2);
            //var excelName = result.Value.Last().FileDownloadName;
            //Response.Headers.Add("Content-Disposition", "inline; filename=" + excelName);
            return file;
        }
        private async Task<Result<IEnumerable<FileContentResult>>> GetCoverSampleResult(InputData inputData)
        {
            try
            {
                var method = _groupingManager.GetGroupingMethods().FirstOrDefault(x => x.MethodName.Equals(inputData.MethodName));

                if (method is null)
                {
                    return new Result<IEnumerable<FileContentResult>>(NieOdnalezionoPodanejMetodyGrupujacej);
                }

                Thread.CurrentThread.IsBackground = true;
                _thread = Thread.CurrentThread;

                var progressBarModel = new ProgressBarModel();

                var step = inputData.Step.HasValue ? Convert.ToDouble(inputData.Step) : 0;

                var source = new CancellationTokenSource();
                var token = source.Token;

                var waitRun = Task.Run(() =>
                {
                    while (!token.IsCancellationRequested)
                    {

                    }

                    token.ThrowIfCancellationRequested();
                }, token);

                var result = await
                    _coverMatrixManager.GetMatrix(inputData.File, inputData.Low, inputData.High, method,
                        inputData.ParamInput, step, progressBarModel, waitRun);

                var dataTxt = DateTime.Now.ToFileTimeUtc() + "DataMatrixTxt.txt";
                var testTxt = DateTime.Now.ToFileTimeUtc() + "TestTxt.txt";
                var dataMatrixStream = _txtExportert.GetTxtStream(result.Value.CoverResult.DataMatrix.DataTable);
                var testMatrixStream = _txtExportert.GetTxtStream(result.Value.CoverResult.TestMatrix.DataTable);

                var excelName = $"{result.Value.FileName.Replace(" ", "").Replace(".", "").Replace('-', '_').ToUpper()}_SLOW_{result.Value.SLOW}_SHIGH_{result.Value.SHIGH}_STEP_{result.Value.STEP}_METHOD_{result.Value.SelecteMethod.Replace(' ', '_').ToUpper().Replace('Ó', 'O').Replace('Ś', 'S').Replace('Ć', 'C')}_PARAM_{result.Value.SelecteMethodParam}_GRADE_{result.Value.CoverResult.Grade}.xlsx";

                var excelStream = _excelWriter.ExportToStream(
                    result.Value,
                    result.Value.CoverResult.DataMatrix.DataTable,
                    result.Value.CoverResult.TestMatrix.DataTable);

                if (excelStream.HasErrors())
                {
                    return new Result<IEnumerable<FileContentResult>>(excelStream.Error);
                }

                progressBarModel.Progress = 100;

                var array = new[] {
                    File(testMatrixStream, "application/txt", testTxt),
                    File(dataMatrixStream, "application/txt",dataTxt),
                    File(excelStream.Value,"application/xlsx", excelName)
                };

                return new Result<IEnumerable<FileContentResult>>(array);
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
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
