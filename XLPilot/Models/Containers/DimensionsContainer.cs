using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLPilot.Models.Containers
{
    [Serializable]
    public class DimensionsContainer
    {
        public List<int> Items { get; set; } = new List<int>();
        public DimensionsContainer() { }
    }

}
