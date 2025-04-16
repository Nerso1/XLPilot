using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using XLPilot.Models.Containers;
using XLPilot.Models;

namespace XLPilot.XmlUtilities
{
    /// <summary>
    /// Main manager class for handling data serialization operations
    /// Handles saving and loading all application data
    /// </summary>
    public class SerializationManager
    {
        // The data object that contains all serialized information
        private SerializationData _data;

        // The file path where the data is stored
        private readonly string _mainFilePath;

        /// <summary>
        /// Creates a new SerializationManager
        /// </summary>
        /// <param name="mainFilePath">The file path to use for saving/loading data</param>
        public SerializationManager(string mainFilePath)
        {
            _mainFilePath = mainFilePath;

            // If the file exists, load data from it, otherwise create new data
            if (File.Exists(mainFilePath))
            {
                _data = XmlSerializer<SerializationData>.Deserialize(mainFilePath);
            }
            else
            {
                _data = new SerializationData();
            }
        }

        /// <summary>
        /// Reloads data from the file (useful when another part of the app might have changed it)
        /// </summary>
        public void ReloadFromFile()
        {
            if (File.Exists(_mainFilePath))
            {
                _data = XmlSerializer<SerializationData>.Deserialize(_mainFilePath);
            }
        }

        /// <summary>
        /// Gets the current data
        /// </summary>
        public SerializationData GetData()
        {
            return _data;
        }

        /// <summary>
        /// Saves all data to the file
        /// </summary>
        public void SaveAllData()
        {
            XmlSerializer<SerializationData>.Serialize(_data, _mainFilePath);
        }

        #region XLPaths Operations
        /// <summary>
        /// Saves XL Paths to a separate XML file
        /// </summary>
        public void SaveXLPaths(string filePath)
        {
            try
            {
                // Create a container with the current XL paths
                var container = new XLPathsContainer { Items = _data.XLPathsList };

                // Remove empty entries
                container.FilterEmptyEntries();

                // Save to the file
                XmlSerializer<XLPathsContainer>.Serialize(container, filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving XL paths: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Loads XL Paths from a separate XML file and replaces current paths
        /// </summary>
        public void LoadXLPaths(string filePath)
        {
            try
            {
                // Load the container from the file
                var container = XmlSerializer<XLPathsContainer>.Deserialize(filePath);

                // Update the paths in our data
                _data.XLPathsList = container.Items;

                // Save the updated data
                SaveAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading XL paths: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Imports XL Paths from a separate XML file and merges them with current paths
        /// </summary>
        public void ImportXLPaths(string filePath)
        {
            try
            {
                // Load the container from the file
                var container = XmlSerializer<XLPathsContainer>.Deserialize(filePath);

                // Add the imported paths to our existing paths
                _data.XLPathsList.AddRange(container.Items);

                // Remove duplicate entries
                _data.XLPathsList = _data.XLPathsList.GroupBy(x => x.Name)
                                                      .Select(g => g.First())
                                                      .ToList();

                // Save the updated data
                SaveAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing XL paths: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Gets all XL paths as a container
        /// </summary>
        public XLPathsContainer GetXLPathsContainer()
        {
            return new XLPathsContainer { Items = _data.XLPathsList };
        }

        /// <summary>
        /// Updates the XLPaths in the main configuration
        /// </summary>
        public void UpdateXLPaths(XLPathsContainer container)
        {
            try
            {
                // Remove empty entries
                container.FilterEmptyEntries();

                // Update the XL paths in our data
                _data.XLPathsList = container.Items;

                // Save the updated data
                SaveAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating XL paths: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Adds a new XL path to the configuration
        /// </summary>
        public void AddXLPath(XLPaths path)
        {
            try
            {
                // Add the path to our paths list
                _data.XLPathsList.Add(path);

                // Save the updated data
                SaveAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding XL path: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Updates an existing XL path in the configuration
        /// </summary>
        public bool UpdateXLPath(XLPaths path)
        {
            try
            {
                // Find the existing path with the same name
                var existingPath = _data.XLPathsList.FirstOrDefault(p => p.Name == path.Name);

                if (existingPath != null)
                {
                    // Update the properties of the existing path
                    existingPath.Path = path.Path;
                    existingPath.Database = path.Database;
                    existingPath.LicenseServer = path.LicenseServer;
                    existingPath.LicenseKey = path.LicenseKey;

                    // Save the updated data
                    SaveAllData();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating XL path: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Removes an XL path from the configuration
        /// </summary>
        public bool RemoveXLPath(XLPaths path)
        {
            try
            {
                // Find the path in our list
                bool removed = _data.XLPathsList.Remove(path);

                if (removed)
                {
                    // Save the updated data
                    SaveAllData();
                }

                return removed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing XL path: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
        #endregion

        #region PilotButtons Operations - XL Icons
        /// <summary>
        /// Saves XL PilotButtons to a separate XML file
        /// </summary>
        public void SaveXLPilotButtons(string filePath)
        {
            try
            {
                // Create a container with the current XL buttons
                var container = new PilotButtonsContainer { Items = _data.XLPilotButtons };

                // Save to the file
                XmlSerializer<PilotButtonsContainer>.Serialize(container, filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving XL icons: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Loads XL PilotButtons from a separate XML file
        /// </summary>
        public void LoadXLPilotButtons(string filePath)
        {
            try
            {
                // Load the container from the file
                var container = XmlSerializer<PilotButtonsContainer>.Deserialize(filePath);

                // Update the buttons in our data
                _data.XLPilotButtons = container.Items;

                // Save the updated data
                SaveAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading XL icons: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Imports XL PilotButtons from a separate XML file
        /// </summary>
        public void ImportXLPilotButtons(string filePath)
        {
            try
            {
                // Load the container from the file
                var importedContainer = XmlSerializer<PilotButtonsContainer>.Deserialize(filePath);

                // Add the imported buttons to our existing buttons
                _data.XLPilotButtons.AddRange(importedContainer.Items);

                // Remove duplicate buttons
                _data.XLPilotButtons = _data.XLPilotButtons.Distinct().ToList();

                // Save the updated data
                SaveAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing XL icons: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Gets the XL PilotButtons as a container
        /// </summary>
        public PilotButtonsContainer GetXLPilotButtonsContainer()
        {
            return new PilotButtonsContainer { Items = _data.XLPilotButtons };
        }

        /// <summary>
        /// Updates the XL PilotButtons in the main configuration
        /// </summary>
        public void UpdateXLPilotButtons(PilotButtonsContainer container)
        {
            try
            {
                // Update the buttons in our data
                _data.XLPilotButtons = container.Items;

                // Save the updated data
                SaveAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating XL icons: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Updates the XL PilotButtons from an ObservableCollection
        /// </summary>
        public void UpdateXLPilotButtons(ObservableCollection<PilotButtonData> buttons)
        {
            try
            {
                // Convert the ObservableCollection to a List and update our data
                _data.XLPilotButtons = new List<PilotButtonData>(buttons);

                // Save the updated data
                SaveAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating XL icons: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
        #endregion

        #region PilotButtons Operations - Other Icons
        /// <summary>
        /// Saves Other PilotButtons to a separate XML file
        /// </summary>
        public void SaveOtherPilotButtons(string filePath)
        {
            try
            {
                // Create a container with the current Other buttons
                var container = new PilotButtonsContainer { Items = _data.OtherPilotButtons };

                // Save to the file
                XmlSerializer<PilotButtonsContainer>.Serialize(container, filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving Other icons: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Loads Other PilotButtons from a separate XML file
        /// </summary>
        public void LoadOtherPilotButtons(string filePath)
        {
            try
            {
                // Load the container from the file
                var container = XmlSerializer<PilotButtonsContainer>.Deserialize(filePath);

                // Update the buttons in our data
                _data.OtherPilotButtons = container.Items;

                // Save the updated data
                SaveAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Other icons: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Imports Other PilotButtons from a separate XML file
        /// </summary>
        public void ImportOtherPilotButtons(string filePath)
        {
            try
            {
                // Load the container from the file
                var importedContainer = XmlSerializer<PilotButtonsContainer>.Deserialize(filePath);

                // Add the imported buttons to our existing buttons
                _data.OtherPilotButtons.AddRange(importedContainer.Items);

                // Remove duplicate buttons
                _data.OtherPilotButtons = _data.OtherPilotButtons.Distinct().ToList();

                // Save the updated data
                SaveAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing Other icons: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Gets the Other PilotButtons as a container
        /// </summary>
        public PilotButtonsContainer GetOtherPilotButtonsContainer()
        {
            return new PilotButtonsContainer { Items = _data.OtherPilotButtons };
        }

        /// <summary>
        /// Updates the Other PilotButtons in the main configuration
        /// </summary>
        public void UpdateOtherPilotButtons(PilotButtonsContainer container)
        {
            try
            {
                // Update the buttons in our data
                _data.OtherPilotButtons = container.Items;

                // Save the updated data
                SaveAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating Other icons: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Updates the Other PilotButtons from an ObservableCollection
        /// </summary>
        public void UpdateOtherPilotButtons(ObservableCollection<PilotButtonData> buttons)
        {
            try
            {
                // Convert the ObservableCollection to a List and update our data
                _data.OtherPilotButtons = new List<PilotButtonData>(buttons);

                // Save the updated data
                SaveAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating Other icons: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
        #endregion

        #region Flags Operations
        /// <summary>
        /// Saves flags to a separate XML file
        /// </summary>
        public void SaveFlags(string filePath)
        {
            var container = new FlagsContainer { Items = _data.Flags };
            XmlSerializer<FlagsContainer>.Serialize(container, filePath);
        }

        /// <summary>
        /// Loads flags from a separate XML file
        /// </summary>
        public void LoadFlags(string filePath)
        {
            // Load the container from the file
            var container = XmlSerializer<FlagsContainer>.Deserialize(filePath);

            // Update our flags
            _data.Flags = container.Items;

            // Make sure we always have exactly 3 flags
            while (_data.Flags.Count < 3)
                _data.Flags.Add(false);

            if (_data.Flags.Count > 3)
                _data.Flags = _data.Flags.Take(3).ToList();
        }
        #endregion

        #region Dimensions Operations
        /// <summary>
        /// Saves window dimensions to a separate XML file
        /// </summary>
        public void SaveDimensions(string filePath)
        {
            var container = new DimensionsContainer { Items = _data.Dimensions };
            XmlSerializer<DimensionsContainer>.Serialize(container, filePath);
        }

        /// <summary>
        /// Loads window dimensions from a separate XML file
        /// </summary>
        public void LoadDimensions(string filePath)
        {
            // Load the container from the file
            var container = XmlSerializer<DimensionsContainer>.Deserialize(filePath);

            // Update our dimensions
            _data.Dimensions = container.Items;

            // Make sure we always have exactly 2 dimensions (width, height)
            while (_data.Dimensions.Count < 2)
                _data.Dimensions.Add(0);

            if (_data.Dimensions.Count > 2)
                _data.Dimensions = _data.Dimensions.Take(2).ToList();
        }

        /// <summary>
        /// Sets the window dimensions
        /// </summary>
        public void SetDimensions(int width, int height)
        {
            // Make sure we have space for the dimensions
            while (_data.Dimensions.Count < 2)
                _data.Dimensions.Add(0);

            // Set the dimensions
            _data.Dimensions[0] = width;
            _data.Dimensions[1] = height;
        }

        /// <summary>
        /// Gets the window dimensions
        /// </summary>
        public (int width, int height) GetDimensions()
        {
            // Make sure we have space for the dimensions
            while (_data.Dimensions.Count < 2)
                _data.Dimensions.Add(0);

            // Return the dimensions
            return (_data.Dimensions[0], _data.Dimensions[1]);
        }
        #endregion
    }
}