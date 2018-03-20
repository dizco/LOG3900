using PolyPaint.Models.MessagingModels;
using PolyPaint.Models.PixelModels;

namespace PolyPaint.Strategy.PixelEditorActionStrategy
{
    internal class EditorActionStrategyContext : IEditorActionStrategy
    {
        private IEditorActionStrategy _strategy;

        public EditorActionStrategyContext(PixelEditorActionModel action)
        {
            PickStrategy(action);
        }

        public void ExecuteStrategy(PixelEditor editor)
        {
            _strategy.ExecuteStrategy(editor);
        }

        private void PickStrategy(PixelEditorActionModel action)
        {
            PixelActionIds incomingPixelAction = (PixelActionIds) action.Action.Id;
            switch (incomingPixelAction)
            {
                case PixelActionIds.NewPixels:
                    _strategy = new EditorActionNewPixelStrategy(action);
                    break;
                default:
                    throw new InvalidActionStrategyException();
            }
        }
    }
}
