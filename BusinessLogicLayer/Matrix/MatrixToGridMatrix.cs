using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Core.Common.Interfaces;
using Core.Common.Items.MatrixFeatures;

namespace BusinessLogicLayer.Matrix
{
    public class MatrixToGridMatrix : IMatrixToGridMatrix
    {
        private struct Column
        {
            public long AttributeIdId { get; set; }
            public string Name { get; set; }
            public Type Type { get; set; }
            public int Order { get; set; }
        }
        private struct DtColumn
        {
            public DtColumn(long colId, object colValue, int ord)
            {
                Id = colId;
                Value = colValue;
                OrdNext = ord;
            }
            public long Id { get; }
            public object Value { get; }
            public int OrdNext { get; }
        }
        public DataTable TransformToDataTable(Core.Common.Items.MatrixFeatures.Matrix gridMatrix)
        {
            var data = new DataTable();
            data.Columns.Add("DataObjectId", typeof(long));
            data.PrimaryKey = new DataColumn[] { data.Columns["DataObjectId"] };
            data.DefaultView.Sort = "DataObjectId asc";

            var row = gridMatrix.Rows.First();

            var unionCols = GetUnionColumns(row);
            
            foreach (var key in unionCols)
            {
                data.Columns.Add(key.Name, key.Type);
            }

            data.Columns.Add(nameof(row.Class), typeof(int));

            GetRows(gridMatrix, data);

            return data.DefaultView.ToTable();

        }
        private static void GetRows(Core.Common.Items.MatrixFeatures.Matrix gridMatrix, DataTable data)
        {
            data.BeginLoadData();
            foreach (var row in gridMatrix.Rows.AsParallel())
            {
                var obj = GetRow(row).ToArray();
                data.LoadDataRow(obj, true);
            }
            data.EndLoadData();
        }

        private static IEnumerable<object> GetRow(MatrixRow row)
        {
            yield return row.DataObjectId;

            var columnsValues = row.HIGHs.AsParallel().Select(o => new DtColumn(o.Id, o.HIGH, 2));

            columnsValues = columnsValues.Concat(row.LOWs.AsParallel().Select(o => new DtColumn(o.Id, o.LOW, 1)));


            var columns = columnsValues
                .OrderBy(x => x.Id)
                .ThenBy(x => x.OrdNext)
                .ToArray();
            
            foreach (var column in columns)
            {
                yield return column.Value;
            }

            yield return row.Class;
        }

        private static Column[] GetUnionColumns(MatrixRow row)
        {
            var type = typeof(int);

            var unionColsLows = (from col in row.LOWs.AsParallel()
                                 select new Column { AttributeIdId = col.Id, Name = $"{col.Name} LOW", Type = type, Order = 1 });

            var unionColsHighs = from col in row.HIGHs.AsParallel()
                                 select new Column { AttributeIdId = col.Id, Name = $"{col.Name} HIGH", Type = type, Order = 2 };

            var unionCols = unionColsLows
                .Concat(unionColsHighs)
                .OrderBy(x => x.AttributeIdId)
                .ThenBy(x => x.Order)
                .ToArray();

            return unionCols;
        }
    }
}
