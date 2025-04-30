using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using XLPilot.Models;
using XLPilot.Models.Enums;
using XLPilot.Services;
using XLPilot.XmlUtilities;

namespace XLPilot
{
    /// <summary>
    /// Interaction logic for CreateIconWindow.xaml
    /// </summary>
    public partial class CreateIconWindow : Window
    {
        // The PilotButtonData being created or edited
        private PilotButtonData _buttonData;

        // Flag to determine if we're creating an XL icon or Other icon
        private bool _isXLIcon = true;

        // Flag to track if required fields are filled
        private bool _isFormValid = false;

        // Default icon path if not provided
        private const string DEFAULT_ICON_PATH = "/XLPilot;component/Resources/Images/detault-profile-picture.png";

        // The first XL path (for XL Icon preview)
        private XLPaths _firstXLPath = null;

        // Stores directory and filename separately
        private string _directory = "";
        private string _fileName = "";

        /// <summary>
        /// Constructor for creating a new icon
        /// </summary>
        /// <param name="isXLIcon">Whether this is an XL icon or Other icon</param>
        public CreateIconWindow(bool isXLIcon = true)
        {
            InitializeComponent();

            _isXLIcon = isXLIcon;
            rbXLIcon.IsChecked = isXLIcon;
            rbOtherIcon.IsChecked = !isXLIcon;

            // Initialize with a new PilotButtonData
            _buttonData = new PilotButtonData("", "", DEFAULT_ICON_PATH);

            // Configure UI based on icon type
            ConfigureUIForIconType();

            // Load the first XL path for XL Icon preview and functionality
            LoadFirstXLPath();

            // Setup the arguments dropdown
            SetupArgumentsDropdown();

            // Initialize preview
            UpdatePreview();
        }

        /// <summary>
        /// Constructor for editing an existing icon
        /// </summary>
        /// <param name="buttonData">The button data to edit</param>
        /// <param name="isXLIcon">Whether this is an XL icon or Other icon</param>
        public CreateIconWindow(PilotButtonData buttonData, bool isXLIcon = true) : this(isXLIcon)
        {
            _buttonData = buttonData ?? new PilotButtonData("", "", DEFAULT_ICON_PATH);

            // Fill in the form with existing data
            txtButtonText.Text = _buttonData.ButtonText;
            _directory = _buttonData.Directory;
            _fileName = _buttonData.FileName;

            // Set the file path display based on directory and file name
            if (!string.IsNullOrEmpty(_fileName))
            {
                // If we have a file name, show full path
                txtFilePath.Text = Path.Combine(_directory, _fileName);
            }
            else if (!string.IsNullOrEmpty(_directory))
            {
                // If only directory, show that
                txtFilePath.Text = _directory;
            }

            txtImageSource.Text = _buttonData.ImageSource;
            chkRunAsAdmin.IsChecked = _buttonData.RunAsAdmin;

            // Handle arguments
            if (!string.IsNullOrEmpty(_buttonData.Arguments))
            {
                if (_isXLIcon && _buttonData.Arguments.Trim().ToLower() == "include")
                {
                    cmbArgumentsOption.SelectedIndex = 2; // "Przekaż bazę i klucz"
                }
                else
                {
                    cmbArgumentsOption.SelectedIndex = 1; // "Tak"
                    txtArguments.Text = _buttonData.Arguments;
                }
            }

            // Update preview
            UpdatePreview();

            // Check form validity
            ValidateForm();
        }

        /// <summary>
        /// Gets the created or edited PilotButtonData
        /// </summary>
        public PilotButtonData ButtonData => _buttonData;

        /// <summary>
        /// Load the first XL path for preview and functionality
        /// </summary>
        private void LoadFirstXLPath()
        {
            try
            {
                var serializationManager = SerializationService.Manager;
                var xlPaths = serializationManager.GetData().XLPathsList;

                if (xlPaths != null && xlPaths.Count > 0)
                {
                    _firstXLPath = xlPaths[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas wczytywania ścieżek XL: {ex.Message}",
                    "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Configure the UI based on icon type (XL or Other)
        /// </summary>
        private void ConfigureUIForIconType()
        {
            if (_isXLIcon)
            {
                // XL Icon
                Title = "Tworzenie nowej ikony XL";
                cbiIncludeDBAndKey.Visibility = Visibility.Visible;
            }
            else
            {
                // Other Icon
                Title = "Tworzenie nowej ikony Inne";
                cbiIncludeDBAndKey.Visibility = Visibility.Collapsed;
            }

            // Disable radio buttons - type is determined by the tab and can't be changed
            rbXLIcon.IsEnabled = false;
            rbOtherIcon.IsEnabled = false;
        }

        /// <summary>
        /// Set up the arguments dropdown options
        /// </summary>
        private void SetupArgumentsDropdown()
        {
            cmbArgumentsOption.SelectedIndex = 0; // "Nie" by default
        }

        /// <summary>
        /// Update the preview button with current data
        /// </summary>
        private void UpdatePreview()
        {
            previewButton.ButtonText = txtButtonText.Text;
            previewButton.FileName = _fileName;

            // Use default icon if none provided
            previewButton.ImageSource = !string.IsNullOrEmpty(txtImageSource.Text)
                ? txtImageSource.Text
                : DEFAULT_ICON_PATH;

            previewButton.RunAsAdmin = chkRunAsAdmin.IsChecked ?? false;

            // Set arguments based on dropdown selection
            switch (cmbArgumentsOption.SelectedIndex)
            {
                case 0: // "Nie"
                    previewButton.Arguments = "";
                    break;
                case 1: // "Tak"
                    previewButton.Arguments = txtArguments.Text;
                    break;
                case 2: // "Przekaż bazę i klucz"
                    previewButton.Arguments = "include";
                    break;
            }

            // Set directory based on icon type
            if (_isXLIcon)
            {
                // For XL icons, we use the first XL path for preview
                if (_firstXLPath != null)
                {
                    previewButton.Directory = _firstXLPath.Path;
                    previewButton.SetAssociatedXLPath(_firstXLPath);
                }
                else
                {
                    previewButton.Directory = "";
                }
            }
            else
            {
                // For Other icons, use the directory from the form
                previewButton.Directory = _directory;
            }
        }

        /// <summary>
        /// Check if the form is valid (required fields filled)
        /// </summary>
        private void ValidateForm()
        {
            bool isValid = !string.IsNullOrWhiteSpace(txtButtonText.Text);

            // Need valid file or directory path
            if (_isXLIcon)
            {
                // For XL icons, need a valid file name in XL directory
                isValid = isValid && !string.IsNullOrWhiteSpace(_fileName);
            }
            else
            {
                // For Other icons, need either a file name or a directory
                isValid = isValid && (!string.IsNullOrWhiteSpace(_fileName) || !string.IsNullOrWhiteSpace(_directory));
            }

            // Update form validity flag and OK button state
            _isFormValid = isValid;
            btnOK.IsEnabled = isValid;
        }

        /// <summary>
        /// Handle icon type radio button change
        /// </summary>
        private void IconType_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;

            _isXLIcon = rbXLIcon.IsChecked ?? false;
            ConfigureUIForIconType();
            ValidateForm();
            UpdatePreview();
        }

        /// <summary>
        /// Handle arguments dropdown selection change
        /// </summary>
        private void CmbArgumentsOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;

            switch (cmbArgumentsOption.SelectedIndex)
            {
                case 0: // "Nie"
                    txtArguments.IsEnabled = false;
                    txtArguments.Text = "";
                    break;
                case 1: // "Tak"
                    txtArguments.IsEnabled = true;
                    break;
                case 2: // "Przekaż bazę i klucz"
                    txtArguments.IsEnabled = false;
                    txtArguments.Text = "include";
                    break;
            }

            UpdatePreview();
        }

        /// <summary>
        /// Handle browse file button click - selects file or folder
        /// </summary>
        private void BtnBrowseFile_Click(object sender, RoutedEventArgs e)
        {
            // Ask user whether they want to select file or folder
            var result = MessageBox.Show(
                "Czy chcesz wybrać plik?\n\nTak - wybierz plik\nNie - wybierz folder",
                "Wybór pliku lub folderu",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // User wants to select a file
                SelectFile();
            }
            else
            {
                // User wants to select a folder
                SelectFolder();
            }
        }

        /// <summary>
        /// Handles file selection
        /// </summary>
        private void SelectFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Wszystkie pliki (*.*)|*.*",
                Title = "Wybierz plik do uruchomienia:"
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedFile = dialog.FileName;
                _fileName = Path.GetFileName(selectedFile);
                _directory = Path.GetDirectoryName(selectedFile);

                if (_isXLIcon)
                {
                    // For XL icons, check if the selected file is within an XL directory hierarchy
                    if (IsWithinXLDirectory(_directory))
                    {
                        // Valid selection in XL directory or subfolder
                        txtFilePath.Text = selectedFile;
                    }
                    else
                    {
                        MessageBox.Show("Wybrany plik nie znajduje się w katalogu Comarch ERP XL ani jego podfolderze.\n" +
                            "Plik musi być w katalogu, który zawiera pliki cdnxl.exe i rejestr.bat, lub w jego podfolderze.",
                            "Nieprawidłowy katalog",
                            MessageBoxButton.OK, MessageBoxImage.Warning);

                        // Reset selection
                        _fileName = "";
                        _directory = "";
                        txtFilePath.Text = "";
                    }
                }
                else
                {
                    // For Other icons, accept any directory
                    txtFilePath.Text = selectedFile;
                }

                ValidateForm();
                UpdatePreview();
            }
        }


        /// <summary>
        /// Handles folder selection
        /// </summary>
        private void SelectFolder()
        {
            // Use OpenFileDialog in a way that allows directory selection
            var dialog = new OpenFileDialog
            {
                CheckFileExists = false,
                FileName = "Folder Selection",
                Title = "Wybierz folder:",
                ValidateNames = false
            };

            if (dialog.ShowDialog() == true)
            {
                // Get the selected folder path
                string folderPath = Path.GetDirectoryName(dialog.FileName);

                if (_isXLIcon)
                {
                    // For XL icons, check if this folder is within an XL directory hierarchy
                    if (IsWithinXLDirectory(folderPath))
                    {
                        // Valid selection - either XL directory itself or its subfolder
                        _directory = folderPath;
                        _fileName = ""; // Folder only, no file name
                        txtFilePath.Text = folderPath;
                    }
                    else
                    {
                        MessageBox.Show("Wybrany folder nie jest katalogiem Comarch ERP XL ani jego podfolderem.\n" +
                            "Folder musi zawierać pliki cdnxl.exe i rejestr.bat, lub być jego podfolderem.",
                            "Nieprawidłowy katalog",
                            MessageBoxButton.OK, MessageBoxImage.Warning);

                        // Reset selection
                        _directory = "";
                        txtFilePath.Text = "";
                    }
                }
                else
                {
                    // For Other icons, accept any directory
                    _directory = folderPath;
                    _fileName = ""; // Folder only, no file name
                    txtFilePath.Text = folderPath;
                }

                ValidateForm();
                UpdatePreview();
            }
        }
        /// <summary>
        /// Check if a directory is within an XL directory hierarchy
        /// (either the XL directory itself or a subfolder of it)
        /// </summary>
        private bool IsWithinXLDirectory(string directory)
        {
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
                return false;

            // Check if this directory itself is an XL directory
            if (IsXLDirectory(directory))
                return true;

            // Check parent directories until we reach the root
            string parentDir = directory;
            while (!string.IsNullOrEmpty(parentDir))
            {
                // Move up to parent directory
                parentDir = Path.GetDirectoryName(parentDir);

                // If we've reached the root, stop searching
                if (string.IsNullOrEmpty(parentDir))
                    break;

                // Check if this parent is an XL directory
                if (IsXLDirectory(parentDir))
                    return true;
            }

            // If we get here, no XL directory was found in the hierarchy
            return false;
        }


        /// <summary>
        /// Handle browse image button click
        /// </summary>
        private void BtnBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Pliki obrazów (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|Wszystkie pliki (*.*)|*.*",
                Title = "Wybierz obraz ikony:"
            };

            if (dialog.ShowDialog() == true)
            {
                txtImageSource.Text = dialog.FileName;
                ValidateForm();
                UpdatePreview();
            }
        }

        /// <summary>
        /// Handle control value changes
        /// </summary>
        private void Control_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;

            ValidateForm();
            UpdatePreview();
        }

        /// <summary>
        /// Handle preview button click
        /// </summary>
        private void PreviewButton_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                // Create a temporary PilotButtonData for testing
                var tempButton = new PilotButtonData(
                    txtButtonText.Text,
                    _fileName,
                    !string.IsNullOrEmpty(txtImageSource.Text) ? txtImageSource.Text : DEFAULT_ICON_PATH,
                    chkRunAsAdmin.IsChecked ?? false,
                    previewButton.Arguments,
                    "Test ikony",
                    _isXLIcon ? (_firstXLPath != null ? _firstXLPath.Path : "") : _directory,
                    PilotButtonType.UserStandard,
                    ""
                );

                // Execute the button action
                if (_isXLIcon && _firstXLPath != null)
                {
                    ButtonActionManager.ExecuteButtonAction(tempButton, _firstXLPath);
                }
                else
                {
                    ButtonActionManager.ExecuteButtonAction(tempButton);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas testowania ikony: {ex.Message}",
                    "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handle OK button click
        /// </summary>
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!_isFormValid)
            {
                MessageBox.Show("Proszę wypełnić wszystkie wymagane pola.",
                    "Formularz niepełny", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get the arguments based on dropdown selection
            string arguments = "";
            switch (cmbArgumentsOption.SelectedIndex)
            {
                case 0: // "Nie"
                    arguments = "";
                    break;
                case 1: // "Tak"
                    arguments = txtArguments.Text;
                    break;
                case 2: // "Przekaż bazę i klucz"
                    arguments = "include";
                    break;
            }

            // Create or update the button data
            _buttonData = new PilotButtonData(
                txtButtonText.Text,
                _fileName,
                !string.IsNullOrEmpty(txtImageSource.Text) ? txtImageSource.Text : DEFAULT_ICON_PATH,
                chkRunAsAdmin.IsChecked ?? false,
                arguments,
                "", // ToolTipText - could be added to the form in the future
                _isXLIcon ? "" : _directory, // Directory is empty for XL icons
                PilotButtonType.UserStandard, // Always UserStandard for user-created icons
                "" // ActionIdentifier - only used for special system actions
            );

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Handle Cancel button click
        /// </summary>
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Check if a directory is an XL directory (contains cdnxl.exe and rejestr.bat)
        /// </summary>
        private bool IsXLDirectory(string directory)
        {
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
                return false;

            bool hasXlExe = File.Exists(Path.Combine(directory, "cdnxl.exe"));
            bool hasRejestrBat = File.Exists(Path.Combine(directory, "rejestr.bat"));

            return hasXlExe && hasRejestrBat;
        }

    }
}