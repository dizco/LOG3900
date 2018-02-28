using PolyPaint.Models;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Strategy.EditorActionStrategy
{
    internal class EditorActionStrategyContext : IEditorActionStrategy
    {
        private IEditorActionStrategy _strategy;

        public EditorActionStrategyContext(EditorActionModel action)
        {
            PickStrategy(action);
        }

        public void ExecuteStrategy(Editor editor)
        {
            _strategy.ExecuteStrategy(editor);
        }

        private void PickStrategy(EditorActionModel action)
        {
            ActionIds incomingActionId = (ActionIds) action.Action.Id;
            switch (incomingActionId)
            {
                case ActionIds.NewStroke:
                    _strategy = new EditorActionNewStrokeStrategy(action);
                    break;
                case ActionIds.Stack:
                    _strategy = new EditorActionStackStrokeStrategy(action);
                    break;
                default:
                    throw new InvalidActionStrategyException();
            }
        }
    }
}