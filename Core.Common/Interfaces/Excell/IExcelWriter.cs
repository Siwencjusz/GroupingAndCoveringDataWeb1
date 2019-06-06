using System.Data;
using System.IO;
using System.Threading.Tasks;
using Core.Common.Items;

namespace Core.Common.Interfaces.Excell
{
    public interface IExcelWriter
    {
        Task<Result<bool>> ExportToExcel(string fileDataFileName, CoverSampleResult coverSample, DataTable dataMatrix,
            DataTable testMatrix);
        Result<byte[]> ExportToStream(CoverSampleResult coverSample, DataTable dataMatrix, DataTable testMatrix);
    }
}
