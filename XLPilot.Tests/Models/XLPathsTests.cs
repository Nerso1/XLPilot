using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XLPilot.Models;

namespace XLPilot.Tests.Models
{
    /// <summary>
    /// Summary description for XLPathsTests
    /// </summary>
    [TestClass]
    public class XLPathsTests
    {
        public void Constructor_SetsAllProperties()
        {
            // Arrange & Act
            var path = new XLPaths("Test Name", "C:\\TestPath", "TestDB", "TestServer", "TestKey");

            // Assert
            Assert.AreEqual("Test Name", path.Name);
            Assert.AreEqual("C:\\TestPath", path.Path);
            Assert.AreEqual("TestDB", path.Database);
            Assert.AreEqual("TestServer", path.LicenseServer);
            Assert.AreEqual("TestKey", path.LicenseKey);
        }

        [TestMethod]
        public void FormattedLicenseInfo_BothServerAndKey_ReturnsFormatted()
        {
            // Arrange
            var path = new XLPaths("Test", "C:\\Test", "TestDB", "TestServer", "TestKey");

            // Act
            string result = path.FormattedLicenseInfo;

            // Assert
            Assert.AreEqual("TestServer::TestKey", result);
        }

        [TestMethod]
        public void FormattedLicenseInfo_OnlyServer_ReturnsServer()
        {
            // Arrange
            var path = new XLPaths("Test", "C:\\Test", "TestDB", "TestServer", null);

            // Act
            string result = path.FormattedLicenseInfo;

            // Assert
            Assert.AreEqual("TestServer", result);
        }

        [TestMethod]
        public void FormattedLicenseInfo_OnlyKey_ReturnsKey()
        {
            // Arrange
            var path = new XLPaths("Test", "C:\\Test", "TestDB", null, "TestKey");

            // Act
            string result = path.FormattedLicenseInfo;

            // Assert
            Assert.AreEqual("TestKey", result);
        }

        [TestMethod]
        public void FormattedLicenseInfo_NeitherServerNorKey_ReturnsEmpty()
        {
            // Arrange
            var path = new XLPaths("Test", "C:\\Test", "TestDB", null, null);

            // Act
            string result = path.FormattedLicenseInfo;

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void PropertyChanged_NameChanges_RaisesEvent()
        {
            // Arrange
            var path = new XLPaths();
            bool eventRaised = false;
            string propertyNameFromEvent = null;

            path.PropertyChanged += (sender, e) =>
            {
                eventRaised = true;
                propertyNameFromEvent = e.PropertyName;
            };

            // Act
            path.Name = "New Name";

            // Assert
            Assert.IsTrue(eventRaised, "PropertyChanged event was not raised");
            Assert.AreEqual("Name", propertyNameFromEvent);
        }

        [TestMethod]
        public void PropertyChanged_NoChange_DoesNotRaiseEvent()
        {
            // Arrange
            var path = new XLPaths("Test", "C:\\Test");
            bool eventRaised = false;

            path.PropertyChanged += (sender, e) =>
            {
                eventRaised = true;
            };

            // Act - set the same value
            path.Name = "Test";

            // Assert
            Assert.IsFalse(eventRaised, "PropertyChanged event should not be raised when value doesn't change");
        }
    }

}
