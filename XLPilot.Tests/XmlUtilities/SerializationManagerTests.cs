using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using XLPilot.Models;
using XLPilot.Models.Containers;
using XLPilot.XmlUtilities;

namespace XLPilot.Tests.XmlUtilities
{
    [TestClass]
    public class SerializationManagerTests
    {
        private Mock<TextWriter> mockWriter;
        private Mock<TextReader> mockReader;
        private string testFilePath = "test_config.xml";

        [TestInitialize]
        public void Setup()
        {
            // Set up mocks
            mockWriter = new Mock<TextWriter>();
            mockReader = new Mock<TextReader>();

            // Delete test file if it exists
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test file
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }

        [TestMethod]
        public void Constructor_FileDoesNotExist_CreatesNewData()
        {
            // Arrange & Act
            var manager = new SerializationManager(testFilePath);

            // Assert
            var data = manager.GetData();
            Assert.IsNotNull(data);
            Assert.IsNotNull(data.XLPathsList);
            Assert.IsNotNull(data.XLPilotButtons);
            Assert.IsNotNull(data.OtherPilotButtons);
        }

        [TestMethod]
        public void AddXLPath_NewPath_AddsToCollection()
        {
            // Arrange
            var manager = new SerializationManager(testFilePath);
            var newPath = new XLPaths("Test", "C:\\Test", "TestDB", "Server", "Key");

            // Act
            manager.AddXLPath(newPath);

            // Assert
            var data = manager.GetData();
            Assert.IsTrue(data.XLPathsList.Contains(newPath));
        }

        [TestMethod]
        public void UpdateXLPath_ExistingPath_UpdatesProperties()
        {
            // Arrange
            var manager = new SerializationManager(testFilePath);
            var originalPath = new XLPaths("Test", "C:\\Original");
            manager.AddXLPath(originalPath);

            var updatedPath = new XLPaths("Test", "C:\\Updated", "UpdatedDB", "UpdatedServer", "UpdatedKey");

            // Act
            bool result = manager.UpdateXLPath(updatedPath);

            // Assert
            Assert.IsTrue(result);
            var data = manager.GetData();
            var retrievedPath = data.XLPathsList.Find(p => p.Name == "Test");
            Assert.IsNotNull(retrievedPath);
            Assert.AreEqual("C:\\Updated", retrievedPath.Path);
            Assert.AreEqual("UpdatedDB", retrievedPath.Database);
            Assert.AreEqual("UpdatedServer", retrievedPath.LicenseServer);
            Assert.AreEqual("UpdatedKey", retrievedPath.LicenseKey);
        }

        [TestMethod]
        public void UpdateXLPath_NonExistingPath_ReturnsFalse()
        {
            // Arrange
            var manager = new SerializationManager(testFilePath);
            var updatedPath = new XLPaths("NonExistent", "C:\\Path");

            // Act
            bool result = manager.UpdateXLPath(updatedPath);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RemoveXLPath_ExistingPath_RemovesAndReturnsTrue()
        {
            // Arrange
            var manager = new SerializationManager(testFilePath);
            var pathToRemove = new XLPaths("Test", "C:\\Test");
            manager.AddXLPath(pathToRemove);

            // Act
            bool result = manager.RemoveXLPath(pathToRemove);

            // Assert
            Assert.IsTrue(result);
            var data = manager.GetData();
            Assert.AreEqual(0, data.XLPathsList.Count);
        }

        [TestMethod]
        public void GetXLPathsContainer_ReturnsContainerWithPaths()
        {
            // Arrange
            var manager = new SerializationManager(testFilePath);
            var path1 = new XLPaths("Test1", "C:\\Test1");
            var path2 = new XLPaths("Test2", "C:\\Test2");
            manager.AddXLPath(path1);
            manager.AddXLPath(path2);

            // Act
            var container = manager.GetXLPathsContainer();

            // Assert
            Assert.IsNotNull(container);
            Assert.AreEqual(2, container.Items.Count);
            Assert.IsTrue(container.Items.Contains(path1));
            Assert.IsTrue(container.Items.Contains(path2));
        }

        [TestMethod]
        public void UpdateXLPilotButtons_UpdatesCollection()
        {
            // Arrange
            var manager = new SerializationManager(testFilePath);
            var container = new PilotButtonsContainer();
            var button1 = new PilotButtonData("Button1", "exe1.exe");
            var button2 = new PilotButtonData("Button2", "exe2.exe");
            container.AddButton(button1);
            container.AddButton(button2);

            // Act
            manager.UpdateXLPilotButtons(container);

            // Assert
            var data = manager.GetData();
            Assert.AreEqual(2, data.XLPilotButtons.Count);
            Assert.IsTrue(data.XLPilotButtons.Exists(b => b.ButtonText == "Button1" && b.FileName == "exe1.exe"));
            Assert.IsTrue(data.XLPilotButtons.Exists(b => b.ButtonText == "Button2" && b.FileName == "exe2.exe"));
        }

        [TestMethod]
        public void GetXLPilotButtonsContainer_ReturnsContainerWithButtons()
        {
            // Arrange
            var manager = new SerializationManager(testFilePath);
            var button = new PilotButtonData("Test", "test.exe");
            manager.GetData().XLPilotButtons.Add(button);

            // Act
            var container = manager.GetXLPilotButtonsContainer();

            // Assert
            Assert.IsNotNull(container);
            Assert.AreEqual(1, container.Items.Count);
            Assert.AreEqual("Test", container.Items[0].ButtonText);
            Assert.AreEqual("test.exe", container.Items[0].FileName);
        }

        [TestMethod]
        public void ReloadFromFile_LoadsDataFromFile()
        {
            // This test requires integration with file system
            // For a pure unit test, we'd need to refactor SerializationManager to use an interface
            // Skipping detailed implementation

            // Arrange
            var manager = new SerializationManager(testFilePath);
            var path = new XLPaths("Test", "C:\\Test");
            manager.AddXLPath(path);
            manager.SaveAllData(); // Save to file

            // Create a new instance to simulate loading
            var manager2 = new SerializationManager(testFilePath);

            // Act
            manager2.ReloadFromFile();

            // Assert
            var data = manager2.GetData();
            Assert.AreEqual(1, data.XLPathsList.Count);
            Assert.AreEqual("Test", data.XLPathsList[0].Name);
        }

        [TestMethod]
        public void SaveAndLoadOtherPilotButtons_WorksCorrectly()
        {
            // Arrange
            var manager = new SerializationManager(testFilePath);
            var button = new PilotButtonData("OtherButton", "other.exe");
            manager.GetData().OtherPilotButtons.Add(button);

            string otherButtonsFile = "test_other_buttons.xml";

            try
            {
                // Act - Save
                manager.SaveOtherPilotButtons(otherButtonsFile);

                // Clear data
                manager.GetData().OtherPilotButtons.Clear();
                Assert.AreEqual(0, manager.GetData().OtherPilotButtons.Count);

                // Act - Load
                manager.LoadOtherPilotButtons(otherButtonsFile);

                // Assert
                Assert.AreEqual(1, manager.GetData().OtherPilotButtons.Count);
                Assert.AreEqual("OtherButton", manager.GetData().OtherPilotButtons[0].ButtonText);
                Assert.AreEqual("other.exe", manager.GetData().OtherPilotButtons[0].FileName);
            }
            finally
            {
                // Clean up
                if (File.Exists(otherButtonsFile))
                {
                    File.Delete(otherButtonsFile);
                }
            }
        }

        [TestMethod]
        public void ImportOtherPilotButtons_AddsToExistingCollection()
        {
            // Arrange
            var manager = new SerializationManager(testFilePath);
            var existingButton = new PilotButtonData("Existing", "existing.exe");
            manager.GetData().OtherPilotButtons.Add(existingButton);

            // Create a file to import - save ONLY the other pilot buttons, not the entire data
            string importFilePath = "other_buttons_import.xml";
            var buttonsContainer = new PilotButtonsContainer();
            var importButton = new PilotButtonData("Import", "import.exe");
            buttonsContainer.AddButton(importButton);

            // Serialize the container directly
            XmlSerializer<PilotButtonsContainer>.Serialize(buttonsContainer, importFilePath);

            try
            {
                // Act
                manager.ImportOtherPilotButtons(importFilePath);

                // Assert
                Assert.AreEqual(2, manager.GetData().OtherPilotButtons.Count);
                Assert.IsTrue(manager.GetData().OtherPilotButtons.Exists(b => b.ButtonText == "Existing"));
                Assert.IsTrue(manager.GetData().OtherPilotButtons.Exists(b => b.ButtonText == "Import"));
            }
            finally
            {
                // Clean up
                if (File.Exists(importFilePath))
                {
                    File.Delete(importFilePath);
                }
            }
        }
    }
}