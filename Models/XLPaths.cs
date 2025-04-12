using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLPilot.Models
{
    /// <summary>
    /// XLPaths class as provided
    /// </summary>
    [Serializable]

        public class XLPaths : INotifyPropertyChanged
        {
            private string name;
            private string path;
            private string database;
            private string licenseServer;
            private string licenseKey;

            public string Name
            {
                get => name;
                set
                {
                    if (name != value)
                    {
                        name = value;
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

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }


            // Parameterless constructor needed for XML serialization
            public XLPaths() { }

        public XLPaths(string name, string path, string database = null, string licenseServer = null, string licenseKey = null)
        {
            Name = name;
            Path = path;
            Database = database;
            LicenseServer = licenseServer;
            LicenseKey = licenseKey;
        }
        // Read-only property to format LicenseServer and LicenseKey
        public string FormattedLicenseInfo
        {
            get
            {
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
