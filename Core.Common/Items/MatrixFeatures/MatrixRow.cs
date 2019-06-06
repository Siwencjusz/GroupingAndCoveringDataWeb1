using System.Collections.Generic;
using System.Linq;

namespace Core.Common.Items.MatrixFeatures
{
    public class MatrixRow
    {
        public MatrixRow(
            long dataObjectDataObjectId,
            double classValue,
            IEnumerable<MatrixRowColumn> higHs,
            IEnumerable<MatrixRowColumn> loWs)
        {
            DataObjectId = dataObjectDataObjectId;
            Class = classValue;
            LOWs = loWs.ToArray();
            HIGHs = higHs.ToArray();
        }

        public long DataObjectId { get; set; }
        public MatrixRowColumn[] LOWs { get; set; }
        public MatrixRowColumn[] HIGHs { get; set; }
        public double Class { get; set; }
    }
}
