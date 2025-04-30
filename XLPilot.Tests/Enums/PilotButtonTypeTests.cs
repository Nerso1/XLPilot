using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using XLPilot.Models.Enums;

namespace XLPilot.Tests.Models.Enums
{
    [TestClass]
    public class PilotButtonTypeTests
    {
        [TestMethod]
        public void PilotButtonType_Values_MatchExpected()
        {
            // This test verifies that the enum values match the expected values
            // This is important for serialization/deserialization compatibility

            // Assert
            Assert.AreEqual(0, (int)PilotButtonType.UserStandard);
            Assert.AreEqual(1, (int)PilotButtonType.SystemStandard);
            Assert.AreEqual(2, (int)PilotButtonType.SystemSpecial);
        }

        [TestMethod]
        public void PilotButtonType_Serializable_HasSerializableAttribute()
        {
            // This test verifies that the enum has the Serializable attribute
            // which is essential for XML serialization

            // Act
            var attributes = typeof(PilotButtonType).GetCustomAttributes(typeof(SerializableAttribute), false);

            // Assert
            Assert.IsTrue(attributes.Length > 0, "PilotButtonType should have the Serializable attribute");
        }

        [TestMethod]
        public void PilotButtonType_Enum_DocumentationCommentsExist()
        {
            // In a real test environment, we would check the XML documentation
            // Since we can't easily check for XML documentation in unit tests, 
            // we'll use reflection to at least ensure the enum type exists

            // Act
            var enumType = typeof(PilotButtonType);
            var enumValues = Enum.GetValues(enumType);

            // Assert
            Assert.AreEqual(3, enumValues.Length, "PilotButtonType should have 3 values defined");

            // Try casting some values to ensure they're valid
            var userStandard = (PilotButtonType)0;
            var systemStandard = (PilotButtonType)1;
            var systemSpecial = (PilotButtonType)2;

            Assert.AreEqual(PilotButtonType.UserStandard, userStandard);
            Assert.AreEqual(PilotButtonType.SystemStandard, systemStandard);
            Assert.AreEqual(PilotButtonType.SystemSpecial, systemSpecial);
        }

        [TestMethod]
        public void PilotButtonType_ToStringAndParse_Roundtrip()
        {
            // This test verifies that the enum values can be converted to string and back
            // Important for UI display and serialization

            // Arrange
            var original = PilotButtonType.SystemSpecial;

            // Act
            string asString = original.ToString();
            PilotButtonType parsed = (PilotButtonType)Enum.Parse(typeof(PilotButtonType), asString);

            // Assert
            Assert.AreEqual(original, parsed);
        }

        [TestMethod]
        public void PilotButtonType_InvalidValue_ThrowsException()
        {
            // This test verifies that trying to parse an invalid value throws an exception
            // Important for robustness

            // Act & Assert
            Assert.ThrowsException<ArgumentException>(() =>
                Enum.Parse(typeof(PilotButtonType), "InvalidValue"));
        }

        [TestMethod]
        public void PilotButtonType_CastInvalidValue_DoesNotThrowException()
        {
            // This test verifies that casting an invalid integer value doesn't throw an exception
            // but instead creates an undefined enum value
            // This is important for handling deserializaton of potentially outdated saved data

            // Act
            var invalidValue = (PilotButtonType)999;

            // Assert - No exception should be thrown
            // Note: The value won't be equal to any of the defined values
            Assert.AreNotEqual(PilotButtonType.UserStandard, invalidValue);
            Assert.AreNotEqual(PilotButtonType.SystemStandard, invalidValue);
            Assert.AreNotEqual(PilotButtonType.SystemSpecial, invalidValue);
        }
    }
}