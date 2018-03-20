using System.Windows;
using System.Windows.Ink;
using System.Windows.Threading;
using PolyPaint.Helpers.Communication;
using PolyPaint.Models;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Strategy.StrokeEditorActionStrategy
{
    internal class EditorActionTransformStrokesStrategy : IEditorActionStrategy
    {
        private readonly StrokeEditorActionModel _transformStrokeAction;

        public EditorActionTransformStrokesStrategy(StrokeEditorActionModel action)
        {
            _transformStrokeAction = action;
        }

        public void ExecuteStrategy(StrokeEditor editor)
        {
            if (_transformStrokeAction.Author.Username == editor.CurrentUsername)
            {
                // Handled locally
                return;
            }

            foreach (StrokeModel stroke in _transformStrokeAction.Delta.Add)
            {
                Stroke transformedStroke =
                    StrokeHelper.BuildIncomingStroke(stroke, _transformStrokeAction.Author.Username);

                Dispatcher dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

                dispatcher.Invoke(() => editor.ReplaceStroke(stroke.Uuid, transformedStroke));
            }
        }
    }
}
