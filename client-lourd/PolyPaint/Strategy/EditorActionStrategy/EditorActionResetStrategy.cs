using System.Windows;
using System.Windows.Threading;
using PolyPaint.Models;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Strategy.EditorActionStrategy
{
    internal class EditorActionResetStrategy : IEditorActionStrategy
    {
        private readonly EditorActionModel _resetAction;

        public EditorActionResetStrategy(EditorActionModel action)
        {
            _resetAction = action;
        }

        public void ExecuteStrategy(StrokeEditor editor)
        {
            if (_resetAction.Author.Username == editor.CurrentUsername)
            {
                // Handled locally
                return;
            }

            (Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher).Invoke(() =>
            {
                editor.Reset(null);
                editor.OnResetStrokeActionReceived();
            });
        }
    }
}
