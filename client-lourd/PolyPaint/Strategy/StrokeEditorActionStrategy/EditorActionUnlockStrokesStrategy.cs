using PolyPaint.Models;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Strategy.StrokeEditorActionStrategy
{
    internal class EditorActionUnlockStrokesStrategy : IEditorActionStrategy
    {
        private readonly StrokeEditorActionModel _unlockStrokesAction;

        public EditorActionUnlockStrokesStrategy(StrokeEditorActionModel action)
        {
            _unlockStrokesAction = action;
        }

        public void ExecuteStrategy(StrokeEditor editor)
        {
            if (_unlockStrokesAction.Author.Username == editor.CurrentUsername)
            {
                // Handled locally
                return;
            }

            foreach (string lockedStroke in _unlockStrokesAction.Delta.Remove)
            {
                editor.LockedStrokes.Remove(lockedStroke);
            }
        }
    }
}
