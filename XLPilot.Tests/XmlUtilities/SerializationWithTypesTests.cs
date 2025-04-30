using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;
using XLPilot.Models;
using XLPilot.Models.Enums;
using XLPilot.XmlUtilities;

namespace XLPilot.Tests.XmlUtilities
{
    [TestClass]
    public class SerializationWithTypesTests
    {
        private string testFilePath = "test_serialization_types.xml";

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test file after each test
            if (File.Exists(testFilePath))
            {
                File.Delete(testFilePath);
            }
        }

        [TestMethod]
        public void Serialize_PilotButtonWithTypes_PreservesTypeInformation()
        {
            // Arrange
            var data = new SerializationData();

            // Create buttons with different types
            var userButton = new PilotButtonData(
                "User Button",
                "user.exe",
                "",
                false,
                "",
                "",
                "",
                PilotButtonType.UserStandard,
                ""
            );

            var systemButton = new PilotButtonData(
                "System Button",
                "system.exe",
                "",
                false,
                "",
                "",
                "",
                PilotButtonType.SystemStandard,
                ""
            );

            var specialButton = new PilotButtonData(
                "Special Button",
                "",
                "",
                true,
                "",
                "",
                "",
                PilotButtonType.SystemSpecial,
                "SpecialAction"
            );

            // Add buttons to the data
            data.XLPilotButtons.Add(userButton);
            data.XLPilotButtons.Add(systemButton);
            data.XLPilotButtons.Add(specialButton);

            // Act
            XmlSerializer<SerializationData>.Serialize(data, testFilePath);
            var loadedData = XmlSerializer<SerializationData>.Deserialize(testFilePath);

            // Assert
            Assert.IsNotNull(loadedData);
            Assert.AreEqual(3, loadedData.XLPilotButtons.Count);

            // Check that the button types were preserved
            var loadedUserButton = FindButtonByText(loadedData.XLPilotButtons, "User Button");
            Assert.IsNotNull(loadedUserButton);
            Assert.AreEqual(PilotButtonType.UserStandard, loadedUserButton.ButtonType);

            var loadedSystemButton = FindButtonByText(loadedData.XLPilotButtons, "System Button");
            Assert.IsNotNull(loadedSystemButton);
            Assert.AreEqual(PilotButtonType.SystemStandard, loadedSystemButton.ButtonType);

            var loadedSpecialButton = FindButtonByText(loadedData.XLPilotButtons, "Special Button");
            Assert.IsNotNull(loadedSpecialButton);
            Assert.AreEqual(PilotButtonType.SystemSpecial, loadedSpecialButton.ButtonType);
            Assert.AreEqual("SpecialAction", loadedSpecialButton.ActionIdentifier);
        }

        [TestMethod]
        public void Serialize_ActionIdentifiers_PreservesInformation()
        {
            // Arrange
            var data = new SerializationData();

            // Create buttons with different action identifiers
            var button1 = new PilotButtonData(
                "Change Path",
                "",
                "",
                true,
                "",
                "",
                "",
                PilotButtonType.SystemSpecial,
                "ChangeEnvVariable"
            );

            var button2 = new PilotButtonData(
                "Rejestr.bat",
                "rejestr.bat",
                "",
                true,
                "",
                "",
                "",
                PilotButtonType.SystemSpecial,
                "RejestrBat"
            );

            var button3 = new PilotButtonData(
                "Temp Folder",
                "",
                "",
                false,
                "",
                "",
                "",
                PilotButtonType.SystemSpecial,
                "TempFolder"
            );

            // Add buttons to the data
            data.OtherPilotButtons.Add(button1);
            data.OtherPilotButtons.Add(button2);
            data.OtherPilotButtons.Add(button3);

            // Act
            XmlSerializer<SerializationData>.Serialize(data, testFilePath);
            var loadedData = XmlSerializer<SerializationData>.Deserialize(testFilePath);

            // Assert
            Assert.IsNotNull(loadedData);
            Assert.AreEqual(3, loadedData.OtherPilotButtons.Count);

            // Check that the action identifiers were preserved
            var loadedButton1 = FindButtonByActionIdentifier(loadedData.OtherPilotButtons, "ChangeEnvVariable");
            Assert.IsNotNull(loadedButton1);
            Assert.AreEqual("Change Path", loadedButton1.ButtonText);

            var loadedButton2 = FindButtonByActionIdentifier(loadedData.OtherPilotButtons, "RejestrBat");
            Assert.IsNotNull(loadedButton2);
            Assert.AreEqual("Rejestr.bat", loadedButton2.ButtonText);

            var loadedButton3 = FindButtonByActionIdentifier(loadedData.OtherPilotButtons, "TempFolder");
            Assert.IsNotNull(loadedButton3);
            Assert.AreEqual("Temp Folder", loadedButton3.ButtonText);
        }

        [TestMethod]
        public void SerializeMultipleTypes_PreservesExactTypes()
        {
            // Arrange
            var data = new SerializationData();

            // Create a mix of button types
            data.XLPilotButtons = new List<PilotButtonData>
            {
                // UserStandard button
                new PilotButtonData("User 1", "user1.exe", "", false, "", "", "", PilotButtonType.UserStandard, ""),
                new PilotButtonData("User 2", "user2.exe", "", false, "", "", "", PilotButtonType.UserStandard, ""),
                
                // SystemStandard button
                new PilotButtonData("System 1", "sys1.exe", "", true, "", "", "", PilotButtonType.SystemStandard, ""),
                
                // SystemSpecial buttons
                new PilotButtonData("Special 1", "", "", true, "", "", "", PilotButtonType.SystemSpecial, "Action1"),
                new PilotButtonData("Special 2", "", "", true, "", "", "", PilotButtonType.SystemSpecial, "Action2")
            };

            // Expected counts by type
            int expectedUserStandard = 2;
            int expectedSystemStandard = 1;
            int expectedSystemSpecial = 2;

            // Act
            XmlSerializer<SerializationData>.Serialize(data, testFilePath);
            var loadedData = XmlSerializer<SerializationData>.Deserialize(testFilePath);

            // Assert
            Assert.IsNotNull(loadedData);
            Assert.AreEqual(5, loadedData.XLPilotButtons.Count);

            // Count buttons by type
            int userStandardCount = 0;
            int systemStandardCount = 0;
            int systemSpecialCount = 0;

            foreach (var button in loadedData.XLPilotButtons)
            {
                switch (button.ButtonType)
                {
                    case PilotButtonType.UserStandard:
                        userStandardCount++;
                        break;
                    case PilotButtonType.SystemStandard:
                        systemStandardCount++;
                        break;
                    case PilotButtonType.SystemSpecial:
                        systemSpecialCount++;
                        break;
                }
            }

            // Check that the counts match expected
            Assert.AreEqual(expectedUserStandard, userStandardCount, "UserStandard count mismatch");
            Assert.AreEqual(expectedSystemStandard, systemStandardCount, "SystemStandard count mismatch");
            Assert.AreEqual(expectedSystemSpecial, systemSpecialCount, "SystemSpecial count mismatch");
        }

        [TestMethod]
        public void ReadOldData_WithoutTypeInfo_DefaultsToUserStandard()
        {
            // Arrange - Create XML without type info
            string oldFormatXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<SerializationData xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <XLPathsList>
    <XLPaths>
      <Name>Test XL</Name>
      <Path>C:\Test</Path>
      <Database>TestDB</Database>
    </XLPaths>
  </XLPathsList>
  <XLPilotButtons>
    <PilotButtonData>
      <ButtonText>Old Button</ButtonText>
      <FileName>old.exe</FileName>
      <ImageSource>/image.png</ImageSource>
      <RunAsAdmin>false</RunAsAdmin>
      <Arguments>-old</Arguments>
      <ToolTipText>Old format button</ToolTipText>
      <Directory>C:\OldDir</Directory>
      <!-- ButtonType and ActionIdentifier are missing -->
    </PilotButtonData>
  </XLPilotButtons>
  <OtherPilotButtons />
</SerializationData>";

            File.WriteAllText(testFilePath, oldFormatXml);

            // Act
            var loadedData = XmlSerializer<SerializationData>.Deserialize(testFilePath);

            // Assert
            Assert.IsNotNull(loadedData);
            Assert.AreEqual(1, loadedData.XLPilotButtons.Count);

            // The old button should default to UserStandard
            var oldButton = loadedData.XLPilotButtons[0];
            Assert.AreEqual("Old Button", oldButton.ButtonText);
            Assert.AreEqual(PilotButtonType.UserStandard, oldButton.ButtonType);
            Assert.AreEqual("", oldButton.ActionIdentifier);
        }

        [TestMethod]
        public void Serialize_DifferentButtonTypes_WithAllProperties()
        {
            // Arrange
            var data = new SerializationData();

            // Create buttons with all properties set
            var userButton = new PilotButtonData(
                "Complete User Button",
                "user_complete.exe",
                "/test/image1.png",
                false,
                "-userarg",
                "User tooltip",
                "C:\\UserDir",
                PilotButtonType.UserStandard,
                ""
            );

            var systemButton = new PilotButtonData(
                "Complete System Button",
                "system_complete.exe",
                "/test/image2.png",
                true,
                "-sysarg",
                "System tooltip",
                "C:\\SysDir",
                PilotButtonType.SystemStandard,
                ""
            );

            var specialButton = new PilotButtonData(
                "Complete Special Button",
                "special_complete.exe",
                "/test/image3.png",
                true,
                "-specialarg",
                "Special tooltip",
                "C:\\SpecialDir",
                PilotButtonType.SystemSpecial,
                "CompleteSpecialAction"
            );

            // Add buttons to the data
            data.XLPilotButtons.Add(userButton);
            data.XLPilotButtons.Add(systemButton);
            data.XLPilotButtons.Add(specialButton);

            // Act
            XmlSerializer<SerializationData>.Serialize(data, testFilePath);
            var loadedData = XmlSerializer<SerializationData>.Deserialize(testFilePath);

            // Assert
            Assert.IsNotNull(loadedData);
            Assert.AreEqual(3, loadedData.XLPilotButtons.Count);

            // Check that all properties were preserved for each button type
            var loadedUserButton = FindButtonByText(loadedData.XLPilotButtons, "Complete User Button");
            Assert.IsNotNull(loadedUserButton);
            Assert.AreEqual("user_complete.exe", loadedUserButton.FileName);
            Assert.AreEqual("/test/image1.png", loadedUserButton.ImageSource);
            Assert.IsFalse(loadedUserButton.RunAsAdmin);
            Assert.AreEqual("-userarg", loadedUserButton.Arguments);
            Assert.AreEqual("User tooltip", loadedUserButton.ToolTipText);
            Assert.AreEqual("C:\\UserDir", loadedUserButton.Directory);
            Assert.AreEqual(PilotButtonType.UserStandard, loadedUserButton.ButtonType);
            Assert.AreEqual("", loadedUserButton.ActionIdentifier);

            var loadedSystemButton = FindButtonByText(loadedData.XLPilotButtons, "Complete System Button");
            Assert.IsNotNull(loadedSystemButton);
            Assert.AreEqual("system_complete.exe", loadedSystemButton.FileName);
            Assert.AreEqual("/test/image2.png", loadedSystemButton.ImageSource);
            Assert.IsTrue(loadedSystemButton.RunAsAdmin);
            Assert.AreEqual("-sysarg", loadedSystemButton.Arguments);
            Assert.AreEqual("System tooltip", loadedSystemButton.ToolTipText);
            Assert.AreEqual("C:\\SysDir", loadedSystemButton.Directory);
            Assert.AreEqual(PilotButtonType.SystemStandard, loadedSystemButton.ButtonType);
            Assert.AreEqual("", loadedSystemButton.ActionIdentifier);

            var loadedSpecialButton = FindButtonByText(loadedData.XLPilotButtons, "Complete Special Button");
            Assert.IsNotNull(loadedSpecialButton);
            Assert.AreEqual("special_complete.exe", loadedSpecialButton.FileName);
            Assert.AreEqual("/test/image3.png", loadedSpecialButton.ImageSource);
            Assert.IsTrue(loadedSpecialButton.RunAsAdmin);
            Assert.AreEqual("-specialarg", loadedSpecialButton.Arguments);
            Assert.AreEqual("Special tooltip", loadedSpecialButton.ToolTipText);
            Assert.AreEqual("C:\\SpecialDir", loadedSpecialButton.Directory);
            Assert.AreEqual(PilotButtonType.SystemSpecial, loadedSpecialButton.ButtonType);
            Assert.AreEqual("CompleteSpecialAction", loadedSpecialButton.ActionIdentifier);
        }

        // Helper method to find a button by text
        private PilotButtonData FindButtonByText(List<PilotButtonData> buttons, string text)
        {
            foreach (var button in buttons)
            {
                if (button.ButtonText == text)
                {
                    return button;
                }
            }
            return null;
        }

        // Helper method to find a button by action identifier
        private PilotButtonData FindButtonByActionIdentifier(List<PilotButtonData> buttons, string actionIdentifier)
        {
            foreach (var button in buttons)
            {
                if (button.ActionIdentifier == actionIdentifier)
                {
                    return button;
                }
            }
            return null;
        }
    }
}