using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XLPilot.XmlUtilities;

namespace XLPilot.Tests.XmlUtilities
{
    [TestClass]
    public class XmlValidatorTests
    {
        [TestMethod]
        public void ValidateInput_NullString_ReturnsTrue()
        {
            // Arrange
            string input = null;

            // Act
            bool result = XmlValidator.ValidateInput(input);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateInput_EmptyString_ReturnsTrue()
        {
            // Arrange
            string input = string.Empty;

            // Act
            bool result = XmlValidator.ValidateInput(input);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateInput_ValidString_ReturnsTrue()
        {
            // Arrange
            string input = "This is a valid string";

            // Act
            bool result = XmlValidator.ValidateInput(input);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateInput_StringWithLessThan_ReturnsFalse()
        {
            // Arrange
            string input = "This contains < character";

            // Act
            bool result = XmlValidator.ValidateInput(input);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateInput_StringWithGreaterThan_ReturnsFalse()
        {
            // Arrange
            string input = "This contains > character";

            // Act
            bool result = XmlValidator.ValidateInput(input);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateInput_StringWithAmpersand_ReturnsFalse()
        {
            // Arrange
            string input = "This contains & character";

            // Act
            bool result = XmlValidator.ValidateInput(input);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateInput_StringWithSingleQuote_ReturnsFalse()
        {
            // Arrange
            string input = "This contains ' character";

            // Act
            bool result = XmlValidator.ValidateInput(input);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateInput_StringWithDoubleQuote_ReturnsFalse()
        {
            // Arrange
            string input = "This contains \" character";

            // Act
            bool result = XmlValidator.ValidateInput(input);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateInput_StringWithNonBreakingSpace_ReturnsFalse()
        {
            // Arrange
            string input = "This contains" + '\u00A0' + "non-breaking space";

            // Act
            bool result = XmlValidator.ValidateInput(input);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateInput_StringWithMultipleInvalidChars_ReturnsFalse()
        {
            // Arrange
            string input = "This contains <> & invalid characters";

            // Act
            bool result = XmlValidator.ValidateInput(input);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateInput_StringWithAllForbiddenChars_ReturnsFalse()
        {
            // Arrange
            string input = "This contains <>&'\"\u00A0 all forbidden characters";

            // Act
            bool result = XmlValidator.ValidateInput(input);

            // Assert
            Assert.IsFalse(result);
        }
    }
}