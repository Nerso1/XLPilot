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

        // FileName Dependency Property
        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }

        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register(
                nameof(FileName),
                typeof(string),
                typeof(PilotButtonMovable),
                new PropertyMetadata(string.Empty));

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

        // RunAsAdmin Dependency Property
        public bool RunAsAdmin
        {
            get => (bool)GetValue(RunAsAdminProperty);
            set => SetValue(RunAsAdminProperty, value);
        }

        public static readonly DependencyProperty RunAsAdminProperty =
            DependencyProperty.Register(
                nameof(RunAsAdmin),
                typeof(bool),
                typeof(PilotButtonMovable),
                new PropertyMetadata(false));

        // Arguments Dependency Property
        public string Arguments
        {
            get => (string)GetValue(ArgumentsProperty);
            set => SetValue(ArgumentsProperty, value);
        }

        public static readonly DependencyProperty ArgumentsProperty =
            DependencyProperty.Register(
                nameof(Arguments),
                typeof(string),
                typeof(PilotButtonMovable),
                new PropertyMetadata(null));

        // Directory Dependency Property
        public string Directory
        {
            get => (string)GetValue(DirectoryProperty);
            set => SetValue(DirectoryProperty, value);
        }

        public static readonly DependencyProperty DirectoryProperty =
            DependencyProperty.Register(
                nameof(Directory),
                typeof(string),
                typeof(PilotButtonMovable),
                new PropertyMetadata(string.Empty));
    }
}