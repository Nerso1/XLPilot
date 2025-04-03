using System.Windows;
using System.Windows.Controls;

namespace XLPilot.UserControls
{
    public partial class PilotButton : UserControl
    {
        public PilotButton()
        {
            InitializeComponent();
        }


        // ImageSource Dependency Property (keep the existing implementation)
        public string ImageSource
        {
            get => (string)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                nameof(ImageSource),
                typeof(string),
                typeof(PilotButton),
                new PropertyMetadata("/XLPilot;component/Resources/Images/detault-profile-picture.png"));

        // ButtonText Dependency Property (keep the existing implementation)
        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(
                nameof(ButtonText),
                typeof(string),
                typeof(PilotButton),
                new PropertyMetadata(string.Empty));
    }

}