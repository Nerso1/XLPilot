using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace XLPilot.UserControls
{
    /// <summary>
    /// User control for a movable button that can be dragged and dropped
    /// </summary>
    public partial class PilotButtonMovable : UserControl
    {
        // Flag to track if we've already fixed the control
        private bool isFixed = false;

        public PilotButtonMovable()
        {
            InitializeComponent();

            // This event handler is important for drag-and-drop to work
            this.PreviewMouseLeftButtonDown += PilotButtonMovable_PreviewMouseLeftButtonDown;

            // Add a Loaded event handler to set up the shield visibility
            this.Loaded += PilotButtonMovable_Loaded;

            // Add handlers for visibility changes to handle tab switching
            this.IsVisibleChanged += PilotButtonMovable_IsVisibleChanged;
        }

        /// <summary>
        /// When the control visibility changes (e.g., when switching tabs)
        /// </summary>
        private void PilotButtonMovable_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // When the control becomes visible again, update the admin shield
            if ((bool)e.NewValue == true)
            {
                UpdateAdminShieldVisibility();
            }
        }

        /// <summary>
        /// When the control is loaded, update the admin shield visibility
        /// </summary>
        private void PilotButtonMovable_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Initial visibility update
                UpdateAdminShieldVisibility();

                // As a failsafe, try again after a short delay to ensure everything is loaded
                System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        // Try one more time after a delay
                        UpdateAdminShieldVisibility();

                        // Fallback if direct property doesn't work
                        if (!isFixed && this.DataContext is XLPilot.Models.PilotButtonData buttonData)
                        {
                            // Get RunAsAdmin directly from the data context
                            bool isAdmin = buttonData.RunAsAdmin;

                            // Set visibility based on the value from the data context
                            if (AdminShieldImage != null)
                            {
                                AdminShieldImage.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
                                isFixed = true;
                            }
                        }
                    }),
                    System.Windows.Threading.DispatcherPriority.Loaded);
            }
            catch (Exception)
            {
                // Silently handle any errors
            }
        }

        /// <summary>
        /// When the mouse button is pressed, we need to make sure the event
        /// bubbles up to the parent ListView for drag operations to work
        /// </summary>
        private void PilotButtonMovable_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Mark the event as unhandled so it continues to bubble up
            e.Handled = false;
        }

        /// <summary>
        /// Updates the admin shield visibility based on RunAsAdmin property
        /// </summary>
        private void UpdateAdminShieldVisibility()
        {
            try
            {
                // This is the simplest and most direct approach
                if (AdminShieldImage != null)
                {
                    // TEMPORARY FIX: Use the file name as a hint - some buttons are always set to run as admin
                    bool shouldBeAdmin = false;

                    // If RunAsAdmin is already true, respect that
                    if (RunAsAdmin)
                    {
                        shouldBeAdmin = true;
                    }
                    // Try to get RunAsAdmin from the DataContext as a fallback
                    else if (this.DataContext is XLPilot.Models.PilotButtonData buttonData && buttonData.RunAsAdmin)
                    {
                        shouldBeAdmin = true;
                    }
                    // Check if filename suggests this should be an admin button
                    else if (!string.IsNullOrEmpty(FileName))
                    {
                        if (FileName.ToLower().Contains("admin") ||
                            ButtonText.ToLower().Contains("admin") ||
                            FileName.ToLower() == "xlservr.exe")
                        {
                            shouldBeAdmin = true;
                        }
                    }

                    // Set visibility based on RunAsAdmin property or our heuristic
                    AdminShieldImage.Visibility = shouldBeAdmin ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
                // Silently handle any errors
            }
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
                new PropertyMetadata(false, OnRunAsAdminChanged));

        // This is called when RunAsAdmin property changes
        private static void OnRunAsAdminChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PilotButtonMovable control)
            {
                // Update shield visibility when RunAsAdmin changes
                control.UpdateAdminShieldVisibility();
            }
        }

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
                typeof(PilotButtonMovable),
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
                typeof(PilotButtonMovable),
                new PropertyMetadata(string.Empty));

        // ButtonType Dependency Property
        public Models.Enums.PilotButtonType ButtonType
        {
            get => (Models.Enums.PilotButtonType)GetValue(ButtonTypeProperty);
            set => SetValue(ButtonTypeProperty, value);
        }

        public static readonly DependencyProperty ButtonTypeProperty =
            DependencyProperty.Register(
                nameof(ButtonType),
                typeof(Models.Enums.PilotButtonType),
                typeof(PilotButtonMovable),
                new PropertyMetadata(Models.Enums.PilotButtonType.UserStandard));

        // ActionIdentifier Dependency Property
        public string ActionIdentifier
        {
            get => (string)GetValue(ActionIdentifierProperty);
            set => SetValue(ActionIdentifierProperty, value);
        }

        public static readonly DependencyProperty ActionIdentifierProperty =
            DependencyProperty.Register(
                nameof(ActionIdentifier),
                typeof(string),
                typeof(PilotButtonMovable),
                new PropertyMetadata(string.Empty));
        #endregion
    }
}