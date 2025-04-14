using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace XLPilot.UserControls
{
    public partial class PilotButton : UserControl
    {
        private Button internalButton;

        public PilotButton()
        {
            InitializeComponent();

            // Find the internal Button control after initialization
            this.Loaded += (s, e) =>
            {
                internalButton = this.FindName("InternalButton") as Button;
                if (internalButton == null)
                {
                    // If you don't have a named button, try to find it in the template
                    internalButton = FindChild<Button>(this);
                }

                if (internalButton != null)
                {
                    internalButton.Click += InternalButton_Click;
                }
            };
        }

        private void InternalButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Clicked");
            RunExecutable(Directory, FileName, RunAsAdmin, Arguments);
        }

        // Helper method to find a child control of a specific type
        private static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T result)
                {
                    return result;
                }

                var foundChild = FindChild<T>(child);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }

            return null;
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
                typeof(PilotButton),
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
                typeof(PilotButton),
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
                typeof(PilotButton),
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
                typeof(PilotButton),
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
                typeof(PilotButton),
                new PropertyMetadata(null));

        // ToolTipText Dependency Property
        public string ToolTipText
        {
            get => (string)GetValue(ToolTipTextProperty);
            set => SetValue(ToolTipTextProperty, value);
        }

        public static readonly DependencyProperty ToolTipTextProperty =
            DependencyProperty.Register(
                nameof(ToolTipText),
                typeof(string),
                typeof(PilotButton),
                new PropertyMetadata(string.Empty));


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
                typeof(PilotButton),
                new PropertyMetadata(string.Empty));

        // Method to run the executable when clicked
        private void RunExecutable(string directory, string fileName, bool runAsAdmin, string arguments = null)
        {
            string filePath = Path.Combine(directory, fileName);
            if (File.Exists(filePath))
            {
                try
                {
                    ProcessStartInfo processInfo = new ProcessStartInfo
                    {
                        FileName = filePath,
                        WorkingDirectory = directory,
                        UseShellExecute = true,
                        Arguments = arguments // Pass the arguments here
                    };
                    // Only set the "runas" verb if runAsAdmin is true
                    if (runAsAdmin)
                    {
                        processInfo.Verb = "runas"; // Run as admin
                    }
                    Process.Start(processInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nie udało się uruchomić {fileName}: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show($"Nie odnaleziono pliku w ścieżce {fileName}: {directory}");
            }
        }
    }
}