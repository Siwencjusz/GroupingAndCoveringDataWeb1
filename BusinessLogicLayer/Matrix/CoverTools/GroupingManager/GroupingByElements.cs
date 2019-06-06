using System;
using System.Collections.Generic;
using Core.Common.Interfaces.GroupingManager;
using Core.Common.Items;

namespace BusinessLogicLayer.Matrix.CoverTools.GroupingManager
{
    public class GroupingByElements : IGroupingMethod
    {
        private const string methodName = "Minimalna ilość elementów";
        public string MethodName => methodName;

        public IEnumerable<AttributeGroupsOfObjects> GroupElementsBy(ICollection<DataObject> objects,
            IEnumerable<AttributeDescription> attributes, double parameter)
        {
            var minElementsInGroup = Convert.ToInt32(parameter);
            return GroupByHelper.Group(objects, attributes, minElementsInGroup);
        }

    }

}
