using System.Collections.Generic;
using System.Linq;
using Core.Common.Items.MatrixFeatures;

namespace Core.Common.Items
{
    public class CoverSampleResult
    {
        public CoverSampleResult(CoverResult cover, IEnumerable<CoverResult> coverResults, string fileName)
        {
            CoverResult = cover;
            FileName = fileName;
            Samples = coverResults.ToArray();
        }

        public CoverResult CoverResult { get; set; }
        public string FileName { get; }
        public CoverResult[] Samples { get; set; }
        public double SLOW { get; set; }
        public double SHIGH { get; set; }
        public double STEP { get; set; }
        public string SelecteMethod { get; set; }
        public double SelecteMethodParam { get; set; }
    }
}
