using System.Collections.Generic;

namespace Core.Common.Interfaces.GroupingManager
{
    public interface IGroupingManager
    {
        IEnumerable<IGroupingMethod> GetGroupingMethods();
    }
}