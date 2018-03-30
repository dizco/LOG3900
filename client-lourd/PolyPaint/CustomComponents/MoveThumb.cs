using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PolyPaint.CustomComponents
{
    public class MoveThumb : Thumb
    {
        private ContentControl _selectedZoneContentControl;

        public MoveThumb()
        {
            DragStarted += MoveThumb_DragStarted;
            DragDelta += MoveThumb_DragDelta;
        }

        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _selectedZoneContentControl = DataContext as ContentControl;
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (_selectedZoneContentControl != null)
            {
                Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);

                Canvas.SetLeft(_selectedZoneContentControl, Canvas.GetLeft(_selectedZoneContentControl) + dragDelta.X);
                Canvas.SetTop(_selectedZoneContentControl, Canvas.GetTop(_selectedZoneContentControl) + dragDelta.Y);
            }
        }
    }
}
