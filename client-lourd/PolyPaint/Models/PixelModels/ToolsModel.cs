using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PolyPaint.Models.PixelModels
{
    public class Tools
    {
        private Point _oldPosition;
        private Point _newPosition;

        private readonly WriteableBitmap _writeableBitmap;

        public Tools(WriteableBitmap writeableBitmap, Point oldPosition, Point newPosition)
        {
            _writeableBitmap = writeableBitmap;
            _oldPosition = oldPosition;
            _newPosition = newPosition;
        }

        /// <summary>
        ///     Draw a serie of pixel between a position and another
        ///     The DrawLine is used to gives a smoother rendering
        /// </summary>
        /// <param name="pixelSize"></param>
        /// <param name="selectedColor"></param>
        /// <param name="isColored"></param>
        public void DrawPixel(int pixelSize, string selectedColor)
        {
            Color color = (Color) ColorConverter.ConvertFromString(selectedColor);

            for (int j = 0; j < pixelSize; j++)
            {
                for (int i = 0; i < pixelSize; i++)
                {
                    _writeableBitmap.DrawLine((int) _oldPosition.X - i, (int) _oldPosition.Y - j,
                                              (int) _newPosition.X - i, (int) _newPosition.Y - j, color);
                }
            }
        }
    }
}
