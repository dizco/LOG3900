using PolyPaint.Models;

namespace PolyPaint.Strategy.EditorActionStrategy
{
    internal interface IEditorActionStrategy
    {
        void ExecuteStrategy(EditorStroke editor);
    }
}
