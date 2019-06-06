namespace Core.Common.Items
{
    public class GroupOfDataObjectsCover
    {
        public GroupOfDataObjectsCover(
            GroupOfDataObjects group)
        {
            Group = group;
        }
        public GroupOfDataObjects Group { get; }
        public double LOW { get; set; }
        public double HIGH { get; set; }
        public double Factor { get; set; }
    }
}
