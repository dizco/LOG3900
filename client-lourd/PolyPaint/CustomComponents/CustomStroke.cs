using System;
using System.Windows.Ink;
using System.Windows.Input;

namespace PolyPaint.CustomComponents
{
    internal class CustomStroke : Stroke
    {
        public CustomStroke(StylusPointCollection stylusPoints) : base(stylusPoints)
        {
            Uuid = Guid.NewGuid();
        }

        public CustomStroke(StylusPointCollection stylusPoints, DrawingAttributes drawingAttributes) :
            base(stylusPoints, drawingAttributes)
        {
            Uuid = Guid.NewGuid();
        }

        public CustomStroke(StylusPointCollection stylusPoints, DrawingAttributes drawingAttributes, string author) :
            base(stylusPoints, drawingAttributes)
        {
            Author = author;
            Uuid = Guid.NewGuid();
        }

        public CustomStroke(StylusPointCollection stylusPoints, DrawingAttributes drawingAttributes, string author,
            string uuid) : base(stylusPoints, drawingAttributes)
        {
            Author = author;
            Uuid = new Guid(uuid);
        }

        public string Author { get; set; }
        public Guid Uuid { get; set; }

        internal void RefreshUuid()
        {
            Uuid = Guid.NewGuid();
        }
    }
}
