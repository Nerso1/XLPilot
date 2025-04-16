using System;
using System.Collections.Generic;
using System.Linq;
using XLPilot.Models;

namespace XLPilot.Models.Containers
{
    /// <summary>
    /// Container for a list of PilotButtonData objects
    /// Used for serializing and deserializing collections of buttons
    /// </summary>
    [Serializable]
    public class PilotButtonsContainer
    {
        /// <summary>
        /// The list of buttons
        /// </summary>
        public List<PilotButtonData> Items { get; set; } = new List<PilotButtonData>();

        /// <summary>
        /// Empty constructor needed for XML serialization
        /// </summary>
        public PilotButtonsContainer() { }

        /// <summary>
        /// Removes empty entries from the list
        /// </summary>
        public void FilterEmptyEntries()
        {
            // Create a new list with only non-empty entries
            var filteredList = new List<PilotButtonData>();

            foreach (var item in Items)
            {
                // Check if essential fields are empty
                bool isEmpty = string.IsNullOrEmpty(item.ButtonText) &&
                               string.IsNullOrEmpty(item.FileName) &&
                               string.IsNullOrEmpty(item.ImageSource);

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
        /// Removes duplicate entries
        /// </summary>
        public void RemoveDuplicates()
        {
            // Create a new list for unique items
            var uniqueItems = new List<PilotButtonData>();

            // Keep track of which items we've seen
            var seenItems = new HashSet<string>();

            foreach (var item in Items)
            {
                // Create a key that represents this item
                string key = item.ButtonText + "|" + item.FileName + "|" + item.ImageSource;

                // If we haven't seen this item before, add it
                if (!seenItems.Contains(key))
                {
                    seenItems.Add(key);
                    uniqueItems.Add(item);
                }
            }

            // Replace the original list with the unique one
            Items = uniqueItems;
        }

        /// <summary>
        /// Adds a new button to the container
        /// </summary>
        public void AddButton(PilotButtonData button)
        {
            if (button != null)
            {
                Items.Add(button);
            }
        }

        /// <summary>
        /// Adds multiple buttons to the container
        /// </summary>
        public void AddRange(IEnumerable<PilotButtonData> buttons)
        {
            if (buttons != null)
            {
                foreach (var button in buttons)
                {
                    Items.Add(button);
                }
            }
        }

        /// <summary>
        /// Removes a button from the container
        /// </summary>
        public bool RemoveButton(PilotButtonData button)
        {
            return Items.Remove(button);
        }

        /// <summary>
        /// Removes all buttons from the container
        /// </summary>
        public void Clear()
        {
            Items.Clear();
        }

        /// <summary>
        /// Gets the number of buttons in the container
        /// </summary>
        public int Count
        {
            get { return Items.Count; }
        }

        /// <summary>
        /// Updates the order of the buttons
        /// </summary>
        public void UpdateOrder(List<PilotButtonData> newOrder)
        {
            if (newOrder != null)
            {
                // Create a new list with the new order
                var orderedList = new List<PilotButtonData>();

                foreach (var item in newOrder)
                {
                    orderedList.Add(item);
                }

                // Replace the original list with the ordered one
                Items = orderedList;
            }
        }
    }
}