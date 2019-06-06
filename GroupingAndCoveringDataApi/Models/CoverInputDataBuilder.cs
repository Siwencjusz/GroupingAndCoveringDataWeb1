using System.Threading;
using Core.Common.Items;

namespace GroupingAndCoveringDataApi.Models
{
    public static class CoverInputDataBuilder
    {
        public static CoverInputData Build(

        FileData inputData,
        CancellationToken token,
        double dataHigh,
        double dataLow,
        string methodName,
        double groupParameter,
        string step)
        {
            return new CoverInputData()
            {
                InputData = inputData,
                Token = token,
                DataHigh = dataHigh,
                DataLow = dataLow,
                MethodName = methodName,
                GroupParameter = groupParameter,
                Step = step
            };
        }
    }
}
