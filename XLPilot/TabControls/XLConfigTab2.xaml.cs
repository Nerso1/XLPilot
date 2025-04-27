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
        // First, let's add the event handler in the constructor
        // Add this code to your XLConfigTab2 constructor
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
        private void BtnClearOldXLPaths_Click(object sender, RoutedEventArgs e)
        {
            // Show a confirmation dialog
            MessageBoxResult result = MessageBox.Show(
                "Program sprawdzi wszystkie ścieżki XL i usunie te, które nie istnieją lub nie zawierają plików cdnxl.exe i rejestr.bat. Czy chcesz kontynuować?",
                "Czyszczenie nieważnych ścieżek",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Get the XL paths from the configuration
                    var xlPaths = serializationManager.GetData().XLPathsList;

                    // Count how many paths we had initially
                    int initialCount = xlPaths.Count;

                    // Create a list to store paths to remove
                    List<XLPaths> pathsToRemove = new List<XLPaths>();

                    // Check each path
                    foreach (var path in xlPaths)
                    {
                        // Skip paths with empty directory
                        if (string.IsNullOrEmpty(path.Path))
                            continue;

                        // First check if the directory exists
                        if (!Directory.Exists(path.Path))
                        {
                            // If directory doesn't exist, add to removal list
                            pathsToRemove.Add(path);
                            continue; // Skip checking files since directory doesn't exist
                        }

                        // Check if the required files exist
                        bool hasXlExe = File.Exists(Path.Combine(path.Path, "cdnxl.exe"));
                        bool hasRejestrBat = File.Exists(Path.Combine(path.Path, "rejestr.bat"));

                        // If either file is missing, add this path to the removal list
                        if (!hasXlExe || !hasRejestrBat)
                        {
                            pathsToRemove.Add(path);
                        }
                    }

                    // Remove the invalid paths
                    foreach (var pathToRemove in pathsToRemove)
                    {
                        serializationManager.RemoveXLPath(pathToRemove);
                    }

                    // Update the UI
                    LoadXLPaths();

                    // Calculate how many paths were removed
                    int removedCount = pathsToRemove.Count;

                    // Show result message
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
                    // Show error message if something went wrong
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
        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            // Show a confirmation dialog
            MessageBoxResult result = MessageBox.Show(
                "Program przeszuka komputer w poszukiwaniu instalacji Comarch ERP XL. Czy chcesz kontynuować?",
                "Szukanie ścieżek XL",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Search for XL installations
                    List<XLPaths> foundPaths = FindXLInstallations();

                    // If we found some installations
                    if (foundPaths.Count > 0)
                    {
                        // Filter out duplicates
                        int addedCount = AddNonDuplicatePaths(foundPaths);

                        // Update the UI
                        LoadXLPaths();

                        // Show success message
                        MessageBox.Show(
                            $"Znaleziono {foundPaths.Count} ścieżek instalacji, dodano {addedCount} nowych ścieżek.",
                            "Szukanie zakończone",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        // Show message if no installations were found
                        MessageBox.Show(
                            "Nie znaleziono instalacji Comarch ERP XL.",
                            "Szukanie zakończone",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    // Show error message if something went wrong
                    MessageBox.Show(
                        $"Wystąpił błąd podczas szukania instalacji: {ex.Message}",
                        "Błąd",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        // This method finds all XL installations based on the specified criteria
        private List<XLPaths> FindXLInstallations()
        {
            List<XLPaths> foundPaths = new List<XLPaths>();

            // Search in Program Files (x86) for folders containing "Comarch ERP XL"
            SearchInDirectory(@"C:\Program Files (x86)", "Comarch ERP XL", foundPaths, false);

            // Search in C:\ and Program Files (x86) for specific folder names
            string[] searchTerms = { "XL", "instalacje", "kompilacje", "Comarch", "Naprawiona", "Debug" };

            foreach (string term in searchTerms)
            {
                // Search in C:\ 
                SearchInDirectory(@"C:\", term, foundPaths, true);

                // Search in Program Files (x86)
                SearchInDirectory(@"C:\Program Files (x86)", term, foundPaths, true);
            }

            return foundPaths;
        }

        // This method searches for XL installations in a specific directory
        private void SearchInDirectory(string rootPath, string searchTerm, List<XLPaths> foundPaths, bool searchInSubfolders)
        {
            try
            {
                // Check if the root path exists
                if (!Directory.Exists(rootPath))
                    return;

                // Get all folders in the root path
                string[] directories = Directory.GetDirectories(rootPath);

                foreach (string directory in directories)
                {
                    // Get the folder name without path
                    string folderName = Path.GetFileName(directory);

                    // Check if the folder name contains the search term
                    if (folderName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        // Check if this folder contains cdnxl.exe and rejestr.bat
                        if (File.Exists(Path.Combine(directory, "cdnxl.exe")) &&
                            File.Exists(Path.Combine(directory, "rejestr.bat")))
                        {
                            // Add this path to the found paths
                            foundPaths.Add(new XLPaths(folderName, directory));
                        }

                        // If required, search in subfolders as well
                        if (searchInSubfolders)
                        {
                            SearchInSubdirectories(directory, foundPaths);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error or handle it as appropriate
                System.Diagnostics.Debug.WriteLine($"Error searching in {rootPath}: {ex.Message}");
            }
        }

        // This method searches for XL installations in all subdirectories of a directory
        private void SearchInSubdirectories(string parentDirectory, List<XLPaths> foundPaths)
        {
            try
            {
                // Get all subfolders
                string[] subdirectories = Directory.GetDirectories(parentDirectory);

                foreach (string subdirectory in subdirectories)
                {
                    // Get the folder name without path
                    string folderName = Path.GetFileName(subdirectory);

                    // Check if this subfolder contains cdnxl.exe and rejestr.bat
                    if (File.Exists(Path.Combine(subdirectory, "cdnxl.exe")) &&
                        File.Exists(Path.Combine(subdirectory, "rejestr.bat")))
                    {
                        // Add this path to the found paths
                        foundPaths.Add(new XLPaths(folderName, subdirectory));
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error or handle it as appropriate
                System.Diagnostics.Debug.WriteLine($"Error searching in subdirectories of {parentDirectory}: {ex.Message}");
            }
        }

        // This method adds non-duplicate paths to the configuration
        private int AddNonDuplicatePaths(List<XLPaths> newPaths)
        {
            int addedCount = 0;

            // Get existing paths from the configuration
            var existingPaths = serializationManager.GetData().XLPathsList;

            // Check each new path
            foreach (var newPath in newPaths)
            {
                // Check if this path is already in the configuration
                bool isDuplicate = false;

                foreach (var existingPath in existingPaths)
                {
                    // Compare paths (case-insensitive)
                    if (string.Equals(existingPath.Path, newPath.Path, StringComparison.OrdinalIgnoreCase))
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                // If this is not a duplicate, add it
                if (!isDuplicate)
                {
                    // Add the path to the configuration
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
                "",
                "Uruchom rejestr.bat",
                "",
                PilotButtonType.SystemStandard));

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

    }
}