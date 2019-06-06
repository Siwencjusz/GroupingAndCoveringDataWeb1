using System.Collections.Generic;
using Core.Common.Interfaces;
using Core.Common.Items;

namespace BusinessLogicLayer.Matrix.CoverTools
{
    public class CoverCalculator : ICoverCalculator
    {
        private const double Positive = 1d;
        private const double Negative = 0d;

        public IEnumerable<AttributeGroupsOfObjectsCover> CalculateCovers(
            AttributeGroupsOfObjects[] listOfGroupsOfDataObjects,
            double lowValue,
            double highValue)
        {
            foreach (var @group in listOfGroupsOfDataObjects)
            {
                var objectsGroups = CalculateCoversByAttribute(@group.ObjectsGroups, lowValue, highValue,group.AttributePositiveDecisions);
                yield return new AttributeGroupsOfObjectsCover(@group, objectsGroups);
            }
        }

        private static IEnumerable<GroupOfDataObjectsCover> CalculateCoversByAttribute(
            GroupOfDataObjects[] groupByAttribute,
            double low,
            double high, 
            double groupAttributePositiveDecisions)
        {
            foreach (var @group in groupByAttribute)
            {
                var factor = GetFactor(@group.AttributeValues.Length, @group.Positive, groupAttributePositiveDecisions);

                yield return new GroupOfDataObjectsCover(@group)
                {
                    Factor = factor,
                    LOW = factor < low ? Positive : Negative,
                    HIGH = factor > high ? Positive : Negative
                };
            }
        }

        private static double GetFactor(
            int groupMembers,
            double numberOfPositive,
            double numberOfPositiveInAttribute)
        {
            return numberOfPositive / groupMembers /
                    numberOfPositiveInAttribute;
        }
    }
}
