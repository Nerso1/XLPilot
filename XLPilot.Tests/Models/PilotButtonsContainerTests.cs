using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XLPilot.Models.Containers;
using XLPilot.Models;

namespace XLPilot.Tests.Models
{
    /// <summary>
    /// Summary description for PilotButtonsContainerTests
    /// </summary>
    [TestClass]
    public class PilotButtonsContainerTests
    {
        [TestMethod]
        public void Constructor_Default_CreatesEmptyList()
        {
            // Arrange & Act
            var container = new PilotButtonsContainer();

            // Assert
            Assert.IsNotNull(container.Items);
            Assert.AreEqual(0, container.Items.Count);
        }

        [TestMethod]
        public void FilterEmptyEntries_RemovesEmptyButtons()
        {
            // Arrange
            var container = new PilotButtonsContainer();

            // Add empty button (all essential fields empty)
            container.Items.Add(new PilotButtonData("", "", ""));

            // Add non-empty button
            container.Items.Add(new PilotButtonData("Test", "file.exe"));

            // Act
            container.FilterEmptyEntries();

            // Assert
            Assert.AreEqual(1, container.Items.Count);
            Assert.AreEqual("Test", container.Items[0].ButtonText);
        }

        [TestMethod]
        public void RemoveDuplicates_RemovesDuplicateEntries()
        {
            // Arrange
            var container = new PilotButtonsContainer();

            // Add a button
            container.Items.Add(new PilotButtonData("Test", "file.exe", "image.png"));

            // Add a duplicate (same key values)
            container.Items.Add(new PilotButtonData("Test", "file.exe", "image.png"));

            // Add a different button
            container.Items.Add(new PilotButtonData("Different", "other.exe"));

            // Act
            container.RemoveDuplicates();

            // Assert
            Assert.AreEqual(2, container.Items.Count);
        }

        [TestMethod]
        public void AddButton_AddsToCollection()
        {
            // Arrange
            var container = new PilotButtonsContainer();
            var button = new PilotButtonData("Test");

            // Act
            container.AddButton(button);

            // Assert
            Assert.AreEqual(1, container.Items.Count);
            Assert.AreSame(button, container.Items[0]);
        }

        [TestMethod]
        public void AddRange_AddsMultipleButtons()
        {
            // Arrange
            var container = new PilotButtonsContainer();
            var buttons = new List<PilotButtonData>
            {
                new PilotButtonData("Button1"),
                new PilotButtonData("Button2"),
                new PilotButtonData("Button3")
            };

            // Act
            container.AddRange(buttons);

            // Assert
            Assert.AreEqual(3, container.Items.Count);
        }

        [TestMethod]
        public void RemoveButton_ExistingButton_RemovesAndReturnsTrue()
        {
            // Arrange
            var container = new PilotButtonsContainer();
            var button = new PilotButtonData("Test");
            container.AddButton(button);

            // Act
            bool result = container.RemoveButton(button);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, container.Items.Count);
        }

        [TestMethod]
        public void RemoveButton_NonExistingButton_ReturnsFalse()
        {
            // Arrange
            var container = new PilotButtonsContainer();
            container.AddButton(new PilotButtonData("Test1"));
            var buttonToRemove = new PilotButtonData("Test2");

            // Act
            bool result = container.RemoveButton(buttonToRemove);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(1, container.Items.Count);
        }

        [TestMethod]
        public void Clear_RemovesAllItems()
        {
            // Arrange
            var container = new PilotButtonsContainer();
            container.AddButton(new PilotButtonData("Test1"));
            container.AddButton(new PilotButtonData("Test2"));

            // Act
            container.Clear();

            // Assert
            Assert.AreEqual(0, container.Items.Count);
        }

        [TestMethod]
        public void Count_ReturnsNumberOfItems()
        {
            // Arrange
            var container = new PilotButtonsContainer();
            container.AddButton(new PilotButtonData("Test1"));
            container.AddButton(new PilotButtonData("Test2"));

            // Act
            int count = container.Count;

            // Assert
            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void UpdateOrder_ReordersItems()
        {
            // Arrange
            var container = new PilotButtonsContainer();
            var button1 = new PilotButtonData("Button1");
            var button2 = new PilotButtonData("Button2");
            var button3 = new PilotButtonData("Button3");

            container.AddRange(new List<PilotButtonData> { button1, button2, button3 });

            // New order: button3, button1, button2
            var newOrder = new List<PilotButtonData> { button3, button1, button2 };

            // Act
            container.UpdateOrder(newOrder);

            // Assert
            Assert.AreEqual(3, container.Items.Count);
            Assert.AreSame(button3, container.Items[0]);
            Assert.AreSame(button1, container.Items[1]);
            Assert.AreSame(button2, container.Items[2]);
        }
    }

}
