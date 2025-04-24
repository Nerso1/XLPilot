using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using XLPilot.Models;
using XLPilot.Services;
using XLPilot.XmlUtilities;
using System.Collections.Generic;

namespace XLPilot.TabControls
{
    /// <summary>
    /// Main tab control that displays all the buttons
    /// </summary>
    public partial class MainTab1 : UserControl
    {
        // Reference to the serialization manager
        private SerializationManager serializationManager => SerializationService.Manager;

        // Max buttons per row
        private const int MaxButtonsPerRow = 5;

        public MainTab1()
        {
            InitializeComponent();

            // Register to the Loaded event to refresh the UI when tab is shown
            this.Loaded += MainTab1_Loaded;
        }

        /// <summary>
        /// Event handler for the Loaded event - refreshes UI when tab is shown
        /// </summary>
        private void MainTab1_Loaded(object sender, RoutedEventArgs e)
        {
            // Reload the UI when the tab is shown
            LoadXLPathsWithIcons();
            LoadOtherIcons();
            //CheckXLIcons();
        }

        /// <summary>
        /// Loads all Other icons into the bottom section of the UI
        /// </summary>
        private void LoadOtherIcons()
        {
            try
            {
                // Get the Other icons
                var otherIcons = serializationManager.GetData().OtherPilotButtons;

                // Get the StackPanel in the second ScrollViewer (Grid.Row="2")
                var otherIconsStackPanel = ((ScrollViewer)grdMainGrid.Children[2]).Content as StackPanel;

                // Clear any existing content
                if (otherIconsStackPanel != null)
                {
                    otherIconsStackPanel.Children.Clear();

                    // Add a title for the section
                    var titleTextBlock = new TextBlock
                    {
                        Text = "Opcje niepowiązane z XL-ami",
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 5, 0, 10)
                    };
                    otherIconsStackPanel.Children.Add(titleTextBlock);

                    // Create an outer StackPanel for the rows of buttons
                    var buttonsStackPanel = new StackPanel
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 5)
                    };

                    // Add the buttons to the stack panel
                    PopulateStackPanelWithOtherIconRows(buttonsStackPanel, otherIcons);

                    // Add the buttons stack panel to the main stack panel
                    otherIconsStackPanel.Children.Add(buttonsStackPanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Other icons: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Populates a StackPanel with rows of PilotButtons for Other icons, with up to 5 buttons per row
        /// </summary>
        private void PopulateStackPanelWithOtherIconRows(StackPanel stackPanel, List<PilotButtonData> otherIcons)
        {
            try
            {
                // If there are no icons, add a message
                if (otherIcons == null || otherIcons.Count == 0)
                {
                    var noIconsTextBlock = new TextBlock
                    {
                        Text = "Brak ikon w tej sekcji.",
                        FontStyle = FontStyles.Italic,
                        Foreground = Brushes.Gray,
                        Margin = new Thickness(10),
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    stackPanel.Children.Add(noIconsTextBlock);
                    return;
                }

                // Calculate how many rows we need
                int numRows = (otherIcons.Count + MaxButtonsPerRow - 1) / MaxButtonsPerRow;

                // Create each row
                for (int row = 0; row < numRows; row++)
                {
                    // Create a horizontal panel for this row
                    var rowPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 5) // Add margin between rows
                    };

                    // Calculate the range of icons for this row
                    int startIndex = row * MaxButtonsPerRow;
                    int endIndex = Math.Min(startIndex + MaxButtonsPerRow, otherIcons.Count);

                    // Add buttons for this row
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        var iconData = otherIcons[i];

                        // Create the PilotButton with properties from the icon data
                        var pilotButton = new XLPilot.UserControls.PilotButton
                        {
                            Width = 100,
                            Height = 100,
                            ButtonText = iconData.ButtonText,
                            FileName = iconData.FileName,
                            ImageSource = iconData.ImageSource,
                            RunAsAdmin = iconData.RunAsAdmin,
                            Arguments = iconData.Arguments,
                            ToolTipText = iconData.ToolTipText,
                            Directory = iconData.Directory,
                            Margin = new Thickness(5) // Add margin between buttons
                        };

                        // Add the button to the row panel
                        rowPanel.Children.Add(pilotButton);
                    }

                    // Add the row to the main stack panel
                    stackPanel.Children.Add(rowPanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating Other icons: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Loads all XL paths and creates UI elements for each
        /// </summary>
        private void LoadXLPathsWithIcons()
        {
            try
            {
                // Get the XL paths
                var pathsContainer = serializationManager.GetXLPathsContainer();

                // Get the XL icons
                var xlIcons = serializationManager.GetData().XLPilotButtons;

                // Get the StackPanel in the ScrollViewer
                var mainStackPanel = ((ScrollViewer)grdMainGrid.Children[0]).Content as StackPanel;

                // Clear any existing content
                if (mainStackPanel != null)
                {
                    mainStackPanel.Children.Clear();

                    // Create UI elements for each XL path
                    foreach (var xlPath in pathsContainer.Items)
                    {
                        CreateXLPathSection(mainStackPanel, xlPath, xlIcons);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading XL paths and icons: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Creates UI elements for a single XL path
        /// </summary>
        private void CreateXLPathSection(StackPanel parentPanel, XLPaths xlPath, List<PilotButtonData> xlIcons)
        {
            // Create a container for this XL path section
            var sectionPanel = new StackPanel
            {
                Margin = new Thickness(5, 15, 5, 10),
                HorizontalAlignment = HorizontalAlignment.Center // Center the whole section
            };

            // Create a horizontal panel for the header text blocks
            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 5),
                HorizontalAlignment = HorizontalAlignment.Center // Center the header text
            };

            // Add the folder name text block
            var folderNameTextBlock = new TextBlock
            {
                Text = xlPath.Name,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 10, 0) // Add margin to separate from database name
            };
            headerPanel.Children.Add(folderNameTextBlock);

            // Add database name if available
            if (!string.IsNullOrEmpty(xlPath.Database))
            {
                var databaseNameTextBlock = new TextBlock
                {
                    Text = $"Baza: {xlPath.Database}",
                    FontSize = 16,
                    FontStyle = FontStyles.Italic,
                    Foreground = Brushes.Gray
                };
                headerPanel.Children.Add(databaseNameTextBlock);
            }

            // Add license info if available
            if (!string.IsNullOrEmpty(xlPath.FormattedLicenseInfo))
            {
                var formattedLicenseInfoTextBlock = new TextBlock
                {
                    Text = $"  Klucz: {xlPath.FormattedLicenseInfo}",
                    FontSize = 16,
                    FontStyle = FontStyles.Italic,
                    Foreground = Brushes.Gray
                };
                headerPanel.Children.Add(formattedLicenseInfoTextBlock);
            }

            // Add the header panel to the section
            sectionPanel.Children.Add(headerPanel);

            // Create an outer StackPanel for the rows of buttons
            var buttonsStackPanel = new StackPanel
            {
                Margin = new Thickness(0, 5, 0, 15),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Add PilotButtons to the StackPanel in rows of 5
            PopulateStackPanelWithIconRows(buttonsStackPanel, xlIcons, xlPath);

            // Add the StackPanel to the section
            sectionPanel.Children.Add(buttonsStackPanel);

            // Add a separator for visual separation between XL path sections
            var separator = new Separator
            {
                Margin = new Thickness(0, 0, 0, 10),
                Width = 550 // Set a fixed width for the separator
            };
            sectionPanel.Children.Add(separator);

            // Add the complete section to the parent panel
            parentPanel.Children.Add(sectionPanel);
        }

        /// <summary>
        /// Populates a StackPanel with rows of PilotButtons, with up to 5 buttons per row
        /// </summary>
        private void PopulateStackPanelWithIconRows(StackPanel stackPanel, List<PilotButtonData> xlIcons, XLPaths xlPath)
        {
            try
            {
                // If there are no icons, add a message
                if (xlIcons == null || xlIcons.Count == 0)
                {
                    var noIconsTextBlock = new TextBlock
                    {
                        Text = "Brak ikon dla tej ścieżki.",
                        FontStyle = FontStyles.Italic,
                        Foreground = Brushes.Gray,
                        Margin = new Thickness(10),
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    stackPanel.Children.Add(noIconsTextBlock);
                    return;
                }

                // Calculate how many rows we need
                int numRows = (xlIcons.Count + MaxButtonsPerRow - 1) / MaxButtonsPerRow;

                // Create each row
                for (int row = 0; row < numRows; row++)
                {
                    // Create a WrapPanel for this row
                    var rowPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 10) // Add margin between rows
                    };

                    // Calculate the range of icons for this row
                    int startIndex = row * MaxButtonsPerRow;
                    int endIndex = Math.Min(startIndex + MaxButtonsPerRow, xlIcons.Count);

                    // Add buttons for this row
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        var iconData = xlIcons[i];

                        // Create the PilotButton with properties from the icon data
                        var pilotButton = new XLPilot.UserControls.PilotButton
                        {
                            Width = 100,
                            Height = 100,
                            ButtonText = iconData.ButtonText,
                            FileName = iconData.FileName,
                            ImageSource = iconData.ImageSource,
                            RunAsAdmin = iconData.RunAsAdmin,
                            Arguments = iconData.Arguments,
                            ToolTipText = iconData.ToolTipText,
                            Directory = string.IsNullOrEmpty(iconData.Directory) ? xlPath.Path : iconData.Directory,
                            Margin = new Thickness(5) // Add margin between buttons
                        };

                        // Add the button to the row panel
                        rowPanel.Children.Add(pilotButton);
                    }

                    // Add the row to the main stack panel
                    stackPanel.Children.Add(rowPanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating icons: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Debug method to check XL paths
        /// </summary>
        private void CheckXLPaths()
        {
            try
            {
                // Get the SerializationManager instance
                var serializationManager = SerializationService.Manager;

                // Get all XL Paths using the GetXLPathsContainer method
                var pathsContainer = serializationManager.GetXLPathsContainer();

                // Check if there are any paths
                if (pathsContainer.Items.Count == 0)
                {
                    MessageBox.Show("No XL Paths found.", "XL Paths", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Build a message with all the path names
                string message = "XL Paths found:\n\n";

                // Loop through each path and add its name to the message
                foreach (var path in pathsContainer.Items)
                {
                    message += $"- {path.Name}\n";
                }

                // Display the message box with the list of paths
                MessageBox.Show(message, "XL Paths", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Show error message if something went wrong
                MessageBox.Show($"Error loading XL Paths: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Debug method to check XL icons
        /// </summary>
        private void CheckXLIcons()
        {
            try
            {
                // Get the SerializationManager instance
                var serializationManager = SerializationService.Manager;

                // Get the XL Pilot Buttons data
                var xlButtons = serializationManager.GetData().XLPilotButtons;

                // Check if there are any buttons
                if (xlButtons == null || xlButtons.Count == 0)
                {
                    MessageBox.Show("No XL Icons found.", "XL Icons", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Build a message with all the button text/names
                string message = "XL Icons found:\n\n";

                // Loop through each button and add its name to the message
                foreach (var button in xlButtons)
                {
                    message += $"- {button.ButtonText}, run as admin? - {button.RunAsAdmin}\n";
                }

                // Display the message box with the list of icons
                MessageBox.Show(message, "XL Icons", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Show error message if something went wrong
                MessageBox.Show($"Error loading XL Icons: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Event handler for running executables (not implemented)
        /// </summary>
        private void RunExecutable(object sender, MouseButtonEventArgs e)
        {
            // This method is not implemented yet
            // It would handle mouse clicks on buttons
        }
    }
}