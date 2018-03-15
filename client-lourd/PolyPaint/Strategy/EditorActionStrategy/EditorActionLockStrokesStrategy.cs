using PolyPaint.Models;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Strategy.EditorActionStrategy
{
    internal class EditorActionLockStrokesStrategy : IEditorActionStrategy
    {
        private readonly EditorActionModel _lockStrokesAction;

        public EditorActionLockStrokesStrategy(EditorActionModel action)
        {
            _lockStrokesAction = action;
        }

        public void ExecuteStrategy(Editor editor)
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
