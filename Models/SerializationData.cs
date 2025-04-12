using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLPilot.Models.Containers;

namespace XLPilot.Models
{
    /// <summary>
    /// Main class for holding all serializable data
    /// </summary>
    public class SerializationData
    {
        public List<XLPaths> XLPathsList { get; set; } = new List<XLPaths>();
        public List<string> XLIcons { get; set; } = new List<string>();
        public List<string> OtherIcons { get; set; } = new List<string>();
        public List<bool> Flags { get; set; } = new List<bool>() { false, false, false };
        public List<int> Dimensions { get; set; } = new List<int>() { 0, 0 }; // Width, Height

        public SerializationData() { }
    }


}
