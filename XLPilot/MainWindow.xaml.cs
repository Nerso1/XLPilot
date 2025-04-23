using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using XLPilot.TabControls;

namespace XLPilot
{
    /// <summary>
    /// Main window of the application - contains the tabs and handles closing events
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // This will run when the user tries to close the window
            this.Closing += MainWindow_Closing;
        }

        // When the user tries to close the window, we save all data first
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveAllTabData();
        }

        // This method saves data from all tabs before the application closes
        private void SaveAllTabData()
        {
            // Save XL configuration tab data
            XLConfigTab2 xlConfigTab = FindTabInWindow<XLConfigTab2>();
            if (xlConfigTab != null)
            {
                xlConfigTab.SaveAllChanges();
            }

            // Save Other configuration tab data
            OtherConfigTab3 otherConfigTab = FindTabInWindow<OtherConfigTab3>();
            if (otherConfigTab != null)
            {
                otherConfigTab.SaveAllChanges();
            }
        }

        // This method searches for a specific type of control in the window
        // For example, it can find the XLConfigTab2 even if it's inside other controls
        private T FindTabInWindow<T>() where T : DependencyObject
        {
            // Start with the main window and look through all visual elements
            return FindVisualChild<T>(this);
        }

        // This method searches through all visual elements to find a specific type
        // It uses recursion to look through all child elements
        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            // Check each child of the parent element
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                // Get the current child
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                // If this child is the type we're looking for, return it
                if (child != null && child is T)
                {
                    return (T)child;
                }

                // Otherwise, search this child's children
                // (This is the recursive part)
                T result = FindVisualChild<T>(child);
                if (result != null)
                {
                    return result;
                }
            }

            // If we get here, we didn't find the element
            return null;
        }
    }
}