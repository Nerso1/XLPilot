using XLPilot.XmlUtilities;

namespace XLPilot.Services
{
    /// <summary>
    /// Static service class that provides access to a single SerializationManager
    /// This ensures we use the same manager throughout the application
    /// </summary>
    public static class SerializationService
    {
        // The name of the config file where data is stored
        private static readonly string CONFIG_FILE = "config.xml";

        // The shared SerializationManager instance
        private static SerializationManager _manager;

        /// <summary>
        /// Gets the shared SerializationManager instance
        /// Creates it if it doesn't exist yet
        /// </summary>
        public static SerializationManager Manager
        {
            get
            {
                // If the manager hasn't been created yet, create it
                if (_manager == null)
                {
                    _manager = new SerializationManager(CONFIG_FILE);
                }

                // Return the manager
                return _manager;
            }
        }
    }
}