using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using XLPilot.Models;
using XLPilot.UserControls;

namespace XLPilot.Tests.UserControls
{
    [TestClass]
    public class IsDefaultEmptyEntryConverterTests
    {
        private IsDefaultEmptyEntryConverter converter;

        [TestInitialize]
        public void Setup()
        {
            // Create a new IsDefaultEmptyEntryConverter for each test
            converter = new IsDefaultEmptyEntryConverter();
        }

        [TestMethod]
        public void Convert_NullValue_ReturnsFalse()
        {
            // Arrange
            object input = null;

            // Act
            object result = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsFalse((bool)result);
        }

        [TestMethod]
        public void Convert_NonXLPathsValue_ReturnsFalse()
        {
            // Arrange
            object input = new object();

            // Act
            object result = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsFalse((bool)result);
        }

        [TestMethod]
        public void Convert_EmptyXLPaths_ReturnsFalse()
        {
            // Arrange
            var input = new XLPaths();

            // Act
            object result = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsFalse((bool)result);
        }

        [TestMethod]
        public void Convert_XLPathsWithEmptyName_ReturnsFalse()
        {
            // Arrange
            var input = new XLPaths(string.Empty, "C:\\Test");

            // Act
            object result = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsTrue((bool)result);
        }

        [TestMethod]
        public void Convert_XLPathsWithEmptyPath_ReturnsFalse()
        {
            // Arrange
            var input = new XLPaths("Test", string.Empty);

            // Act
            object result = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsTrue((bool)result);
        }

        [TestMethod]
        public void Convert_XLPathsWithNameAndPath_ReturnsTrue()
        {
            // Arrange
            var input = new XLPaths("Test", "C:\\Test");

            // Act
            object result = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsTrue((bool)result);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ConvertBack_ThrowsNotImplementedException()
        {
            // Act & Assert - Should throw NotImplementedException
            converter.ConvertBack(true, typeof(XLPaths), null, CultureInfo.InvariantCulture);
        }

        [TestMethod]
        public void Convert_XLPathsWithAllPropertiesEmpty_ReturnsFalse()
        {
            // Arrange
            var input = new XLPaths(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

            // Act
            object result = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsFalse((bool)result);
        }
    }
}