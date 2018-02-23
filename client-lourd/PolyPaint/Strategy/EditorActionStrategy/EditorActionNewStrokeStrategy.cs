﻿using System;
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
        private readonly EditorActionModel _newStrokeAction;

        public EditorActionNewStrokeStrategy(EditorActionModel action)
        {
            _newStrokeAction = action;
        }

        public void ExecuteStrategy(Editor editor)
        {
            StylusPointCollection strokePoints =
                new StylusPointCollection(_newStrokeAction.Stroke.Dots.Select(point => new StylusPoint(point.x, point.y)));
            DrawingAttributes strokeAttributes = new DrawingAttributes
            {
                Height = _newStrokeAction.Stroke.DrawingAttributes.Height,
                Width = _newStrokeAction.Stroke.DrawingAttributes.Width,
                Color = (Color) ColorConverter.ConvertFromString(_newStrokeAction.Stroke.DrawingAttributes.Color),
                StylusTip =
                    (StylusTip) Enum.Parse(typeof(StylusTip), _newStrokeAction.Stroke.DrawingAttributes.StylusTip)
            };
            Stroke newStroke = new Stroke(strokePoints, strokeAttributes);

            editor.AddIncomingStroke(newStroke);
        }
    }
}