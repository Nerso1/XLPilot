using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using XLPilot.Models;
using XLPilot.Models.Containers;
using XLPilot.Models.Enums;
using XLPilot.XmlUtilities;

namespace XLPilot.Tests.XmlUtilities
{
    [TestClass]
    public class XmlUtilitiesIntegrationTests
    {
        private string testConfigFile = "test_integration.xml";
        private string tempDirectory = "test_integration_temp";

        [TestInitialize]
        public void Setup()
        {
            // Create temp directory if it doesn't exist
            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
            }

            // Clean up test files
            if (File.Exists(testConfigFile))
            {
                File.Delete(testConfigFile);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test files
            if (File.Exists(testConfigFile))
            {
                File.Delete(testConfigFile);
            }

            // Clean up temp directory and files
            if (Directory.Exists(tempDirectory))
            {
                foreach (var file in Directory.GetFiles(tempDirectory))
                {
                    File.Delete(file);
                }
                Directory.Delete(tempDirectory);
            }
        }

        [TestMethod]
        public void Integration_XmlSerializer_SerializationManager_XmlValidator()
        {
            // This test verifies that all components work together

            // 1. Create test data with validated inputs
            var buttonName = "Test Button";
            var fileName = "test.exe";
            var imagePath = "/test/image.png";
            var tooltip = "Test Tooltip";
            var directory = "C:\\TestDir";
            var buttonType = PilotButtonType.SystemSpecial;
            var actionIdentifier = "TestAction";

            // Verify inputs are valid
            Assert.IsTrue(XmlValidator.ValidateInput(buttonName));
            Assert.IsTrue(XmlValidator.ValidateInput(fileName));
            Assert.IsTrue(XmlValidator.ValidateInput(imagePath));
            Assert.IsTrue(XmlValidator.ValidateInput(tooltip));
            Assert.IsTrue(XmlValidator.ValidateInput(directory));
            Assert.IsTrue(XmlValidator.ValidateInput(actionIdentifier));

            // 2. Create a SerializationManager
            var manager = new SerializationManager(Path.Combine(tempDirectory, testConfigFile));

            // 3. Add data to the manager
            var button = new PilotButtonData(buttonName, fileName, imagePath, true, "", tooltip, directory, buttonType, actionIdentifier);
            manager.GetData().XLPilotButtons.Add(button);

            var path = new XLPaths("TestPath", "C:\\TestPath", "TestDB", "TestServer", "TestKey");
            manager.AddXLPath(path);

            // 4. Save the data
            manager.SaveAllData();

            // 5. Create a new manager to load the data
            var manager2 = new SerializationManager(Path.Combine(tempDirectory, testConfigFile));

            // 6. Verify the data was loaded correctly
            var data = manager2.GetData();

            Assert.IsNotNull(data);

            // Check XL Paths
            Assert.AreEqual(1, data.XLPathsList.Count);
            Assert.AreEqual("TestPath", data.XLPathsList[0].Name);
            Assert.AreEqual("C:\\TestPath", data.XLPathsList[0].Path);

            // Check Pilot Buttons
            Assert.AreEqual(1, data.XLPilotButtons.Count);
            Assert.AreEqual(buttonName, data.XLPilotButtons[0].ButtonText);
            Assert.AreEqual(fileName, data.XLPilotButtons[0].FileName);
            Assert.AreEqual(imagePath, data.XLPilotButtons[0].ImageSource);
            Assert.IsTrue(data.XLPilotButtons[0].RunAsAdmin);
            Assert.AreEqual(tooltip, data.XLPilotButtons[0].ToolTipText);
            Assert.AreEqual(directory, data.XLPilotButtons[0].Directory);
            Assert.AreEqual(buttonType, data.XLPilotButtons[0].ButtonType);
            Assert.AreEqual(actionIdentifier, data.XLPilotButtons[0].ActionIdentifier);
        }

        [TestMethod]
        public void Serialize_Deserialize_RoundTrip_WithNewProperties()
        {
            // Create a button with the new properties
            var originalButton = new PilotButtonData(
                "Special Button",
                "special.exe",
                "/image.png",
                true,
                "-args",
                "Tooltip",
                "C:\\Dir",
                PilotButtonType.SystemSpecial,
                "SpecialAction");

            // Create container for the button
            var container = new PilotButtonsContainer();
            container.AddButton(originalButton);

            // Serialize the container
            string filePath = Path.Combine(tempDirectory, "serialization_test.xml");
            XmlSerializer<PilotButtonsContainer>.Serialize(container, filePath);

            // Deserialize the container
            var loadedContainer = XmlSerializer<PilotButtonsContainer>.Deserialize(filePath);

            // Verify the deserialized button
            Assert.AreEqual(1, loadedContainer.Items.Count);
            var loadedButton = loadedContainer.Items[0];

            Assert.AreEqual(originalButton.ButtonText, loadedButton.ButtonText);
            Assert.AreEqual(originalButton.FileName, loadedButton.FileName);
            Assert.AreEqual(originalButton.ImageSource, loadedButton.ImageSource);
            Assert.AreEqual(originalButton.RunAsAdmin, loadedButton.RunAsAdmin);
            Assert.AreEqual(originalButton.Arguments, loadedButton.Arguments);
            Assert.AreEqual(originalButton.ToolTipText, loadedButton.ToolTipText);
            Assert.AreEqual(originalButton.Directory, loadedButton.Directory);
            Assert.AreEqual(originalButton.ButtonType, loadedButton.ButtonType);
            Assert.AreEqual(originalButton.ActionIdentifier, loadedButton.ActionIdentifier);
        }
    }
}