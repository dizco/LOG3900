using PolyPaint.Models.PixelModels;

namespace PolyPaint.Strategy.PixelEditorActionStrategy
{
    internal interface IEditorActionStrategy
    {
        void ExecuteStrategy(PixelEditor editor);
    }
}
