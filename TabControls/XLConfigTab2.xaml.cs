using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using XLPilot.UserControls;
using XLPilot.XmlUtilities;
using XLPilot.Models;
using System.Globalization;

namespace XLPilot.TabControls
{
    /// <summary>
    /// Interaction logic for XLConfigTab2.xaml
    /// </summary>
    public partial class XLConfigTab2 : UserControl
    {
        private ObservableCollection<XLPaths> directories;


        public XLConfigTab2()
        {
            InitializeComponent();
            directories = new ObservableCollection<XLPaths>();
            LoadDirectories();
            LoadRegistryEntries(); // Load registry entries into the ComboBox


            XLDragDropControl.ToolboxItems = new ObservableCollection<WrapPanel_DragAndDrop.PilotButtonData>
            {
                //droppedItem.ButtonText,
                //droppedItem.FileName,
                //droppedItem.ImageSource,
                //droppedItem.RunAsAdmin,
                //droppedItem.Arguments,
                //droppedItem.ToolTip,
                //droppedItem.Directory
                new WrapPanel_DragAndDrop.PilotButtonData("XL Item 1", "", "/XLPilot;component/Resources/Images/detault-profile-picture.png", true, "", "Przycisk typu fajny", "aaa"),
                new WrapPanel_DragAndDrop.PilotButtonData("XL Item 1", "", "/XLPilot;component/Resources/Images/Google chrome icon.png")
            };

            XLDragDropControl.ProjectItems = new ObservableCollection<WrapPanel_DragAndDrop.PilotButtonData>
            {
                new WrapPanel_DragAndDrop.PilotButtonData("XL Item 1", "", "/XLPilot;component/Resources/Images/detault-profile-picture.png"),
            };

        }

        private void LoadDirectories()
        {
            string mainDataFile = "config.xml";

            // Clear the current directories collection
            directories.Clear();

            // Add an empty row at the top of the ObservableCollection
            directories.Add(new XLPaths(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));

            try
            {
                // Check if the configuration file exists
                if (File.Exists(mainDataFile))
                {
                    // Create a serialization manager to load the data
                    var manager = new SerializationManager(mainDataFile);

                    // Get the deserialized data
                    var loadedData = manager.GetData();

                    // Add all the XLPaths from the deserialized data to the directories collection
                    foreach (var xlPath in loadedData.XLPathsList)
                    {
                        directories.Add(xlPath);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that might occur during deserialization
                MessageBox.Show($"Błąd podczas wczytywania ścieżek do XL-i: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Bind the ObservableCollection to the ListView
            DirectoriesListView.ItemsSource = directories;


        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string path = txtXLPath.Text.Trim();
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {

                string folderName = txtXLName.Text.Trim();
                if (string.IsNullOrEmpty(folderName))
                {
                    folderName = System.IO.Path.GetFileName(path);
                }

                string databaseName = cmbXLDatabase.SelectedItem as string ?? string.Empty;
                string licenseServer = txtXLLicenseServer.Text.Trim();
                string licenseKey = txtXLLicenseKey.Text.Trim();

                if (XmlValidator.ValidateInput(path) && XmlValidator.ValidateInput(folderName) && XmlValidator.ValidateInput(databaseName) 
                    && XmlValidator.ValidateInput(licenseServer) && XmlValidator.ValidateInput(licenseKey))
                {
                    var newPath = new XLPaths(folderName, path, databaseName, licenseServer, licenseKey);

                    // Add the new path to the ObservableCollection
                    directories.Add(newPath);

                    // Provide feedback to the user
                    MessageBox.Show("Ścieżka dodana prawidłowo.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Clear the input fields
                    txtXLPath.Clear();
                    txtXLName.Clear();
                    cmbXLDatabase.SelectedIndex = -1;
                    txtXLLicenseServer.Clear();
                    txtXLLicenseKey.Clear();

                    SaveDirectories();
                }

            }
            else
            {
                MessageBox.Show("Niewłaściwa ścieżka. Proszę, upewnij się że ścieżka jest prawidłowa.", "Niewłaściwa ścieżka", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DirectoriesListView.SelectedItem != null)
            {
                var selectedEntry = (XLPaths)DirectoriesListView.SelectedItem;

                // Skip the empty row
                if (string.IsNullOrEmpty(selectedEntry.Name) && string.IsNullOrEmpty(selectedEntry.Path))
                {
                    return;
                }

                string newFolderName = txtXLName.Text.Trim();
                string newPath = txtXLPath.Text.Trim();
                string newDatabaseName = cmbXLDatabase.SelectedItem as string;
                string newLicenseServer = txtXLLicenseServer.Text.Trim();
                string newLicenseKey = txtXLLicenseKey.Text.Trim();

                if (!string.IsNullOrEmpty(newPath) && Directory.Exists(newPath))
                {
                    if (XmlValidator.ValidateInput(newPath) && XmlValidator.ValidateInput(newFolderName) && XmlValidator.ValidateInput(newDatabaseName)
                        && XmlValidator.ValidateInput(newLicenseServer) && XmlValidator.ValidateInput(newLicenseKey))
                    {

                        // Update the selected entry
                        selectedEntry.Name = newFolderName;
                        selectedEntry.Path = newPath;
                        selectedEntry.Database = newDatabaseName;
                        selectedEntry.LicenseServer = newLicenseServer;
                        selectedEntry.LicenseKey = newLicenseKey;

                        MessageBox.Show("Wybrana ścieżka została prawidłowo zaktualizowana.", "Edycja zakończona powdzeniem", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Clear the input fields
                        //txtXLName.Clear();
                        //txtXLPath.Clear();
                        //cmbXLDatabase.SelectedIndex = -1;
                        //txtXLLicenseServer.Clear();
                        //txtXLLicenseKey.Clear();
                    }

                }
                else
                {
                    MessageBox.Show("Niewłaściwa ścieżka: jest pusta, lub nie istnieje. Proszę podać prawidłową ścieżkę.", "Niewłaściwa ścieżka", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Nie wybrano ścieżki do edycji. Zaznacz śceiżkę którą chcesz edytować.", "Nie wybrano ścieżki", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DirectoriesListView.SelectedItem != null)
            {
                var selectedEntry = (XLPaths)DirectoriesListView.SelectedItem;

                if (directories.Contains(selectedEntry))
                {
                    directories.Remove(selectedEntry);

                    MessageBox.Show("Zaznaczona ścieżką została usunięta prawidłowo.", "Usuwanie zakończone powodzeniem", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Zaznaczona ścieżka nie mogłą być znaleziona na liście.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Brak zaznaczonej ścieżki do usunięcia. Zaznacz śceiżkę którą chcesz usunąć.", "Nie wybrano ścieżki", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        #region Load registry entries to combo box cmbXLDatabase
        private void LoadRegistryEntries()
        {
            // Define the registry paths
            string userRegistryPath = @"SOFTWARE\CDN\CDNXL\MSSQL\Bazy";
            string machineRegistryPath = @"SOFTWARE\WOW6432Node\CDN\CDNXL\MSSQL\Bazy";

            // Create a dictionary to store the entries
            Dictionary<string, string> registryEntries = new Dictionary<string, string>();

            // Read from HKEY_CURRENT_USER
            ReadRegistryEntries(Registry.CurrentUser, userRegistryPath, registryEntries);

            // Read from HKEY_LOCAL_MACHINE
            ReadRegistryEntries(Registry.LocalMachine, machineRegistryPath, registryEntries);

            // Add an empty value at the top of the ComboBox
            var comboBoxItems = new List<string> { string.Empty }; // Add empty value
            comboBoxItems.AddRange(registryEntries.Keys); // Add registry keys

            // Populate the ComboBox
            cmbXLDatabase.ItemsSource = comboBoxItems;
        }
        private void ReadRegistryEntries(RegistryKey baseKey, string subKeyPath, Dictionary<string, string> entries)
        {
            try
            {
                // Open the registry key
                using (RegistryKey subKey = baseKey.OpenSubKey(subKeyPath))
                {
                    if (subKey != null)
                    {
                        // Get all value names (entries) under the key
                        string[] valueNames = subKey.GetValueNames();

                        // Process each value
                        foreach (string valueName in valueNames)
                        {
                            if (!string.IsNullOrEmpty(valueName))
                            {
                                // Get the value data
                                object valueData = subKey.GetValue(valueName);
                                if (valueData != null)
                                {
                                    // Format the value data
                                    string formattedValueData = FormatValueData(valueData.ToString());

                                    // Add the entry to the dictionary
                                    entries[valueName] = formattedValueData;
                                }
                            }
                        }
                    }
                    else
                    {
                        //Console.WriteLine($"Nie znaleziono klucza rejestru: {baseKey.Name}\\{subKeyPath}");
                    }
                }
            }
            catch /*(Exception ex)*/
            {
                //Console.WriteLine($"Błąd przy wczytywaniu klucza rejestru {baseKey.Name}\\{subKeyPath}: {ex.Message}");
            }
        }
        private string FormatValueData(string valueData)
        {
            // Replace "NET:" with "baza: "
            valueData = valueData.Replace("NET:", "baza: ");

            // Replace the first comma with ", serwer: "
            int firstCommaIndex = valueData.IndexOf(',');
            if (firstCommaIndex != -1)
            {
                valueData = valueData.Substring(0, firstCommaIndex) + ", serwer: " + valueData.Substring(firstCommaIndex + 1);
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

        // Pre-fill the text boxes when a directory is selected
        private void DirectoriesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (DirectoriesListView.SelectedItem != null)
            {
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
                    // Populate the fields with the selected entry's data
                    txtXLName.Text = selectedEntry.Name;
                    txtXLPath.Text = selectedEntry.Path;
                    cmbXLDatabase.SelectedItem = selectedEntry.Database ?? string.Empty; // Set to empty if null
                    txtXLLicenseServer.Text = selectedEntry.LicenseServer ?? string.Empty;
                    txtXLLicenseKey.Text = selectedEntry.LicenseKey ?? string.Empty;
                }
            }

        }
        private void SaveDirectories()
        {
            string mainDataFile = "config.xml";

            // Create a serialization manager
            var manager = new SerializationManager(mainDataFile);

            // Get a filtered copy of directories without the empty row
            var filteredDirectories = directories
                .Where(d => !(string.IsNullOrEmpty(d.Name) &&
                             string.IsNullOrEmpty(d.Path) &&
                             string.IsNullOrEmpty(d.Database) &&
                             string.IsNullOrEmpty(d.LicenseServer) &&
                             string.IsNullOrEmpty(d.LicenseKey)))
                .ToList();

            // Update the manager's data with the filtered directories
            manager.GetData().XLPathsList = filteredDirectories;

            // Save all data
            manager.SaveAllData();
        }

    }

}
