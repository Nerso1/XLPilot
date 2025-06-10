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
        /// Gets all XL paths as a container
        /// </summary>
        public XLPathsContainer GetXLPathsContainer()
        {
            return new XLPathsContainer { Items = _data.XLPathsList };
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
        #endregion
    }
}