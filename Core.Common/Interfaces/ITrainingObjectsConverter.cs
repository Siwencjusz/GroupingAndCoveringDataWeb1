using Core.Common.Items;

namespace Core.Common.Interfaces
{
    public interface ITrainingObjectsConverter
    {
        DataObject[] ConvertRows2DataObjects(AttributeDescription[] attributes,
            string[] observations, long startId = 0);
    }
}