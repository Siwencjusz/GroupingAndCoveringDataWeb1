using System.Collections.Generic;
using System.Linq;
using Core.Common.Items;

namespace BusinessLogicLayer.Matrix.CoverTools.GroupingManager
{
    public static class GroupByHelper
    {
        public static IEnumerable<AttributeGroupsOfObjects> Group(
            ICollection<DataObject> objects,
            IEnumerable<AttributeDescription> attributes,
            int minElementsInGroup)
        {
            attributes =
                attributes
                    .Where(x => !x.Id.Equals(objects.First().Class.Id));

            foreach (var attribute in attributes)
            {
                var groups = GroupByAttributeValue(objects, attribute.Id);
                var foundedGroups = GetAttributeGroup(groups, minElementsInGroup).ToArray();
                var groupsWithRanges = CreateGroups(foundedGroups);

                yield return new AttributeGroupsOfObjects(attribute, groupsWithRanges);
            }
        }

        private static IEnumerable<GroupOfDataObjects> CreateGroups(List<GroupMember>[] groupOfDataObjects)
        {
            var min = groupOfDataObjects.First().ToArray().Min(member => member.Value);

            foreach (var groupOfDataObject in groupOfDataObjects)
            {
                var elemGroup = new GroupOfDataObjects(groupOfDataObject.ToArray());
                FindGroupRange(elemGroup, min);
                min = elemGroup.MaxValue;
                yield return elemGroup;
            }
        }

        private static IEnumerable<List<GroupMember>> GetAttributeGroup(
            ICollection<IGrouping<double, GroupMember>> groups,
            int minimumNumberOfMembers)
        {
            var elemGroup = groups.First().ToList();
            var previous = elemGroup;

            foreach (var group in groups.Skip(1))
            {
                if (minimumNumberOfMembers >= elemGroup.Count)
                {
                    elemGroup.AddRange(group);
                    continue;
                }

                previous = elemGroup;

                yield return elemGroup;

                elemGroup = group.ToList();
            }

            if (minimumNumberOfMembers >= elemGroup.Count)
            {
                previous.AddRange(elemGroup);
            }
            else
            {
                yield return elemGroup;
            }
        }

        //private static IEnumerable<GroupOfDataObjects> GetAttributeGroup1(
        //    ICollection<IGrouping<double, GroupMember>> groups,
        //    int minimumNumberOfMembers)
        //{
        //    var elemGroup = new GroupOfDataObjects(groups.First().ToList());
        //    elemGroup.MinValue = elemGroup.AttributeValues.Min(member => member.Value);
        //    var min = elemGroup.MinValue;
        //    var previous = elemGroup;

        //    foreach (var group in groups.Skip(1))
        //    {
        //        if (minimumNumberOfMembers >= elemGroup.AttributeValues.Count)
        //        {
        //            elemGroup.AttributeValues.AddRange(group);
        //            continue;
        //        }

        //        previous = elemGroup;

        //        min = FindGroupRange(elemGroup, min);

        //        yield return elemGroup;

        //        elemGroup = new GroupOfDataObjects(group.ToList());
        //    }

        //    if (minimumNumberOfMembers >= elemGroup.AttributeValues.Count)
        //    {
        //        previous.AttributeValues.AddRange(elemGroup.AttributeValues);
        //        FindGroupRange(previous, min);
        //    }
        //    else
        //    {
        //        yield return elemGroup;
        //    }
        //}

        private static double FindGroupRange(
            GroupOfDataObjects elemGroup, 
            double min)
        {
            elemGroup.MaxValue = elemGroup.AttributeValues.Max(member => member.Value);
            elemGroup.MinValue = min;
            min = elemGroup.MaxValue;
            return min;
        }

        private static ICollection<IGrouping<double, GroupMember>> GroupByAttributeValue(
            ICollection<DataObject> objects,
            long attributeId)
        {
            return objects
                .AsParallel()
                .Select(o => CreateGroupMember(attributeId, o))
                .GroupBy(x => x.Value)
                .OrderBy(x => x.Key)
                .ToArray();
        }

        private static GroupMember CreateGroupMember(long attributeId, DataObject x)
        {
            return new GroupMember
            {
                Id = x.Id,
                AttributeId = attributeId,
                Class = x.Class.Value,
                Value =
                    x.Attributes
                        .First(y => y.Id.Equals(attributeId))
                        .Value
            };
        }
    }
}
