using System.Collections.Generic;
using System.Linq;

namespace Core.Common.Items
{
    public class AttributeGroupsOfObjects
    {
        public AttributeGroupsOfObjects(
            AttributeDescription attribute,
            IEnumerable<GroupOfDataObjects> attributeGroup)
        {
            ObjectsGroups = attributeGroup.ToArray();
            Attribute = attribute;
        }
        public GroupOfDataObjects[] ObjectsGroups { get; set; }
        public AttributeDescription Attribute { get; set; }
        public double AttributePositiveDecisions { get; set; }
    }
}
