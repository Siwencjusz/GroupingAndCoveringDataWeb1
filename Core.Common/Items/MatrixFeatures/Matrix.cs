using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Core.Common.Items.MatrixFeatures
{
    public class Matrix
    {
        public Matrix(
            double LOW,
            double HIGH, 
            AttributeGroupsOfObjectsCover[] listOfAttributeGroupsOfObjects, 
            MatrixRow[] rows)
        {
            Rows = rows;
            ListOfAttributeGroupsOfObjects = listOfAttributeGroupsOfObjects;
            this.LOW = LOW;
            this.HIGH = HIGH;
        }

        public Matrix(IEnumerable<MatrixRow> rows)
        {
            Rows = rows.ToArray();
        }

        public double LOW { get; set; }
        public double HIGH { get; set; }
        public MatrixRow[] Rows { get; set; }
        public DataTable DataTable { get; set; }
        public AttributeGroupsOfObjectsCover[] ListOfAttributeGroupsOfObjects {get;set;}
    }
}
