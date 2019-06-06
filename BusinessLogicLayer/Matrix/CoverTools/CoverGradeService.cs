using System.Linq;
using System.Threading.Tasks;
using Core.Common.Interfaces;
using Core.Common.Items.MatrixFeatures;

namespace BusinessLogicLayer.Matrix.CoverTools
{
    public class CoverGradeService : ICoverGradeService
    {
        public async Task Grade(CoverResult coverResult)
        {
           
           var task1 =
                Task.Run(() =>
                {
                    var attributeGroupsOfObjectsCovers = coverResult
                        .AttributesCovers
                        .Where(x => !coverResult.HighZeroColumns.Any(c => c.Equals(x.Attribute.Attribute.Id)))
                        .ToArray();
                    //ocena: jeżeli klasa = 0 i HIGH = 0 to dobrze
                    //ocena: jeżeli klasa = 1 i HIGH = 1 to dobrze
                    coverResult.HIGHGood =
                     attributeGroupsOfObjectsCovers
                         .Sum(x =>
                             x.ObjectsGroups.
                                 Sum(y => y.Group.AttributeValues.Count(z => z.Class.Equals(y.HIGH))));

                    //ocena: jeżeli klasa = 1 i HIGH = 0 to żle
                    //ocena: jeżeli klasa = 0 i HIGH = 1 to żle

                    coverResult.HIGHBad =
                         attributeGroupsOfObjectsCovers
                         .Length
                         *
                         coverResult
                         .DataMatrix
                         .Rows.Length
                         -
                         coverResult.HIGHGood;
                });

            var task2 =
               Task.Run(() =>
               {
                   var attributeGroupsOfObjectsCovers = coverResult
                       .AttributesCovers
                       .Where(x => !coverResult.LowZeroColumns.Any(c => c.Equals(x.Attribute.Attribute.Id)))
                       .ToArray();
                   //ocena: jeżeli klasa = 0 i LOW = 0 to żle
                   //ocena: jeżeli klasa = 1 i LOW = 1 to żle
                   coverResult.LOWBad =
                      attributeGroupsOfObjectsCovers
                          .Sum(x =>
                              x.ObjectsGroups
                                  .Sum(y => y.Group.AttributeValues.Count(z => z.Class.Equals(y.LOW))));

                   //ocena: jeżeli klasa = 0 i LOW = 1 to dobrze
                   //ocena: jeżeli klasa = 1 i LOW = 0 to dobrze
                   coverResult.LOWGood =
                          attributeGroupsOfObjectsCovers
                          .Length
                          *
                          coverResult
                          .DataMatrix
                          .Rows.Length
                          -
                          coverResult.LOWBad;
               });
            await task1;
            await task2;
        }
    }
}