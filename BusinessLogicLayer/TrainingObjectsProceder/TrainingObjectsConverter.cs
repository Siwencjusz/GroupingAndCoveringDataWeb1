using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Helpers;
using Core.Common.Interfaces;
using Core.Common.Items;

namespace BusinessLogicLayer.TrainingObjectsProceder
{
    public class TrainingObjectsConverter : ITrainingObjectsConverter
    {
        private const string Separator = " ";

        public DataObject[] ConvertRows2DataObjects(
            AttributeDescription[] attributes,
            string[] observations,
            long startId = 0L)
        {

                if (DataObjectHelper.SchemeObject is null)
                {
                    DataObject.SchemeObject = PrepareSchemeDataObject(attributes);
                }

                var result = observations
                    .AsParallel()
                    .Select((s, i) => new {s, i})
                    .ToDictionary(x => Convert.ToInt64(x.i), x => x.s);

               return GetObjects(startId, result);
        }

        private static DataObject[] GetObjects(
            long startId, 
            Dictionary<long, string> result)
        {
            return result
                .AsParallel()
                .Select(observation => GetNewObject(startId, observation))
                .ToArray();
        }

        private static DataObject GetNewObject(
            long startId, 
            KeyValuePair<long, string> observation)
        {
            var newObj = DataObject.SchemeObject;
            newObj.Id = startId + observation.Key;

            var splicedObservation = observation.Value.SplitString(Separator);

            foreach (var attribute in newObj.Attributes.AsParallel())
            {
                var value = splicedObservation[attribute.Id];
                attribute.Value = double.Parse(value);
            }
            
            var classValue = splicedObservation.Last();
            newObj.Class.Value = double.Parse(classValue);

            return newObj;
        }

        private static DataObject PrepareSchemeDataObject(
            AttributeDescription[] attributes)
        {
            var classAttribute = attributes.Last();

            attributes = attributes.Take(attributes.Length -1).ToArray();

            var objectAttributes = attributes
                .AsParallel()
                .Select(description =>
                    CreateAttribute(description))
                .ToArray();

            return new DataObject(CreateAttribute(classAttribute, true), objectAttributes);
        }
        
        private static RowColumnObject CreateAttribute(
            AttributeDescription attribute, 
            bool isClass = false)
        {
            return new RowColumnObject
            {
                Precision = Convert.ToInt32(attribute.Precision),
                Name = attribute.Name,
                Type = attribute.Type,
                Id = attribute.Id,
                IsClass = isClass
            };
        }

        private static bool GetValue(
            string value,
            RowColumnObject attribute)
        {
            bool convertresult = double.TryParse(value, out var converted);
            if (!convertresult) return true;
            attribute.Value = converted;
            return false;
        }
    }

}
