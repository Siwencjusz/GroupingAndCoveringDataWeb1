using System.Data;
using Core.Common.Items.MatrixFeatures;

namespace Core.Common.Interfaces
{
    public interface IMatrixToGridMatrix
    {
        DataTable TransformToDataTable(Matrix gridMatrix);
    }
}