using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace XLPilot.UserControls
{
    public partial class PilotButtonMovable : UserControl
    {
        public PilotButtonMovable()
        {
            InitializeComponent();

            // We need to prevent the PilotButton from handling mouse events that should
            // be used for drag and drop in the parent ListView
            this.PreviewMouseLeftButtonDown += PilotButtonMovable_PreviewMouseLeftButtonDown;
        }

        private void PilotButtonMovable_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Mark this event as unhandled so it bubbles up to the ListView
            // This is crucial for drag operations to work
            e.Handled = false;
        }

        // ImageSource Dependency Property
        public string ImageSource
        {
            get => (string)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                nameof(ImageSource),
                typeof(string),
                typeof(PilotButtonMovable),
                new PropertyMetadata("/XLPilot;component/Resources/Images/detault-profile-picture.png"));

        // ButtonText Dependency Property
        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(
                nameof(ButtonText),
                typeof(string),
                typeof(PilotButtonMovable),
                new PropertyMetadata(string.Empty));
    }
}