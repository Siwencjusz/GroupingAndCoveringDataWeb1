using System.Threading;
using Core.Common.Items;

namespace GroupingAndCoveringDataApi.Models
{

    public class CoverInputData
    {
        public FileData InputData { get; set; }
        public CancellationToken Token { get; set; }
        public double DataHigh { get; set; }
        public double DataLow { get; set; }
        public string MethodName { get; set; }
        public double GroupParameter { get; set; }
        public string Step { get; set; }
    }
}
