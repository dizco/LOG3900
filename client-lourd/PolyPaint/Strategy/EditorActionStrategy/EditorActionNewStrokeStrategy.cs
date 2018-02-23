using System;
using System.Linq;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using PolyPaint.Models;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Strategy.EditorActionStrategy
{
    internal class EditorActionNewStrokeStrategy : IEditorActionStrategy
    {
        private readonly EditorActionModel _stroke;

        public EditorActionNewStrokeStrategy(EditorActionModel action)
        {
            _stroke = action;
        }

        public void ExecuteStrategy(Editor editor)
        {
            StylusPointCollection strokePoints =
                new StylusPointCollection(_stroke.Stroke.Dots.Select(point => new StylusPoint(point.x, point.y)));
            DrawingAttributes strokeAttributes = new DrawingAttributes
            {
                Height = _stroke.Stroke.DrawingAttributes.Height,
                Width = _stroke.Stroke.DrawingAttributes.Width,
                Color = (Color) ColorConverter.ConvertFromString(_stroke.Stroke.DrawingAttributes.Color),
                StylusTip =
                    (StylusTip) Enum.Parse(typeof(StylusTip), _stroke.Stroke.DrawingAttributes.StylusTip)
            };
            Stroke newStroke = new Stroke(strokePoints, strokeAttributes);

            editor.AddIncomingStroke(newStroke);
        }
    }
}