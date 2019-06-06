using BusinessLogicLayer.Reader;
using Core.Common.Interfaces;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace UnitTests
{
    [TestFixture]
    public class DataReadUnitTest
    {
        public DataReadUnitTest()
        {
            _reader = new FileChecker();
        }

        private readonly FileChecker _reader;
        [TestCase("")]
        [TestCase(null)]
        public void _OpenFile_IsValidFileName_ReturnsTrue(string path)
        {
            // Arrange
            bool excepted = false;
            // Act
            bool actual = _reader.IsFileExsist(path);
            // Assert
            Assert.AreEqual(excepted, actual);
        }
        [TestCase("notExist",".txt")]
        [TestCase("fakePath.fake",".txt")]
        public void PickFile_NotExsistOrHasWrongExtension_ReturnFalse(string path, string extension)
        {
            // Arrange
            bool excepted = false;            
            // Act
            bool actual = _reader.IsFileHasAppriopriateExtension(path);
            // Assert
            Assert.AreEqual(excepted,actual);
        }
    }
}
