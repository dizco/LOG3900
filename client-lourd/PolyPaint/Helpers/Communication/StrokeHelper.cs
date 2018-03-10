using System;
using System.Linq;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using PolyPaint.CustomComponents;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Helpers.Communication
{
    internal static class StrokeHelper
    {
        public static CustomStroke BuildIncomingStroke(StrokeModel stroke, string author)
        {
            StylusPointCollection strokePoints =
                new StylusPointCollection(stroke.Dots.Select(point => new StylusPoint(point.X, point.Y)));
            DrawingAttributes strokeAttributes = new DrawingAttributes
            {
                Height = stroke.DrawingAttributes.Height,
                Width = stroke.DrawingAttributes.Width,
                Color = (Color) ColorConverter.ConvertFromString(stroke.DrawingAttributes.Color),
                StylusTip =
                    (StylusTip) Enum.Parse(typeof(StylusTip), stroke.DrawingAttributes.StylusTip)
            };

            return new CustomStroke(strokePoints, strokeAttributes, author, stroke.Uuid);
        }

        public static bool AreSameStroke(CustomStroke stroke1, CustomStroke stroke2)
        {
            return stroke1?.Uuid == stroke2?.Uuid;
        }
    }
}
