using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Interfaces;
using Core.Common.Items;
using DataTable = System.Data.DataTable;

namespace BusinessLogicLayer.TrainingObjectsProceder
{
    public class DataObjectsConverter : IDataObjectsConverter
    {
        private static readonly Type Type = typeof(double);

        public DataTable ConvertToDataTable(
            DataObject[] objects)
        {
            var data = DefineDataTable(objects);

            data.BeginLoadData();

            foreach (var dataObject in objects)
            {
                data.LoadDataRow(LoadRow(dataObject).ToArray(), true);
            }             

            data.EndLoadData();

            return data.DefaultView.ToTable();
        }

        private static DataTable DefineDataTable(
            DataObject[] objects)
        {
            var data = new System.Data.DataTable();
            data.Columns.Add("DataObjectId", typeof(long));
            data.PrimaryKey = new[] { data.Columns["DataObjectId"] };
            data.DefaultView.Sort = "DataObjectId asc";

            var row = objects.First();

            var unionCols = row.Attributes.OrderBy(x => x.Id)
                .ThenBy(x => x.Name)
                .Select(x => x.Name);

            foreach (var key in unionCols)
            {
                data.Columns.Add(key, Type);
            }

            data.Columns.Add(nameof(row.Class), Type);
            return data;
        }

        private static IEnumerable<object> LoadRow(
            DataObject matrixRow
            )
        {
            yield return matrixRow.Id;

            var row = matrixRow.Attributes
                .OrderBy(x => x.Id)
                .AsParallel()
                .Select(x => (object)x.Value);

            foreach (var o in row)
            {
                yield return o;
            }
            yield return matrixRow.Class.Value;
        }
    }
}