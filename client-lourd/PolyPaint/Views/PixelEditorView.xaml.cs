using System;
using System.Diagnostics;
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
        private bool _controlResized;

        // Selector Attributes
        private bool _isMouseDownSelector; // Set to 'true' when mouse is held down.
        private Point _mouseDownPositionSelector; // The point where the mouse button was clicked down.

        // Drawing Attributes
        private Point _newPositionDrawing;
        private Point _oldPositionDrawing;

        private Rect _regionBeforeResize = Rect.Empty;

        public PixelEditorView()
        {
            InitializeComponent();
            DataContext = new PixelEditorViewModel(DrawingSurface);
            PreviewMouseUp += OnGlobalMouseUp;
        }

        private void PixelEditorActivated(object sender, EventArgs e)
        {
            (DataContext as PixelEditorViewModel)?.OpenFirstTimeTutorial("pixelStep");
        }

        private void OnGlobalMouseUp(object sender, MouseButtonEventArgs args)
        {
            if (_controlResized)
            {
                if (DataContext is PixelEditorViewModel vm)
                {
                    Point upperLeftCorner =
                        ContentControl.TransformToAncestor(SelectedZoneCanvas).Transform(new Point(0, 0));
                    Point lowerRightCorner = new Point(upperLeftCorner.X + ContentControl.ActualWidth,
                                                       upperLeftCorner.Y + ContentControl.ActualHeight);

                    vm.SelectTempZone(lowerRightCorner, upperLeftCorner);
                    if (!_regionBeforeResize.IsEmpty)
                    {
                        vm.BlitOnDrawing(ContentControl, vm.CropWriteableBitmap, false, _regionBeforeResize);
                        _regionBeforeResize = Rect.Empty;
                    }
                }

                _controlResized = false;
            }
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
            _oldPositionDrawing = new Point(_oldPositionDrawing.X / 1.90, _oldPositionDrawing.Y / 1.90);
        }

        private void DrawingSurfacePreviewMouseDown(object sender, MouseEventArgs e)
        {
            _oldPositionDrawing = e.GetPosition(DrawingSurface);
            _oldPositionDrawing = new Point(_oldPositionDrawing.X / 1.90, _oldPositionDrawing.Y / 1.90);

            //The tool is selected on click with a distance of one pixel to
            //enable it
            Point onePixelPoint = new Point(_oldPositionDrawing.X + 1, _oldPositionDrawing.Y);
            (DataContext as PixelEditorViewModel)?.PixelDraw(_oldPositionDrawing, onePixelPoint);

            // Blit the selector when another tool is selected
            if ((DataContext as PixelEditorViewModel)?.ToolSelected != "selector"
                && (DataContext as PixelEditorViewModel)?.CropWriteableBitmap != null)
            {
                ((PixelEditorViewModel) DataContext)?.BlitOnDrawing(ContentControl,
                                                                    ((PixelEditorViewModel) DataContext)
                                                                    ?.CropWriteableBitmap,
                                                                    true);

                // Hide the selector and the adorners
                foreach (Control child in SelectedZoneCanvas.Children)
                {
                    Selector.SetIsSelected(child, false);
                }

                SelectedZoneCanvas.Visibility = Visibility.Collapsed;
            }

            if ((DataContext as PixelEditorViewModel)?.ToolSelected == "fill")
            {
                ((PixelEditorViewModel) DataContext)?.Fill(_oldPositionDrawing,
                                                           DrawingSurface.ActualWidth, DrawingSurface.ActualHeight);
            }
        }

        private void DrawingSurfaceMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //Action tool on mouse move
                _newPositionDrawing = e.GetPosition(DrawingSurface);
                _newPositionDrawing = new Point(_newPositionDrawing.X / 1.90, _newPositionDrawing.Y / 1.90);
                (DataContext as PixelEditorViewModel)?.PixelDraw(_oldPositionDrawing, _newPositionDrawing);
                _oldPositionDrawing = _newPositionDrawing;
            }
        }

        private void GridSurfaceMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Capture and track the mouse.
            _isMouseDownSelector = true;
            _mouseDownPositionSelector = e.GetPosition(DrawingSurface);
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
                && !e.OriginalSource.Equals(SelectedZoneCanvas))
            {
                // Make the drag selection box visible.           
                selectionBox.Visibility = Visibility.Visible;

                if (((PixelEditorViewModel) DataContext).IsWriteableBitmapOnEdition)
                {
                    ((PixelEditorViewModel) DataContext)?.BlitOnDrawing(ContentControl,
                                                                        ((PixelEditorViewModel) DataContext)
                                                                        ?.CropWriteableBitmap, true);
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
            Point mouseUpPosition = e.GetPosition(DrawingSurface);

            mouseUpPosition.X = mouseUpPosition.X < 0 ? 0 : mouseUpPosition.X;
            mouseUpPosition.X = mouseUpPosition.X > DrawingSurface.ActualWidth
                                    ? mouseUpPosition.X = DrawingSurface.ActualWidth
                                    : mouseUpPosition.X;
            mouseUpPosition.Y = mouseUpPosition.Y < 0 ? 0 : mouseUpPosition.Y;
            mouseUpPosition.Y = mouseUpPosition.Y > DrawingSurface.ActualHeight
                                    ? mouseUpPosition.Y = DrawingSurface.ActualHeight
                                    : mouseUpPosition.Y;

            // The mouse has been released, select the pixel in this rectangle
            if ((!(DataContext as PixelEditorViewModel)?.IsWriteableBitmapOnEdition ?? false)
                && ((PixelEditorViewModel) DataContext)?.ToolSelected == "selector")
            {
                ((PixelEditorViewModel) DataContext).IsWriteableBitmapOnEdition = false;
                if (!_mouseDownPositionSelector.Equals(mouseUpPosition))
                {
                    // Show the selector and the adorners
                    SelectedZoneCanvas.Visibility = Visibility.Visible;
                    foreach (Control child in SelectedZoneCanvas.Children)
                    {
                        Selector.SetIsSelected(child, true);
                    }

                    // Fonction of the selection box
                    ((PixelEditorViewModel) DataContext)?.SelectZone(ContentControl, _mouseDownPositionSelector,
                                                                     mouseUpPosition);
                }
                else
                {
                    // Hide the selector and the adorners
                    SelectedZoneCanvas.Visibility = Visibility.Collapsed;
                    foreach (Control child in SelectedZoneCanvas.Children)
                    {
                        Selector.SetIsSelected(child, false);
                    }
                }
            }
        }

        private void GridSurfaceMouseMove(object sender, MouseEventArgs e)
        {
            Point mouseUpPosition = e.GetPosition(DrawingSurface);

            // Confine the selectionBox in the DrawingSurface
            //When the selection box gets out of the border, reposition the extremities. 
            mouseUpPosition.X = mouseUpPosition.X < 0 ? 0 : mouseUpPosition.X;
            mouseUpPosition.X = mouseUpPosition.X > DrawingSurface.ActualWidth
                                    ? mouseUpPosition.X = DrawingSurface.ActualWidth
                                    : mouseUpPosition.X;
            mouseUpPosition.Y = mouseUpPosition.Y < 0 ? 0 : mouseUpPosition.Y;
            mouseUpPosition.Y = mouseUpPosition.Y > DrawingSurface.ActualHeight
                                    ? mouseUpPosition.Y = DrawingSurface.ActualHeight
                                    : mouseUpPosition.Y;

            if (_isMouseDownSelector)
            {
                // When the mouse is held down, reposition the drag selection box.

                if (_mouseDownPositionSelector.X < mouseUpPosition.X)
                {
                    Canvas.SetLeft(selectionBox, _mouseDownPositionSelector.X);
                    selectionBox.Width = mouseUpPosition.X - _mouseDownPositionSelector.X;
                }
                else
                {
                    Canvas.SetLeft(selectionBox, mouseUpPosition.X);
                    selectionBox.Width = _mouseDownPositionSelector.X - mouseUpPosition.X;
                }

                if (_mouseDownPositionSelector.Y < mouseUpPosition.Y)
                {
                    Canvas.SetTop(selectionBox, _mouseDownPositionSelector.Y);
                    selectionBox.Height = mouseUpPosition.Y - _mouseDownPositionSelector.Y;
                }
                else
                {
                    Canvas.SetTop(selectionBox, mouseUpPosition.Y);
                    selectionBox.Height = _mouseDownPositionSelector.Y - mouseUpPosition.Y;
                }
            }
        }

        private void ContentControlOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((DataContext as PixelEditorViewModel)?.IsWriteableBitmapOnEdition ?? false)
            {
                Point upperLeftCorner =
                    ContentControl.TransformToAncestor(SelectedZoneCanvas).Transform(new Point(0, 0));
                Point lowerRightCorner = new Point(upperLeftCorner.X + ContentControl.ActualWidth,
                                                   upperLeftCorner.Y + ContentControl.ActualHeight);

                ((PixelEditorViewModel) DataContext)?.SelectTempZone(lowerRightCorner, upperLeftCorner);
                ((PixelEditorViewModel) DataContext)?.BlitOnDrawing(ContentControl,
                                                                    ((PixelEditorViewModel) DataContext)
                                                                    ?.CropWriteableBitmap, false);
            }
        }

        private void ContentControlOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((DataContext as PixelEditorViewModel)?.IsWriteableBitmapOnEdition ?? false)

            {
                ((PixelEditorViewModel) DataContext)?.BlitOnDrawing(ContentControl,
                                                                    ((PixelEditorViewModel) DataContext)
                                                                    ?.TempWriteableBitmap, false);
            }
        }

        private void ContentControlOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _controlResized = !e.PreviousSize.Equals(new Size(0, 0));

            if (e.PreviousSize.Equals(new Size(0, 0)) || _regionBeforeResize.IsEmpty)
            {
                if (DataContext is PixelEditorViewModel vm)
                {
                    Point upperLeft = vm.CropWriteableBitmapPosition;
                    Point bottomRight = new Point(upperLeft.X + vm.CropWriteableBitmap.Width,
                                                  upperLeft.Y + vm.CropWriteableBitmap.Height);

                    Debug.WriteLine("Saving initial size");
                    _regionBeforeResize = new Rect(upperLeft, bottomRight);
                }
            }

            if (!e.PreviousSize.Equals(new Size(0, 0)))
            {
                (DataContext as PixelEditorViewModel)?.ReloadTempWriteableBitmap();
            }
        }
    }
}
