using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Helpers;
using Core.Common.Interfaces;
using Core.Common.Items;


namespace BusinessLogicLayer.AttributeData
{
    enum DataType
    {
        Symbolic,
        Numeric
    }

    enum AttributeColumn
    {
        Name,
        Type,
        Precision
    }
    public class AttributeColumnConverter : IAttributeColumnConverter
    {
        public IEnumerable<AttributeDescription> ConvertColumns2Attributes(string[] dataDescription)
        {
                var counter = 0;
                foreach (var row in dataDescription)
                {
                    var isatributeRow = IsAttributeRow(row);
                    if (isatributeRow)
                    {
                        var attribute = ConvertRow2Attribute(row, counter);
                        counter++;
                        yield return attribute;
                    }
                    
                }
        }

        private static bool IsAttributeRow(string row)
        {
            var enumValues = Enum.GetNames(typeof(DataType));

            bool isAttributeRow = enumValues.Any(x => row.ToUpper().Contains(x.ToUpper()));

            return isAttributeRow;
        }

        private static AttributeDescription ConvertRow2Attribute(string row, int rowId)
        {
            var rowAttribute = row.SplitString(" ");
            return new AttributeDescription
            {
                Id = rowId,
                Type = GetAttributeValue(rowAttribute, nameof(AttributeDescription.Type)),
                Name = GetAttributeValue(rowAttribute, nameof(AttributeDescription.Name)),
                Precision = GetAttributeValue(rowAttribute, nameof(AttributeDescription.Precision))
            };

        }

        private static string GetAttributeValue(string[] stringArray, string attributeName)
        {
            AttributeColumn column;
            var result = Enum.TryParse(attributeName,true,out column);
            if (!result) return default(string);
            if ((int)column >= stringArray.Length) return default(string);
            return stringArray[(int)column];
        }
    }
}
