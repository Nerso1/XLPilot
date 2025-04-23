using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XLPilot.Models;

namespace XLPilot.Tests.Models
{
    [TestClass]
    public class SerializationDataTests
    {
        [TestMethod]
        public void Constructor_Default_InitializesEmptyLists()
        {
            // Arrange & Act
            var data = new SerializationData();

            // Assert
            Assert.IsNotNull(data.XLPathsList);
            Assert.IsNotNull(data.XLPilotButtons);
            Assert.IsNotNull(data.OtherPilotButtons);

            Assert.AreEqual(0, data.XLPathsList.Count);
            Assert.AreEqual(0, data.XLPilotButtons.Count);
            Assert.AreEqual(0, data.OtherPilotButtons.Count);
        }

        [TestMethod]
        public void XLPathsList_AddItem_ItemAddedToList()
        {
            // Arrange
            var data = new SerializationData();
            var path = new XLPaths("Test", "C:\\Test");

            // Act
            data.XLPathsList.Add(path);

            // Assert
            Assert.AreEqual(1, data.XLPathsList.Count);
            Assert.AreSame(path, data.XLPathsList[0]);
        }

        [TestMethod]
        public void XLPilotButtons_AddItem_ItemAddedToList()
        {
            // Arrange
            var data = new SerializationData();
            var button = new PilotButtonData("Test", "file.exe");

            // Act
            data.XLPilotButtons.Add(button);

            // Assert
            Assert.AreEqual(1, data.XLPilotButtons.Count);
            Assert.AreSame(button, data.XLPilotButtons[0]);
        }

        [TestMethod]
        public void OtherPilotButtons_AddItem_ItemAddedToList()
        {
            // Arrange
            var data = new SerializationData();
            var button = new PilotButtonData("Test", "file.exe");

            // Act
            data.OtherPilotButtons.Add(button);

            // Assert
            Assert.AreEqual(1, data.OtherPilotButtons.Count);
            Assert.AreSame(button, data.OtherPilotButtons[0]);
        }
    }

}
