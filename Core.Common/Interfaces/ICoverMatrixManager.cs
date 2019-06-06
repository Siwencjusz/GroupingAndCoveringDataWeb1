using System.Threading;
using System.Threading.Tasks;
using Core.Common.Interfaces.GroupingManager;
using Core.Common.Items;

namespace Core.Common.Interfaces
{
    public interface ICoverMatrixManager
    {
        Task<Result<CoverSampleResult>> GetMatrix(
            FileData fileData, 
            double low, 
            double high,
            IGroupingMethod groupMethod,
            double paramInput, 
            double step, 
            ProgressBarModel progressBarModel, 
            Task waitRun);
    }
}