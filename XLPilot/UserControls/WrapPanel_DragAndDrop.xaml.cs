using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using XLPilot.Models;

namespace XLPilot.UserControls
{
    /// <summary>
    /// Custom control that allows dragging and dropping items in a WrapPanel
    /// </summary>
    public partial class WrapPanel_DragAndDrop : UserControl
    {
        // Event raised when items are dropped
        public event DragEventHandler ItemsDropped;

        // Properties for the items in the control
        public static readonly DependencyProperty ToolboxItemsProperty =
        DependencyProperty.Register("ToolboxItems", typeof(ObservableCollection<PilotButtonData>),
        typeof(WrapPanel_DragAndDrop), new PropertyMetadata(new ObservableCollection<PilotButtonData>()));

        public static readonly DependencyProperty ProjectItemsProperty =
        DependencyProperty.Register("ProjectItems", typeof(ObservableCollection<PilotButtonData>),
        typeof(WrapPanel_DragAndDrop), new PropertyMetadata(new ObservableCollection<PilotButtonData>()));

        // Property wrappers
        public ObservableCollection<PilotButtonData> ToolboxItems
        {
            get { return (ObservableCollection<PilotButtonData>)GetValue(ToolboxItemsProperty); }
            set { SetValue(ToolboxItemsProperty, value); }
        }

        public ObservableCollection<PilotButtonData> ProjectItems
        {
            get { return (ObservableCollection<PilotButtonData>)GetValue(ProjectItemsProperty); }
            set { SetValue(ProjectItemsProperty, value); }
        }

        // Variables for drag and drop operations
        private Point startPoint;
        private bool isDragging = false;
        private PilotButtonData draggedItem = null;
        private ListView sourceListView = null;
        private AdornerLayer adornerLayer = null;
        private InsertionAdorner insertionAdorner = null;

        // Track whether we've made changes
        private bool hasChanges = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public WrapPanel_DragAndDrop()
        {
            InitializeComponent();

            // Set DataContext to this instance so bindings work properly
            this.DataContext = this;
        }

        /// <summary>
        /// Event handler for mouse button down
        /// </summary>
        private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get the ListView and store it
            sourceListView = sender as ListView;
            if (sourceListView == null) return;

            // Store the mouse position
            startPoint = e.GetPosition(null);

            // Reset dragging state
            isDragging = false;
        }

        /// <summary>
        /// Event handler for mouse movement
        /// </summary>
        private void ListView_MouseMove(object sender, MouseEventArgs e)
        {
            // Only proceed if the left mouse button is pressed and we're not already dragging
            if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
            {
                // Check if the mouse has moved far enough to begin a drag operation
                Point currentPosition = e.GetPosition(null);

                // Calculate how far the mouse has moved
                double xDistance = Math.Abs(startPoint.X - currentPosition.X);
                double yDistance = Math.Abs(startPoint.Y - currentPosition.Y);

                // If the mouse has moved far enough, start dragging
                if (xDistance > SystemParameters.MinimumHorizontalDragDistance ||
                    yDistance > SystemParameters.MinimumVerticalDragDistance)
                {
                    // Get the ListView
                    ListView listView = sender as ListView;
                    if (listView == null) return;

                    // Find the ListViewItem being dragged
                    ListViewItem listViewItem = FindParentListViewItem(e.OriginalSource as DependencyObject);

                    // If we couldn't find the item by the original source, use hit testing
                    if (listViewItem == null)
                    {
                        // Hit test to find the item under the mouse
                        HitTestResult result = VisualTreeHelper.HitTest(listView, currentPosition);
                        if (result != null)
                        {
                            listViewItem = FindParentListViewItem(result.VisualHit);
                        }
                    }

                    // If we found a ListViewItem, proceed with drag operation
                    if (listViewItem != null)
                    {
                        // Get the data item from the ListViewItem
                        object itemObj = listView.ItemContainerGenerator.ItemFromContainer(listViewItem);

                        // Make sure it's a PilotButtonData
                        PilotButtonData item = itemObj as PilotButtonData;
                        if (item == null) return;

                        // Store the dragged item and set the dragging flag
                        draggedItem = item;
                        isDragging = true;

                        // Show the trash indicator if dragging from project panel
                        if (listView == projectListView)
                        {
                            trashIndicator.Visibility = Visibility.Visible;
                        }

                        // Handle the event to prevent other handlers from processing it
                        e.Handled = true;

                        // Start the drag operation
                        DataObject dragData = new DataObject("DraggedPilotButton", draggedItem);

                        // Store whether the source is the toolbox
                        dragData.SetData("SourceIsToolbox", listView == toolboxListView);

                        // Start the drag and drop operation
                        DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Copy | DragDropEffects.Move);

                        // Hide the trash indicator when drag operation completes
                        trashIndicator.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for drag enter
        /// </summary>
        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the data is a PilotButtonData
            if (!e.Data.GetDataPresent("DraggedPilotButton"))
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            // Get the target ListView
            ListView targetListView = sender as ListView;
            if (targetListView == null)
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            // Check if the source is the toolbox
            object sourceIsToolboxObj = e.Data.GetData("SourceIsToolbox");
            if (!(sourceIsToolboxObj is bool))
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            bool sourceIsToolbox = (bool)sourceIsToolboxObj;

            // Determine the allowed effects based on the source and target
            if (targetListView == toolboxListView)
            {
                if (!sourceIsToolbox)
                {
                    // From project to toolbox - allow drop (will delete)
                    e.Effects = DragDropEffects.Move;
                }
                else
                {
                    // From toolbox to toolbox - prevent drop
                    e.Effects = DragDropEffects.None;
                }
            }
            else // target is project
            {
                if (sourceIsToolbox)
                {
                    // From toolbox to project - copy
                    e.Effects = DragDropEffects.Copy;
                }
                else
                {
                    // From project to project - move (rearrange)
                    e.Effects = DragDropEffects.Move;
                }
            }
        }

        /// <summary>
        /// Event handler for drag over
        /// </summary>
        private void ListView_DragOver(object sender, DragEventArgs e)
        {
            // Check if the data is a PilotButtonData
            if (!e.Data.GetDataPresent("DraggedPilotButton"))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            // Get the target ListView
            ListView targetListView = sender as ListView;
            if (targetListView == null)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            // Check if the source is the toolbox
            object sourceIsToolboxObj = e.Data.GetData("SourceIsToolbox");
            if (!(sourceIsToolboxObj is bool))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            bool sourceIsToolbox = (bool)sourceIsToolboxObj;

            // Only show insertion indicator for the project panel
            if (targetListView == projectListView &&
                (sourceIsToolbox || (!sourceIsToolbox && sourceListView == projectListView)))
            {
                // Get the WrapPanel
                WrapPanel wrapPanel = FindVisualChild<WrapPanel>(targetListView);
                if (wrapPanel == null)
                {
                    e.Handled = true;
                    return;
                }

                // Find the insertion point
                Point mousePos = e.GetPosition(wrapPanel);
                UIElement targetElement = null;
                int insertIndex = -1;
                bool insertAfter = false;

                // Find the closest item to the mouse position
                double minDistance = double.MaxValue;

                // Check each child element
                for (int i = 0; i < wrapPanel.Children.Count; i++)
                {
                    UIElement child = wrapPanel.Children[i];

                    // Get the bounds of the child element
                    Point childPos = child.TranslatePoint(new Point(0, 0), wrapPanel);
                    Size childSize = child.RenderSize;

                    // Calculate center point of the child
                    Point childCenter = new Point(
                        childPos.X + childSize.Width / 2,
                        childPos.Y + childSize.Height / 2);

                    // Calculate distance to the center
                    double distance = Math.Sqrt(
                        Math.Pow(mousePos.X - childCenter.X, 2) +
                        Math.Pow(mousePos.Y - childCenter.Y, 2));

                    // If this is closer than the previous closest, update
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        targetElement = child;

                        // Determine if we should insert before or after this element
                        insertAfter = (mousePos.X > childCenter.X);

                        // Find the index of this item in the collection
                        ListViewItem item = child as ListViewItem;
                        if (item != null)
                        {
                            insertIndex = targetListView.ItemContainerGenerator.IndexFromContainer(item);
                            if (insertAfter)
                                insertIndex++;
                        }
                    }
                }

                // If no children, insert at beginning
                if (wrapPanel.Children.Count == 0)
                {
                    insertIndex = 0;
                }

                // Show the insertion indicator
                if (targetElement != null)
                {
                    ShowInsertionAdorner(targetListView, targetElement, insertAfter);
                }

                // Store the insertion index in the DragEventArgs
                e.Data.SetData("InsertionIndex", insertIndex);
            }
            else
            {
                // Remove any existing adorner
                RemoveInsertionAdorner();
            }

            e.Handled = true;
        }

        /// <summary>
        /// Event handler for drop
        /// </summary>
        private void ListView_Drop(object sender, DragEventArgs e)
        {
            // Remove any existing adorner
            RemoveInsertionAdorner();

            // Check if the data is a PilotButtonData
            if (!e.Data.GetDataPresent("DraggedPilotButton")) return;

            // Get the dropped item
            PilotButtonData droppedItem = e.Data.GetData("DraggedPilotButton") as PilotButtonData;
            if (droppedItem == null) return;

            // Get the target ListView
            ListView targetListView = sender as ListView;
            if (targetListView == null) return;

            // Check if the source is the toolbox
            object sourceIsToolboxObj = e.Data.GetData("SourceIsToolbox");
            if (!(sourceIsToolboxObj is bool)) return;

            bool sourceIsToolbox = (bool)sourceIsToolboxObj;

            // Set the hasChanges flag to true
            hasChanges = true;

            // Handle drop based on source and target
            if (targetListView == toolboxListView)
            {
                if (!sourceIsToolbox)
                {
                    // From project to toolbox - delete from project
                    RemoveItemFromProjectItems(droppedItem);
                }
            }
            else // target is project
            {
                // Get the insertion index
                int insertIndex = ProjectItems.Count;
                if (e.Data.GetDataPresent("InsertionIndex"))
                {
                    object indexObj = e.Data.GetData("InsertionIndex");
                    if (indexObj is int)
                    {
                        insertIndex = (int)indexObj;
                    }
                }

                if (sourceIsToolbox)
                {
                    // From toolbox to project - copy to project
                    AddItemToProjectItems(droppedItem, insertIndex);
                }
                else if (sourceListView == projectListView)
                {
                    // Rearranging within project - move within project
                    MoveItemWithinProjectItems(droppedItem, insertIndex);
                }
            }

            // Reset dragging state
            isDragging = false;
            draggedItem = null;
            sourceListView = null;

            // Hide the trash indicator when drag operation completes
            trashIndicator.Visibility = Visibility.Collapsed;

            // Notify the parent of the drop event if changes were made
            if (hasChanges)
            {
                ItemsDropped?.Invoke(this, e);
                hasChanges = false;
            }
        }

        /// <summary>
        /// Removes an item from the ProjectItems collection
        /// </summary>
        private void RemoveItemFromProjectItems(PilotButtonData item)
        {
            // Find the item to remove
            PilotButtonData itemToRemove = null;

            foreach (var projectItem in ProjectItems)
            {
                // Check if this is the item we're looking for
                if (projectItem.ButtonText == item.ButtonText &&
                    projectItem.FileName == item.FileName &&
                    projectItem.ImageSource == item.ImageSource &&
                    projectItem.RunAsAdmin == item.RunAsAdmin &&
                    projectItem.Arguments == item.Arguments &&
                    projectItem.ToolTipText == item.ToolTipText &&
                    projectItem.Directory == item.Directory)
                {
                    itemToRemove = projectItem;
                    break;
                }
            }

            // Remove the item if found
            if (itemToRemove != null)
            {
                ProjectItems.Remove(itemToRemove);
            }
        }

        /// <summary>
        /// Adds an item to the ProjectItems collection
        /// </summary>
        private void AddItemToProjectItems(PilotButtonData item, int insertIndex)
        {
            // Create a new item with the same properties
            PilotButtonData newItem = new PilotButtonData(
                item.ButtonText,
                item.FileName,
                item.ImageSource,
                item.RunAsAdmin,
                item.Arguments,
                item.ToolTipText,
                item.Directory
            );

            // Add it at the specified index
            if (insertIndex >= 0 && insertIndex <= ProjectItems.Count)
            {
                ProjectItems.Insert(insertIndex, newItem);
            }
            else
            {
                ProjectItems.Add(newItem);
            }
        }

        /// <summary>
        /// Moves an item within the ProjectItems collection
        /// </summary>
        private void MoveItemWithinProjectItems(PilotButtonData item, int insertIndex)
        {
            // Find the index of the item
            int sourceIndex = -1;

            for (int i = 0; i < ProjectItems.Count; i++)
            {
                // Check if this is the item we're looking for
                if (ProjectItems[i].ButtonText == item.ButtonText &&
                    ProjectItems[i].FileName == item.FileName &&
                    ProjectItems[i].ImageSource == item.ImageSource &&
                    ProjectItems[i].RunAsAdmin == item.RunAsAdmin &&
                    ProjectItems[i].Arguments == item.Arguments &&
                    ProjectItems[i].ToolTipText == item.ToolTipText &&
                    ProjectItems[i].Directory == item.Directory)
                {
                    sourceIndex = i;
                    break;
                }
            }

            // If found, move it
            if (sourceIndex != -1)
            {
                // Get the item
                PilotButtonData movedItem = ProjectItems[sourceIndex];

                // Remove it from its current position
                ProjectItems.RemoveAt(sourceIndex);

                // Adjust insertion index if necessary
                if (insertIndex > sourceIndex)
                {
                    insertIndex--;
                }

                // Insert it at the new position
                if (insertIndex >= 0 && insertIndex <= ProjectItems.Count)
                {
                    ProjectItems.Insert(insertIndex, movedItem);
                }
                else
                {
                    ProjectItems.Add(movedItem);
                }
            }
        }

        /// <summary>
        /// Event handler for drag leave
        /// </summary>
        private void ListView_DragLeave(object sender, DragEventArgs e)
        {
            // Remove any insertion adorner
            RemoveInsertionAdorner();
        }

        /// <summary>
        /// Shows an insertion adorner at the specified position
        /// </summary>
        private void ShowInsertionAdorner(ListView listView, UIElement targetElement, bool insertAfter)
        {
            // Remove any existing adorner
            RemoveInsertionAdorner();

            // Get the adorner layer for the ListView
            adornerLayer = AdornerLayer.GetAdornerLayer(listView);

            if (adornerLayer != null)
            {
                // Create a new insertion adorner
                insertionAdorner = new InsertionAdorner(targetElement, insertAfter);

                // Add it to the adorner layer
                adornerLayer.Add(insertionAdorner);
            }
        }

        /// <summary>
        /// Removes the insertion adorner
        /// </summary>
        private void RemoveInsertionAdorner()
        {
            // If we have an adorner layer and an insertion adorner
            if (adornerLayer != null && insertionAdorner != null)
            {
                // Remove the adorner
                adornerLayer.Remove(insertionAdorner);
                insertionAdorner = null;
            }
        }

        /// <summary>
        /// Finds a parent ListViewItem for a given element
        /// </summary>
        private static ListViewItem FindParentListViewItem(DependencyObject element)
        {
            // Start with the given element
            DependencyObject current = element;

            // Walk up the visual tree until we find a ListViewItem or run out of parents
            while (current != null && !(current is ListViewItem))
            {
                current = VisualTreeHelper.GetParent(current);
            }

            // Return the result (will be null if no ListViewItem was found)
            return current as ListViewItem;
        }

        /// <summary>
        /// Finds a child element of a specific type
        /// </summary>
        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            // If the parent is null, return default value
            if (parent == null) return null;

            // Check each child
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                // Get the current child
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                // If this child is the type we're looking for, return it
                if (child is T)
                {
                    return (T)child;
                }
                else
                {
                    // Otherwise, search this child's children
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }

            // If we get here, we didn't find anything
            return null;
        }

        /// <summary>
        /// Event handler for mouse wheel to enable scrolling
        /// </summary>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Get the ScrollViewer
            ScrollViewer scrollViewer = (ScrollViewer)sender;

            // Scroll up or down based on the mouse wheel delta
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);

            // Mark the event as handled
            e.Handled = true;
        }
    }

    /// <summary>
    /// Adorner class for showing the insertion point during drag and drop
    /// </summary>
    public class InsertionAdorner : Adorner
    {
        // Whether to insert after the target element
        private readonly bool insertAfter;

        // Pen for drawing the insertion line
        private static readonly Pen pen;

        // Triangle shape for the insertion indicator
        private static readonly PathGeometry triangle;

        /// <summary>
        /// Static constructor - initializes the pen and triangle
        /// </summary>
        static InsertionAdorner()
        {
            // Create a red pen for drawing the insertion line
            pen = new Pen(Brushes.Red, 2);
            pen.Freeze(); // Freezing makes it more efficient

            // Create a triangle for the insertion indicator
            // The triangle points in the direction of the insertion
            LineSegment firstLine = new LineSegment(new Point(0, 5), false);
            LineSegment secondLine = new LineSegment(new Point(0, -5), false);

            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(5, 0);
            figure.Segments.Add(firstLine);
            figure.Segments.Add(secondLine);
            figure.IsClosed = true;

            triangle = new PathGeometry();
            triangle.Figures.Add(figure);
            triangle.Freeze(); // Freezing makes it more efficient
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InsertionAdorner(UIElement adornedElement, bool insertAfter)
            : base(adornedElement)
        {
            this.insertAfter = insertAfter;
            this.IsHitTestVisible = false; // Don't interfere with mouse events
        }

        /// <summary>
        /// Draw the insertion indicator
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            Point startPoint, endPoint;

            // Calculate the insertion line position based on whether we're inserting before or after
            if (insertAfter)
            {
                // If inserting after, draw on the right side
                startPoint = new Point(AdornedElement.RenderSize.Width, 0);
                endPoint = new Point(AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height);
            }
            else
            {
                // If inserting before, draw on the left side
                startPoint = new Point(0, 0);
                endPoint = new Point(0, AdornedElement.RenderSize.Height);
            }

            // Draw the insertion line
            drawingContext.DrawLine(pen, startPoint, endPoint);

            // Draw triangles at the top and bottom of the line
            // These help make the insertion point more visible

            // Draw the top triangle
            drawingContext.PushTransform(new TranslateTransform(startPoint.X, startPoint.Y));
            drawingContext.DrawGeometry(Brushes.Red, null, triangle);
            drawingContext.Pop();

            // Draw the bottom triangle
            drawingContext.PushTransform(new TranslateTransform(endPoint.X, endPoint.Y));
            drawingContext.DrawGeometry(Brushes.Red, null, triangle);
            drawingContext.Pop();
        }
    }
}