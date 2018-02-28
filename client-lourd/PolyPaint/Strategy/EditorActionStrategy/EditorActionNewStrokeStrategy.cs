using System;
using System.Linq;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using PolyPaint.CustomComponents;
using PolyPaint.Helpers.Communication;
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
            if (_newStrokeAction.Author.Username == editor.CurrentUsername) return;

            CustomStroke newStroke = StrokeHelper.BuildStrokeFromAction(_newStrokeAction);

            editor.AddIncomingStroke(newStroke);
        }
    }
}