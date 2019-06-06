using ClosedXML.Excel;
using Core.Common.Items;

namespace Core.Common.Interfaces.Excell
{
    public interface IClassViewer
    {
        Result<bool> DrawResultTable(XLWorkbook wb, AttributeGroupsOfObjects[] dataSet);
    }
}
