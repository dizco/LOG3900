using PolyPaint.Models.MessagingModels;
using PolyPaint.Models.PixelModels;

namespace PolyPaint.Strategy.PixelEditorActionStrategy
{
    internal interface IEditorActionStrategy
    {
        void ExecuteStrategy(PixelEditor editor);
    }

    class EditorActionNewPixelStrategy : IEditorActionStrategy
    {
        private PixelEditorActionModel _newPixelAction;

        public EditorActionNewPixelStrategy(PixelEditorActionModel newPixels)
        {
            _newPixelAction = newPixels;
        }
        public void ExecuteStrategy(PixelEditor editor)
        {
            throw new System.NotImplementedException();
        }
    }
}
