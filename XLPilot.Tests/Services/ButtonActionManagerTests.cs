using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using XLPilot.Models;
using XLPilot.Models.Enums;
using XLPilot.Services;

namespace XLPilot.Tests.Services
{
    [TestClass]
    public class ButtonActionManagerTests
    {
        [TestMethod]
        public void DetermineArguments_EmptyArguments_WithXLPath_GeneratesFromXLPath()
        {
            // Arrange
            var button = new PilotButtonData(
                "Test Button",
                "test.exe",
                "",
                false,
                "",  // Empty arguments
                "Test tooltip",
                ""
            );

            var xlPath = new XLPaths(
                "Test XL",
                "C:\\TestPath",
                "TestDB",
                "TestServer",
                "TestKey"
            );

            // Act - Use reflection to call the private method
            var methodInfo = typeof(ButtonActionManager).GetMethod(
                "DetermineArguments",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
            );

            string result = (string)methodInfo.Invoke(null, new object[] { button, xlPath });

            // Assert
            Assert.IsTrue(result.Contains("baza=TestDB"));
            Assert.IsTrue(result.Contains("klucz=TestServer::TestKey"));
        }

        [TestMethod]
        public void DetermineArguments_IncludeArgument_WithXLPath_GeneratesFromXLPath()
        {
            // Arrange
            var button = new PilotButtonData(
                "Test Button",
                "test.exe",
                "",
                false,
                "include",  // Special include keyword
                "Test tooltip",
                ""
            );

            var xlPath = new XLPaths(
                "Test XL",
                "C:\\TestPath",
                "TestDB",
                "TestServer",
                "TestKey"
            );

            // Act - Use reflection to call the private method
            var methodInfo = typeof(ButtonActionManager).GetMethod(
                "DetermineArguments",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
            );

            string result = (string)methodInfo.Invoke(null, new object[] { button, xlPath });

            // Assert
            Assert.IsTrue(result.Contains("baza=TestDB"));
            Assert.IsTrue(result.Contains("klucz=TestServer::TestKey"));
        }

        [TestMethod]
        public void DetermineArguments_SkipArgument_WithXLPath_ReturnsEmptyString()
        {
            // Arrange
            var button = new PilotButtonData(
                "Test Button",
                "test.exe",
                "",
                false,
                "skip",  // Special skip keyword
                "Test tooltip",
                ""
            );

            var xlPath = new XLPaths(
                "Test XL",
                "C:\\TestPath",
                "TestDB",
                "TestServer",
                "TestKey"
            );

            // Act - Use reflection to call the private method
            var methodInfo = typeof(ButtonActionManager).GetMethod(
                "DetermineArguments",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
            );

            string result = (string)methodInfo.Invoke(null, new object[] { button, xlPath });

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void DetermineArguments_CustomArgument_WithXLPath_ReturnsCustomArgument()
        {
            // Arrange
            var button = new PilotButtonData(
                "Test Button",
                "test.exe",
                "",
                false,
                "-custom arg1 arg2",  // Custom arguments
                "Test tooltip",
                ""
            );

            var xlPath = new XLPaths(
                "Test XL",
                "C:\\TestPath",
                "TestDB",
                "TestServer",
                "TestKey"
            );

            // Act - Use reflection to call the private method
            var methodInfo = typeof(ButtonActionManager).GetMethod(
                "DetermineArguments",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
            );

            string result = (string)methodInfo.Invoke(null, new object[] { button, xlPath });

            // Assert
            Assert.AreEqual("-custom arg1 arg2", result);
        }

        [TestMethod]
        public void DetermineArguments_EmptyArguments_NoXLPath_ReturnsEmptyString()
        {
            // Arrange
            var button = new PilotButtonData(
                "Test Button",
                "test.exe",
                "",
                false,
                "",  // Empty arguments
                "Test tooltip",
                "C:\\CustomDir"  // Has directory, so not an XL button
            );

            // Act - Use reflection to call the private method
            var methodInfo = typeof(ButtonActionManager).GetMethod(
                "DetermineArguments",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
            );

            string result = (string)methodInfo.Invoke(null, new object[] { button, null });

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void DetermineArguments_CustomArguments_NoXLPath_ReturnsCustomArguments()
        {
            // Arrange
            var button = new PilotButtonData(
                "Test Button",
                "test.exe",
                "",
                false,
                "-custom args",  // Custom arguments
                "Test tooltip",
                "C:\\CustomDir"  // Has directory, so not an XL button
            );

            // Act - Use reflection to call the private method
            var methodInfo = typeof(ButtonActionManager).GetMethod(
                "DetermineArguments",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
            );

            string result = (string)methodInfo.Invoke(null, new object[] { button, null });

            // Assert
            Assert.AreEqual("-custom args", result);
        }

        [TestMethod]
        public void GenerateArguments_WithDatabaseOnly_GeneratesCorrectArguments()
        {
            // Arrange
            var xlPath = new XLPaths(
                "Test XL",
                "C:\\TestPath",
                "TestDB",
                "",  // No license server
                ""   // No license key
            );

            // Act - Use reflection to call the private method
            var methodInfo = typeof(ButtonActionManager).GetMethod(
                "GenerateArguments",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
            );

            string result = (string)methodInfo.Invoke(null, new object[] { xlPath });

            // Assert
            Assert.AreEqual("baza=TestDB", result);
        }

        [TestMethod]
        public void GenerateArguments_WithLicenseServerAndKeyOnly_GeneratesCorrectArguments()
        {
            // Arrange
            var xlPath = new XLPaths(
                "Test XL",
                "C:\\TestPath",
                "",  // No database
                "TestServer",
                "TestKey"
            );

            // Act - Use reflection to call the private method
            var methodInfo = typeof(ButtonActionManager).GetMethod(
                "GenerateArguments",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
            );

            string result = (string)methodInfo.Invoke(null, new object[] { xlPath });

            // Assert
            Assert.AreEqual("klucz=TestServer::TestKey", result);
        }

        [TestMethod]
        public void GenerateArguments_WithDatabaseAndLicense_GeneratesCorrectArguments()
        {
            // Arrange
            var xlPath = new XLPaths(
                "Test XL",
                "C:\\TestPath",
                "TestDB",
                "TestServer",
                "TestKey"
            );

            // Act - Use reflection to call the private method
            var methodInfo = typeof(ButtonActionManager).GetMethod(
                "GenerateArguments",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
            );

            string result = (string)methodInfo.Invoke(null, new object[] { xlPath });

            // Assert
            Assert.IsTrue(result.Contains("baza=TestDB"));
            Assert.IsTrue(result.Contains("klucz=TestServer::TestKey"));
        }
    }
}