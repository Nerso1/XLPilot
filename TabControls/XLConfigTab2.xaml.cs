using System;
using System.Collections.Generic;
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
        }

        // If you need these properties in this class, register them with XLConfigTab2 as the owner
    //    public string ImageSource
    //    {
    //        get => (string)GetValue(ImageSourceProperty);
    //        set => SetValue(ImageSourceProperty, value);
    //    }
    //    public static readonly DependencyProperty ImageSourceProperty =
    //        DependencyProperty.Register(
    //            nameof(ImageSource),
    //            typeof(string),
    //            typeof(XLConfigTab2), // Use XLConfigTab2 as the owner, not PilotButton
    //            new PropertyMetadata("/XLPilot;component/Resources/Images/default-profile-picture.png"));

    //    public string ButtonText
    //    {
    //        get => (string)GetValue(ButtonTextProperty);
    //        set => SetValue(ButtonTextProperty, value);
    //    }
    //    public static readonly DependencyProperty ButtonTextProperty =
    //        DependencyProperty.Register(
    //            nameof(ButtonText),
    //            typeof(string),
    //            typeof(XLConfigTab2), // Use XLConfigTab2 as the owner, not PilotButton
    //            new PropertyMetadata(string.Empty));
    }



}
