using Core.Common.Items;
using Core.Common.Items.MatrixFeatures;

namespace Core.Common.Interfaces
{
    public interface ICoverMatrixGenerator
    {
        Matrix CreateMatrix(DataObject[] objects,
            double LOW,
            double HIGH,
            AttributeGroupsOfObjectsCover[] listOfGroupsOfDataObjects,
            long[] LowZeroColumns,
            long[] highZeroColumns);
    }
}