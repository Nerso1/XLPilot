using System;
using System.Collections.Generic;
using XLPilot.Models.Containers;

namespace XLPilot.Models
{
    /// <summary>
    /// Main class for holding all serializable data
    /// This is the root object that gets saved to the configuration file
    /// </summary>
    [Serializable]
    public class SerializationData
    {
        /// <summary>
        /// List of XL Paths (installations)
        /// </summary>
        public List<XLPaths> XLPathsList { get; set; } = new List<XLPaths>();

        /// <summary>
        /// XL Icons - PilotButtonData objects for the XL tab
        /// </summary>
        public List<PilotButtonData> XLPilotButtons { get; set; } = new List<PilotButtonData>();

        /// <summary>
        /// Other Icons - PilotButtonData objects for the Other tab
        /// </summary>
        public List<PilotButtonData> OtherPilotButtons { get; set; } = new List<PilotButtonData>();

        /// <summary>
        /// Configuration flags for application settings
        /// [0] - Whether "other" icons are visible on XL tab
        /// [1] - Whether to remember window size
        /// [2] - Reserved for future use
        /// </summary>
        public List<bool> Flags { get; set; } = new List<bool>() { false, false, false };

        /// <summary>
        /// Window dimensions - [0] = Width, [1] = Height
        /// </summary>
        public List<int> Dimensions { get; set; } = new List<int>() { 0, 0 };

        /// <summary>
        /// Empty constructor needed for XML serialization
        /// </summary>
        public SerializationData() { }
    }
}