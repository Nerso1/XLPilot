using System;
using System.Globalization;
using System.Windows.Data;

namespace XLPilot.UserControls
{
    /// <summary>
    /// Converter that checks if a string is empty or null
    /// Used to control when certain UI elements appear
    /// </summary>
    public class StringEmptyConverter : IValueConverter
    {
        /// <summary>
        /// Converts a string to a boolean - returns true if the string is empty or null
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check if the value is null or an empty string
            string strValue = value as string;
            return string.IsNullOrEmpty(strValue);
        }

        /// <summary>
        /// This method is not used but required by the interface
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // We don't need to convert back for this converter
            throw new NotImplementedException();
        }
    }
}