using PolyPaint.Helpers.Communication;
using PolyPaint.Models;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Strategy.StrokeEditorActionStrategy
{
    internal class EditorActionNewStrokeStrategy : IEditorActionStrategy
    {
        private readonly StrokeEditorActionModel _newStrokeAction;

        public EditorActionNewStrokeStrategy(StrokeEditorActionModel action)
        {
            _newStrokeAction = action;
        }

        public void ExecuteStrategy(StrokeEditor editor)
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
