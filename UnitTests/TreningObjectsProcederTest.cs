using BusinessLogicLayer.TrainingObjectsProceder;
using Core.Common.Items;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class TreningObjectsProcederTest
    {
        private readonly TrainingObjectsConverter _converter;
        public TreningObjectsProcederTest()
        {
            _converter = new TrainingObjectsConverter();
        }
        [TestCase(null, null)]
        public void NullObservations_TryToProceder_ReturnEmpty(AttributeDescription[] attributeDescriptions, string[] observations)
        {
            // Arrange            
            var expected = 0;
            // Act
            var acctual = _converter.ConvertRows2DataObjects(attributeDescriptions, observations);
            // Assert
            Assert.AreEqual(acctual.Length, expected);
        }
        [TestCase(null)]
        public void NullObservations_TryToProceder_ReturnEmpty(string[] data)
        {
            // Arrange
            AttributeDescription[] description =  {new AttributeDescription() { Name = "adas", Precision = "2137", Type = "numeric" }};
            var expected = 0;
            // Act
            var acctual = _converter.ConvertRows2DataObjects(description, data);
            // Assert
            Assert.AreEqual(acctual.Length,0 );
        }
    }
}
