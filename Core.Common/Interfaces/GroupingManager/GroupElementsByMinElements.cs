using System.Collections.Generic;
using Core.Common.Items;

namespace Core.Common.Interfaces.GroupingManager
{
    public interface IGroupingMethod
    {
        string MethodName { get; }
        IEnumerable<AttributeGroupsOfObjects> GroupElementsBy(ICollection<DataObject> objects,
            IEnumerable<AttributeDescription> attributes, double parameter);
    }
}
