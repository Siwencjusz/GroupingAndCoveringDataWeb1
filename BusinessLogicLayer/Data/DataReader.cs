using System;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Common.Interfaces;
using Core.Common.Items;

namespace BusinessLogicLayer.Data
{
    public  class DataReader : IDataReader
    {
        private const string NumericRegexBegin = "^([ ]*[0-9]+(\\";
        private const string NumericRegexEnd = ")?([0-9]+)?)*[ ]*$";
        private static readonly string[] SeparatorsList = {",", "."};

        public Result<InputData> GetDataModel(string[] rawData)
        {
            try
            {
                var data = rawData.Where(IsDataRowRegex).ToArray();
                var dataDescription = rawData.Except(data).ToArray();

                return new Result<InputData>(new InputData(dataDescription, data));
            }
            catch (Exception e)
            {
                return new Result<InputData>(e);
            }
            
        }
        private static bool IsDataRowRegex(string row)
        {
            return SeparatorsList.Any(separator => Regex.Match(row, $"{NumericRegexBegin}{separator}{NumericRegexEnd}").Success);
        }

    }
}
