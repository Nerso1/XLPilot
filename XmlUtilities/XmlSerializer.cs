using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace XLPilot.XmlUtilities
{
    /// <summary>
    /// Static utility class for serialization and deserialization operations
    /// </summary>
    public static class XmlSerializer<T> where T : class
    {
        /// <summary>
        /// Serializes object to XML file
        /// </summary>
        public static void Serialize(T obj, string filePath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    serializer.Serialize(writer, obj);
                }
                //Console.WriteLine($"Serializacja zakończona powodzeniem {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas serializacji: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deserializes XML file to object
        /// </summary>
        public static T Deserialize(string filePath)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StreamReader reader = new StreamReader(filePath))
                {
                    return (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas deserializacji: {ex.Message}");
                throw;
            }
        }
    }

}
