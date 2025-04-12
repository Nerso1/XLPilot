using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLPilot.Models.Containers
{
    [Serializable]
    public class OtherIconsContainer
    {
        public List<string> Items { get; set; } = new List<string>();
        public OtherIconsContainer() { }
    }

}
