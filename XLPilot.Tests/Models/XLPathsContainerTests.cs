using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XLPilot.Models.Containers;
using XLPilot.Models;

namespace XLPilot.Tests.Models
{
    [TestClass]
    public class XLPathsContainerTests
    {
        [TestMethod]
        public void Constructor_Default_CreatesEmptyList()
        {
            // Arrange & Act
            var container = new XLPathsContainer();

            // Assert
            Assert.IsNotNull(container.Items);
            Assert.AreEqual(0, container.Items.Count);
        }

        [TestMethod]
        public void FilterEmptyEntries_RemovesEmptyEntries()
        {
            // Arrange
            var container = new XLPathsContainer();

            // Add an empty entry
            container.Items.Add(new XLPaths());

            // Add a non-empty entry
            container.Items.Add(new XLPaths("Test", "C:\\Test"));

            // Act
            container.FilterEmptyEntries();

            // Assert
            Assert.AreEqual(1, container.Items.Count);
            Assert.AreEqual("Test", container.Items[0].Name);
        }

        [TestMethod]
        public void RemoveDuplicates_RemovesDuplicateNames()
        {
            // Arrange
            var container = new XLPathsContainer();

            // Add a path
            container.Items.Add(new XLPaths("Test", "C:\\Test1"));

            // Add another path with the same name
            container.Items.Add(new XLPaths("Test", "C:\\Test2"));

            // Add a different path
            container.Items.Add(new XLPaths("Different", "C:\\Different"));

            // Act
            container.RemoveDuplicates();

            // Assert
            Assert.AreEqual(2, container.Items.Count);
            // Should keep the first one with that name
            Assert.AreEqual("C:\\Test1", container.Items[0].Path);
        }

        [TestMethod]
        public void AddEntry_AddsToCollection()
        {
            // Arrange
            var container = new XLPathsContainer();
            var path = new XLPaths("Test", "C:\\Test");

            // Act
            container.AddEntry(path);

            // Assert
            Assert.AreEqual(1, container.Items.Count);
            Assert.AreSame(path, container.Items[0]);
        }

        [TestMethod]
        public void UpdateEntry_ExistingName_UpdatesPropertiesAndReturnsTrue()
        {
            // Arrange
            var container = new XLPathsContainer();
            container.Items.Add(new XLPaths("Test", "C:\\OldPath"));

            var updatedPath = new XLPaths("Test", "C:\\NewPath", "NewDB", "NewServer", "NewKey");

            // Act
            bool result = container.UpdateEntry(updatedPath);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("C:\\NewPath", container.Items[0].Path);
            Assert.AreEqual("NewDB", container.Items[0].Database);
            Assert.AreEqual("NewServer", container.Items[0].LicenseServer);
            Assert.AreEqual("NewKey", container.Items[0].LicenseKey);
        }

        [TestMethod]
        public void UpdateEntry_NonExistingName_ReturnsFalse()
        {
            // Arrange
            var container = new XLPathsContainer();
            container.Items.Add(new XLPaths("Test1", "C:\\Test1"));

            var updatedPath = new XLPaths("Test2", "C:\\Test2");

            // Act
            bool result = container.UpdateEntry(updatedPath);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(1, container.Items.Count);
            Assert.AreEqual("Test1", container.Items[0].Name);
        }

        [TestMethod]
        public void RemoveEntry_ByObject_ExistingPath_RemovesAndReturnsTrue()
        {
            // Arrange
            var container = new XLPathsContainer();
            var path = new XLPaths("Test", "C:\\Test");
            container.AddEntry(path);

            // Act
            bool result = container.RemoveEntry(path);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, container.Items.Count);
        }

        [TestMethod]
        public void RemoveEntry_ByObject_NonExistingPath_ReturnsFalse()
        {
            // Arrange
            var container = new XLPathsContainer();
            container.AddEntry(new XLPaths("Test1", "C:\\Test1"));
            var pathToRemove = new XLPaths("Test2", "C:\\Test2");

            // Act
            bool result = container.RemoveEntry(pathToRemove);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(1, container.Items.Count);
        }

        [TestMethod]
        public void RemoveEntry_ByName_ExistingName_RemovesAndReturnsTrue()
        {
            // Arrange
            var container = new XLPathsContainer();
            container.AddEntry(new XLPaths("Test", "C:\\Test"));

            // Act
            bool result = container.RemoveEntry("Test");

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, container.Items.Count);
        }

        [TestMethod]
        public void RemoveEntry_ByName_NonExistingName_ReturnsFalse()
        {
            // Arrange
            var container = new XLPathsContainer();
            container.AddEntry(new XLPaths("Test1", "C:\\Test1"));

            // Act
            bool result = container.RemoveEntry("Test2");

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(1, container.Items.Count);
        }
    }

}
