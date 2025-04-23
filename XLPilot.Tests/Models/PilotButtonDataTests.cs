using Microsoft.VisualStudio.TestTools.UnitTesting;
using XLPilot.Models;

namespace XLPilot.Tests.Models
{
    [TestClass]
    public class PilotButtonDataTests
    {
        [TestMethod]
        public void Equals_SameValues_ReturnsTrue()
        {
            // Arrange
            var button1 = new PilotButtonData("Test", "file.exe", "image.png", true, "args", "tooltip", "dir");
            var button2 = new PilotButtonData("Test", "file.exe", "image.png", true, "args", "tooltip", "dir");

            // Act
            bool result = button1.Equals(button2);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_DifferentValues_ReturnsFalse()
        {
            // Arrange
            var button1 = new PilotButtonData("Test1", "file.exe", "image.png", true, "args", "tooltip", "dir");
            var button2 = new PilotButtonData("Test2", "file.exe", "image.png", true, "args", "tooltip", "dir");

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
            }

            [TestMethod]
            public void GetHashCode_SameValues_ReturnsSameHashCode()
            {
                // Arrange
                var button1 = new PilotButtonData("Test", "file.exe", "image.png", true, "args", "tooltip", "dir");
                var button2 = new PilotButtonData("Test", "file.exe", "image.png", true, "args", "tooltip", "dir");

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
                var button1 = new PilotButtonData("Test1", "file.exe", "image.png", true, "args", "tooltip", "dir");
                var button2 = new PilotButtonData("Test2", "file.exe", "image.png", true, "args", "tooltip", "dir");

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
            public void Equals_EachPropertyDifferent_ReturnsFalse()
            {
                // Arrange - Test each property individually
                var baseButton = new PilotButtonData("Test", "file.exe", "image.png", true, "args", "tooltip", "dir");

                // Different text
                var diffText = new PilotButtonData("Different", "file.exe", "image.png", true, "args", "tooltip", "dir");
                // Different file
                var diffFile = new PilotButtonData("Test", "different.exe", "image.png", true, "args", "tooltip", "dir");
                // Different image
                var diffImage = new PilotButtonData("Test", "file.exe", "different.png", true, "args", "tooltip", "dir");
                // Different admin
                var diffAdmin = new PilotButtonData("Test", "file.exe", "image.png", false, "args", "tooltip", "dir");
                // Different args
                var diffArgs = new PilotButtonData("Test", "file.exe", "image.png", true, "different", "tooltip", "dir");
                // Different tooltip
                var diffTooltip = new PilotButtonData("Test", "file.exe", "image.png", true, "args", "different", "dir");
                // Different directory
                var diffDir = new PilotButtonData("Test", "file.exe", "image.png", true, "args", "tooltip", "different");

                // Act & Assert - Check each property difference causes Equals to return false
                Assert.IsFalse(baseButton.Equals(diffText));
                Assert.IsFalse(baseButton.Equals(diffFile));
                Assert.IsFalse(baseButton.Equals(diffImage));
                Assert.IsFalse(baseButton.Equals(diffAdmin));
                Assert.IsFalse(baseButton.Equals(diffArgs));
                Assert.IsFalse(baseButton.Equals(diffTooltip));
                Assert.IsFalse(baseButton.Equals(diffDir));
            }
        }

    }
