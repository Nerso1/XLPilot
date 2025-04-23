using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;
using System.Windows.Controls;
using XLPilot.UserControls;

namespace XLPilot.Tests.UserControls
{
    [TestClass]
    public class PilotButtonTests
    {
        private PilotButton pilotButton;

        [TestInitialize]
        public void Setup()
        {
            // Create a new PilotButton for each test
            pilotButton = new PilotButton();

            // Initialize dependency properties with test values
            pilotButton.ButtonText = "Test Button";
            pilotButton.FileName = "test.exe";
            pilotButton.ImageSource = "/test/image.png";
            pilotButton.RunAsAdmin = true;
            pilotButton.Arguments = "-test";
            pilotButton.ToolTipText = "Test Tooltip";
            pilotButton.Directory = "C:\\TestDir";
        }

        [TestMethod]
        public void Constructor_InitializesProperties()
        {
            // Arrange
            var button = new PilotButton();

            // Assert - Check that default values are set correctly
            Assert.AreEqual(string.Empty, button.ButtonText);
            Assert.AreEqual(string.Empty, button.FileName);
            Assert.AreEqual("/XLPilot;component/Resources/Images/detault-profile-picture.png", button.ImageSource);
            Assert.IsFalse(button.RunAsAdmin);
            Assert.AreEqual(null, button.Arguments);
            Assert.AreEqual(string.Empty, button.ToolTipText);
            Assert.AreEqual(string.Empty, button.Directory);
        }

        [TestMethod]
        public void ButtonText_SetAndGet_WorksCorrectly()
        {
            // Arrange
            string expectedText = "New Button Text";

            // Act
            pilotButton.ButtonText = expectedText;

            // Assert
            Assert.AreEqual(expectedText, pilotButton.ButtonText);
        }

        [TestMethod]
        public void FileName_SetAndGet_WorksCorrectly()
        {
            // Arrange
            string expectedFileName = "newfile.exe";

            // Act
            pilotButton.FileName = expectedFileName;

            // Assert
            Assert.AreEqual(expectedFileName, pilotButton.FileName);
        }

        [TestMethod]
        public void ImageSource_SetAndGet_WorksCorrectly()
        {
            // Arrange
            string expectedImageSource = "/new/image.png";

            // Act
            pilotButton.ImageSource = expectedImageSource;

            // Assert
            Assert.AreEqual(expectedImageSource, pilotButton.ImageSource);
        }

        [TestMethod]
        public void RunAsAdmin_SetAndGet_WorksCorrectly()
        {
            // Arrange
            bool expectedValue = !pilotButton.RunAsAdmin;

            // Act
            pilotButton.RunAsAdmin = expectedValue;

            // Assert
            Assert.AreEqual(expectedValue, pilotButton.RunAsAdmin);
        }

        [TestMethod]
        public void Arguments_SetAndGet_WorksCorrectly()
        {
            // Arrange
            string expectedArguments = "-new -argument";

            // Act
            pilotButton.Arguments = expectedArguments;

            // Assert
            Assert.AreEqual(expectedArguments, pilotButton.Arguments);
        }

        [TestMethod]
        public void ToolTipText_SetAndGet_WorksCorrectly()
        {
            // Arrange
            string expectedToolTip = "New tooltip text";

            // Act
            pilotButton.ToolTipText = expectedToolTip;

            // Assert
            Assert.AreEqual(expectedToolTip, pilotButton.ToolTipText);
        }

        [TestMethod]
        public void Directory_SetAndGet_WorksCorrectly()
        {
            // Arrange
            string expectedDirectory = "C:\\NewDir";

            // Act
            pilotButton.Directory = expectedDirectory;

            // Assert
            Assert.AreEqual(expectedDirectory, pilotButton.Directory);
        }

        // Note: We can't easily test the click handler or FindChildControl method
        // without setting up a visual tree in a test framework like UI Automation
    }
}