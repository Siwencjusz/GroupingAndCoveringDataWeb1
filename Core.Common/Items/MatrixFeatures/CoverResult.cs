namespace Core.Common.Items.MatrixFeatures
{
    public class CoverResult
    {
        public Matrix DataMatrix { get; set; }
        public Matrix TestMatrix { get; set; }
        public double Grade => HIGHGood - HIGHBad + LOWGood - LOWBad;
        public double GoodgGrade => HIGHGood + LOWGood;
        public DataObject[] TestObjects { get; set; }
        public DataObject[] DataObjects { get; set; }
        public int HIGHGood { get; set; }
        public int HIGHBad { get; set; }
        public int LOWGood { get; set; }
        public int LOWBad { get; set; }
        public long[] LowZeroColumns { get; set; }
        public long[] HighZeroColumns { get; set; }
        public AttributeGroupsOfObjects[] Groups { get; set; }
        public AttributeGroupsOfObjectsCover[] AttributesCovers { get; set; }
    }
}
