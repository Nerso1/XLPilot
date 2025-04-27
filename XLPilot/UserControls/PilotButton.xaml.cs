using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using XLPilot.Models;
using XLPilot.Services;

namespace XLPilot.UserControls
{
    /// <summary>
    /// User control for a clickable button that can launch an application.
    /// This control displays an image and text, and can be configured to run 
    /// applications with or without administrator privileges.
    /// </summary>
    /// <remarks>
    /// The PilotButton provides hover and click visual feedback to the user.
    /// When the RunAsAdmin property is set to true, it displays an admin shield icon
    /// in the bottom-right corner to indicate that the application will be launched
    /// with administrator privileges.
    /// </remarks>
    public partial class PilotButton : UserControl
    {
        /// <summary>
        /// Reference to the internal button control that handles the visual presentation
        /// and click events
        /// </summary>
        private Button internalButton;

        /// <summary>
        /// Reference to the admin shield image that indicates administrator privileges
        /// </summary>
        private Image adminShieldImage;

        // The XL path associated with this button (if any)
        private XLPaths associatedXLPath;

        /// <summary>
        /// Initializes a new instance of the PilotButton class
        /// </summary>
        public PilotButton()
        {
            // Initialize the XAML components
            InitializeComponent();

            // Wait until the control is loaded before finding the button and setting up events
            this.Loaded += (s, e) =>
            {
                // Try to find the button by name - this is the preferred method
                internalButton = this.FindName("InternalButton") as Button;

                // If not found by name, try to find it in the visual tree as a fallback method
                if (internalButton == null)
                {
                    internalButton = FindChildControl<Button>(this);
                }

                // Set up the functionality once we have the button
                if (internalButton != null)
                {
                    // Register the click event handler for the internal button
                    internalButton.Click += InternalButton_Click;

                    // Find the admin shield image in the visual tree
                    adminShieldImage = FindChildControl<Image>(internalButton, "AdminShieldImage");

                    // Update the admin shield visibility based on current RunAsAdmin value
                    UpdateAdminShieldVisibility();
                }
            };

            // Register for visibility changes to ensure the admin shield is updated when the control becomes visible
            this.IsVisibleChanged += PilotButton_IsVisibleChanged;
        }

        /// <summary>
        /// Sets the associated XL path for this button
        /// </summary>
        public void SetAssociatedXLPath(XLPaths xlPath)
        {
            associatedXLPath = xlPath;
        }

        /// <summary>
        /// Handles visibility changes to update the admin shield.
        /// This is needed because sometimes the shield visibility
        /// might not be properly set when switching tabs or when
        /// the control is initially created.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">Event data containing the old and new visibility state</param>
        private void PilotButton_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Only update the shield when the control becomes visible
            if ((bool)e.NewValue == true)
            {
                // Call the update method to ensure proper visibility
                UpdateAdminShieldVisibility();
            }
        }

        /// <summary>
        /// Updates the admin shield image visibility based on the RunAsAdmin property.
        /// The shield is only visible when RunAsAdmin is true, indicating that the 
        /// application will be launched with elevated privileges.
        /// </summary>
        private void UpdateAdminShieldVisibility()
        {
            try
            {
                // Check if we have successfully found the admin shield image
                if (adminShieldImage != null)
                {
                    // Set the visibility based on the RunAsAdmin property
                    // Visible when RunAsAdmin is true, otherwise Collapsed
                    adminShieldImage.Visibility = RunAsAdmin ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
                // Silently handle any errors to prevent crashes
                // This is a UI-only feature and shouldn't break functionality if it fails
            }
        }

        /// <summary>
        /// Event handler for when the internal button is clicked.
        /// Uses the ButtonActionManager to execute the appropriate action.
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">Event data</param>
        private void InternalButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a PilotButtonData from the current properties
            var buttonData = new PilotButtonData
            {
                ButtonText = ButtonText,
                FileName = FileName,
                ImageSource = ImageSource,
                RunAsAdmin = RunAsAdmin,
                Arguments = Arguments,
                ToolTipText = ToolTipText,
                Directory = Directory,
                ButtonType = ButtonType,
                ActionIdentifier = ActionIdentifier
            };

            // Execute the appropriate action using the ButtonActionManager
            ButtonActionManager.ExecuteButtonAction(buttonData, associatedXLPath);
        }

        /// <summary>
        /// Helper method to find a child control of a specific type in the visual tree.
        /// This method searches recursively through all children of the parent element.
        /// </summary>
        /// <typeparam name="T">The type of control to find</typeparam>
        /// <param name="parent">The parent element to search within</param>
        /// <returns>The first child of type T found, or null if none is found</returns>
        private static T FindChildControl<T>(DependencyObject parent) where T : DependencyObject
        {
            // Exit early if parent is null
            if (parent == null) return null;

            // Get the number of child elements using VisualTreeHelper
            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            // Iterate through each child
            for (int i = 0; i < childCount; i++)
            {
                // Get the current child element
                var child = VisualTreeHelper.GetChild(parent, i);

                // If this child is of the type we're looking for, return it immediately
                if (child is T result)
                {
                    return result;
                }

                // Otherwise, recursively search this child's children (depth-first search)
                var foundChild = FindChildControl<T>(child);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }

            // If we've checked all children and found nothing, return null
            return null;
        }

        /// <summary>
        /// Helper method to find a named child control of a specific type in the visual tree.
        /// This method searches recursively for a control with the specified name and type.
        /// </summary>
        /// <typeparam name="T">The type of control to find</typeparam>
        /// <param name="parent">The parent element to search within</param>
        /// <param name="childName">The name of the control to find</param>
        /// <returns>The child control matching both type and name, or null if none is found</returns>
        private static T FindChildControl<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            // Exit early if parent is null or childName is null/empty
            if (parent == null || string.IsNullOrEmpty(childName)) return null;

            // Get the number of child elements
            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            // Iterate through each child
            for (int i = 0; i < childCount; i++)
            {
                // Get the current child
                var child = VisualTreeHelper.GetChild(parent, i);

                // Check if this is a FrameworkElement with the matching name and type
                if (child is FrameworkElement element &&
                    element.Name == childName &&
                    child is T typedChild)
                {
                    return typedChild;
                }

                // If not, recursively search this child's children
                var foundChild = FindChildControl<T>(child, childName);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }

            // If no matching child was found, return null
            return null;
        }

        #region Dependency Properties
        /// <summary>
        /// Gets or sets the text displayed below the button image.
        /// </summary>
        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        /// <summary>
        /// Identifies the ButtonText dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(
                nameof(ButtonText),
                typeof(string),
                typeof(PilotButton),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// <summary>
        /// Gets or sets the name of the executable file to launch when clicked.
        /// </summary>
        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }

        /// <summary>
        /// Identifies the FileName dependency property.
        /// </summary>
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register(
                nameof(FileName),
                typeof(string),
                typeof(PilotButton),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the image source path to display as the button's main image.
        /// </summary>
        public string ImageSource
        {
            get => (string)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the ImageSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                nameof(ImageSource),
                typeof(string),
                typeof(PilotButton),
                new PropertyMetadata("/XLPilot;component/Resources/Images/detault-profile-picture.png"));

        /// <summary>
        /// Gets or sets a value indicating whether the application should be launched
        /// with administrator privileges. When true, an admin shield icon will be displayed
        /// in the bottom-right corner of the button.
        /// </summary>
        public bool RunAsAdmin
        {
            get => (bool)GetValue(RunAsAdminProperty);
            set => SetValue(RunAsAdminProperty, value);
        }

        /// <summary>
        /// Identifies the RunAsAdmin dependency property.
        /// </summary>
        public static readonly DependencyProperty RunAsAdminProperty =
            DependencyProperty.Register(
                nameof(RunAsAdmin),
                typeof(bool),
                typeof(PilotButton),
                new PropertyMetadata(false, OnRunAsAdminChanged));

        /// <summary>
        /// Called when the RunAsAdmin property changes.
        /// Updates the visibility of the admin shield icon.
        /// </summary>
        /// <param name="d">The dependency object (PilotButton)</param>
        /// <param name="e">The event arguments containing the old and new values</param>
        private static void OnRunAsAdminChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PilotButton button)
            {
                // Update shield visibility when RunAsAdmin changes
                button.UpdateAdminShieldVisibility();
            }
        }

        /// <summary>
        /// Gets or sets the command-line arguments to pass to the executable.
        /// </summary>
        public string Arguments
        {
            get => (string)GetValue(ArgumentsProperty);
            set => SetValue(ArgumentsProperty, value);
        }

        /// <summary>
        /// Identifies the Arguments dependency property.
        /// </summary>
        public static readonly DependencyProperty ArgumentsProperty =
            DependencyProperty.Register(
                nameof(Arguments),
                typeof(string),
                typeof(PilotButton),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the tooltip text to display when hovering over the button.
        /// The tooltip will only be shown if this property has a non-empty value.
        /// </summary>
        public string ToolTipText
        {
            get => (string)GetValue(ToolTipTextProperty);
            set => SetValue(ToolTipTextProperty, value);
        }

        /// <summary>
        /// Identifies the ToolTipText dependency property.
        /// </summary>
        public static readonly DependencyProperty ToolTipTextProperty =
            DependencyProperty.Register(
                nameof(ToolTipText),
                typeof(string),
                typeof(PilotButton),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the directory containing the executable file.
        /// This is combined with FileName to create the full path to the executable.
        /// </summary>
        public string Directory
        {
            get => (string)GetValue(DirectoryProperty);
            set => SetValue(DirectoryProperty, value);
        }

        /// <summary>
        /// Identifies the Directory dependency property.
        /// </summary>
        public static readonly DependencyProperty DirectoryProperty =
            DependencyProperty.Register(
                nameof(Directory),
                typeof(string),
                typeof(PilotButton),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the type of button (UserStandard, SystemStandard, SystemSpecial)
        /// </summary>
        public Models.Enums.PilotButtonType ButtonType
        {
            get => (Models.Enums.PilotButtonType)GetValue(ButtonTypeProperty);
            set => SetValue(ButtonTypeProperty, value);
        }

        /// <summary>
        /// Identifies the ButtonType dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonTypeProperty =
            DependencyProperty.Register(
                nameof(ButtonType),
                typeof(Models.Enums.PilotButtonType),
                typeof(PilotButton),
                new PropertyMetadata(Models.Enums.PilotButtonType.UserStandard));

        /// <summary>
        /// Gets or sets the action identifier for special actions
        /// </summary>
        public string ActionIdentifier
        {
            get => (string)GetValue(ActionIdentifierProperty);
            set => SetValue(ActionIdentifierProperty, value);
        }

        /// <summary>
        /// Identifies the ActionIdentifier dependency property.
        /// </summary>
        public static readonly DependencyProperty ActionIdentifierProperty =
            DependencyProperty.Register(
                nameof(ActionIdentifier),
                typeof(string),
                typeof(PilotButton),
                new PropertyMetadata(string.Empty));
    }
}
#endregion
