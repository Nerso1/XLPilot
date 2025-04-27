using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using XLPilot.Models;
using XLPilot.Models.Enums;

namespace XLPilot.Services
{
    /// <summary>
    /// Manager class that handles execution of actions for PilotButtons
    /// </summary>
    public static class ButtonActionManager
    {
        /// <summary>
        /// Execute the appropriate action for a button based on its type and properties
        /// </summary>
        /// <param name="button">The button data</param>
        /// <param name="xlPath">The XL path (may be null for OtherIcons)</param>
        public static void ExecuteButtonAction(PilotButtonData button, XLPaths xlPath = null)
        {
            try
            {
                // Determine the execution path based on the context
                string executionDirectory = string.Empty;

                // If this is an XL button (no specific directory), use the XL path
                if (string.IsNullOrEmpty(button.Directory) && xlPath != null)
                {
                    executionDirectory = xlPath.Path;
                }
                // Otherwise use the button's directory
                else
                {
                    executionDirectory = button.Directory;
                }

                // Determine what action to take based on button type
                switch (button.ButtonType)
                {
                    case PilotButtonType.UserStandard:
                    case PilotButtonType.SystemStandard:
                        ExecuteStandardAction(button, executionDirectory);
                        break;

                    case PilotButtonType.SystemSpecial:
                        ExecuteSpecialAction(button, executionDirectory, xlPath);
                        break;

                    default:
                        MessageBox.Show("Nieznany typ przycisku", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd wykonania akcji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Execute a standard button action (run application or open folder)
        /// </summary>
        private static void ExecuteStandardAction(PilotButtonData button, string directory)
        {
            // If no directory is specified, show an error
            if (string.IsNullOrEmpty(directory))
            {
                MessageBox.Show("Brak określonego katalogu", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // If no file name is specified, just open the directory
            if (string.IsNullOrEmpty(button.FileName))
            {
                // Debug message
                MessageBox.Show($"Otwieranie katalogu: {directory}", "Informacja Debugowania", MessageBoxButton.OK, MessageBoxImage.Information);

                // Open the directory in File Explorer
                Process.Start("explorer.exe", directory);
                return;
            }

            // Build the full path to the file
            string filePath = Path.Combine(directory, button.FileName);

            // Check if the path exists
            if (File.Exists(filePath))
            {
                // If path exists and is a file, run it
                // Debug message
                MessageBox.Show($"Uruchamianie aplikacji: {filePath} {(button.RunAsAdmin ? "(jako administrator)" : "")}",
                    "Informacja Debugowania", MessageBoxButton.OK, MessageBoxImage.Information);

                RunExecutable(filePath, directory, button.RunAsAdmin, button.Arguments);
            }
            else if (Directory.Exists(filePath))
            {
                // If path exists and is a directory, open it
                // Debug message
                MessageBox.Show($"Otwieranie katalogu: {filePath}", "Informacja Debugowania", MessageBoxButton.OK, MessageBoxImage.Information);

                Process.Start("explorer.exe", filePath);
            }
            else
            {
                // If path doesn't exist, show error
                MessageBox.Show($"Plik nie istnieje: {filePath}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Execute a special button action based on the ActionIdentifier
        /// </summary>
        private static void ExecuteSpecialAction(PilotButtonData button, string directory, XLPaths xlPath)
        {
            switch (button.ActionIdentifier)
            {
                case "ChangeEnvVariable":
                    // Debug message
                    MessageBox.Show($"Zmiana zmiennej środowiskowej Path dla XL: {(xlPath != null ? xlPath.Name : "brak ścieżki XL")}",
                        "Informacja Debugowania", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Placeholder for actual implementation
                    ChangeEnvironmentalVariable(xlPath);
                    break;

                case "ComputerConfigRegistry":
                    // Debug message
                    MessageBox.Show("Otwieranie rejestru z konfiguracją komputera dla XL",
                        "Informacja Debugowania", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Placeholder for actual implementation
                    OpenComputerConfigRegistry();
                    break;

                case "UserDatabasesRegistry":
                    // Debug message
                    MessageBox.Show("Otwieranie rejestru z bazami użytkownika (HKCU)",
                        "Informacja Debugowania", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Placeholder for actual implementation
                    OpenUserDatabasesRegistry();
                    break;

                case "ComputerDatabasesRegistry":
                    // Debug message
                    MessageBox.Show("Otwieranie rejestru z bazami komputera (HKLM)",
                        "Informacja Debugowania", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Placeholder for actual implementation
                    OpenComputerDatabasesRegistry();
                    break;

                case "DSServicesRegistry":
                    // Debug message
                    MessageBox.Show("Otwieranie rejestru z usługami DS",
                        "Informacja Debugowania", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Placeholder for actual implementation
                    OpenDSServicesRegistry();
                    break;

                default:
                    MessageBox.Show($"Nieznana akcja specjalna: {button.ActionIdentifier}",
                        "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }
        }

        /// <summary>
        /// Runs an executable with specified parameters
        /// </summary>
        private static void RunExecutable(string filePath, string workingDirectory, bool runAsAdmin, string arguments = null)
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = true,
                    Arguments = arguments
                };

                if (runAsAdmin)
                {
                    processInfo.Verb = "runas";
                }

                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd uruchamiania {Path.GetFileName(filePath)}: {ex.Message}",
                               "Błąd uruchamiania aplikacji",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }

        // Placeholder methods for special actions - to be implemented later

        /// <summary>
        /// Changes the PATH environmental variable to include XL path 
        /// </summary>
        private static void ChangeEnvironmentalVariable(XLPaths xlPath)
        {
            // Placeholder - will be implemented later
            MessageBox.Show("Funkcja zmiany zmiennej PATH zostanie zaimplementowana w przyszłości.",
                "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Opens registry with computer configuration for XL
        /// </summary>
        private static void OpenComputerConfigRegistry()
        {
            // Placeholder - will be implemented later
            MessageBox.Show("Funkcja otwierania rejestru konfiguracji komputera zostanie zaimplementowana w przyszłości.",
                "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Opens registry with user databases configuration
        /// </summary>
        private static void OpenUserDatabasesRegistry()
        {
            // Placeholder - will be implemented later
            MessageBox.Show("Funkcja otwierania rejestru baz użytkownika zostanie zaimplementowana w przyszłości.",
                "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Opens registry with computer databases configuration
        /// </summary>
        private static void OpenComputerDatabasesRegistry()
        {
            // Placeholder - will be implemented later
            MessageBox.Show("Funkcja otwierania rejestru baz komputera zostanie zaimplementowana w przyszłości.",
                "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Opens registry with Data Services configuration
        /// </summary>
        private static void OpenDSServicesRegistry()
        {
            // Placeholder - will be implemented later
            MessageBox.Show("Funkcja otwierania rejestru usług DS zostanie zaimplementowana w przyszłości.",
                "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}