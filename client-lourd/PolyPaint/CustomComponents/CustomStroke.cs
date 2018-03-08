using System;
using System.Windows.Ink;
using System.Windows.Input;

namespace PolyPaint.CustomComponents
{
    internal class CustomStroke : Stroke
    {
        public CustomStroke(StylusPointCollection stylusPoints) : base(stylusPoints)
        {
            Uuid = Guid.NewGuid().ToString();
        }

        public CustomStroke(StylusPointCollection stylusPoints, DrawingAttributes drawingAttributes) :
            base(stylusPoints, drawingAttributes)
        {
            Uuid = Guid.NewGuid().ToString();
        }

        public CustomStroke(StylusPointCollection stylusPoints, DrawingAttributes drawingAttributes, string author) :
            base(stylusPoints, drawingAttributes)
        {
            Author = author;
            Uuid = Guid.NewGuid().ToString();
        }

        public CustomStroke(StylusPointCollection stylusPoints, DrawingAttributes drawingAttributes, string author,
            string uuid) : base(stylusPoints, drawingAttributes)
        {
            Author = author;
            Uuid = uuid;
        }

        public string Author { get; set; }
        public string Uuid { get; set; }
    }
}
