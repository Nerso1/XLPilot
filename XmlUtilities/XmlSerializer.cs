using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                //Console.WriteLine($"Successfully serialized data to {filePath}");
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error serializing data: {ex.Message}");
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
                //Console.WriteLine($"Error deserializing data: {ex.Message}");
                throw;
            }
        }
    }

}
