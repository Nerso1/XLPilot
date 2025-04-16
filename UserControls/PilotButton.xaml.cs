using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace XLPilot.UserControls
{
    /// <summary>
    /// User control for a clickable button that can launch an application
    /// </summary>
    public partial class PilotButton : UserControl
    {
        // The internal button control
        private Button internalButton;

        public PilotButton()
        {
            InitializeComponent();

            // Wait until the control is loaded before finding the button
            this.Loaded += (s, e) =>
            {
                // Try to find the button by name
                internalButton = this.FindName("InternalButton") as Button;

                // If not found by name, try to find it in the visual tree
                if (internalButton == null)
                {
                    internalButton = FindChildControl<Button>(this);
                }

                // Set up the click event handler
                if (internalButton != null)
                {
                    internalButton.Click += InternalButton_Click;
                }
            };
        }

        // This is called when the button is clicked
        private void InternalButton_Click(object sender, RoutedEventArgs e)
        {
            // Show a message (for debugging)
            MessageBox.Show("Clicked");

            // Run the specified executable
            RunExecutable(Directory, FileName, RunAsAdmin, Arguments);
        }

        // Helper method to find a child control of a specific type
        private static T FindChildControl<T>(DependencyObject parent) where T : DependencyObject
        {
            // Get the number of child elements
            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            // Check each child
            for (int i = 0; i < childCount; i++)
            {
                // Get the current child
                var child = VisualTreeHelper.GetChild(parent, i);

                // If this child is the type we're looking for, return it
                if (child is T result)
                {
                    return result;
                }

                // Otherwise, search this child's children
                var foundChild = FindChildControl<T>(child);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }

            // If we get here, we didn't find anything
            return null;
        }

        #region Dependency Properties
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
        #endregion

        /// <summary>
        /// Runs the specified executable with optional arguments
        /// </summary>
        /// <param name="directory">The directory containing the executable</param>
        /// <param name="fileName">The name of the executable file</param>
        /// <param name="runAsAdmin">Whether to run as administrator</param>
        /// <param name="arguments">Command line arguments to pass to the executable</param>
        private void RunExecutable(string directory, string fileName, bool runAsAdmin, string arguments = null)
        {
            // Build the full path to the executable
            string filePath = Path.Combine(directory, fileName);

            // Check if the file exists
            if (File.Exists(filePath))
            {
                try
                {
                    // Set up the process information
                    ProcessStartInfo processInfo = new ProcessStartInfo
                    {
                        FileName = filePath,
                        WorkingDirectory = directory,
                        UseShellExecute = true,
                        Arguments = arguments
                    };

                    // Set the "runas" verb if we need to run as administrator
                    if (runAsAdmin)
                    {
                        processInfo.Verb = "runas";
                    }

                    // Start the process
                    Process.Start(processInfo);
                }
                catch (Exception ex)
                {
                    // Show an error message if the process couldn't be started
                    MessageBox.Show($"Failed to run {fileName}: {ex.Message}");
                }
            }
            else
            {
                // Show an error message if the file doesn't exist
                MessageBox.Show($"File not found at {fileName}: {directory}");
            }
        }
    }
}