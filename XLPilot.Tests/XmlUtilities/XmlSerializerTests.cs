using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using XLPilot.Models;
using XLPilot.XmlUtilities;

namespace XLPilot.Tests.XmlUtilities
{
    [TestClass]
    public class XmlSerializerTests
    {
        private string testFilePath = "test_serializer.xml";

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
        public void Serialize_ValidObject_WritesToFile()
        {
            // Arrange
            var testData = new SerializationData();
            testData.XLPathsList.Add(new XLPaths("Test", "C:\\Test"));

            // Act
            XmlSerializer<SerializationData>.Serialize(testData, testFilePath);

            // Assert
            Assert.IsTrue(File.Exists(testFilePath));
            string fileContent = File.ReadAllText(testFilePath);
            Assert.IsTrue(fileContent.Contains("<Name>Test</Name>"));
            Assert.IsTrue(fileContent.Contains("<Path>C:\\Test</Path>"));
        }

        [TestMethod]
        public void Deserialize_ValidFile_ReadsObject()
        {
            // Arrange - Create a file with test data
            var testData = new SerializationData();
            testData.XLPathsList.Add(new XLPaths("Test", "C:\\Test"));
            XmlSerializer<SerializationData>.Serialize(testData, testFilePath);

            // Act
            var loadedData = XmlSerializer<SerializationData>.Deserialize(testFilePath);

            // Assert
            Assert.IsNotNull(loadedData);
            Assert.AreEqual(1, loadedData.XLPathsList.Count);
            Assert.AreEqual("Test", loadedData.XLPathsList[0].Name);
            Assert.AreEqual("C:\\Test", loadedData.XLPathsList[0].Path);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Deserialize_FileDoesNotExist_ThrowsException()
        {
            // Arrange
            string nonExistentFile = "non_existent_file.xml";

            // Make sure the file doesn't exist
            if (File.Exists(nonExistentFile))
            {
                File.Delete(nonExistentFile);
            }

            // Act - Should throw FileNotFoundException
            XmlSerializer<SerializationData>.Deserialize(nonExistentFile);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Deserialize_InvalidXml_ThrowsException()
        {
            // Arrange - Create file with invalid XML
            File.WriteAllText(testFilePath, "<InvalidXml>This is not valid XML for the SerializationData class");

            // Act - Should throw InvalidOperationException
            XmlSerializer<SerializationData>.Deserialize(testFilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Serialize_InvalidFilePath_ThrowsException()
        {
            // Arrange
            var testData = new SerializationData();

            // Invalid path with characters that aren't allowed in filenames
            string invalidPath = "invalid:*?\\/<>|.xml";

            // Act - Should throw ArgumentException
            XmlSerializer<SerializationData>.Serialize(testData, invalidPath);
        }

        [TestMethod]
        public void Serialize_Deserialize_RoundTrip_PreservesData()
        {
            // Arrange
            var originalData = new SerializationData();

            // Add XL paths
            originalData.XLPathsList.Add(new XLPaths("Path1", "C:\\Path1", "DB1", "Server1", "Key1"));
            originalData.XLPathsList.Add(new XLPaths("Path2", "C:\\Path2", "DB2", "Server2", "Key2"));

            // Add XL pilot buttons
            originalData.XLPilotButtons.Add(new PilotButtonData("XLButton1", "xl1.exe", "/image1.png", true, "-arg1", "Tooltip1", "C:\\Dir1"));
            originalData.XLPilotButtons.Add(new PilotButtonData("XLButton2", "xl2.exe", "/image2.png", false, "-arg2", "Tooltip2", "C:\\Dir2"));

            // Add Other pilot buttons
            originalData.OtherPilotButtons.Add(new PilotButtonData("OtherButton1", "other1.exe"));
            originalData.OtherPilotButtons.Add(new PilotButtonData("OtherButton2", "other2.exe"));

            // Act
            XmlSerializer<SerializationData>.Serialize(originalData, testFilePath);
            var loadedData = XmlSerializer<SerializationData>.Deserialize(testFilePath);

            // Assert
            Assert.IsNotNull(loadedData);

            // Check XL paths
            Assert.AreEqual(2, loadedData.XLPathsList.Count);
            Assert.AreEqual("Path1", loadedData.XLPathsList[0].Name);
            Assert.AreEqual("C:\\Path1", loadedData.XLPathsList[0].Path);
            Assert.AreEqual("DB1", loadedData.XLPathsList[0].Database);
            Assert.AreEqual("Server1", loadedData.XLPathsList[0].LicenseServer);
            Assert.AreEqual("Key1", loadedData.XLPathsList[0].LicenseKey);

            // Check XL buttons
            Assert.AreEqual(2, loadedData.XLPilotButtons.Count);
            Assert.AreEqual("XLButton1", loadedData.XLPilotButtons[0].ButtonText);
            Assert.AreEqual("xl1.exe", loadedData.XLPilotButtons[0].FileName);
            Assert.AreEqual("/image1.png", loadedData.XLPilotButtons[0].ImageSource);
            Assert.IsTrue(loadedData.XLPilotButtons[0].RunAsAdmin);
            Assert.AreEqual("-arg1", loadedData.XLPilotButtons[0].Arguments);
            Assert.AreEqual("Tooltip1", loadedData.XLPilotButtons[0].ToolTipText);
            Assert.AreEqual("C:\\Dir1", loadedData.XLPilotButtons[0].Directory);

            // Check Other buttons
            Assert.AreEqual(2, loadedData.OtherPilotButtons.Count);
            Assert.AreEqual("OtherButton1", loadedData.OtherPilotButtons[0].ButtonText);
            Assert.AreEqual("other1.exe", loadedData.OtherPilotButtons[0].FileName);
        }

        [TestMethod]
        public void Serialize_EmptyCollections_CreatesValidXml()
        {
            // Arrange
            var testData = new SerializationData();
            // No items added to collections

            // Act
            XmlSerializer<SerializationData>.Serialize(testData, testFilePath);
            var loadedData = XmlSerializer<SerializationData>.Deserialize(testFilePath);

            // Assert
            Assert.IsNotNull(loadedData);
            Assert.IsNotNull(loadedData.XLPathsList);
            Assert.IsNotNull(loadedData.XLPilotButtons);
            Assert.IsNotNull(loadedData.OtherPilotButtons);
            Assert.AreEqual(0, loadedData.XLPathsList.Count);
            Assert.AreEqual(0, loadedData.XLPilotButtons.Count);
            Assert.AreEqual(0, loadedData.OtherPilotButtons.Count);
        }
    }
}