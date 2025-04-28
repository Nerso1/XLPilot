using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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
                        ExecuteStandardAction(button, executionDirectory, xlPath);
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
        private static void ExecuteStandardAction(PilotButtonData button, string directory, XLPaths xlPath)
        {
            //MessageBox.Show($"ExecuteStandardAction called with button: {button.ButtonText}",
            //   "DEBUG - Method Entry",
            //   MessageBoxButton.OK,
            //   MessageBoxImage.Information);

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
                //MessageBox.Show("FilePath exists");
                // Determine arguments based on button context
                string arguments = DetermineArguments(button, xlPath);

                // If path exists and is a file, run it
                // Debug message
                string debugMessage = $"Uruchamianie aplikacji: {filePath} {(button.RunAsAdmin ? "(jako administrator)" : "")}";
                if (!string.IsNullOrEmpty(arguments))
                {
                    debugMessage += $" z argumentami: {arguments}";
                }

                MessageBox.Show(debugMessage, "Informacja Debugowania", MessageBoxButton.OK, MessageBoxImage.Information);

                RunExecutable(filePath, directory, button.RunAsAdmin, arguments);
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
        /// Determines the arguments to use for a button based on its context
        /// </summary>
        private static string DetermineArguments(PilotButtonData button, XLPaths xlPath)
        {
            MessageBox.Show($"DetermineArguments called with button: {button.ButtonText}\nDirectory: {button.Directory}\nArguments: {button.Arguments} \nxlPath: {xlPath}",
                   "DEBUG - Method Entry",
                   MessageBoxButton.OK,
                   MessageBoxImage.Information);

            // For XL button (no specific directory set)
            if (xlPath != null)
            {
                // If arguments is empty or "include", generate arguments from XL path
                if (string.IsNullOrEmpty(button.Arguments) || button.Arguments.Trim().ToLower() == "include")
                {
                    MessageBox.Show("If arguments is empty or \"include\", generate arguments from XL path");
                    return GenerateArguments(xlPath);
                }
                // If arguments is "skip", return empty string
                else if (button.Arguments.Trim().ToLower() == "skip")
                {
                    MessageBox.Show("If arguments is \"skip\", return empty string");
                    return string.Empty;
                }
                // Otherwise use the provided arguments
                else
                {
                    MessageBox.Show("Otherwise use the provided arguments");
                    return button.Arguments;
                }
            }
            // For Other button (directory is set)
            else
            {
                // If no arguments, return empty string
                if (string.IsNullOrEmpty(button.Arguments))
                {
                    MessageBox.Show("If no arguments, return empty string");
                    return string.Empty;
                }
                // Otherwise use the provided arguments
                else
                {
                    MessageBox.Show("Otherwise use the provided arguments");
                    return button.Arguments;
                }
            }
        }

        /// <summary>
        /// Generates arguments from XL path properties
        /// </summary>
        private static string GenerateArguments(XLPaths xlPath)
        {
            var arguments = new List<string>();

            // Add Database if it's not empty
            if (!string.IsNullOrEmpty(xlPath.Database))
            {
                arguments.Add($"baza={xlPath.Database}");
            }

            // Add FormattedLicenseInfo if it's not empty
            if (!string.IsNullOrEmpty(xlPath.FormattedLicenseInfo))
            {
                arguments.Add($"klucz={xlPath.FormattedLicenseInfo}");
            }

            // Join the arguments with a space
            return string.Join(" ", arguments);
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

                case "RejestrBat":
                    // Debug message
                    MessageBox.Show("Uruchamiam rejestr.bat ze wskazaniem ścieżki do XL-a",
                        "Informacja Debugowania", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Placeholder for actual implementation
                    RunRejestrBat(xlPath);
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
        /// Opens Registry Editor at the Data Services registry location
        /// </summary>
        private static void OpenDSServicesRegistry()
        {
            try
            {
                // Registry key to open
                string registryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Google";

                // This technique works by setting a "last key" value that regedit will open to
                // when it starts. Registry Editor reads this value when it launches.
                using (Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit"))
                {
                    // Set the last opened key
                    using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                        @"Software\Microsoft\Windows\CurrentVersion\Applets\Regedit", true))
                    {
                        key.SetValue("LastKey", registryPath);
                    }
                }

                // Now launch regedit, which will open to the location we just set
                Process.Start("regedit.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas otwierania rejestru: {ex.Message}",
                               "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Runs rejestr.bat in the XL directory with administrator privileges
        /// </summary>
        private static void RunRejestrBat(XLPaths xlPath)
        {
            try
            {
                // Check if XL path is provided
                if (xlPath == null || string.IsNullOrEmpty(xlPath.Path))
                {
                    MessageBox.Show("Brak określonego katalogu XL. Wybierz ścieżkę XL.",
                        "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Create a temporary batch file to execute commands
                string tempBatchFile = Path.Combine(Path.GetTempPath(), "run_rejestr_" + Guid.NewGuid().ToString() + ".bat");

                // Prepare batch file content
                string batchContent = $@"@echo off
                                        cd /d ""{xlPath.Path}""
                                        call rejestr.bat
                                        pause
                                        ";

                // Write the batch content to the temporary file
                File.WriteAllText(tempBatchFile, batchContent);

                // Create process info to run the batch file as administrator
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = tempBatchFile,
                    UseShellExecute = true,
                    Verb = "runas" // Run as administrator
                };

                // Start the process
                Process.Start(processInfo);

                // Schedule deletion of the temporary batch file after a delay
                // (This gives the process time to start before deleting the file)
                Task.Delay(5000).ContinueWith(t =>
                {
                    try
                    {
                        if (File.Exists(tempBatchFile))
                        {
                            File.Delete(tempBatchFile);
                        }
                    }
                    catch
                    {
                        // Ignore errors when trying to delete the temp file
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas uruchamiania rejestr.bat: {ex.Message}",
                               "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}