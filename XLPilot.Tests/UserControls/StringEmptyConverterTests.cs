using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using XLPilot.UserControls;

namespace XLPilot.Tests.UserControls
{
    [TestClass]
    public class StringEmptyConverterTests
    {
        private StringEmptyConverter converter;

        [TestInitialize]
        public void Setup()
        {
            // Create a new StringEmptyConverter for each test
            converter = new StringEmptyConverter();
        }

        [TestMethod]
        public void Convert_NullString_ReturnsTrue()
        {
            // Arrange
            string input = null;

            // Act
            object result = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsTrue((bool)result);
        }

        [TestMethod]
        public void Convert_EmptyString_ReturnsTrue()
        {
            // Arrange
            string input = string.Empty;

            // Act
            object result = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsTrue((bool)result);
        }

        [TestMethod]
        public void Convert_WhitespaceString_ReturnsFalse()
        {
            // Arrange
            string input = "   ";

            // Act
            object result = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsFalse((bool)result);
        }

        [TestMethod]
        public void Convert_NonEmptyString_ReturnsFalse()
        {
            // Arrange
            string input = "This is not empty";

            // Act
            object result = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsFalse((bool)result);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ConvertBack_ThrowsNotImplementedException()
        {
            // Act & Assert - Should throw NotImplementedException
            converter.ConvertBack(true, typeof(string), null, CultureInfo.InvariantCulture);
        }

        [TestMethod]
        public void Convert_NonStringInput_ReturnsTrue()
        {
            // Arrange
            object input = new object();

            // Act
            object result = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsTrue((bool)result);
        }

        [TestMethod]
        public void Convert_IntInput_ReturnsTrue()
        {
            // Arrange
            object input = 42;

            // Act
            object result = converter.Convert(input, typeof(bool), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsInstanceOfType(result, typeof(bool));
            Assert.IsTrue((bool)result);
        }
    }
}