using System.Windows.Ink;
using System.Windows.Input;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.CustomComponents
{
    internal class CustomStroke : Stroke
    {
        public CustomStroke(StylusPointCollection stylusPoints) : base(stylusPoints)
        {
        }

        public CustomStroke(StylusPointCollection stylusPoints, DrawingAttributes drawingAttributes, string author) :
            base(stylusPoints, drawingAttributes)
        {
            Author = author;
        }

        public string Author { get; set; }
    }
}
