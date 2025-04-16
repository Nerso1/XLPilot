using System;
using System.Collections.Generic;
using System.Linq;

namespace XLPilot.Models.Containers
{
    /// <summary>
    /// Container for a list of XLPaths objects
    /// Used for serializing and deserializing collections of paths
    /// </summary>
    [Serializable]
    public class XLPathsContainer
    {
        /// <summary>
        /// The list of XL paths
        /// </summary>
        public List<XLPaths> Items { get; set; } = new List<XLPaths>();

        /// <summary>
        /// Empty constructor needed for XML serialization
        /// </summary>
        public XLPathsContainer() { }

        /// <summary>
        /// Removes empty entries from the list
        /// </summary>
        public void FilterEmptyEntries()
        {
            // Create a new list with only non-empty entries
            var filteredList = new List<XLPaths>();

            foreach (var item in Items)
            {
                // Check if all fields are empty
                bool isEmpty = string.IsNullOrEmpty(item.Name) &&
                               string.IsNullOrEmpty(item.Path) &&
                               string.IsNullOrEmpty(item.Database) &&
                               string.IsNullOrEmpty(item.LicenseServer) &&
                               string.IsNullOrEmpty(item.LicenseKey);

                // Add to the filtered list if not empty
                if (!isEmpty)
                {
                    filteredList.Add(item);
                }
            }

            // Replace the original list with the filtered one
            Items = filteredList;
        }

        /// <summary>
        /// Removes entries with duplicate names
        /// </summary>
        public void RemoveDuplicates()
        {
            // Group by name and take the first item from each group
            var uniqueItems = new List<XLPaths>();
            var groupedByName = new Dictionary<string, XLPaths>();

            foreach (var item in Items)
            {
                // Skip items with no name
                if (string.IsNullOrEmpty(item.Name))
                    continue;

                // If we haven't seen this name before, add it to our dictionary
                if (!groupedByName.ContainsKey(item.Name))
                {
                    groupedByName[item.Name] = item;
                }
            }

            // Add all the unique items to our list
            foreach (var item in groupedByName.Values)
            {
                uniqueItems.Add(item);
            }

            // Replace the original list with the unique one
            Items = uniqueItems;
        }

        /// <summary>
        /// Adds a new entry to the container
        /// </summary>
        public void AddEntry(XLPaths path)
        {
            Items.Add(path);
        }

        /// <summary>
        /// Updates an existing entry by matching the Name property
        /// </summary>
        public bool UpdateEntry(XLPaths updatedPath)
        {
            // Find the existing path with the same name
            XLPaths existingPath = null;

            foreach (var item in Items)
            {
                if (item.Name == updatedPath.Name)
                {
                    existingPath = item;
                    break;
                }
            }

            // If found, update its properties
            if (existingPath != null)
            {
                existingPath.Path = updatedPath.Path;
                existingPath.Database = updatedPath.Database;
                existingPath.LicenseServer = updatedPath.LicenseServer;
                existingPath.LicenseKey = updatedPath.LicenseKey;
                return true;
            }

            // Not found
            return false;
        }

        /// <summary>
        /// Removes an entry from the container
        /// </summary>
        public bool RemoveEntry(XLPaths path)
        {
            return Items.Remove(path);
        }

        /// <summary>
        /// Removes an entry by name
        /// </summary>
        public bool RemoveEntry(string name)
        {
            // Find the entry with the given name
            XLPaths entryToRemove = null;

            foreach (var item in Items)
            {
                if (item.Name == name)
                {
                    entryToRemove = item;
                    break;
                }
            }

            // If found, remove it
            if (entryToRemove != null)
            {
                return Items.Remove(entryToRemove);
            }

            // Not found
            return false;
        }
    }
}