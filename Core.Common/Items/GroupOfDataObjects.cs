namespace Core.Common.Items
{
    public class GroupOfDataObjects
    {
        public GroupOfDataObjects(GroupMember[] members)
        {
            AttributeValues = members;
        }
        public double Positive { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public GroupMember[] AttributeValues { get; set; }
        
    }
}