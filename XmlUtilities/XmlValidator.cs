using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace XLPilot.XmlUtilities
{
    public static class XmlValidator
    {
        private static readonly char[] ForbiddenCharacters = new char[] { '<', '>', '&', '\'', '\"', '\u00A0' }; // Includes non-breaking space (&nbsp;)

        /// <summary>
        /// Validates the input for forbidden XML characters.
        /// </summary>
        /// <param name="input">User-provided text to validate.</param>
        /// <returns>True if the input is valid, otherwise false.</returns>
        public static bool ValidateInput(string input)
        {
            var invalidCharacters = input.Where(c => ForbiddenCharacters.Contains(c)).Distinct().ToArray();

            if (invalidCharacters.Any())
            {
                string message = "W treści: '" + input + "' wykryto znaki niedozwolone w plikach XML: \n" +
                                 string.Join(", ", invalidCharacters.Select(c => c == '\u00A0' ? "Spacja niełamiąca (\\u00A0)" : c.ToString())) +
                                 "\nProszę o usunięcie danych znaków.";
                MessageBox.Show(message, "Niedozwolony znak", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    //Example usage:
    //if (XmlValidator.ValidateInput(userInput))
    //{
    //    // Proceed with serialization
    //    MessageBox.Show("Input is valid. Proceeding with serialization.", "Validation Success", MessageBoxButton.OK, MessageBoxImage.Information);

    //    // Your XML serialization logic goes here
    //}
    //else
    //{
    //    // Validation failed; no further action needed
    //}

    }

}

