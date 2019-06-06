using Core.Common.Items;

namespace Core.Common.Interfaces
{
    public interface IDataReader
    {
        Result<InputData> GetDataModel(string[] rawData);
    }
}