using System;
using System.Globalization;
using System.Windows.Data;
using XLPilot.Models;

namespace XLPilot.UserControls
{
    /// <summary>
    /// This converter checks if a selected XLPaths item is not empty.
    /// It's used to enable/disable buttons when an item is selected.
    /// </summary>
    public class IsDefaultEmptyEntryConverter : IValueConverter
    {
        // This method converts from the source value to the target value
        // It returns true if the selected item is not empty (has name or path)
        // This is used to enable buttons when a real item is selected
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check if the value is an XLPaths object
            if (value is XLPaths selectedEntry)
            {
                // If either Name or Path has a value, it's not an empty entry
                bool isNotEmpty = !string.IsNullOrEmpty(selectedEntry.Name) ||
                                  !string.IsNullOrEmpty(selectedEntry.Path);

                // Return true to enable the button if it's not empty
                return isNotEmpty;
            }

            // If there's no selection or it's not an XLPaths, disable the button
            return false;
        }

        // This method is not used but required by the interface
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // We don't need to convert back for this case
            throw new NotImplementedException();
        }
    }
}