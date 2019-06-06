using System.Collections.Generic;
using System.Linq;

namespace Core.Common.Items
{
    public struct AttributeGroupsOfObjectsCover
    {
        
        public AttributeGroupsOfObjectsCover(
            AttributeGroupsOfObjects attribute,
            IEnumerable<GroupOfDataObjectsCover> objectsGroups)
        {
            Attribute = attribute;
            ObjectsGroups = objectsGroups.ToArray();
        }

        public AttributeGroupsOfObjects Attribute { get; set; }
        public GroupOfDataObjectsCover[] ObjectsGroups { get; set; }
    }
}
