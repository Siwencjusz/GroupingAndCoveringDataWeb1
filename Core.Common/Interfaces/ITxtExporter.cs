using System.Data;
using System.IO;
using System.Threading.Tasks;
using Core.Common.Items;

namespace Core.Common.Interfaces
{
    public interface ITxtExporter
    {
        Task<Result<bool>> ExportToTxt(DataTable coverResultDataObjects, string suffix);

        byte[] GetTxtStream(DataTable matrix);
    }
}
