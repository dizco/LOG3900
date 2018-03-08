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
        public static CustomStroke BuildStrokeFromAction(EditorActionModel action)
        {
            //StylusPointCollection strokePoints =
            //    new StylusPointCollection(action.Stroke.Dots.Select(point => new StylusPoint(point.x, point.y)));
            //DrawingAttributes strokeAttributes = new DrawingAttributes
            //{
            //    Height = action.Stroke.DrawingAttributes.Height,
            //    Width = action.Stroke.DrawingAttributes.Width,
            //    Color = (Color)ColorConverter.ConvertFromString(action.Stroke.DrawingAttributes.Color),
            //    StylusTip =
            //        (StylusTip)Enum.Parse(typeof(StylusTip), action.Stroke.DrawingAttributes.StylusTip)
            //};

            //return new CustomStroke(strokePoints, strokeAttributes, action.Author.Username);

            return null;
        }

        public static CustomStroke BuildIncomingStroke(StrokeModel stroke, string author)
        {
            StylusPointCollection strokePoints = new StylusPointCollection(stroke.Dots.Select(point => new StylusPoint(point.X, point.Y)));
            DrawingAttributes strokeAttributes = new DrawingAttributes
            {
                Height = stroke.DrawingAttributes.Height,
                Width = stroke.DrawingAttributes.Width,
                Color = (Color)ColorConverter.ConvertFromString(stroke.DrawingAttributes.Color),
                StylusTip =
                    (StylusTip)Enum.Parse(typeof(StylusTip), stroke.DrawingAttributes.StylusTip)
            };
            
            return new CustomStroke(strokePoints, strokeAttributes, author, stroke.Uuid);
        }

        public static bool AreSameStroke(Stroke stroke1, Stroke stroke2)
        {
            bool isSameColor = stroke1.DrawingAttributes.Color.Equals(stroke2.DrawingAttributes.Color);
            bool isSameStylusTip = stroke1.DrawingAttributes.StylusTip
                                          .Equals(stroke2.DrawingAttributes.StylusTip);
            bool isSameWidth = stroke1.DrawingAttributes.Width.Equals(stroke2.DrawingAttributes.Width);
            bool isSameHeight = stroke1.DrawingAttributes.Height.Equals(stroke2.DrawingAttributes.Height);
            bool isSameStroke = stroke1.StylusPoints.SequenceEqual(stroke2.StylusPoints);

            return isSameColor && isSameStylusTip && isSameWidth && isSameHeight && isSameStroke;
        }
    }
}
