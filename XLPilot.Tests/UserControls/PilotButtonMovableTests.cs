using Microsoft.VisualStudio.TestTools.UnitTesting;
using XLPilot.UserControls;

namespace XLPilot.Tests.UserControls
{
    [TestClass]
    public class PilotButtonMovableTests
    {
        private PilotButtonMovable pilotButtonMovable;

        [TestInitialize]
        public void Setup()
        {
            // Create a new PilotButtonMovable for each test
            pilotButtonMovable = new PilotButtonMovable();

            // Initialize dependency properties with test values
            pilotButtonMovable.ButtonText = "Test Button";
            pilotButtonMovable.FileName = "test.exe";
            pilotButtonMovable.ImageSource = "/test/image.png";
            pilotButtonMovable.RunAsAdmin = true;
            pilotButtonMovable.Arguments = "-test";
            pilotButtonMovable.ToolTipText = "Test Tooltip";
            pilotButtonMovable.Directory = "C:\\TestDir";
        }

        [TestMethod]
        public void Constructor_InitializesProperties()
        {
            // Arrange
            var button = new PilotButtonMovable();

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
            pilotButtonMovable.ButtonText = expectedText;

            // Assert
            Assert.AreEqual(expectedText, pilotButtonMovable.ButtonText);
        }

        [TestMethod]
        public void FileName_SetAndGet_WorksCorrectly()
        {
            // Arrange
            string expectedFileName = "newfile.exe";

            // Act
            pilotButtonMovable.FileName = expectedFileName;

            // Assert
            Assert.AreEqual(expectedFileName, pilotButtonMovable.FileName);
        }

        [TestMethod]
        public void ImageSource_SetAndGet_WorksCorrectly()
        {
            // Arrange
            string expectedImageSource = "/new/image.png";

            // Act
            pilotButtonMovable.ImageSource = expectedImageSource;

            // Assert
            Assert.AreEqual(expectedImageSource, pilotButtonMovable.ImageSource);
        }

        [TestMethod]
        public void RunAsAdmin_SetAndGet_WorksCorrectly()
        {
            // Arrange
            bool expectedValue = !pilotButtonMovable.RunAsAdmin;

            // Act
            pilotButtonMovable.RunAsAdmin = expectedValue;

            // Assert
            Assert.AreEqual(expectedValue, pilotButtonMovable.RunAsAdmin);
        }

        [TestMethod]
        public void Arguments_SetAndGet_WorksCorrectly()
        {
            // Arrange
            string expectedArguments = "-new -argument";

            // Act
            pilotButtonMovable.Arguments = expectedArguments;

            // Assert
            Assert.AreEqual(expectedArguments, pilotButtonMovable.Arguments);
        }

        [TestMethod]
        public void ToolTipText_SetAndGet_WorksCorrectly()
        {
            // Arrange
            string expectedToolTip = "New tooltip text";

            // Act
            pilotButtonMovable.ToolTipText = expectedToolTip;

            // Assert
            Assert.AreEqual(expectedToolTip, pilotButtonMovable.ToolTipText);
        }

        [TestMethod]
        public void Directory_SetAndGet_WorksCorrectly()
        {
            // Arrange
            string expectedDirectory = "C:\\NewDir";

            // Act
            pilotButtonMovable.Directory = expectedDirectory;

            // Assert
            Assert.AreEqual(expectedDirectory, pilotButtonMovable.Directory);
        }

        // Note: Testing the PreviewMouseLeftButtonDown event handler would require
        // more complex test setup with UI Automation testing frameworks
    }
}