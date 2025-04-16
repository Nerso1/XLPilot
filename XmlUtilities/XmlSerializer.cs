using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace XLPilot.XmlUtilities
{
    /// <summary>
    /// Helper class for saving and loading objects to/from XML files
    /// </summary>
    public static class XmlSerializer<T> where T : class
    {
        /// <summary>
        /// Saves an object to an XML file
        /// </summary>
        /// <param name="obj">The object to save</param>
        /// <param name="filePath">The file path to save to</param>
        public static void Serialize(T obj, string filePath)
        {
            try
            {
                // Create an XML serializer for the type
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

                // Open a file stream for writing
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Serialize the object to the file
                    serializer.Serialize(writer, obj);
                }

                // Success message could be uncommented for debugging
                // MessageBox.Show($"Saved successfully to {filePath}");
            }
            catch (Exception ex)
            {
                // Show error message if something went wrong
                MessageBox.Show($"Błąd podczas zapisywania: {ex.Message}");
                throw; // Re-throw the exception so the calling code knows something went wrong
            }
        }

        /// <summary>
        /// Loads an object from an XML file
        /// </summary>
        /// <param name="filePath">The file path to load from</param>
        /// <returns>The loaded object</returns>
        public static T Deserialize(string filePath)
        {
            try
            {
                // Create an XML serializer for the type
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

                // Open a file stream for reading
                using (StreamReader reader = new StreamReader(filePath))
                {
                    // Deserialize the object from the file
                    return (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                // Show error message if something went wrong
                MessageBox.Show($"Błąd podczas wczytywania: {ex.Message}");
                throw; // Re-throw the exception so the calling code knows something went wrong
            }
        }
    }
}