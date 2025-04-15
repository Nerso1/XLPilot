using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using WrapPanel_DragAndDrop;
using static XLPilot.UserControls.WrapPanel_DragAndDrop;

namespace XLPilot.Models.Containers
{
    /// <summary>
    /// Container class for serializing and deserializing lists of PilotButtonData
    /// </summary>
    public class PilotButtonDataContainer
    {
        public List<PilotButtonData> Items { get; set; } = new List<PilotButtonData>();
    }
}