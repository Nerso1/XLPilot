using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLPilot.Models.Containers
{
    /// <summary>
    /// Data container for XLPath for separate serialization
    /// </summary>
    [Serializable]
    public class XLPathsContainer
    {
        public List<XLPaths> Items { get; set; } = new List<XLPaths>();
        public XLPathsContainer() { }
    }

}
