using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLPilot.Models.Containers
{
    [Serializable]
    public class FlagsContainer
    {
        public List<bool> Items { get; set; } = new List<bool>();
        public FlagsContainer() { }
    }

}
