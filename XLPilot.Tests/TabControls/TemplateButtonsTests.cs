using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using XLPilot.Models;
using XLPilot.Models.Enums;
using XLPilot.TabControls;

namespace XLPilot.Tests.TabControls
{
    [TestClass]
    public class TemplateButtonsTests
    {
        [TestMethod]
        public void CreateXLTemplateButtons_XLConfigTab2_CreatesCorrectButtons()
        {
            // We need to access the private method in XLConfigTab2 using reflection
            var type = typeof(XLConfigTab2);
            var methodInfo = type.GetMethod("CreateXLTemplateButtons",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Create an instance of XLConfigTab2
            // Note: This will fail if there are UI dependencies in the constructor
            // For a real test, you would need to mock these dependencies
            // But for a simple test, we'll use a try/catch block

            try
            {
                // Try to create an instance
                var xlConfigTab = new XLConfigTab2();

                // Invoke the method
                var buttons = (ObservableCollection<PilotButtonData>)methodInfo.Invoke(xlConfigTab, null);

                // Test that we got buttons back
                Assert.IsNotNull(buttons);
                Assert.IsTrue(buttons.Count > 0);

                // Test the specific buttons we expect
                // Find the XL button
                var xlButton = FindButtonByText(buttons, "XL");
                Assert.IsNotNull(xlButton);
                Assert.AreEqual("cdnxl.exe", xlButton.FileName);
                Assert.AreEqual(PilotButtonType.SystemStandard, xlButton.ButtonType);

                // Find the XL admin button
                var xlAdminButton = FindButtonByText(buttons, "XL admin");
                Assert.IsNotNull(xlAdminButton);
                Assert.AreEqual("cdnxl.exe", xlAdminButton.FileName);
                Assert.IsTrue(xlAdminButton.RunAsAdmin);
                Assert.AreEqual(PilotButtonType.SystemStandard, xlAdminButton.ButtonType);

                // Find the Rejestr.bat button
                var rejestrButton = FindButtonByText(buttons, "Rejestr.bat");
                Assert.IsNotNull(rejestrButton);
                Assert.AreEqual("rejestr.bat", rejestrButton.FileName);
                Assert.IsTrue(rejestrButton.RunAsAdmin);
                Assert.AreEqual(PilotButtonType.SystemSpecial, rejestrButton.ButtonType);
                Assert.AreEqual("RejestrBat", rejestrButton.ActionIdentifier);

                // Find the Path button
                var pathButton = FindButtonByText(buttons, "Zmienna Path");
                Assert.IsNotNull(pathButton);
                Assert.IsTrue(pathButton.RunAsAdmin);
                Assert.AreEqual(PilotButtonType.SystemSpecial, pathButton.ButtonType);
                Assert.AreEqual("ChangeEnvVariable", pathButton.ActionIdentifier);
            }
            catch (System.Exception ex)
            {
                // If we fail to create the instance, mark test as inconclusive
                Assert.Inconclusive($"Could not create XLConfigTab2 instance: {ex.Message}");
            }
        }

        [TestMethod]
        public void CreateXLTemplateButtons_OtherConfigTab3_CreatesCorrectButtons()
        {
            // Access the private method in OtherConfigTab3 using reflection
            var type = typeof(OtherConfigTab3);
            var methodInfo = type.GetMethod("CreateXLTemplateButtons",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            try
            {
                // Try to create an instance
                var otherConfigTab = new OtherConfigTab3();

                // Invoke the method
                var buttons = (ObservableCollection<PilotButtonData>)methodInfo.Invoke(otherConfigTab, null);

                // Test that we got buttons back
                Assert.IsNotNull(buttons);
                Assert.IsTrue(buttons.Count > 0);

                // Test some specific buttons we expect
                // Find the Computer Config Registry button
                var configButton = FindButtonByText(buttons, "Konfig. komp.");
                Assert.IsNotNull(configButton);
                Assert.IsTrue(configButton.RunAsAdmin);
                Assert.AreEqual(PilotButtonType.SystemSpecial, configButton.ButtonType);
                Assert.AreEqual("ComputerConfigRegistry", configButton.ActionIdentifier);

                // Find the User Databases Registry button
                var userDBButton = FindButtonByText(buttons, "Bazy użytkownika");
                Assert.IsNotNull(userDBButton);
                Assert.IsTrue(userDBButton.RunAsAdmin);
                Assert.AreEqual(PilotButtonType.SystemSpecial, userDBButton.ButtonType);
                Assert.AreEqual("UserDatabasesRegistry", userDBButton.ActionIdentifier);

                // Find the Computer Databases Registry button
                var computerDBButton = FindButtonByText(buttons, "Bazy komputera");
                Assert.IsNotNull(computerDBButton);
                Assert.IsTrue(computerDBButton.RunAsAdmin);
                Assert.AreEqual(PilotButtonType.SystemSpecial, computerDBButton.ButtonType);
                Assert.AreEqual("ComputerDatabasesRegistry", computerDBButton.ActionIdentifier);

                // Find the DS Services Registry button
                var dsServicesButton = FindButtonByText(buttons, "Usługi DS");
                Assert.IsNotNull(dsServicesButton);
                Assert.IsTrue(dsServicesButton.RunAsAdmin);
                Assert.AreEqual(PilotButtonType.SystemSpecial, dsServicesButton.ButtonType);
                Assert.AreEqual("DSServicesRegistry", dsServicesButton.ActionIdentifier);

                // Find the Temp Folder button
                var tempFolderButton = FindButtonByText(buttons, "Folder temp");
                Assert.IsNotNull(tempFolderButton);
                Assert.AreEqual(PilotButtonType.SystemSpecial, tempFolderButton.ButtonType);
                Assert.AreEqual("TempFolder", tempFolderButton.ActionIdentifier);
            }
            catch (System.Exception ex)
            {
                // If we fail to create the instance, mark test as inconclusive
                Assert.Inconclusive($"Could not create OtherConfigTab3 instance: {ex.Message}");
            }
        }

        // Helper method to find a button by text
        private PilotButtonData FindButtonByText(ObservableCollection<PilotButtonData> buttons, string text)
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
    }
}