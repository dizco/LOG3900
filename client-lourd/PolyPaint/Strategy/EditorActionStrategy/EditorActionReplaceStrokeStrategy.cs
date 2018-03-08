using System.Windows.Ink;
using PolyPaint.Helpers.Communication;
using PolyPaint.Models;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Strategy.EditorActionStrategy
{
    internal class EditorActionReplaceStrokeStrategy : IEditorActionStrategy
    {
        private readonly EditorActionModel _replaceStrokeAction;

        public EditorActionReplaceStrokeStrategy(EditorActionModel action)
        {
            _replaceStrokeAction = action;
        }

        public void ExecuteStrategy(Editor editor)
        {
            if (_replaceStrokeAction.Author.Username == editor.CurrentUsername)
            {
                return;
            }

            string[] removedStrokes = _replaceStrokeAction.Delta.Remove;

            StrokeCollection addedStrokes = new StrokeCollection();

            if (_replaceStrokeAction.Delta.Add != null)
            {
                foreach (StrokeModel stroke in _replaceStrokeAction.Delta.Add)
                {
                    addedStrokes.Add(StrokeHelper.BuildIncomingStroke(stroke, _replaceStrokeAction.Author.Username));
                }
            }

            foreach (string removedStroke in removedStrokes)
            {
                editor.ReplaceStroke(removedStroke, addedStrokes);
            }
        }
    }
}
