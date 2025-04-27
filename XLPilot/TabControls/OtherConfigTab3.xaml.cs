using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using XLPilot.UserControls;
using XLPilot.XmlUtilities;
using XLPilot.Models;
using XLPilot.Services;
using XLPilot.Models.Enums;

namespace XLPilot.TabControls
{
    /// <summary>
    /// Tab for configuring other (non-XL) buttons
    /// </summary>
    public partial class OtherConfigTab3 : UserControl
    {
        // Reference to the serialization manager
        private SerializationManager serializationManager => SerializationService.Manager;

        /// <summary>
        /// Constructor - initializes the tab
        /// </summary>
        public OtherConfigTab3()
        {
            InitializeComponent();

            // Load other pilot buttons
            LoadOtherPilotButtons();

            // Set up event handlers
            SetupDragDropEvents();
            //SetupButtonHandlers();
        }

        /// <summary>
        /// Sets up event handlers for buttons
        /// </summary>
        //private void SetupButtonHandlers()
        //{
        //    // Set up export button
        //    if (BtnExportSelected != null)
        //        BtnExportSelected.Click += BtnExportSelected_Click;

        //    // Set up import button
        //    if (BtnImport != null)
        //        BtnImport.Click += BtnImport_Click;
        //}

        /// <summary>
        /// Sets up event handlers for drag and drop operations
        /// </summary>
        private void SetupDragDropEvents()
        {
            // This event is raised when items are dropped
            OtherDragDropControl.ItemsDropped += OtherDragDropControl_ItemsDropped;
        }

        /// <summary>
        /// Event handler for when items are dropped
        /// </summary>
        private void OtherDragDropControl_ItemsDropped(object sender, DragEventArgs e)
        {
            // Save changes when items are dropped
            SaveAllChanges();
        }

        /// <summary>
        /// Saves all changes in this tab
        /// </summary>
        public void SaveAllChanges()
        {
            // Reload data from file first
            ReloadData();

            // Then save our changes
            SaveOtherPilotButtons();
        }

        /// <summary>
        /// Reloads data from the file
        /// </summary>
        private void ReloadData()
        {
            // Force the SerializationManager to reload data from file
            serializationManager.ReloadFromFile();
        }

        /// <summary>
        /// Loads Other buttons from the configuration
        /// </summary>
        private void LoadOtherPilotButtons()
        {
            try
            {
                // Collection for the drag and drop control
                var projectItems = new ObservableCollection<PilotButtonData>();

                // Get the buttons from the configuration
                var pilotButtons = serializationManager.GetData().OtherPilotButtons;

                // Add default button if no buttons exist
                if (pilotButtons == null)
                {
                    // Add a sample button
                    projectItems.Add(new PilotButtonData(
                        "Other Item 1",
                        "",
                        "/XLPilot;component/Resources/Images/detault-profile-picture.png"
                        ));
                }
                else
                {
                    // Add each button to the collection
                    foreach (var button in pilotButtons)
                    {
                        projectItems.Add(button);
                    }
                }

                // Set up template buttons for the toolbox
                var toolboxItems = CreateXLTemplateButtons();

                // Assign the collections to the control
                OtherDragDropControl.ProjectItems = projectItems;
                OtherDragDropControl.ToolboxItems = toolboxItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas wczytywania przycisków 'inne': {ex.Message}",
                              "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Creates template buttons for the XL toolbox
        /// </summary>
        private ObservableCollection<PilotButtonData> CreateXLTemplateButtons()
        {
            var buttons = new ObservableCollection<PilotButtonData>();

            // Add special registry buttons with appropriate action identifiers
            buttons.Add(new PilotButtonData(
                "Konfig. komp.",
                "",
                "/XLPilot;component/Resources/Images/regedit.png",
                true,
                "",
                "Pokaż rejestr zawierający konfigurację komputera ze shella XL-a",
                "",
                PilotButtonType.SystemSpecial,
                "ComputerConfigRegistry"));

            buttons.Add(new PilotButtonData(
                "Bazy użytkownika",
                "",
                "/XLPilot;component/Resources/Images/regedit.png",
                true,
                "",
                "Pokaż rejestr zawierający bazy HKCU",
                "",
                PilotButtonType.SystemSpecial,
                "UserDatabasesRegistry"));

            buttons.Add(new PilotButtonData(
                "Bazy komputera",
                "",
                "/XLPilot;component/Resources/Images/regedit.png",
                true,
                "",
                "Pokaż rejestr zawierający bazy HKLM",
                "",
                PilotButtonType.SystemSpecial,
                "ComputerDatabasesRegistry"));

            buttons.Add(new PilotButtonData(
                "Usługi DS",
                "",
                "/XLPilot;component/Resources/Images/regedit.png",
                true,
                "",
                "Pokaż rejestr zawierający usługi Data Service",
                "",
                PilotButtonType.SystemSpecial,
                "DSServicesRegistry"));

            buttons.Add(new PilotButtonData(
                "Folder temp",
                "temp",
                "/XLPilot;component/Resources/Images/folder.png",
                false,
                "",
                "Pokaż folder %temp% danego użytkownika",
                "",
                PilotButtonType.SystemStandard));

            return buttons;
        }

        /// <summary>
        /// Saves Other buttons to the configuration
        /// </summary>
        private void SaveOtherPilotButtons()
        {
            try
            {
                // Create a list for the buttons
                var buttons = new List<PilotButtonData>();

                // Convert each button to PilotButtonData
                foreach (var item in OtherDragDropControl.ProjectItems)
                {
                    buttons.Add(new PilotButtonData(
                        item.ButtonText,
                        item.FileName,
                        item.ImageSource,
                        item.RunAsAdmin,
                        item.Arguments,
                        item.ToolTipText,
                        item.Directory,
                        item.ButtonType,
                        item.ActionIdentifier
                    ));
                }

                // Update the buttons in the configuration
                serializationManager.GetData().OtherPilotButtons = buttons;

                // Save all data
                serializationManager.SaveAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisywania przycisków 'inne': {ex.Message}",
                               "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Event handler for export button
        /// </summary>
        private void BtnExportSelected_Click(object sender, RoutedEventArgs e)
        {
            // Create a save file dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml",
                Title = "Zapisz ikony inne",
                FileName = "other_icons.xml"
            };

            // Show the dialog
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Export the buttons
                    serializationManager.SaveOtherPilotButtons(saveFileDialog.FileName);

                    // Show success message
                    MessageBox.Show("Eksport ikon 'inne' zakończony powodzeniem.",
                                   "Eksport", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    // Show error message
                    MessageBox.Show($"Błąd podczas eksportu ikon 'inne': {ex.Message}",
                                   "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Event handler for import button
        /// </summary>
        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            // Create an open file dialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml",
                Title = "Wybierz plik XML z ikonami 'inne'"
            };

            // Show the dialog
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Import the buttons
                    serializationManager.ImportOtherPilotButtons(openFileDialog.FileName);

                    // Reload the UI
                    LoadOtherPilotButtons();

                    // Show success message
                    MessageBox.Show("Import ikon 'inne' zakończony powodzeniem.",
                                   "Import", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    // Show error message
                    MessageBox.Show($"Błąd podczas importu ikon 'inne': {ex.Message}",
                                   "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}