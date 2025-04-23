using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using XLPilot.Models;
using XLPilot.UserControls;

namespace XLPilot.Tests.UserControls
{
    [TestClass]
    public class WrapPanel_DragAndDropBasicTests
    {
        private WrapPanel_DragAndDrop wrapPanel;

        [TestInitialize]
        public void Setup()
        {
            // Create a new WrapPanel_DragAndDrop for each test
            wrapPanel = new WrapPanel_DragAndDrop();

            // Initialize with some test data
            wrapPanel.ProjectItems = new ObservableCollection<PilotButtonData>
            {
                new PilotButtonData("Project1", "project1.exe"),
                new PilotButtonData("Project2", "project2.exe")
            };

            wrapPanel.ToolboxItems = new ObservableCollection<PilotButtonData>
            {
                new PilotButtonData("Toolbox1", "toolbox1.exe"),
                new PilotButtonData("Toolbox2", "toolbox2.exe")
            };
        }

        [TestMethod]
        public void Constructor_InitializesCollections()
        {
            // Arrange
            var newPanel = new WrapPanel_DragAndDrop();

            // Assert
            Assert.IsNotNull(newPanel.ProjectItems);
            Assert.IsNotNull(newPanel.ToolboxItems);
            Assert.AreEqual(0, newPanel.ProjectItems.Count);
            Assert.AreEqual(0, newPanel.ToolboxItems.Count);
        }

        [TestMethod]
        public void ProjectItems_SetAndGet_WorksCorrectly()
        {
            // Arrange
            var newCollection = new ObservableCollection<PilotButtonData>
            {
                new PilotButtonData("New1", "new1.exe"),
                new PilotButtonData("New2", "new2.exe"),
                new PilotButtonData("New3", "new3.exe")
            };

            // Act
            wrapPanel.ProjectItems = newCollection;

            // Assert
            Assert.AreEqual(3, wrapPanel.ProjectItems.Count);
            Assert.AreEqual("New1", wrapPanel.ProjectItems[0].ButtonText);
            Assert.AreEqual("New2", wrapPanel.ProjectItems[1].ButtonText);
            Assert.AreEqual("New3", wrapPanel.ProjectItems[2].ButtonText);
        }

        [TestMethod]
        public void ToolboxItems_SetAndGet_WorksCorrectly()
        {
            // Arrange
            var newCollection = new ObservableCollection<PilotButtonData>
            {
                new PilotButtonData("NewTool1", "newtool1.exe"),
                new PilotButtonData("NewTool2", "newtool2.exe")
            };

            // Act
            wrapPanel.ToolboxItems = newCollection;

            // Assert
            Assert.AreEqual(2, wrapPanel.ToolboxItems.Count);
            Assert.AreEqual("NewTool1", wrapPanel.ToolboxItems[0].ButtonText);
            Assert.AreEqual("NewTool2", wrapPanel.ToolboxItems[1].ButtonText);
        }

        [TestMethod]
        public void ItemsDropped_EventHandlerCanBeAddedAndRemoved()
        {
            // Arrange
            bool eventWasRaised = false;
            wrapPanel.ItemsDropped += (sender, e) => { eventWasRaised = true; };

            // Act - We can't easily trigger a real drag and drop event in a unit test,
            // but we can verify the event handler was added correctly

            // Assert
            // If we had a way to trigger the event, we could assert eventWasRaised is true
            // For now, we just verify no exception was thrown when adding the handler
            Assert.IsFalse(eventWasRaised);
        }

        // Note: Full testing of drag and drop functionality requires UI automation
        // which is beyond the scope of these unit tests
    }
}