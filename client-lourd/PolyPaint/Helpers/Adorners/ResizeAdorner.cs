using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace PolyPaint.Helpers.Adorners
{
    public class ResizeAdorner : Adorner
    {
        private readonly ResizeChrome _chrome;
        private readonly VisualCollection _visuals;

        public ResizeAdorner(ContentControl selectedZoneContentControl)
            : base(selectedZoneContentControl)
        {
            SnapsToDevicePixels = true;
            _chrome = new ResizeChrome
            {
                DataContext = selectedZoneContentControl
            };
            _visuals = new VisualCollection(this)
            {
                _chrome
            };
        }

        protected override int VisualChildrenCount => _visuals.Count;

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            _chrome.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }
    }
}
