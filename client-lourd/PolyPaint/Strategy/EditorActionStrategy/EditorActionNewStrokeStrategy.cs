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

        public void ExecuteStrategy(EditorStroke editor)
        {
            if (_newStrokeAction.Author.Username == editor.CurrentUsername)
            {
                // Handled locally
                return;
            }

            foreach (StrokeModel stroke in _newStrokeAction.Delta.Add)
            {
                editor.AddIncomingStroke(StrokeHelper.BuildIncomingStroke(stroke, _newStrokeAction.Author.Username));
            }
        }
    }
}
