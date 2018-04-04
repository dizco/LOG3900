using System.Windows;
using System.Windows.Threading;
using PolyPaint.Models.MessagingModels;
using PolyPaint.Models.PixelModels;

namespace PolyPaint.Strategy.PixelEditorActionStrategy
{
    internal class EditorActionNewPixelStrategy : IEditorActionStrategy
    {
        private readonly PixelEditorActionModel _newPixelAction;

        public EditorActionNewPixelStrategy(PixelEditorActionModel newPixels)
        {
            _newPixelAction = newPixels;
        }

        public void ExecuteStrategy(PixelEditor editor)
        {
            if (_newPixelAction.Author.Username == editor.CurrentUsername)
            {
                // Handled locally
                return;
            }
            (Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher).Invoke(() =>
            {
                editor.WriteableBitmap.Lock();
                foreach (PixelModel pixel in _newPixelAction.Pixels)
                {
                        editor.DrawIncomingPixel((int) pixel.X, (int) pixel.Y, pixel.Color);
                
                }
                editor.WriteableBitmap.Unlock();
            });
        }
    }
}