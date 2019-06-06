using System.Collections.Generic;
using System.Linq;
using Core.Common.Interfaces;
using Core.Common.Items;
using Core.Common.Items.MatrixFeatures;

namespace BusinessLogicLayer.Matrix
{
    public enum Mode
    {
        HIGH,
        LOW
    }

    public static class RowColumnObjectHelper
    {
        public static double SelectByMode(
            this GroupOfDataObjectsCover o,
            Mode mode)
        {
            if (o is null)
            {
                return 0;
            }

            if (mode == Mode.HIGH)
            {
                return o.HIGH;
            }

            return o.LOW;
        }

        public static void SetByMode(
            this MatrixRowColumn o,
            Mode mode,
            double value)
        {
            if (mode == Mode.HIGH)
            {
                o.HIGH = value;
            }
            else
            {
                o.LOW = value;
            }
        }

        public static IEnumerable<MatrixRowColumn> Find(
        AttributeGroupsOfObjectsCover[] groupsByAttribute,
        DataObject testObject,
        Mode mode,
        AttributeGroupsOfObjectsCover[] both,
        bool setBoth = false)
        {
            foreach (var attributeGroupsOfObjects in groupsByAttribute)
            {
                yield return SetObjectFields(testObject, mode, setBoth, attributeGroupsOfObjects);
            }
        }

        private static MatrixRowColumn SetObjectFields(
        DataObject testObject, 
        Mode mode, 
        bool setBoth,
        AttributeGroupsOfObjectsCover attributeGroupsOfObjects)
        {
            var tstValue =
                testObject
                    .Attributes
                    .First(o => o.Id.Equals(attributeGroupsOfObjects.Attribute.Attribute.Id));

            double value;

            var obj = FindByRange(tstValue.Value, attributeGroupsOfObjects.ObjectsGroups);

            var row = new MatrixRowColumn { Id = testObject.Id, Name = tstValue.Name, Value = tstValue.Value };

            if (setBoth)
            {
                value = obj.SelectByMode(Mode.HIGH);
                row.SetByMode(Mode.HIGH, value);

                value = obj.SelectByMode(Mode.LOW);
                row.SetByMode(Mode.LOW, value);
            }
            else
            {
                value = obj.SelectByMode(mode);
                row.SetByMode(mode, value);
            }
            return row;
        }

        private static GroupOfDataObjectsCover FindByRange(
        double tstValue,
        GroupOfDataObjectsCover[] objectsGroups)
        {
            foreach (var cover in objectsGroups)
            {
                if (IsInRange(tstValue, cover))
                {
                    return cover;
                }
            }

            return null;
        }

        private static bool IsInRange(double tstValue, GroupOfDataObjectsCover cover)
        {
            return (tstValue - cover.Group.MinValue) * (cover.Group.MaxValue - tstValue) >= 0;
        }

        public static IEnumerable<MatrixRowColumn> Get(IEnumerable<long> ids,
        DataObject testObject,
        AttributeGroupsOfObjectsCover[] listOfGroupsOfDataObjects)
        {
            var attributes = from attribute in testObject.Attributes
                             join id in ids on attribute.Id equals id
                             join cover in listOfGroupsOfDataObjects on id equals cover.Attribute.Attribute.Id
                             select (testObject.Id, attribute, cover.ObjectsGroups);


            foreach (var attribute in attributes)
            {
                yield return GetMatrixRowColumn(attribute.Item1, attribute.Item2, attribute.Item3);
            }
        }

        private static MatrixRowColumn GetMatrixRowColumn(
        long testObjectId,
        RowColumnObject attribute,
        GroupOfDataObjectsCover[] objectsGroups)
        {
            var first = FindById(testObjectId, objectsGroups);

            return new MatrixRowColumn { Id = attribute.Id, Name = attribute.Name, Value = attribute.Value, LOW = first.LOW, HIGH = first.HIGH };
        }

        private static GroupOfDataObjectsCover FindById(
        long testObjectId,
        GroupOfDataObjectsCover[] objectsGroups)
        {
            foreach (var cover in objectsGroups)
            {
                if (cover.Group.AttributeValues.Any(xz => xz.Id.Equals(testObjectId)))
                {
                    return cover;
                }
            }
            return null;
        }
    }

    public class CoverMatrixClassificator : ICoverMatrixClassificator
    {
        public IEnumerable<MatrixRow> Classify(
        DataObject[] fileDataTestObjects,
        AttributeGroupsOfObjectsCover[] dataObjectsMatrix,
        long[] highZeroColumns,
        long[] lowZeroColumns)
        {
            var first = fileDataTestObjects.First().Attributes;

            var lowZero =
                from rowCol in first
                join zeroColumn in highZeroColumns on rowCol.Id equals zeroColumn
                select rowCol;

            var LOWs = first
                .Except(lowZero)
                .Select(o => o.Id);

            var highZero =
                from rowCol in first
                join zeroColumn in highZeroColumns on rowCol.Id equals zeroColumn
                select rowCol;

            var HIGHs =
                first
                    .Except(highZero)
                    .Select(o => o.Id);

            var lowsAtrs = (from loW in LOWs
                            join groupOfObj in dataObjectsMatrix on loW equals groupOfObj.Attribute.Attribute.Id
                            select groupOfObj)
                .ToArray();

            var highsAtrs = (from HIGH in HIGHs
                             join groupOfObj in dataObjectsMatrix on HIGH equals groupOfObj.Attribute.Attribute.Id
                             select groupOfObj)
                .ToArray();

            var both = (from higH in HIGHs
                        join loW in LOWs on higH equals loW
                        join groupOfObj in dataObjectsMatrix on higH equals groupOfObj.Attribute.Attribute.Id
                        select groupOfObj)
                .ToArray();

            lowsAtrs = lowsAtrs.Except(both).ToArray();


            foreach (var obj in fileDataTestObjects)
            {
                yield return ClassifyObject(obj, lowsAtrs, highsAtrs, both);
            }
        }

        private static MatrixRow ClassifyObject(
        DataObject testObject,
        AttributeGroupsOfObjectsCover[] Lows,
        AttributeGroupsOfObjectsCover[] Highs,
        AttributeGroupsOfObjectsCover[] both)
        {
            var higHs = RowColumnObjectHelper.Find(Highs, testObject, Mode.HIGH, both, true).ToArray();
            var bothRows = from attribute in higHs
                           join id in both on attribute.Id equals id.Attribute.Attribute.Id
                           select attribute;
            var lows = bothRows.Concat(RowColumnObjectHelper.Find(Lows, testObject, Mode.LOW, both)).ToArray();
            
            return new MatrixRow(testObject.Id, testObject.Class.Value, higHs,lows);
        }
    }
}
