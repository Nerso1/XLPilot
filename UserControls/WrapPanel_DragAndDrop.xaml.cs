using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace XLPilot.UserControls
{
    /// <summary>
    /// Interaction logic for WrapPanel_DragAndDrop.xaml
    /// </summary>
    public partial class WrapPanel_DragAndDrop : UserControl
    {
        // Custom class to hold button data
        public class PilotButtonData
        {
            public string ImageSource { get; set; }
            public string ButtonText { get; set; }

            public PilotButtonData(string imageSource, string buttonText)
            {
                ImageSource = imageSource;
                ButtonText = buttonText;
            }
        }

        public ObservableCollection<PilotButtonData> ToolboxItems { get; set; }
        public ObservableCollection<PilotButtonData> ProjectItems { get; set; }

        private Point startPoint;
        private bool isDragging = false;
        private PilotButtonData draggedItem = null;
        private ListView sourceListView = null;
        private AdornerLayer adornerLayer = null;
        private InsertionAdorner insertionAdorner = null;

        // Constructor
        public WrapPanel_DragAndDrop()
        {
            InitializeComponent();

            // Initialize the collections with sample data
            ToolboxItems = new ObservableCollection<PilotButtonData>
            {
                new PilotButtonData("/XLPilot;component/Resources/Images/Google chrome icon.png", "Goblin"),
                new PilotButtonData("/XLPilot;component/Resources/Images/detault-profile-picture.png", "Ship")
            };

            ProjectItems = new ObservableCollection<PilotButtonData>
            {
                new PilotButtonData("/XLPilot;component/Resources/Images/Google chrome icon.png", "Project Ship"),
            };

            // Set DataContext to this instance so bindings work properly
            this.DataContext = this;
        }

        private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get the ListView and the clicked item
            sourceListView = sender as ListView;
            if (sourceListView == null) return;

            // Store the mouse position
            startPoint = e.GetPosition(null);

            // Reset dragging state
            isDragging = false;
        }

        private void ListView_MouseMove(object sender, MouseEventArgs e)
        {
            // Only proceed if the left mouse button is pressed and we're not already dragging
            if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
            {
                // Check if the mouse has moved far enough to begin a drag operation
                Point currentPosition = e.GetPosition(null);
                Vector diff = startPoint - currentPosition;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    ListView listView = sender as ListView;
                    if (listView == null) return;

                    // We need to get the ListViewItem being dragged
                    // First, try to find the ListViewItem directly from the original source
                    ListViewItem listViewItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

                    // If not found, try to use hit testing to find the item under the mouse
                    if (listViewItem == null)
                    {
                        HitTestResult result = VisualTreeHelper.HitTest(listView, currentPosition);
                        if (result != null)
                        {
                            listViewItem = FindAncestor<ListViewItem>(result.VisualHit);
                        }
                    }

                    // If we found a ListViewItem, proceed with drag operation
                    if (listViewItem != null)
                    {
                        // Get the dragged item
                        object itemObj = listView.ItemContainerGenerator.ItemFromContainer(listViewItem);
                        PilotButtonData item = itemObj as PilotButtonData;
                        if (item == null) return;

                        draggedItem = item;
                        isDragging = true;

                        // Prevent PilotButton's click animation from interfering
                        // by handling the event
                        e.Handled = true;

                        // Start the drag and drop operation with additional information about source
                        DataObject dragData = new DataObject("DraggedPilotButton", draggedItem);
                        dragData.SetData("SourceIsToolbox", listView == toolboxListView);
                        DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Copy | DragDropEffects.Move);
                    }
                }
            }
        }

        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("DraggedPilotButton"))
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            ListView targetListView = sender as ListView;
            if (targetListView == null)
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            object sourceIsToolboxObj = e.Data.GetData("SourceIsToolbox");
            if (!(sourceIsToolboxObj is bool))
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            bool sourceIsToolbox = (bool)sourceIsToolboxObj;

            // If target is toolbox and source is project, allow Move (delete)
            // If target is project, allow Copy (from toolbox) or Move (rearrange within project)
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

        private void ListView_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("DraggedPilotButton"))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            ListView targetListView = sender as ListView;
            if (targetListView == null)
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            object sourceIsToolboxObj = e.Data.GetData("SourceIsToolbox");
            if (!(sourceIsToolboxObj is bool))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            bool sourceIsToolbox = (bool)sourceIsToolboxObj;

            // Only show insertion indicator for the project panel,
            // or when rearranging within the project panel
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

                // Find the closest item to the mouse position
                double minDistance = double.MaxValue;
                bool insertAfter = false;

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

                    // Calculate distance to center
                    double distance = Math.Sqrt(
                        Math.Pow(mousePos.X - childCenter.X, 2) +
                        Math.Pow(mousePos.Y - childCenter.Y, 2));

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        targetElement = child;

                        // Determine if we should insert before or after this element
                        insertAfter = (mousePos.X > childCenter.X);

                        // Get the index in the data collection (not visual tree)
                        int visualIndex = wrapPanel.Children.IndexOf(child);
                        ListViewItem item = wrapPanel.Children[visualIndex] as ListViewItem;
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

                // Show adorner at the insertion point if we have a target element
                if (targetElement != null)
                {
                    ShowInsertionAdorner(targetListView, targetElement, insertAfter);
                }

                // Store the insertion index in the DragEventArgs
                e.Data.SetData("InsertionIndex", insertIndex);
            }
            else
            {
                // Remove any existing adorner if we're over the toolbox
                RemoveInsertionAdorner();
            }

            e.Handled = true;
        }

        private void ListView_Drop(object sender, DragEventArgs e)
        {
            // Remove any existing adorner
            RemoveInsertionAdorner();

            if (!e.Data.GetDataPresent("DraggedPilotButton")) return;

            PilotButtonData droppedItem = e.Data.GetData("DraggedPilotButton") as PilotButtonData;
            if (droppedItem == null) return;

            ListView targetListView = sender as ListView;
            if (targetListView == null) return;

            object sourceIsToolboxObj = e.Data.GetData("SourceIsToolbox");
            if (!(sourceIsToolboxObj is bool)) return;

            bool sourceIsToolbox = (bool)sourceIsToolboxObj;

            // Handle drop based on source and target panels
            if (targetListView == toolboxListView)
            {
                if (!sourceIsToolbox)
                {
                    // From project to toolbox - delete from project
                    // Find the item in the ProjectItems by comparing properties
                    PilotButtonData itemToRemove = null;
                    foreach (var item in ProjectItems)
                    {
                        if (item.ImageSource == droppedItem.ImageSource &&
                            item.ButtonText == droppedItem.ButtonText)
                        {
                            itemToRemove = item;
                            break;
                        }
                    }
                    if (itemToRemove != null)
                    {
                        ProjectItems.Remove(itemToRemove);
                    }
                }
            }
            else // target is project
            {
                // Get the insertion index (default to end of list if not available)
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
                    // From toolbox to project - copy to project (no removal from toolbox)
                    // Create a new instance with the same properties
                    PilotButtonData newItem = new PilotButtonData(
                        droppedItem.ImageSource,
                        droppedItem.ButtonText
                    );

                    if (insertIndex >= 0 && insertIndex <= ProjectItems.Count)
                    {
                        ProjectItems.Insert(insertIndex, newItem);
                    }
                    else
                    {
                        ProjectItems.Add(newItem);
                    }
                }
                else if (sourceListView == projectListView)
                {
                    // Rearranging within project - move within project
                    // Find the item in the ProjectItems by comparing properties
                    int sourceIndex = -1;
                    for (int i = 0; i < ProjectItems.Count; i++)
                    {
                        if (ProjectItems[i].ImageSource == droppedItem.ImageSource &&
                            ProjectItems[i].ButtonText == droppedItem.ButtonText)
                        {
                            sourceIndex = i;
                            break;
                        }
                    }

                    if (sourceIndex != -1)
                    {
                        PilotButtonData movedItem = ProjectItems[sourceIndex];
                        ProjectItems.RemoveAt(sourceIndex);

                        // Adjust insertion index if necessary
                        if (insertIndex > sourceIndex)
                        {
                            insertIndex--;
                        }

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
            }

            // Reset dragging state
            isDragging = false;
            draggedItem = null;
            sourceListView = null;
        }

        private void ListView_DragLeave(object sender, DragEventArgs e)
        {
            RemoveInsertionAdorner();
        }

        private void ShowInsertionAdorner(ListView listView, UIElement targetElement, bool insertAfter)
        {
            RemoveInsertionAdorner();

            adornerLayer = AdornerLayer.GetAdornerLayer(listView);
            if (adornerLayer != null)
            {
                insertionAdorner = new InsertionAdorner(targetElement, insertAfter);
                adornerLayer.Add(insertionAdorner);
            }
        }

        private void RemoveInsertionAdorner()
        {
            if (adornerLayer != null && insertionAdorner != null)
            {
                adornerLayer.Remove(insertionAdorner);
                insertionAdorner = null;
            }
        }

        // Helper method to find a parent of a specific type
        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null && !(current is T))
            {
                current = VisualTreeHelper.GetParent(current);
            }
            return current as T;
        }

        // Helper method to find a child of a specific type
        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return default(T);

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }

            return default(T);
        }
    }

    // Adorner class for insertion indicator
    public class InsertionAdorner : Adorner
    {
        private readonly bool insertAfter;
        private static readonly Pen pen;
        private static readonly PathGeometry triangle;

        static InsertionAdorner()
        {
            // Create a pen for drawing the insertion line
            pen = new Pen(Brushes.Red, 2);
            pen.Freeze();

            // Create a triangle for the insertion indicator
            LineSegment firstLine = new LineSegment(new Point(0, 5), false);
            LineSegment secondLine = new LineSegment(new Point(0, -5), false);

            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(5, 0);
            figure.Segments.Add(firstLine);
            figure.Segments.Add(secondLine);
            figure.IsClosed = true;

            triangle = new PathGeometry();
            triangle.Figures.Add(figure);
            triangle.Freeze();
        }

        public InsertionAdorner(UIElement adornedElement, bool insertAfter)
            : base(adornedElement)
        {
            this.insertAfter = insertAfter;
            this.IsHitTestVisible = false;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Point startPoint, endPoint;

            if (insertAfter)
            {
                startPoint = new Point(AdornedElement.RenderSize.Width, 0);
                endPoint = new Point(AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height);
            }
            else
            {
                startPoint = new Point(0, 0);
                endPoint = new Point(0, AdornedElement.RenderSize.Height);
            }

            // Draw insertion line
            drawingContext.DrawLine(pen, startPoint, endPoint);

            // Draw triangles at top and bottom of the line
            drawingContext.PushTransform(new TranslateTransform(startPoint.X, startPoint.Y));
            drawingContext.DrawGeometry(Brushes.Red, null, triangle);
            drawingContext.Pop();

            drawingContext.PushTransform(new TranslateTransform(endPoint.X, endPoint.Y));
            drawingContext.DrawGeometry(Brushes.Red, null, triangle);
            drawingContext.Pop();
        }
    }
}