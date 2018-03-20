using PolyPaint.Models;

namespace PolyPaint.Strategy.StrokeEditorActionStrategy
{
    internal interface IEditorActionStrategy
    {
        void ExecuteStrategy(StrokeEditor editor);
    }
}
