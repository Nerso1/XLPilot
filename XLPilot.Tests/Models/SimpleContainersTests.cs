using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XLPilot.Models.Containers;

namespace XLPilot.Tests.Models
{
    [TestClass]
    public class SimpleContainersTests
    {
        [TestMethod]
        public void DimensionsContainer_Constructor_CreatesEmptyList()
        {
            // Arrange & Act
            var container = new DimensionsContainer();

            // Assert
            Assert.IsNotNull(container.Items);
            Assert.AreEqual(0, container.Items.Count);
        }

        [TestMethod]
        public void FlagsContainer_Constructor_CreatesEmptyList()
        {
            // Arrange & Act
            var container = new FlagsContainer();

            // Assert
            Assert.IsNotNull(container.Items);
            Assert.AreEqual(0, container.Items.Count);
        }

        [TestMethod]
        public void OtherIconsContainer_Constructor_CreatesEmptyList()
        {
            // Arrange & Act
            var container = new OtherIconsContainer();

            // Assert
            Assert.IsNotNull(container.Items);
            Assert.AreEqual(0, container.Items.Count);
        }

        [TestMethod]
        public void XLIconsContainer_Constructor_CreatesEmptyList()
        {
            // Arrange & Act
            var container = new XLIconsContainer();

            // Assert
            Assert.IsNotNull(container.Items);
            Assert.AreEqual(0, container.Items.Count);
        }

        //[TestMethod]
        //public void PilotButtonDataContainer_Constructor_CreatesEmptyList()
        //{
        //    // Arrange & Act
        //    var container = new PilotButtonDataContainer();

        //    // Assert
        //    Assert.IsNotNull(container.Items);
        //    Assert.AreEqual(0, container.Items.Count);
        //}
    }

}
