using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogicLayer.AttributeData;
using Core.Common.Interfaces;
using Core.Common.Items;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class AttributeColumnprocessingUnitTest
    {
        private static readonly IAttributeColumnConverter AttributeColumnConverter = new AttributeColumnConverter();

        [TestCase(null)]
        public void GetEmpty_InputNull_ReturnEmpty(string[] data)
        {
            // Arrange
            var expected = new List<AttributeDescription>();
            // Act
            var actual = AttributeColumnConverter.ConvertColumns2Attributes(data);
            // Assert
            Assert.AreEqual(expected, actual);
        }
        
        [TestCase]
        public void InsertList_CheckIfDataIsIncorrect_ReturnEmptyList()
        {
            // Arrange
            string[] input = new string[] {"fdfdsfd", "nsfdsf"};
            var expected = new List<AttributeDescription>() {};
            // Act
            var actual = AttributeColumnConverter.ConvertColumns2Attributes(input);
            // Assert
            Assert.AreEqual(expected.Count, actual.Count());
        }
        [TestCase]
        public void ListProvided_DataIsCorrect_ReturnList()
        {
            // Arrange
            string[] input = new string[] {"fdfdsfd symbolic", "nsfdsf numeric"};
            var expected = new List<AttributeDescription>() { 
                new AttributeDescription { Name = "fdfdsfd symbolic",Precision = null, Type = null},
                new AttributeDescription { Name = "nsfdsf numeric",Precision = null, Type = null},
            };
            // Act
            var actual = AttributeColumnConverter.ConvertColumns2Attributes(input);
            // Assert
            Assert.AreEqual(expected.Count, actual.Count());
        }
            
}
}
