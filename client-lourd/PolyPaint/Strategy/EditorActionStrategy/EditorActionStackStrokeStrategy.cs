using PolyPaint.CustomComponents;
using PolyPaint.Helpers.Communication;
using PolyPaint.Models;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Strategy.EditorActionStrategy
{
    class EditorActionStackStrokeStrategy : IEditorActionStrategy
    {
        private readonly EditorActionModel _stackedStrokeAction;

        public EditorActionStackStrokeStrategy(EditorActionModel action)
        {
            _stackedStrokeAction = action;
        }
        public void ExecuteStrategy(Editor editor)
        {
            if (_stackedStrokeAction.Author.Username == editor.CurrentUsername) return;

            CustomStroke stroke = StrokeHelper.BuildStrokeFromAction(_stackedStrokeAction);

            editor.RemoveStackedStroke(stroke);
        }
    }
}