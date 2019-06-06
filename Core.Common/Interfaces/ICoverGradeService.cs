using System.Threading.Tasks;
using Core.Common.Items.MatrixFeatures;

namespace Core.Common.Interfaces
{
    public interface ICoverGradeService
    {
        Task Grade(CoverResult coverResult);
    }
}
