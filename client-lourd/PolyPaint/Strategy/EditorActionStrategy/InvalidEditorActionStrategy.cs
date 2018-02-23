using System;
using PolyPaint.Models;

namespace PolyPaint.Strategy.EditorActionStrategy
{
    internal class InvalidEditorActionStrategy : IEditorActionStrategy
    {
        public void ExecuteStrategy(Editor editor)
        {
            throw new InvalidActionStrategyException("Invalid editor action");
        }
    }

    internal class InvalidActionStrategyException : Exception
    {
        public InvalidActionStrategyException()
        {
        }

        public InvalidActionStrategyException(string message) : base(message)
        {
        }

        public InvalidActionStrategyException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}