using ClosedXML.Excel;
using Core.Common.Items;
using Core.Common.Items.MatrixFeatures;

namespace Core.Common.Interfaces.Excell
{
    public interface IAttributeWriter
    {
        Result<bool> AddAttributes(XLWorkbook wb, CoverResult dataSet);
        Result<bool> AddRaport(XLWorkbook wb, CoverSampleResult coverMatrix);
    }
}
