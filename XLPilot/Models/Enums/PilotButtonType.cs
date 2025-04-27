using System;

namespace XLPilot.Models.Enums
{
    /// <summary>
    /// Type of PilotButton specifying its behavior and origin
    /// </summary>
    [Serializable]
    public enum PilotButtonType
    {
        /// <summary>
        /// User-created button with standard functionality
        /// </summary>
        UserStandard = 0,

        /// <summary>
        /// System-provided button with standard functionality
        /// </summary>
        SystemStandard = 1,

        /// <summary>
        /// System-provided button with special functionality
        /// </summary>
        SystemSpecial = 2
    }
}