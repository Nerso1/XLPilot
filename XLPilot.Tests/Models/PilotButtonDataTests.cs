using Microsoft.VisualStudio.TestTools.UnitTesting;
using XLPilot.Models;
using XLPilot.Models.Enums;

namespace XLPilot.Tests.Models
{
    [TestClass]
    public class PilotButtonDataTests
    {
        [TestMethod]
        public void Equals_SameValues_ReturnsTrue()
        {
            // Arrange
            var button1 = new PilotButtonData("Test", "file.exe", "image.png", true, "args", "tooltip", "dir", PilotButtonType.SystemSpecial, "TestAction");
            var button2 = new PilotButtonData("Test", "file.exe", "image.png", true, "args", "tooltip", "dir", PilotButtonType.SystemSpecial, "TestAction");

            // Act
            bool result = button1.Equals(button2);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            // Arrange
            var button1 = new PilotButtonData("Test1", "file.exe", "image.png", true, "args", "tooltip", "dir", PilotButtonType.SystemSpecial, "TestAction");
            var button2 = new PilotButtonData("Test2", "file.exe", "image.png", true, "args", "tooltip", "dir", PilotButtonType.SystemSpecial, "TestAction");

            // Act
            bool result = button1.Equals(button2);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Constructor_Default_InitializesWithDefaultValues()
        {
            // Arrange & Act
            var button = new PilotButtonData();

            // Assert
            Assert.IsNull(button.ButtonText);
            Assert.AreEqual("", button.FileName);
            Assert.AreEqual("", button.ImageSource);
            Assert.IsFalse(button.RunAsAdmin);
            Assert.AreEqual("", button.Arguments);
            Assert.AreEqual("", button.ToolTipText);
            Assert.AreEqual("", button.Directory);
            Assert.AreEqual(PilotButtonType.UserStandard, button.ButtonType);
            Assert.AreEqual("", button.ActionIdentifier);
        }

        [TestMethod]
        public void GetHashCode_SameValues_ReturnsSameHashCode()
        {
            // Arrange
            var button1 = new PilotButtonData("Test", "file.exe", "image.png", true, "args", "tooltip", "dir", PilotButtonType.SystemSpecial, "TestAction");
            var button2 = new PilotButtonData("Test", "file.exe", "image.png", true, "args", "tooltip", "dir", PilotButtonType.SystemSpecial, "TestAction");

            // Act
            int hash1 = button1.GetHashCode();
            int hash2 = button2.GetHashCode();

            // Assert
            Assert.AreEqual(hash1, hash2);
        }

        [TestMethod]
        public void GetHashCode_DifferentValues_ReturnsDifferentHashCodes()
        {
            // Arrange
            var button1 = new PilotButtonData("Test1", "file.exe", "image.png", true, "args", "tooltip", "dir", PilotButtonType.SystemSpecial, "TestAction");
            var button2 = new PilotButtonData("Test2", "file.exe", "image.png", true, "args", "tooltip", "dir", PilotButtonType.UserStandard, "TestAction");

            // Act
            int hash1 = button1.GetHashCode();
            int hash2 = button2.GetHashCode();

            // Assert - Note: This could theoretically fail due to hash collisions, but it's very unlikely
            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void Equals_NullObject_ReturnsFalse()
        {
            // Arrange
            var button = new PilotButtonData("Test", "file.exe");

            // Act
            bool result = button.Equals(null);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_DifferentType_ReturnsFalse()
        {
            // Arrange
            var button = new PilotButtonData("Test", "file.exe");
            var differentObject = new object();

            // Act
            bool result = button.Equals(differentObject);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_DifferentButtonType_ReturnsFalse()
        {
            // Arrange
            var button1 = new PilotButtonData("Test", "file.exe", "image.png", true, "args", "tooltip", "dir", PilotButtonType.SystemSpecial, "TestAction");
            var button2 = new PilotButtonData("Test", "file.exe", "image.png", true, "args", "tooltip", "dir", PilotButtonType.UserStandard, "TestAction");

            // Act
            bool result = button1.Equals(button2);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_DifferentActionIdentifier_ReturnsFalse()
        {
            // Arrange
            var button1 = new PilotButtonData("Test", "file.exe", "image.png", true, "args", "tooltip", "dir", PilotButtonType.SystemSpecial, "TestAction1");
            var button2 = new PilotButtonData("Test", "file.exe", "image.png", true, "args", "tooltip", "dir", PilotButtonType.SystemSpecial, "TestAction2");

            // Act
            bool result = button1.Equals(button2);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Constructor_WithParams_SetsAllProperties()
        {
            // Arrange & Act
            var button = new PilotButtonData(
                "Test Button",
                "test.exe",
                "image.png",
                true,
                "-arg",
                "Tooltip text",
                "C:\\TestDir",
                PilotButtonType.SystemSpecial,
                "TestAction");

            // Assert
            Assert.AreEqual("Test Button", button.ButtonText);
            Assert.AreEqual("test.exe", button.FileName);
            Assert.AreEqual("image.png", button.ImageSource);
            Assert.IsTrue(button.RunAsAdmin);
            Assert.AreEqual("-arg", button.Arguments);
            Assert.AreEqual("Tooltip text", button.ToolTipText);
            Assert.AreEqual("C:\\TestDir", button.Directory);
            Assert.AreEqual(PilotButtonType.SystemSpecial, button.ButtonType);
            Assert.AreEqual("TestAction", button.ActionIdentifier);
        }
    }
}