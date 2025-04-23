using System;
using System.ComponentModel;

namespace XLPilot.Models
{
    /// <summary>
    /// Represents the path to an XL installation along with its configuration
    /// Implements INotifyPropertyChanged to support data binding
    /// </summary>
    [Serializable]
    public class XLPaths : INotifyPropertyChanged
    {
        // Private fields to store the property values
        private string name;
        private string path;
        private string database;
        private string licenseServer;
        private string licenseKey;

        // Public properties with property change notification
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    // Notify UI that the property has changed
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Path
        {
            get => path;
            set
            {
                if (path != value)
                {
                    path = value;
                    OnPropertyChanged(nameof(Path));
                }
            }
        }

        public string Database
        {
            get => database;
            set
            {
                if (database != value)
                {
                    database = value;
                    OnPropertyChanged(nameof(Database));
                }
            }
        }

        public string LicenseServer
        {
            get => licenseServer;
            set
            {
                if (licenseServer != value)
                {
                    licenseServer = value;
                    OnPropertyChanged(nameof(LicenseServer));
                }
            }
        }

        public string LicenseKey
        {
            get => licenseKey;
            set
            {
                if (licenseKey != value)
                {
                    licenseKey = value;
                    OnPropertyChanged(nameof(LicenseKey));
                }
            }
        }

        // Event that's raised when a property changes
        public event PropertyChangedEventHandler PropertyChanged;

        // Method to raise the PropertyChanged event
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Empty constructor needed for XML serialization
        public XLPaths() { }

        // Constructor with parameters
        public XLPaths(string name, string path, string database = null, string licenseServer = null, string licenseKey = null)
        {
            Name = name;
            Path = path;
            Database = database;
            LicenseServer = licenseServer;
            LicenseKey = licenseKey;
        }

        // Read-only property that formats license information for display
        public string FormattedLicenseInfo
        {
            get
            {
                // Format the license information based on what's available
                if (!string.IsNullOrEmpty(LicenseServer) && !string.IsNullOrEmpty(LicenseKey))
                {
                    return $"{LicenseServer}::{LicenseKey}";
                }
                else if (!string.IsNullOrEmpty(LicenseServer))
                {
                    return LicenseServer;
                }
                else if (!string.IsNullOrEmpty(LicenseKey))
                {
                    return LicenseKey;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}