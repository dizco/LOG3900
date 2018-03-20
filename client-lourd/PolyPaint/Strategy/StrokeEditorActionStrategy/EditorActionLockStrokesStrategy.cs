using PolyPaint.Models;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Strategy.StrokeEditorActionStrategy
{
    internal class EditorActionLockStrokesStrategy : IEditorActionStrategy
    {
        private readonly StrokeEditorActionModel _lockStrokesAction;

        public EditorActionLockStrokesStrategy(StrokeEditorActionModel action)
        {
            _lockStrokesAction = action;
        }

        public void ExecuteStrategy(StrokeEditor editor)
        {
            if (_lockStrokesAction.Author.Username == editor.CurrentUsername)
            {
                // Handled locally
                return;
            }

            foreach (string lockedStroke in _lockStrokesAction.Delta.Remove)
            {
                editor.LockedStrokes.Add(lockedStroke);
            }
        }
    }
}
