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
using XLPilot.UserControls;

namespace XLPilot.TabControls
{
    /// <summary>
    /// Interaction logic for XLConfigTab2.xaml
    /// </summary>
    public partial class XLConfigTab2 : UserControl
    {
        public XLConfigTab2()
        {
            InitializeComponent();

            XLDragDropControl.ToolboxItems = new ObservableCollection<WrapPanel_DragAndDrop.PilotButtonData>
            {
                new WrapPanel_DragAndDrop.PilotButtonData("/XLPilot;component/Resources/Images/detault-profile-picture.png", "XL Item 1"),
                new WrapPanel_DragAndDrop.PilotButtonData("/XLPilot;component/Resources/Images/Google chrome icon.png", "XL Item 2")
            };

            XLDragDropControl.ProjectItems = new ObservableCollection<WrapPanel_DragAndDrop.PilotButtonData>
            {
                new WrapPanel_DragAndDrop.PilotButtonData("/XLPilot;component/Resources/Images/detault-profile-picture.png", "XL Project 1")
            };

        }


    }



}
