using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using XLPilot.Models;
using XLPilot.Models.Containers;
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

            // Verify inputs are valid
            Assert.IsTrue(XmlValidator.ValidateInput(buttonName));
            Assert.IsTrue(XmlValidator.ValidateInput(fileName));
            Assert.IsTrue(XmlValidator.ValidateInput(imagePath));
            Assert.IsTrue(XmlValidator.ValidateInput(tooltip));
            Assert.IsTrue(XmlValidator.ValidateInput(directory));

            // 2. Create a SerializationManager
            var manager = new SerializationManager(Path.Combine(tempDirectory, testConfigFile));

            // 3. Add data to the manager
            var button = new PilotButtonData(buttonName, fileName, imagePath, true, "", tooltip, directory);
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
        }

        [TestMethod]
        public void Integration_ExportImportFunctionality()
        {
            // This test verifies export/import functionality

            // 1. Set up a manager with initial data
            var manager = new SerializationManager(Path.Combine(tempDirectory, testConfigFile));

            // Add an XL button
            var xlButton = new PilotButtonData("XL Button", "xl.exe");
            manager.GetData().XLPilotButtons.Add(xlButton);

            // Add an Other button
            var otherButton = new PilotButtonData("Other Button", "other.exe");
            manager.GetData().OtherPilotButtons.Add(otherButton);

            // Save the data
            manager.SaveAllData();

            // 2. Export the XL buttons
            string xlExportFile = Path.Combine(tempDirectory, "exported_xl_buttons.xml");
            manager.SaveXLPilotButtons(xlExportFile);

            // 3. Export the Other buttons
            string otherExportFile = Path.Combine(tempDirectory, "exported_other_buttons.xml");
            manager.SaveOtherPilotButtons(otherExportFile);

            // 4. Create a new manager with empty data
            var importManager = new SerializationManager(Path.Combine(tempDirectory, "import_test.xml"));

            // 5. Import the XL buttons
            importManager.LoadXLPilotButtons(xlExportFile);

            // Verify XL buttons imported correctly
            Assert.AreEqual(1, importManager.GetData().XLPilotButtons.Count);
            Assert.AreEqual("XL Button", importManager.GetData().XLPilotButtons[0].ButtonText);
            Assert.AreEqual("xl.exe", importManager.GetData().XLPilotButtons[0].FileName);

            // 6. Import the Other buttons
            importManager.LoadOtherPilotButtons(otherExportFile);

            // Verify Other buttons imported correctly
            Assert.AreEqual(1, importManager.GetData().OtherPilotButtons.Count);
            Assert.AreEqual("Other Button", importManager.GetData().OtherPilotButtons[0].ButtonText);
            Assert.AreEqual("other.exe", importManager.GetData().OtherPilotButtons[0].FileName);
        }

        [TestMethod]
        public void Integration_UpdateExistingData()
        {
            // This test verifies updating existing data

            // 1. Set up a manager with initial data
            var manager = new SerializationManager(Path.Combine(tempDirectory, testConfigFile));

            // Add XL path
            var originalPath = new XLPaths("TestPath", "C:\\Original");
            manager.AddXLPath(originalPath);

            // Save the data
            manager.SaveAllData();

            // 2. Update the XL path
            var updatedPath = new XLPaths("TestPath", "C:\\Updated", "UpdatedDB", "UpdatedServer", "UpdatedKey");
            bool updateResult = manager.UpdateXLPath(updatedPath);

            // Verify the update succeeded
            Assert.IsTrue(updateResult);

            // 3. Save the updated data
            manager.SaveAllData();

            // 4. Create a new manager to verify the changes were saved
            var verifyManager = new SerializationManager(Path.Combine(tempDirectory, testConfigFile));

            // Verify the path was updated
            var data = verifyManager.GetData();
            Assert.AreEqual(1, data.XLPathsList.Count);
            Assert.AreEqual("TestPath", data.XLPathsList[0].Name);
            Assert.AreEqual("C:\\Updated", data.XLPathsList[0].Path);
            Assert.AreEqual("UpdatedDB", data.XLPathsList[0].Database);
            Assert.AreEqual("UpdatedServer", data.XLPathsList[0].LicenseServer);
            Assert.AreEqual("UpdatedKey", data.XLPathsList[0].LicenseKey);
        }

        [TestMethod]
        public void Integration_ImportAddsToExistingCollection()
        {
            // This test verifies that import adds to existing collections

            // 1. Set up a manager with initial data
            var manager = new SerializationManager(Path.Combine(tempDirectory, testConfigFile));

            // Add an XL button
            var existingButton = new PilotButtonData("Existing Button", "existing.exe");
            manager.GetData().XLPilotButtons.Add(existingButton);

            // Save the data
            manager.SaveAllData();

            // 2. Create a container and buttons to import
            var buttonsContainer = new PilotButtonsContainer();
            var importButton = new PilotButtonData("Import Button", "import.exe");
            buttonsContainer.AddButton(importButton);

            // Export the buttons container directly
            string exportFile = Path.Combine(tempDirectory, "export_buttons.xml");
            XmlSerializer<PilotButtonsContainer>.Serialize(buttonsContainer, exportFile);

            // 3. Import the buttons
            manager.ImportXLPilotButtons(exportFile);

            // 4. Verify the import added to the existing collection
            var data = manager.GetData();
            Assert.AreEqual(2, data.XLPilotButtons.Count);

            // Check the buttons exist by name
            bool foundExisting = false;
            bool foundImport = false;

            foreach (var button in data.XLPilotButtons)
            {
                if (button.ButtonText == "Existing Button" && button.FileName == "existing.exe")
                {
                    foundExisting = true;
                }
                else if (button.ButtonText == "Import Button" && button.FileName == "import.exe")
                {
                    foundImport = true;
                }
            }

            Assert.IsTrue(foundExisting, "Existing button should still be present");
            Assert.IsTrue(foundImport, "Imported button should be added");
        }

        [TestMethod]
        public void Integration_GetAndUpdateContainers()
        {
            // This test verifies getting and updating containers

            // 1. Set up a manager with initial data
            var manager = new SerializationManager(Path.Combine(tempDirectory, testConfigFile));

            // Add buttons
            manager.GetData().XLPilotButtons.Add(new PilotButtonData("Button1", "exe1.exe"));
            manager.SaveAllData();

            // 2. Get the container
            var container = manager.GetXLPilotButtonsContainer();

            // Verify the container has the initial data
            Assert.AreEqual(1, container.Items.Count);
            Assert.AreEqual("Button1", container.Items[0].ButtonText);

            // 3. Modify the container
            container.AddButton(new PilotButtonData("Button2", "exe2.exe"));
            container.RemoveButton(container.Items[0]); // Remove Button1

            // 4. Update with the modified container
            manager.UpdateXLPilotButtons(container);

            // 5. Verify the changes were saved
            var data = manager.GetData();
            Assert.AreEqual(1, data.XLPilotButtons.Count);
            Assert.AreEqual("Button2", data.XLPilotButtons[0].ButtonText);
            Assert.AreEqual("exe2.exe", data.XLPilotButtons[0].FileName);
        }
    }
}