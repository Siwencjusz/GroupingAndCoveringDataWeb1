using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Core.Common.Interfaces;
using Core.Common.Interfaces.GroupingManager;
using Core.Common.Items;
using Core.Common.Items.MatrixFeatures;

namespace BusinessLogicLayer.Matrix
{
    public class CoverMatrixManager : ICoverMatrixManager
    {
        private const double PositiveClassValue = 1d;
        private const double NegativeClassValue = 0d;
        private readonly ICoverMatrixGenerator _coverMatrixGenerator;
        private readonly ICoverCalculator _coverCalculator;
        private readonly ICoverGradeService _gradeService;
        private readonly ICoverMatrixClassificator _coverMatrixClassificator;
        private readonly IMatrixToGridMatrix _matrixToGridMatrix;

        public CoverMatrixManager(
            ICoverMatrixGenerator coverMatrixGenerator,
            ICoverGradeService gradeService,
            ICoverMatrixClassificator coverMatrixClassificator,
            IMatrixToGridMatrix matrixToGridMatrix, ICoverCalculator coverCalculator)
        {
            _matrixToGridMatrix = matrixToGridMatrix;
            _coverCalculator = coverCalculator;
            _coverMatrixGenerator = coverMatrixGenerator;
            _gradeService = gradeService;
            _coverMatrixClassificator = coverMatrixClassificator;
        }
        public async Task<Result<CoverSampleResult>> GetMatrix(
            FileData fileData,
            double low,
            double high,
            IGroupingMethod groupMethod,
            double paramInput,
            double step,
            ProgressBarModel progressBarModel,
            Task waitRun)
        {



            var both = GetLowsAndHighs(low, high, step).ToArray();

            var lows = both.AsParallel().Select(x => x.Item1).ToArray();
            var highs = both.AsParallel().Select(x => x.Item2).ToArray();

            var matrices = (await ComputeForEvery2(fileData, groupMethod, paramInput, highs, lows, progressBarModel, waitRun)).OrderByDescending(y => y.Grade).ToList();

            var max = matrices.First();

            var testMatrixRows = _coverMatrixClassificator.Classify(fileData.TestObjects, max.AttributesCovers, max.HighZeroColumns, max.LowZeroColumns);
            max.TestMatrix =
                new Core.Common.Items.MatrixFeatures.Matrix(testMatrixRows);

            var dataMatrix = _matrixToGridMatrix.TransformToDataTable(max.DataMatrix);
            var testMatrix = _matrixToGridMatrix.TransformToDataTable(max.TestMatrix);

            var result = new CoverSampleResult(max, matrices, fileData.FileName)
            {
                SLOW = low,
                SHIGH = high,
                SelecteMethod = groupMethod.MethodName,
                SelecteMethodParam = paramInput,
                STEP = step
            };

            max.DataMatrix.DataTable = dataMatrix;
            max.TestMatrix.DataTable = testMatrix;
            DataObject.SchemeObject = null;

            return new Result<CoverSampleResult>(result);
        }

        private static void CountPositiveDecisions(
            FileData fileData,
            AttributeGroupsOfObjects[] listOfGroupsOfDataObjects)
        {
            listOfGroupsOfDataObjects.AsParallel().ForAll(groupByAttribute =>
            {
                groupByAttribute.ObjectsGroups.AsParallel().ForAll(objectsGroup =>
                {
                    objectsGroup.Positive =
                        objectsGroup
                            .AttributeValues
                            .Count(x => x.Class.Equals(PositiveClassValue));
                });

                var attributePositiveDecisions =
                    groupByAttribute
                        .ObjectsGroups
                        .Sum(o => o.Positive) / fileData.DataObjects.Length;

                groupByAttribute
                    .AttributePositiveDecisions = attributePositiveDecisions;
            });
        }

        private static long[] GetZerosColumns(
            AttributeGroupsOfObjectsCover[] listOfGroupsOfDataObjects,
            Func<GroupOfDataObjectsCover, bool> predicate0,
            Func<GroupOfDataObjectsCover, bool> predicate1)
        {
            return listOfGroupsOfDataObjects
                .Where(x =>
                 x.ObjectsGroups.All(predicate0) || x.ObjectsGroups.All(predicate1))
                .AsParallel()
                .Select(x => x.Attribute.Attribute.Id)
                .ToArray();
        }

        private async Task<IEnumerable<CoverResult>> ComputeForEvery2(FileData fileData,
            IGroupingMethod groupMethod,
            double paramInput,
            double[] highs,
            double[] lows,
            ProgressBarModel progressBarModel,
            Task waitRun)
        {
            progressBarModel.Progress = 0;

            var all = (
                      from highl in highs
                      from lowl in lows
                      select (highl, lowl)
                    )
                .ToArray();

            progressBarModel.Progress = 1;

            var step = 97.00 / all.Length;
            var list = new ConcurrentBag<Task<CoverResult>>();
            //var listr = new ConcurrentBag<CoverResult>();
            var listOfGroupsOfDataObjects = groupMethod.GroupElementsBy(fileData.DataObjects, fileData.Attributes, paramInput).ToArray();

            CountPositiveDecisions(fileData, listOfGroupsOfDataObjects);
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();

            all
            .AsParallel()
            .ForAll(pair =>
            {
                if (waitRun.IsCompleted)
                {
                    return;
                }

                var cover =
                    Compute(
                        fileData,
                        pair.Item1,
                        pair.Item2,
                        listOfGroupsOfDataObjects,
                        progressBarModel,
                        step);

                list.Add(cover);
            });

            var result =
                await Task.WhenAll(list);

            return result;

            //    stopwatch.Stop();
            //    Debug.WriteLine(stopwatch.ElapsedTicks);

            //    progressBarModel.Progress = 1;
            //    stopwatch.Restart();
            //    foreach (var pair in all.AsParallel())
            //    {
            //        var cover =
            //            Compute(
            //                fileData,
            //                pair.Item1,
            //                pair.Item2,
            //                listOfGroupsOfDataObjects,
            //                progressBarModel,
            //                step);

            //        list.Add(cover);
            //    };
            //    await Task.WhenAll(list);
            //    stopwatch.Stop();
            //    Debug.WriteLine(stopwatch.ElapsedTicks);
            //    progressBarModel.Progress = 1;
            //    stopwatch.Restart();
            //    foreach (var pair in all.AsParallel())
            //    {
            //        var cover =
            //           Compute(
            //                fileData,
            //                pair.Item1,
            //                pair.Item2,
            //                listOfGroupsOfDataObjects,
            //                progressBarModel,
            //                step);

            //        listr.Add(await cover);
            //    }
            //    stopwatch.Stop();
            //    Debug.WriteLine(stopwatch.ElapsedTicks);

            //    progressBarModel.Progress = 1;
            //    stopwatch.Restart();
            //    all.AsParallel().ForAll(async pair =>
            //    {
            //        var cover =
            //            Compute(
            //                fileData,
            //                pair.Item1,
            //                pair.Item2,
            //                listOfGroupsOfDataObjects,
            //                progressBarModel,
            //                step);

            //        listr.Add(await cover);
            //    });

            //    stopwatch.Stop();
            //    Debug.WriteLine(stopwatch.ElapsedTicks);

            //    progressBarModel.Progress = 1;
            //    stopwatch.Restart();

            //    await Task.WhenAll(GetCoverTasks(fileData, progressBarModel, all, listOfGroupsOfDataObjects, step));

            //    stopwatch.Stop();
            //    Debug.WriteLine(stopwatch.ElapsedTicks);
            //    return listr;
        }

        //private IEnumerable<Task<CoverResult>> GetCoverTasks(FileData fileData, 
        //    ProgressBarModel progressBarModel, 
        //    (double, double)[] all,
        //    AttributeGroupsOfObjects[] listOfGroupsOfDataObjects,
        //    double step)
        //{
        //    foreach (var pair in all.AsParallel())
        //    {
        //        yield return
        //            Compute(
        //                fileData,
        //                pair.Item1,
        //                pair.Item2,
        //                listOfGroupsOfDataObjects,
        //                progressBarModel,
        //                step);
        //    }
        //}

        private static IEnumerable<(double, double)> GetLowsAndHighs(
            double low,
            double high,
            double step)
        {
            yield return (low, high);

            while (low >= 0 && low <= 1 && high >= 1 && high <= 2 && step > 0)
            {
                low += step;
                high -= step;
                if (!(low >= 0 && low <= 1 && high >= 1 && high <= 2))
                    break;
                yield return (low, high);
            }
        }

        private async Task<CoverResult> Compute(FileData fileData,
            double LOW,
            double HIGH,
            AttributeGroupsOfObjects[] listOfGroupsOfDataObjects,
            ProgressBarModel progressBarModel,
            double step)
        {


            var result = new CoverResult
            {
                TestObjects = fileData.TestObjects,
                DataObjects = fileData.DataObjects,
                Groups = listOfGroupsOfDataObjects,
                AttributesCovers = _coverCalculator.CalculateCovers(listOfGroupsOfDataObjects, LOW, HIGH).ToArray(),
            };

            result.LowZeroColumns =
                GetZerosColumns(result.AttributesCovers, z => z.LOW.Equals(NegativeClassValue),
                    z => z.LOW.Equals(PositiveClassValue));

            result.HighZeroColumns =
                GetZerosColumns(result.AttributesCovers, z => z.HIGH.Equals(NegativeClassValue),
                    z => z.HIGH.Equals(PositiveClassValue));


            result.DataMatrix = _coverMatrixGenerator.CreateMatrix(fileData.DataObjects, LOW, HIGH,
                result.AttributesCovers, result.LowZeroColumns, result.HighZeroColumns);

            await _gradeService.Grade(result);

            progressBarModel.Progress += step;

            return result;

        }
    }
}
