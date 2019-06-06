using System;
using Core.Common.Items;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class ResultUnitTest
    {

        [TestCase]
        public void GetResultInfo_IsSet_ReturnTrue()
        {
            // Arrange
            var expected = true;
            var result = new Result<object>(default(bool));
            // Act
            var actual = result.HasValue();
            // Assert
            Assert.AreEqual(expected, actual);
        }


        [TestCase("")]
        [TestCase(null)]        
        public void SetError_NullOrEmpty_ReturnFalse(string error)
        {
            // Arrange
            var expected = false;
            // Act
            var actual = new Result<object>(new Exception(error));
            // Assert
            Assert.AreEqual(expected,actual.HasValue());
        }
        [TestCase("fdf")]
        public void SetError_NotNullOrEmpty_ReturnTrue(string error)
        {
            // Arrange
            var expected = true;
            // Act
            var actual = new Result<object>(new Exception(error));
            // Assert
            Assert.AreEqual(expected, actual.HasErrors());
        }
        [TestCase]
        public void CheckErrors_ListIsNotEmpty_ReturnTrue()
        {
            // Arrange
            var excepted = true;
            var error = "any";
            var result = new Result<object>(new Exception(error));
            // Act
            var actual = result.HasErrors();
            // Assert
            Assert.AreEqual(excepted, actual);
        }
    }
}
