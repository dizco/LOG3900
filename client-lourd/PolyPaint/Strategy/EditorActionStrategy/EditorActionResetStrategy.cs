using PolyPaint.Models;

namespace PolyPaint.Strategy.EditorActionStrategy
{
    internal class EditorActionResetStrategy : IEditorActionStrategy
    {
        public void ExecuteStrategy(StrokeEditor editor)
        {
            editor.Reset(null);
            editor.OnResetStrokeActionReceived();
        }
    }
}
