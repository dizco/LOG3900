using PolyPaint.Models;
using PolyPaint.Models.MessagingModels;

namespace PolyPaint.Strategy.StrokeEditorActionStrategy
{
    internal class EditorActionStrategyContext : IEditorActionStrategy
    {
        private IEditorActionStrategy _strategy;

        public EditorActionStrategyContext(StrokeEditorActionModel action)
        {
            PickStrategy(action);
        }

        public void ExecuteStrategy(StrokeEditor editor)
        {
            _strategy.ExecuteStrategy(editor);
        }

        private void PickStrategy(StrokeEditorActionModel action)
        {
            StrokeActionIds incomingStrokeActionId = (StrokeActionIds) action.Action.Id;
            switch (incomingStrokeActionId)
            {
                case StrokeActionIds.NewStroke:
                    _strategy = new EditorActionNewStrokeStrategy(action);
                    break;
                case StrokeActionIds.ReplaceStroke:
                    _strategy = new EditorActionReplaceStrokeStrategy(action);
                    break;
                case StrokeActionIds.LockStrokes:
                    _strategy = new EditorActionLockStrokesStrategy(action);
                    break;
                case StrokeActionIds.UnlockStrokes:
                    _strategy = new EditorActionUnlockStrokesStrategy(action);
                    break;
                case StrokeActionIds.Transform:
                    _strategy = new EditorActionTransformStrokesStrategy(action);
                    break;
                case StrokeActionIds.Reset:
                    _strategy = new EditorActionResetStrategy(action);
                    break;
                default:
                    throw new InvalidActionStrategyException();
            }
        }
    }
}
