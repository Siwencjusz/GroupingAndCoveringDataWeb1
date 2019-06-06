using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Interfaces.GroupingManager;
using Core.Common.Items;

namespace BusinessLogicLayer.Matrix.CoverTools.GroupingManager
{
    public class GroupingByPercents : IGroupingMethod
    {
        private const string methodName = "Minimalna ilość procentowo";

        public string MethodName => methodName;

        public IEnumerable<AttributeGroupsOfObjects> GroupElementsBy(ICollection<DataObject> objects,
            IEnumerable<AttributeDescription> attributes, double parameter)
        {
            decimal value = (decimal) (objects.Count() * parameter / 100d);
            var numberOfElements = Convert.ToInt32(value);
            
            return GroupByHelper.Group(objects, attributes, numberOfElements);
        }
    }
}
