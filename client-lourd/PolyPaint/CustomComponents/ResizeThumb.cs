using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PolyPaint.CustomComponents
{
    public class ResizeThumb : Thumb
    {
        private ContentControl _selectedZoneContentControl;
        private Point _transformOrigin;

        public ResizeThumb()
        {
            DragStarted += ResizeThumb_DragStarted;
            DragDelta += ResizeThumb_DragDelta;
        }

        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _selectedZoneContentControl = DataContext as ContentControl;

            if (_selectedZoneContentControl != null)
            {
                {
                    _transformOrigin = _selectedZoneContentControl.RenderTransformOrigin;
                }
            }
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_selectedZoneContentControl != null)
            {
                double deltaVertical, deltaHorizontal;

                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        deltaVertical = Math.Min(-e.VerticalChange, _selectedZoneContentControl.ActualHeight - _selectedZoneContentControl.MinHeight);
                        Canvas.SetTop(_selectedZoneContentControl, Canvas.GetTop(_selectedZoneContentControl));
                        Canvas.SetLeft(_selectedZoneContentControl, Canvas.GetLeft(_selectedZoneContentControl));
                        _selectedZoneContentControl.Height -= deltaVertical;
                        break;

                    case VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange, _selectedZoneContentControl.ActualHeight -  _selectedZoneContentControl.MinHeight);
                        Canvas.SetTop(_selectedZoneContentControl, Canvas.GetTop(_selectedZoneContentControl) + deltaVertical);
                        Canvas.SetLeft(_selectedZoneContentControl, Canvas.GetLeft(_selectedZoneContentControl));
                        _selectedZoneContentControl.Height -= deltaVertical;
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(e.HorizontalChange, _selectedZoneContentControl.ActualWidth - _selectedZoneContentControl.MinWidth);
                        Canvas.SetTop(_selectedZoneContentControl, Canvas.GetTop(_selectedZoneContentControl));
                        Canvas.SetLeft(_selectedZoneContentControl, Canvas.GetLeft(_selectedZoneContentControl) + deltaHorizontal);
                        _selectedZoneContentControl.Width -= deltaHorizontal;
                        break;

                    case HorizontalAlignment.Right:
                        deltaHorizontal = Math.Min(-e.HorizontalChange, _selectedZoneContentControl.ActualWidth - _selectedZoneContentControl.MinWidth);
                        Canvas.SetTop(_selectedZoneContentControl, Canvas.GetTop(_selectedZoneContentControl));
                        Canvas.SetLeft(_selectedZoneContentControl, Canvas.GetLeft(_selectedZoneContentControl));
                        _selectedZoneContentControl.Width -= deltaHorizontal;
                        break;
                }
            }

            e.Handled = true;
        }
    }
}
