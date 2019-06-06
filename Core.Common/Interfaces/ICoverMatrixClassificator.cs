using System.Collections.Generic;
using Core.Common.Items;
using Core.Common.Items.MatrixFeatures;

namespace Core.Common.Interfaces
{
    public interface ICoverMatrixClassificator
    {
        IEnumerable<MatrixRow> Classify(DataObject[] fileDataTestObjects,
            AttributeGroupsOfObjectsCover[] dataObjectsMatrix,
            long[] highZeroColumns,
            long[] lowZeroColumns);
    }
}
