using System.Collections.Generic;
using Core.Common.Items;
namespace Core.Common.Interfaces
{
    public interface ICoverCalculator
    {
        IEnumerable<AttributeGroupsOfObjectsCover> CalculateCovers(AttributeGroupsOfObjects[] listOfGroupsOfDataObjects,
            double LOWValue,
            double HIGHValue);
    }
}
