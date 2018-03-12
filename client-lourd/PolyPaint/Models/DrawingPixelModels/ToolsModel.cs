using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PolyPaint.Models.DrawingPixelModels
{
    public class Tools
    {
        public Tools(WriteableBitmap writeableBitmap, Point oldPosition, Point newPosition)
        {
            WriteableBitmap = writeableBitmap;
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }

        private Point OldPosition { get; }
        private Point NewPosition { get; }
        private WriteableBitmap WriteableBitmap { get; }

        /// <summary>
        ///     Draw a serie of pixel between a position and another
        ///     The DrawLine is used to gives a smoother rendering
        /// </summary>
        /// <param name="pixelSize"></param>
        /// <param name="selectedColor"></param>
        /// <param name="isColored"></param>
        public void DrawPixel(int pixelSize, string selectedColor, bool isColored)
        {
            Color color = Colors.Transparent;

            if (isColored)
            {
                color = (Color) ColorConverter.ConvertFromString(selectedColor);
            }

            for (int j = 0; j < pixelSize; j++)
            {
                for (int i = 0; i < pixelSize; i++)
                {
                    WriteableBitmap.DrawLine((int) OldPosition.X + i, (int) OldPosition.Y + j,
                                             (int) NewPosition.X + i, (int) NewPosition.Y + j, color);
                }
            }
        }
    }
}
