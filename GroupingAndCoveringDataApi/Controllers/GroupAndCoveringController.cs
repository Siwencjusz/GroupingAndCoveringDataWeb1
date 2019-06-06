using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GroupingAndCoveringDataApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace GroupingAndCoveringDataApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupingAndCoveringDataController : ControllerBase
    {
        private static Dictionary<Guid, CancellationTokenSource> _cancelTokensList = new Dictionary<Guid, CancellationTokenSource>();


        private const string NiepoprawnyIdentyfikator = "Niepoprawny identyfikator";
        private const string ZadanieZostaałoAnulowane = "Zadanie zostaało anulowane";

        private readonly Lazy<CoverManagerAsync> _coverManagerAsync;

        public GroupingAndCoveringDataController(Lazy<CoverManagerAsync> coverManagerAsync)
        {
            _coverManagerAsync = coverManagerAsync;
        }

        // GET api/values
        [HttpGet]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string[]>> GetMethods()
        {
            return Ok(await _coverManagerAsync.Value.GetGroupingMethodsAsync());
        }

        // GET api/values/5
        [HttpPost("CancelCompute")]
        [ProducesResponseType(typeof(ActionResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> CancelCompute(Guid guid)
        {
            var containsKey = _cancelTokensList.ContainsKey(guid);

            if (!containsKey)
            {
                return BadRequest(NiepoprawnyIdentyfikator);
            }

            var thread = _cancelTokensList.First(x => x.Key.Equals(guid));

            thread.Value.Cancel(true);

            return Ok(ZadanieZostaałoAnulowane);
        }

        
        // POST api/GenerateCover
        [HttpPost("GenerateCoverFiles")]
        //[ProducesResponseType(typeof(ActionResult<FileContentResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult<FileContentResult>> GenerateCover([FromForm] GroupingAndCoveringDataController.InputData inputData)
        public async Task<IActionResult> GenerateCover([FromForm] InputDataModel inputData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Keys.First());
            }

            if (_cancelTokensList.ContainsKey(inputData.CancelTokenGuid))
            {
                var oldToken = _cancelTokensList.First(x => x.Key.Equals(inputData.CancelTokenGuid));
                _cancelTokensList.Remove(oldToken.Key);
            }

            var runCancelToken = new CancellationTokenSource();
            var token = runCancelToken.Token;
            _cancelTokensList.Add(inputData.CancelTokenGuid, runCancelToken);

            var result = await _coverManagerAsync.Value.CoverTaskRunner(inputData, token);
            
            if (result.HasErrors())
            {
                return BadRequest(result.Error);
            }

            var file = result.Value.ToArray().Last();
            Response.Headers.Add("Content-Disposition", "inline; filename=" + file.FileDownloadName);
            //var fileTxt2 = result.Value.Skip(1).First().FileDownloadName;
            //Response.Headers.Add("Content-Disposition", "inline; filename=" + fileTxt2);
            //var excelName = result.Value.Last().FileDownloadName;
            //Response.Headers.Add("Content-Disposition", "inline; filename=" + excelName);
            return file;
        }

        // POST api/GenerateCover
        [HttpPost("GenerateCoverExcelReportWithTxtCovers")]
        [ProducesResponseType(typeof(ActionResult<FileContentResult[]>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FileContentResult[]>> GenerateCoverExcelReportWithTxtCovers([FromForm] InputDataModel inputData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.Keys.First());
            }

            var runCancelToken = new CancellationTokenSource();
            var token = runCancelToken.Token;
            token.ThrowIfCancellationRequested();

            _cancelTokensList.Add(inputData.CancelTokenGuid, runCancelToken);
            var result = await _coverManagerAsync.Value.CoverTaskRunner(inputData, token);

            if (result.HasErrors())
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value.ToArray());
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
