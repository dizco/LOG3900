using System;

namespace PolyPaint.Strategy.EditorActionStrategy
{
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
