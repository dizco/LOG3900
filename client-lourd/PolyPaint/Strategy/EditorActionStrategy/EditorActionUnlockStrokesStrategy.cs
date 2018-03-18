using PolyPaint.Models;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Strategy.EditorActionStrategy
{
    internal class EditorActionUnlockStrokesStrategy : IEditorActionStrategy
    {
        private readonly EditorActionModel _unlockStrokesAction;

        public EditorActionUnlockStrokesStrategy(EditorActionModel action)
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
