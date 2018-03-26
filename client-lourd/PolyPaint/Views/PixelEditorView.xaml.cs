using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using PolyPaint.ViewModels;

namespace PolyPaint.Views
{
    public partial class PixelEditorView : Window
    {
        // Selector Attributes
        private bool _isMouseDownSelector; // Set to 'true' when mouse is held down.
        private Point _mouseDownPositionSelector; // The point where the mouse button was clicked down.

        // Drawing Attributes
        private Point _newPositionDrawing;
        private Point _oldPositionDrawing;

        public PixelEditorView()
        {
            InitializeComponent();
            DataContext = new PixelEditorViewModel();
        }

        private void GlisserCommence(object sender, DragStartedEventArgs e)
        {
            (sender as Thumb).Background = Brushes.Black;
        }

        private void GlisserTermine(object sender, DragCompletedEventArgs e)
        {
            (sender as Thumb).Background = Brushes.White;
        }

        private void GlisserMouvementRecu(object sender, DragDeltaEventArgs e)
        {
            string nom = (sender as Thumb).Name;
            if (nom == "horizontal" || nom == "diagonal")
            {
                colonne.Width = new GridLength(Math.Max(32, colonne.Width.Value + e.HorizontalChange));
            }

            if (nom == "vertical" || nom == "diagonal")
            {
                ligne.Height = new GridLength(Math.Max(32, ligne.Height.Value + e.VerticalChange));
            }
        }

        private void DrawingSurfaceMouseEnter(object sender, MouseEventArgs e)
        {
            (DataContext as PixelEditorViewModel)?.PixelCursors(DisplayArea);
            _oldPositionDrawing = e.GetPosition(DrawingSurface);
        }

        private void DrawingSurfacePreviewMouseDown(object sender, MouseEventArgs e)
        {
            _oldPositionDrawing = e.GetPosition(DrawingSurface);

            //The tool is selected on click with a distance of one pixel to
            //enable it
            Point onePixelPoint = new Point(_oldPositionDrawing.X + 1, _oldPositionDrawing.Y);
            (DataContext as PixelEditorViewModel)?.PixelDraw(_oldPositionDrawing, onePixelPoint);
        }

        private void DrawingSurfaceMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //Action tool on mouse move
                _newPositionDrawing = e.GetPosition(DrawingSurface);
                (DataContext as PixelEditorViewModel)?.PixelDraw(_oldPositionDrawing, _newPositionDrawing);
                _oldPositionDrawing = _newPositionDrawing;
            }

            // Blit the selector when another tool is selected
            if ((DataContext as PixelEditorViewModel)?.ToolSelected != "selector")
            {
                (DataContext as PixelEditorViewModel)?.BlitZoneSelector();
                SelectedZoneThumb.Visibility = Visibility.Hidden;
            }
        }

        private void GridSurfaceMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Capture and track the mouse.
            _isMouseDownSelector = true;
            _mouseDownPositionSelector = e.GetPosition(DisplayArea);
            DrawingSurface.CaptureMouse();

            // Initial placement of the drag selection box.         
            Canvas.SetLeft(selectionBox, _mouseDownPositionSelector.X);
            Canvas.SetTop(selectionBox, _mouseDownPositionSelector.Y);
            selectionBox.Width = 0;
            selectionBox.Height = 0;

            // Blit the selector on the original drawing
            //during the edition on the original draw when
            //clicked outside the selector. A new selectionBox appears then
            if ((DataContext as PixelEditorViewModel)?.ToolSelected == "selector"
                && !e.OriginalSource.Equals(SelectedZoneThumb))
            {
                // Make the drag selection box visible.           
                selectionBox.Visibility = Visibility.Visible;

                if ((DataContext as PixelEditorViewModel).IsWriteableBitmapOnEdition)
                {
                    (DataContext as PixelEditorViewModel)?.BlitZoneSelector();
                }
            }
        }

        private void GridSurfaceMouseUp(object sender, MouseButtonEventArgs e)
        {
            // Release the mouse capture and stop tracking it.
            _isMouseDownSelector = false;
            DrawingSurface.ReleaseMouseCapture();

            // Hide the drag selection box.
            selectionBox.Visibility = Visibility.Collapsed;
            Point mouseUpPosition = e.GetPosition(DisplayArea);

            // The mouse has been released, select the pixel in this rectangle
            if (!(DataContext as PixelEditorViewModel).IsWriteableBitmapOnEdition
                && (DataContext as PixelEditorViewModel)?.ToolSelected == "selector")
            {
                SelectedZoneThumb.Visibility = _mouseDownPositionSelector.Equals(mouseUpPosition)
                                               ? Visibility.Hidden
                                               : Visibility.Visible;
                
                // Fonction of the selection box
                (DataContext as PixelEditorViewModel)?.ZoneSelector(SelectedZoneThumb, _mouseDownPositionSelector, mouseUpPosition);
            }
        }

        private void GridSurfaceMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePosition = e.GetPosition(DrawingSurface);

            // Confine the selectionBox in the DrawingSurface
            //When the selection box gets out of the border, reposition the extremities. 
            mousePosition.X = mousePosition.X < 0 ? 0 : mousePosition.X;
            mousePosition.X = mousePosition.X > DrawingSurface.ActualWidth ? mousePosition.X = DrawingSurface.ActualWidth : mousePosition.X;
            mousePosition.Y = mousePosition.Y < 0 ? 0 : mousePosition.Y;
            mousePosition.Y = mousePosition.Y > DrawingSurface.ActualHeight ? mousePosition.Y = DrawingSurface.ActualHeight : mousePosition.Y;

            if (_isMouseDownSelector)
            {
                // When the mouse is held down, reposition the drag selection box.

                if (_mouseDownPositionSelector.X < mousePosition.X)
                {
                    Canvas.SetLeft(selectionBox, _mouseDownPositionSelector.X);
                    selectionBox.Width = mousePosition.X - _mouseDownPositionSelector.X;
                }
                else
                {
                    Canvas.SetLeft(selectionBox, mousePosition.X);
                    selectionBox.Width = _mouseDownPositionSelector.X - mousePosition.X;
                }

                if (_mouseDownPositionSelector.Y < mousePosition.Y)
                {
                    Canvas.SetTop(selectionBox, _mouseDownPositionSelector.Y);
                    selectionBox.Height = mousePosition.Y - _mouseDownPositionSelector.Y;
                }
                else
                {
                    Canvas.SetTop(selectionBox, mousePosition.Y);
                    selectionBox.Height = _mouseDownPositionSelector.Y - mousePosition.Y;
                }
            }
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            UIElement thumb = e.Source as UIElement;

            Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) + e.HorizontalChange);
            Canvas.SetTop(thumb, Canvas.GetTop(thumb) + e.VerticalChange);

            //  The cropWriteableBitmap on the selected zone is also moved
            Point position = new Point(Canvas.GetLeft(thumb), Canvas.GetTop(thumb));
            (DataContext as PixelEditorViewModel)?.ChangeCropWriteableBitmapPosition(position);
        }
    }
}
