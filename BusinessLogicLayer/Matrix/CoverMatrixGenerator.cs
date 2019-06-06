using System.Collections.Generic;
using System.Linq;
using Core.Common.Interfaces;
using Core.Common.Items;
using Core.Common.Items.MatrixFeatures;

namespace BusinessLogicLayer.Matrix
{
    public class CoverMatrixGenerator : ICoverMatrixGenerator
    {

        public Core.Common.Items.MatrixFeatures.Matrix CreateMatrix(DataObject[] objects,
            double LOW,
            double HIGH,
            AttributeGroupsOfObjectsCover[] listOfGroupsOfDataObjects,
            long[] LowZeroColumns,
            long[] highZeroColumns)
        {

            var rows = AddRowsToMatrix(objects, LowZeroColumns, highZeroColumns, listOfGroupsOfDataObjects);

            return new Core.Common.Items.MatrixFeatures.Matrix
                (LOW,
                HIGH,
                listOfGroupsOfDataObjects,
                rows.ToArray()
            );
        }

        private static IEnumerable<MatrixRow> AddRowsToMatrix(
            DataObject[] objects,
            long[] lowZeroColumns,
            long[] highZeroColumns,
            AttributeGroupsOfObjectsCover[] listOfGroupsOfDataObjects)
        {
            var first = listOfGroupsOfDataObjects.Select(x => x.Attribute.Attribute);

            var LOWs =
                (first
                        .Where(x => !lowZeroColumns
                            .Any(c => c == x.Id)))
                    .Select(x => x.Id);

            var HIGHs =
                (first
                       .Where(x => !highZeroColumns
                           .Any(c => c == x.Id)))
                   .Select(x => x.Id);


            var both = (from low in LOWs
                        join high in HIGHs on low equals high
                        select low);

            LOWs = LOWs.Except(both);

            HIGHs = HIGHs.Except(both);

            var rows = GetRows(objects, LOWs.ToArray(), HIGHs.ToArray(), both.ToArray(), listOfGroupsOfDataObjects);

            return rows;
        }

        private static IEnumerable<MatrixRow> GetRows(
            DataObject[] objects,
            long[] LOWs,
            long[] HIGHs,
            long[] both,
            AttributeGroupsOfObjectsCover[] listOfGroupsOfDataObjects)
        {
            foreach (var dataObject in objects)
            {
                yield return ClassifyObject(dataObject, LOWs, HIGHs, both, listOfGroupsOfDataObjects); ;
            }
        }
        private static MatrixRow ClassifyObject(
            DataObject testObject,
            long[] LOWs,
            long[] HIGHs,
            long[] both,
            AttributeGroupsOfObjectsCover[] listOfGroupsOfDataObjects)
        {
            var highs = RowColumnObjectHelper.Get(HIGHs, testObject, listOfGroupsOfDataObjects);
            var lows = RowColumnObjectHelper.Get(LOWs, testObject, listOfGroupsOfDataObjects);
            var boths = RowColumnObjectHelper.Get(both, testObject, listOfGroupsOfDataObjects);
            

            return new MatrixRow(testObject.Id, testObject.Class.Value, highs.Concat(boths), lows.Concat(boths));
        }
    }
}
