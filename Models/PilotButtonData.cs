using System;

namespace XLPilot.Models
{
    /// <summary>
    /// Data model for PilotButton controls - represents one button in the application
    /// </summary>
    [Serializable]
    public class PilotButtonData
    {
        // Properties that store the button's data
        public string ButtonText { get; set; }
        public string FileName { get; set; } = "";
        public string ImageSource { get; set; } = "";
        public bool RunAsAdmin { get; set; } = false;
        public string Arguments { get; set; } = "";
        public string ToolTipText { get; set; } = "";
        public string Directory { get; set; } = "";

        // Empty constructor needed for XML serialization
        public PilotButtonData() { }

        // Constructor with parameters to easily create a button
        public PilotButtonData(string buttonText, string fileName = "", string imageSource = "",
                              bool runAsAdmin = false, string arguments = "", string toolTipText = "",
                              string directory = "")
        {
            ButtonText = buttonText;
            FileName = fileName;
            ImageSource = imageSource;
            RunAsAdmin = runAsAdmin;
            Arguments = arguments;
            ToolTipText = toolTipText;
            Directory = directory;
        }

        /// <summary>
        /// Checks if two PilotButtonData objects have the same values
        /// </summary>
        public override bool Equals(object obj)
        {
            // If the object is null or not a PilotButtonData, they're not equal
            if (obj == null || !(obj is PilotButtonData))
                return false;

            // Cast the object to PilotButtonData
            PilotButtonData other = (PilotButtonData)obj;

            // Check if all properties are equal
            bool textEqual = ButtonText == other.ButtonText;
            bool fileNameEqual = FileName == other.FileName;
            bool imageEqual = ImageSource == other.ImageSource;
            bool adminEqual = RunAsAdmin == other.RunAsAdmin;
            bool argsEqual = Arguments == other.Arguments;
            bool tooltipEqual = ToolTipText == other.ToolTipText;
            bool dirEqual = Directory == other.Directory;

            // All properties must be equal for the objects to be equal
            return textEqual && fileNameEqual && imageEqual &&
                   adminEqual && argsEqual && tooltipEqual && dirEqual;
        }

        /// <summary>
        /// Creates a numeric code that represents this object
        /// Objects that are equal should have the same hash code
        /// </summary>
        public override int GetHashCode()
        {
            // Simple hash code that combines all properties
            int hash = 0;

            // Add hash codes for each property
            // The ?? operator means "if null, use 0 instead"
            if (ButtonText != null) hash += ButtonText.GetHashCode();
            if (FileName != null) hash += FileName.GetHashCode();
            if (ImageSource != null) hash += ImageSource.GetHashCode();
            hash += RunAsAdmin.GetHashCode();
            if (Arguments != null) hash += Arguments.GetHashCode();
            if (ToolTipText != null) hash += ToolTipText.GetHashCode();
            if (Directory != null) hash += Directory.GetHashCode();

            return hash;
        }
    }
}