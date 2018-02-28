using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using PolyPaint.CustomComponents;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Helpers.Communication
{
    static class StrokeHelper
    {
        public static CustomStroke BuildStrokeFromAction(EditorActionModel action)
        {
            StylusPointCollection strokePoints =
                new StylusPointCollection(action.Stroke.Dots.Select(point => new StylusPoint(point.x, point.y)));
            DrawingAttributes strokeAttributes = new DrawingAttributes
            {
                Height = action.Stroke.DrawingAttributes.Height,
                Width = action.Stroke.DrawingAttributes.Width,
                Color = (Color)ColorConverter.ConvertFromString(action.Stroke.DrawingAttributes.Color),
                StylusTip =
                    (StylusTip)Enum.Parse(typeof(StylusTip), action.Stroke.DrawingAttributes.StylusTip)
            };

            return new CustomStroke(strokePoints, strokeAttributes, action.Author.Username);
        }

        public static bool AreSameStroke(Stroke stroke1, Stroke stroke2)
        {
            bool isSameColor = stroke1.DrawingAttributes.Color.Equals(stroke2.DrawingAttributes.Color);
            bool isSameStylusTip = stroke1.DrawingAttributes.StylusTip
                                      .Equals(stroke2.DrawingAttributes.StylusTip);
            bool isSameWidth = stroke1.DrawingAttributes.Width.Equals(stroke2.DrawingAttributes.Width);
            bool isSameHeight = stroke1.DrawingAttributes.Height.Equals(stroke2.DrawingAttributes.Height);
            bool isSameStroke = stroke1.StylusPoints.SequenceEqual(stroke2.StylusPoints);

            return (isSameColor && isSameStylusTip && isSameWidth && isSameHeight && isSameStroke);
        }
    }
}
