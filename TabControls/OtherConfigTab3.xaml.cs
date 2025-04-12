using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using XLPilot.UserControls;

namespace XLPilot.TabControls
{
    /// <summary>
    /// Interaction logic for OtherConfigTab3.xaml
    /// </summary>
    public partial class OtherConfigTab3 : UserControl
    {
        public OtherConfigTab3()
        {
            InitializeComponent();


            OtherDragDropControl.ToolboxItems = new ObservableCollection<WrapPanel_DragAndDrop.PilotButtonData>
            {
                new WrapPanel_DragAndDrop.PilotButtonData("/XLPilot;component/Resources/Images/detault-profile-picture.png", "Other Item 1"),
                new WrapPanel_DragAndDrop.PilotButtonData("/XLPilot;component/Resources/Images/Google chrome icon.png", "Other Item 2")
            };

            OtherDragDropControl.ProjectItems = new ObservableCollection<WrapPanel_DragAndDrop.PilotButtonData>
            {
                new WrapPanel_DragAndDrop.PilotButtonData("/XLPilot;component/Resources/Images/detault-profile-picture.png", "Other Project 1")
            };
        }
    }
}
