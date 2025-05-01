using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using XLPilot.UserControls;
using XLPilot.XmlUtilities;
using XLPilot.Models;
using XLPilot.Models.Containers;
using XLPilot.Services;
using XLPilot.Models.Enums;
//using WinForms = System.Windows.Forms;


namespace XLPilot.TabControls
{
    /// <summary>
    /// Tab for configuring XL paths and buttons
    /// </summary>
    public partial class XLConfigTab2 : UserControl
    {
        // The list of XL directories/paths
        private ObservableCollection<XLPaths> directories;

        // The list of XL buttons
        private ObservableCollection<PilotButtonData> xlPilotButtons;

        // Reference to the serialization manager
        private SerializationManager serializationManager => SerializationService.Manager;

        /// <summary>
        /// Constructor - initializes the tab
        /// </summary>
        public XLConfigTab2()
        {
            InitializeComponent();

            // Initialize collections
            directories = new ObservableCollection<XLPaths>();
            xlPilotButtons = new ObservableCollection<PilotButtonData>();

            // Load configuration from files
            LoadXLPaths();
            LoadRegistryEntries();
            LoadXLPilotButtons();

            // Set up drag and drop event handlers
            SetupDragDropEvents();

            BtnImport.Click += BtnImport_Click;
            BtnClearOldXLPaths.Click += BtnClearOldXLPaths_Click;
        }

        #region ClearOldXLPaths
        /// <summary>
        /// Event handler for the BtnClearOldXLPaths button click.
        /// Validates all XL paths to ensure they exist and contain required files.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">Event data</param>
        private void BtnClearOldXLPaths_Click(object sender, RoutedEventArgs e)
        {
            // Show a confirmation dialog before proceeding with the cleanup
            MessageBoxResult result = MessageBox.Show(
                "Program sprawdzi wszystkie ścieżki XL i usunie te, które nie istnieją lub nie zawierają plików cdnxl.exe i rejestr.bat. Czy chcesz kontynuować?",
                "Czyszczenie nieważnych ścieżek",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Retrieve all XL paths from the configuration
                    var xlPaths = serializationManager.GetData().XLPathsList;

                    // Store the initial count to track how many were removed
                    int initialCount = xlPaths.Count;

                    // Collection to store the paths that should be removed
                    List<XLPaths> pathsToRemove = new List<XLPaths>();

                    // Validate each path in the configuration
                    foreach (var path in xlPaths)
                    {
                        // Skip empty entries (no path specified)
                        if (string.IsNullOrEmpty(path.Path))
                            continue;

                        // First check if the directory exists on the file system
                        if (!Directory.Exists(path.Path))
                        {
                            // If directory doesn't exist, mark for removal
                            pathsToRemove.Add(path);
                            continue; // Skip file checking since directory doesn't exist
                        }

                        // Check if the required XL executable files exist in the directory
                        bool hasXlExe = File.Exists(Path.Combine(path.Path, "cdnxl.exe"));
                        bool hasRejestrBat = File.Exists(Path.Combine(path.Path, "rejestr.bat"));

                        // If either required file is missing, mark this path for removal
                        if (!hasXlExe || !hasRejestrBat)
                        {
                            pathsToRemove.Add(path);
                        }
                    }

                    // Remove all invalid paths from the configuration
                    foreach (var pathToRemove in pathsToRemove)
                    {
                        serializationManager.RemoveXLPath(pathToRemove);
                    }

                    // Refresh the UI to reflect the changes
                    LoadXLPaths();

                    // Calculate how many paths were removed
                    int removedCount = pathsToRemove.Count;

                    // Show appropriate feedback message based on results
                    if (removedCount > 0)
                    {
                        MessageBox.Show(
                            $"Usunięto {removedCount} nieprawidłowych ścieżek.",
                            "Czyszczenie zakończone",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Wszystkie ścieżki są prawidłowe. Nie usunięto żadnej ścieżki.",
                            "Czyszczenie zakończone",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    // Display error information if the cleanup process fails
                    MessageBox.Show(
                        $"Wystąpił błąd podczas czyszczenia ścieżek: {ex.Message}",
                        "Błąd",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region ImportXLPaths
        /// <summary>
        /// Event handler for the BtnImport button click.
        /// Automatically scans the computer for Comarch ERP XL installations.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">Event data</param>
        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            // Show a confirmation dialog before starting the search
            MessageBoxResult result = MessageBox.Show(
                "Program przeszuka komputer w poszukiwaniu instalacji Comarch ERP XL. Czy chcesz kontynuować?",
                "Szukanie ścieżek XL",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Start the search process for XL installations
                    List<XLPaths> foundPaths = FindXLInstallations();

                    // Process found installations if any were discovered
                    if (foundPaths.Count > 0)
                    {
                        // Filter out and add only non-duplicate paths
                        int addedCount = AddNonDuplicatePaths(foundPaths);

                        // Update the UI with the new paths
                        LoadXLPaths();

                        // Show a success message with count information
                        MessageBox.Show(
                            $"Znaleziono {foundPaths.Count} ścieżek instalacji, dodano {addedCount} nowych ścieżek.",
                            "Szukanie zakończone",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        // If no installations were found, inform the user
                        MessageBox.Show(
                            "Nie znaleziono instalacji Comarch ERP XL.",
                            "Szukanie zakończone",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    // Display error information if the search process fails
                    MessageBox.Show(
                        $"Wystąpił błąd podczas szukania instalacji: {ex.Message}",
                        "Błąd",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }


        /// <summary>
        /// Searches for Comarch ERP XL installations on the computer based on predefined criteria.
        /// Looks in standard installation locations and for folders with specific naming patterns.
        /// </summary>
        /// <returns>A list of XLPaths objects representing the found installations</returns>
        private List<XLPaths> FindXLInstallations()
        {
            // Collection to store all found XL installations
            List<XLPaths> foundPaths = new List<XLPaths>();

            // Search in Program Files (x86) for folders containing "Comarch ERP XL"
            SearchInDirectory(@"C:\Program Files (x86)", "Comarch ERP XL", foundPaths, false);

            // Define common naming patterns for XL installation folders
            string[] searchTerms = { "XL", "instalacje", "kompilacje", "Comarch", "Naprawiona", "Debug" };

            // Search for each term in both C:\ and Program Files (x86)
            foreach (string term in searchTerms)
            {
                // Search in C:\ root directory (with subfolder checking)
                SearchInDirectory(@"C:\", term, foundPaths, true);

                // Search in Program Files (x86) directory (with subfolder checking)
                SearchInDirectory(@"C:\Program Files (x86)", term, foundPaths, true);
            }

            return foundPaths;
        }

        /// <summary>
        /// Searches for XL installations in a specific directory based on a search term.
        /// </summary>
        /// <param name="rootPath">The root directory to search in</param>
        /// <param name="searchTerm">The term to match in folder names</param>
        /// <param name="foundPaths">Collection to store found XL paths</param>
        /// <param name="searchInSubfolders">Whether to also search in subfolders of matching folders</param>
        private void SearchInDirectory(string rootPath, string searchTerm, List<XLPaths> foundPaths, bool searchInSubfolders)
        {
            try
            {
                // Verify the root path exists before attempting to search
                if (!Directory.Exists(rootPath))
                    return;

                // Get all folders in the specified root path
                string[] directories = Directory.GetDirectories(rootPath);

                // Examine each directory for potential XL installations
                foreach (string directory in directories)
                {
                    // Extract just the folder name without the full path
                    string folderName = Path.GetFileName(directory);

                    // Check if the folder name contains the search term (case-insensitive)
                    if (folderName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        // Verify this folder contains the required XL executable files
                        if (File.Exists(Path.Combine(directory, "cdnxl.exe")) &&
                            File.Exists(Path.Combine(directory, "rejestr.bat")))
                        {
                            // Found a valid XL installation - add it to the results
                            foundPaths.Add(new XLPaths(folderName, directory));
                        }

                        // If requested, also check subfolders of matching folders
                        if (searchInSubfolders)
                        {
                            SearchInSubdirectories(directory, foundPaths);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the search process
                System.Diagnostics.Debug.WriteLine($"Error searching in {rootPath}: {ex.Message}");
            }
        }


        /// <summary>
        /// Searches for XL installations in all subdirectories of a given parent directory.
        /// </summary>
        /// <param name="parentDirectory">The parent directory to search within</param>
        /// <param name="foundPaths">Collection to store found XL paths</param>
        private void SearchInSubdirectories(string parentDirectory, List<XLPaths> foundPaths)
        {
            try
            {
                // Get all subdirectories of the parent directory
                string[] subdirectories = Directory.GetDirectories(parentDirectory);

                // Check each subdirectory for XL installation files
                foreach (string subdirectory in subdirectories)
                {
                    // Extract just the folder name without the full path
                    string folderName = Path.GetFileName(subdirectory);

                    // Check if this subfolder contains the required XL executable files
                    if (File.Exists(Path.Combine(subdirectory, "cdnxl.exe")) &&
                        File.Exists(Path.Combine(subdirectory, "rejestr.bat")))
                    {
                        // Found a valid XL installation - add it to the results
                        foundPaths.Add(new XLPaths(folderName, subdirectory));
                    }
                }
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the subdirectory search
                System.Diagnostics.Debug.WriteLine($"Error searching in subdirectories of {parentDirectory}: {ex.Message}");
            }
        }


        /// <summary>
        /// Adds non-duplicate paths to the configuration.
        /// Prevents adding the same installation path multiple times.
        /// </summary>
        /// <param name="newPaths">Collection of new paths to add</param>
        /// <returns>Number of paths actually added (excluding duplicates)</returns>
        private int AddNonDuplicatePaths(List<XLPaths> newPaths)
        {
            // Counter for tracking how many new paths were added
            int addedCount = 0;

            // Get the existing paths from the configuration
            var existingPaths = serializationManager.GetData().XLPathsList;

            // Check each new path against existing ones
            foreach (var newPath in newPaths)
            {
                // Flag to track if this path is already in the configuration
                bool isDuplicate = false;

                // Compare with all existing paths
                foreach (var existingPath in existingPaths)
                {
                    // Case-insensitive path comparison to avoid duplicates with different casing
                    if (string.Equals(existingPath.Path, newPath.Path, StringComparison.OrdinalIgnoreCase))
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                // If this is not a duplicate, add it to the configuration
                if (!isDuplicate)
                {
                    // Add the new path to the configuration
                    serializationManager.AddXLPath(newPath);
                    addedCount++;
                }
            }

            return addedCount;
        }

        #endregion

        /// <summary>
        /// Sets up event handlers for drag and drop operations
        /// </summary>
        private void SetupDragDropEvents()
        {
            // This event is raised when items are dropped
            XLDragDropControl.ItemsDropped += XLDragDropControl_ItemsDropped;
        }

        /// <summary>
        /// Event handler for when items are dropped
        /// </summary>
        private void XLDragDropControl_ItemsDropped(object sender, DragEventArgs e)
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
            SaveXLPilotButtons();
            UpdateXLPaths();
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
        /// Loads XL Paths from the configuration
        /// </summary>
        private void LoadXLPaths()
        {
            // Clear the current collection
            directories.Clear();

            // Add an empty row at the top
            directories.Add(new XLPaths(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

            try
            {
                // Get the XL paths from the configuration
                var xlPathsList = serializationManager.GetData().XLPathsList;

                // Add each path to the collection
                foreach (var xlPath in xlPathsList)
                {
                    directories.Add(xlPath);
                }
            }
            catch (Exception ex)
            {
                // Show error message if something went wrong
                MessageBox.Show($"Błąd podczas wczytywania ścieżek do XL-i: {ex.Message}",
                                "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Bind the collection to the ListView
            DirectoriesListView.ItemsSource = directories;
        }

        /// <summary>
        /// Loads XL buttons from the configuration
        /// </summary>
        private void LoadXLPilotButtons()
        {
            try
            {
                // Clear the current collection
                xlPilotButtons.Clear();

                // Collection for the drag and drop control
                var projectItems = new ObservableCollection<PilotButtonData>();

                // Get the buttons from the configuration
                var pilotButtons = serializationManager.GetData().XLPilotButtons;

                // Add default button if no buttons exist
                if (pilotButtons == null)
                {
                    // Add default sample button if no data exists at all
                    projectItems.Add(new PilotButtonData(
                        "XL Item 1",
                        "",
                        "/XLPilot;component/Resources/Images/detault-profile-picture.png"));
                }
                else
                {
                    // Convert and add each button to the collection
                    foreach (var button in pilotButtons)
                    {
                        projectItems.Add(button);
                    }
                }

                // Set up template buttons for the toolbox
                var toolboxItems = CreateXLTemplateButtons();


                // Assign the collections to the control
                XLDragDropControl.ProjectItems = projectItems;
                XLDragDropControl.ToolboxItems = toolboxItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas wczytywania przycisków XL: {ex.Message}",
                              "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Creates template buttons for the XL toolbox
        /// </summary>
        private ObservableCollection<PilotButtonData> CreateXLTemplateButtons()
        {
            var buttons = new ObservableCollection<PilotButtonData>();

            // Add existing buttons with new button types and action identifiers
            buttons.Add(new PilotButtonData(
                "XL",
                "cdnxl.exe",
                "/XLPilot;component/Resources/Images/cdnxl.png",
                false,
                "",
                "Uruchom Comarch ERP XL",
                "",
                PilotButtonType.SystemStandard));

            buttons.Add(new PilotButtonData(
                "XL admin",
                "cdnxl.exe",
                "/XLPilot;component/Resources/Images/cdnxl.png",
                true,
                "",
                "Uruchom Comarch ERP XL jako administrator",
                "",
                PilotButtonType.SystemStandard));

            buttons.Add(new PilotButtonData(
                "Rejestr.bat",
                "rejestr.bat",
                "/XLPilot;component/Resources/Images/cmd.png",
                true,
                "skip",
                "Uruchom rejestr.bat",
                "",
                PilotButtonType.SystemSpecial,
                "RejestrBat"));

            buttons.Add(new PilotButtonData(
                "Zmienna Path",
                "",
                "/XLPilot;component/Resources/Images/env_var.png",
                true,
                "",
                "Ustaw zmienną środowiskową Path do danego XL-a górze",
                "",
                PilotButtonType.SystemSpecial,
                "ChangeEnvVariable"));

            buttons.Add(new PilotButtonData(
                "Folder XL",
                "",
                "/XLPilot;component/Resources/Images/folder.png",
                false,
                "",
                "Otwórz folder z Comarch ERP XL",
                "",
                PilotButtonType.SystemStandard));

            return buttons;
        }


        /// <summary>
        /// Saves XL buttons to the configuration
        /// </summary>
        private void SaveXLPilotButtons()
        {
            try
            {
                // Create a list for the buttons
                var buttons = new List<PilotButtonData>();

                // Convert each button to PilotButtonData
                foreach (var item in XLDragDropControl.ProjectItems)
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
                serializationManager.GetData().XLPilotButtons = buttons;

                // Save all data
                serializationManager.SaveAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisywania przycisków XL: {ex.Message}",
                               "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Saves XL paths to the configuration
        /// </summary>
        private void UpdateXLPaths()
        {
            try
            {
                // Create a list for the paths
                var paths = new List<XLPaths>();

                // Add each non-empty path to the list
                foreach (var dir in directories)
                {
                    // Skip the empty row
                    if (string.IsNullOrEmpty(dir.Name) &&
                        string.IsNullOrEmpty(dir.Path) &&
                        string.IsNullOrEmpty(dir.Database) &&
                        string.IsNullOrEmpty(dir.LicenseServer) &&
                        string.IsNullOrEmpty(dir.LicenseKey))
                    {
                        continue;
                    }

                    // Add the path to the list
                    paths.Add(dir);
                }

                // Update the paths in the configuration
                serializationManager.GetData().XLPathsList = paths;

                // Save all data
                serializationManager.SaveAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas zapisywania konfiguracji: {ex.Message}",
                               "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Event handler for Add button
        /// </summary>
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Get the path from the text box
            string path = txtXLPath.Text.Trim();

            // Check if the path is valid
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                // Get the name from the text box or use the folder name
                string folderName = txtXLName.Text.Trim();
                if (string.IsNullOrEmpty(folderName))
                {
                    folderName = System.IO.Path.GetFileName(path);
                }

                // Get the other values
                string databaseName = cmbXLDatabase.SelectedItem as string ?? string.Empty;
                string licenseServer = txtXLLicenseServer.Text.Trim();
                string licenseKey = txtXLLicenseKey.Text.Trim();

                // Validate the values
                if (XmlValidator.ValidateInput(path) &&
                    XmlValidator.ValidateInput(folderName) &&
                    XmlValidator.ValidateInput(databaseName) &&
                    XmlValidator.ValidateInput(licenseServer) &&
                    XmlValidator.ValidateInput(licenseKey))
                {
                    // Create a new XL path
                    var newPath = new XLPaths(folderName, path, databaseName, licenseServer, licenseKey);

                    // Add it to the collection
                    directories.Add(newPath);

                    // Save changes
                    UpdateXLPaths();

                    // Show success message
                    MessageBox.Show("Ścieżka dodana prawidłowo.", "Sukces",
                                    MessageBoxButton.OK, MessageBoxImage.Information);

                    // Clear the input fields
                    txtXLPath.Clear();
                    txtXLName.Clear();
                    cmbXLDatabase.SelectedIndex = -1;
                    txtXLLicenseServer.Clear();
                    txtXLLicenseKey.Clear();
                }
            }
            else
            {
                MessageBox.Show("Niewłaściwa ścieżka. Proszę, upewnij się że ścieżka jest prawidłowa.",
                                "Niewłaściwa ścieżka", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Event handler for Edit button
        /// </summary>
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Check if an item is selected
            if (DirectoriesListView.SelectedItem != null)
            {
                // Get the selected item
                var selectedEntry = (XLPaths)DirectoriesListView.SelectedItem;

                // Skip the empty row
                if (string.IsNullOrEmpty(selectedEntry.Name) && string.IsNullOrEmpty(selectedEntry.Path))
                {
                    return;
                }

                // Get the values from the UI
                string newFolderName = txtXLName.Text.Trim();
                string newPath = txtXLPath.Text.Trim();
                string newDatabaseName = cmbXLDatabase.SelectedItem as string;
                string newLicenseServer = txtXLLicenseServer.Text.Trim();
                string newLicenseKey = txtXLLicenseKey.Text.Trim();

                // Check if the path is valid
                if (!string.IsNullOrEmpty(newPath) && Directory.Exists(newPath))
                {
                    // Validate the values
                    if (XmlValidator.ValidateInput(newPath) &&
                        XmlValidator.ValidateInput(newFolderName) &&
                        XmlValidator.ValidateInput(newDatabaseName) &&
                        XmlValidator.ValidateInput(newLicenseServer) &&
                        XmlValidator.ValidateInput(newLicenseKey))
                    {
                        // Update the selected entry
                        selectedEntry.Name = newFolderName;
                        selectedEntry.Path = newPath;
                        selectedEntry.Database = newDatabaseName;
                        selectedEntry.LicenseServer = newLicenseServer;
                        selectedEntry.LicenseKey = newLicenseKey;

                        // Save changes
                        UpdateXLPaths();

                        // Show success message
                        MessageBox.Show("Wybrana ścieżka została prawidłowo zaktualizowana.",
                                       "Edycja zakończona powdzeniem",
                                       MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Niewłaściwa ścieżka: jest pusta, lub nie istnieje. Proszę podać prawidłową ścieżkę.",
                                   "Niewłaściwa ścieżka", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Nie wybrano ścieżki do edycji. Zaznacz śceiżkę którą chcesz edytować.",
                               "Nie wybrano ścieżki", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Event handler for Delete button
        /// </summary>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Check if an item is selected
            if (DirectoriesListView.SelectedItem != null)
            {
                // Get the selected item
                var selectedEntry = (XLPaths)DirectoriesListView.SelectedItem;

                // Skip the empty row
                if (string.IsNullOrEmpty(selectedEntry.Name) && string.IsNullOrEmpty(selectedEntry.Path))
                {
                    return;
                }

                // Remove the selected item
                directories.Remove(selectedEntry);

                // Save changes
                UpdateXLPaths();

                // Show success message
                MessageBox.Show("Zaznaczona ścieżką została usunięta prawidłowo.",
                               "Usuwanie zakończone powodzeniem",
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Brak zaznaczonej ścieżki do usunięcia. Zaznacz śceiżkę którą chcesz usunąć.",
                               "Nie wybrano ścieżki", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #region Load registry entries to combo box cmbXLDatabase
        /// <summary>
        /// Loads database entries from the registry to the combobox
        /// </summary>
        private void LoadRegistryEntries()
        {
            // Registry paths where database entries might be found
            string userRegistryPath = @"SOFTWARE\CDN\CDNXL\MSSQL\Bazy";
            string machineRegistryPath = @"SOFTWARE\WOW6432Node\CDN\CDNXL\MSSQL\Bazy";

            // Dictionary to store the registry entries
            Dictionary<string, string> registryEntries = new Dictionary<string, string>();

            // Read from HKEY_CURRENT_USER
            ReadRegistryEntries(Registry.CurrentUser, userRegistryPath, registryEntries);

            // Read from HKEY_LOCAL_MACHINE
            ReadRegistryEntries(Registry.LocalMachine, machineRegistryPath, registryEntries);

            // List for the ComboBox items
            var comboBoxItems = new List<string>();

            // Add an empty value at the top
            comboBoxItems.Add(string.Empty);

            // Add the registry keys
            foreach (string key in registryEntries.Keys)
            {
                comboBoxItems.Add(key);
            }

            // Set the ComboBox items
            cmbXLDatabase.ItemsSource = comboBoxItems;
        }

        /// <summary>
        /// Reads registry entries from a specific key
        /// </summary>
        private void ReadRegistryEntries(RegistryKey baseKey, string subKeyPath, Dictionary<string, string> entries)
        {
            try
            {
                // Open the registry key
                using (RegistryKey subKey = baseKey.OpenSubKey(subKeyPath))
                {
                    // Check if the key exists
                    if (subKey != null)
                    {
                        // Get all the value names
                        string[] valueNames = subKey.GetValueNames();

                        // Process each value
                        foreach (string valueName in valueNames)
                        {
                            // Skip empty names
                            if (!string.IsNullOrEmpty(valueName))
                            {
                                // Get the value data
                                object valueData = subKey.GetValue(valueName);

                                // If data exists, add it to the dictionary
                                if (valueData != null)
                                {
                                    // Format the value data for display
                                    string formattedValueData = FormatValueData(valueData.ToString());

                                    // Add to the dictionary
                                    entries[valueName] = formattedValueData;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Ignore registry access errors
                // This can happen if the registry key doesn't exist or we don't have permission
            }
        }

        /// <summary>
        /// Formats the registry value data for display
        /// </summary>
        private string FormatValueData(string valueData)
        {
            // Replace "NET:" with "baza: "
            valueData = valueData.Replace("NET:", "baza: ");

            // Replace the first comma with ", serwer: "
            int firstCommaIndex = valueData.IndexOf(',');
            if (firstCommaIndex != -1)
            {
                string firstPart = valueData.Substring(0, firstCommaIndex);
                string secondPart = valueData.Substring(firstCommaIndex + 1);
                valueData = firstPart + ", serwer: " + secondPart;
            }

            // Remove everything after the second comma
            int secondCommaIndex = valueData.IndexOf(',', firstCommaIndex + 1);
            if (secondCommaIndex != -1)
            {
                valueData = valueData.Substring(0, secondCommaIndex);
            }

            return valueData;
        }
        #endregion

        /// <summary>
        /// Event handler for ListView selection changed
        /// </summary>
        private void DirectoriesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if an item is selected
            if (DirectoriesListView.SelectedItem != null)
            {
                // Get the selected item
                var selectedEntry = (XLPaths)DirectoriesListView.SelectedItem;

                // If the empty row is selected, clear the fields
                if (string.IsNullOrEmpty(selectedEntry.Name) && string.IsNullOrEmpty(selectedEntry.Path))
                {
                    txtXLName.Clear();
                    txtXLPath.Clear();
                    cmbXLDatabase.SelectedIndex = -1;
                    txtXLLicenseServer.Clear();
                    txtXLLicenseKey.Clear();
                }
                else
                {
                    // Fill the fields with the selected entry's data
                    txtXLName.Text = selectedEntry.Name;
                    txtXLPath.Text = selectedEntry.Path;

                    // Find the database in the combobox
                    if (string.IsNullOrEmpty(selectedEntry.Database))
                    {
                        cmbXLDatabase.SelectedIndex = -1;
                    }
                    else
                    {
                        cmbXLDatabase.SelectedItem = selectedEntry.Database;
                    }

                    // Set the license server and key
                    txtXLLicenseServer.Text = selectedEntry.LicenseServer ?? string.Empty;
                    txtXLLicenseKey.Text = selectedEntry.LicenseKey ?? string.Empty;
                }
            }
        }

        /// <summary>
        /// Event handler for Browse Path button
        /// Opens a folder browser dialog to select the XL path
        /// </summary>
        private void BtnBrowsePath_Click(object sender, RoutedEventArgs e)
        {
            // Create the dialog
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                // Configure to select folders, not files
                CheckFileExists = false,
                FileName = "Folder Selection",
                Title = "Wybierz ścieżkę do instalacji Comarch ERP XL",
                ValidateNames = false
            };

            // If there's already a path, start from that directory
            if (!string.IsNullOrEmpty(txtXLPath.Text) && System.IO.Directory.Exists(txtXLPath.Text))
            {
                dialog.InitialDirectory = txtXLPath.Text;
            }

            // Show the dialog
            if (dialog.ShowDialog() == true)
            {
                // Get the selected folder (extract folder path from the selected "file")
                string folderPath = System.IO.Path.GetDirectoryName(dialog.FileName);
                txtXLPath.Text = folderPath;

                // Extract the folder name from the path and set it to txtXLName if it's empty
                string folderName = System.IO.Path.GetFileName(folderPath);

                // Check if the name field is empty or we're adding a new entry
                //if (string.IsNullOrEmpty(txtXLName.Text) || DirectoriesListView.SelectedItem == null ||
                //    (DirectoriesListView.SelectedItem is XLPaths selectedPath && string.IsNullOrEmpty(selectedPath.Name)))
                {
                    txtXLName.Text = folderName;
                }
            }
        }
    }
    }