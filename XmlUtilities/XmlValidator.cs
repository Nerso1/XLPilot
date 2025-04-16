using System.Linq;
using System.Windows;

namespace XLPilot.XmlUtilities
{
    /// <summary>
    /// Helper class to validate strings before saving them to XML
    /// </summary>
    public static class XmlValidator
    {
        // These characters are not allowed in XML
        private static readonly char[] ForbiddenCharacters = new char[] {
            '<', '>', '&', '\'', '\"', '\u00A0' // \u00A0 is a non-breaking space
        };

        /// <summary>
        /// Checks if a string contains any characters that are not allowed in XML
        /// </summary>
        /// <param name="input">The string to check</param>
        /// <returns>True if the string is valid (has no forbidden characters)</returns>
        public static bool ValidateInput(string input)
        {
            // If input is null or empty, it's valid
            if (string.IsNullOrEmpty(input))
                return true;

            // Find any forbidden characters in the input
            char[] badCharsFound = input.Where(c => ForbiddenCharacters.Contains(c))
                                        .Distinct() // Remove duplicates
                                        .ToArray();

            // If we found any forbidden characters
            if (badCharsFound.Length > 0)
            {
                // Build a message listing the forbidden characters
                string charList = string.Join(", ", badCharsFound.Select(c => {
                    if (c == '\u00A0')
                        return "Spacja niełamiąca";
                    else
                        return c.ToString();
                }));

                // Show a warning message
                string message = $"Treść '{input}' zawiera znaki niedozwolone w XML: \n" +
                                 $"{charList}\n" +
                                 "Proszę, usuń te znaki.";

                MessageBox.Show(message, "Niedozwolone znaki", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // No forbidden characters found - the input is valid
            return true;
        }
    }
}