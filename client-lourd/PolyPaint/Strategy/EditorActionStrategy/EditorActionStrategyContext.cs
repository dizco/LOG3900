﻿using PolyPaint.Models;
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
                case ActionIds.ReplaceStroke:
                    _strategy = new EditorActionReplaceStrokeStrategy(action);
                    break;
                case ActionIds.LockStrokes:
                    _strategy = new EditorActionLockStrokesStrategy(action);
                    break;
                case ActionIds.UnlockStrokes:
                    _strategy = new EditorActionUnlockStrokesStrategy(action);
                    break;
                default:
                    throw new InvalidActionStrategyException();
            }
        }
    }
}
